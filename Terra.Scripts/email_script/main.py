import smtplib, ssl, certifi
from string import Template
from email.message import EmailMessage
import json
from influx import InfluxHelper
from gcp import FirestoreHelper, StorageBucketHelper
from matplot_helper import MatplotHelper


# assume there is only one mcu at the moment

def main():
    mail = ""
    password = ""
    with open('config.json', 'r') as data:
        config = json.load(data)
        mail = config["SMTP"]["ROOT"]
        password = config["SMTP"]["PASS"]
    port = 465 #SSL
    context = ssl.create_default_context()
    message = EmailMessage()

    influxHelper = InfluxHelper()
    gcp_bucket_helper = StorageBucketHelper()
    matplot_helper = MatplotHelper()

    # generate content
    light_exposure = influxHelper.get_time_lightexp()
    dark_exposure = influxHelper.get_time_darkexp()
    mcu = influxHelper.get_data_source()
    report_date = influxHelper.get_report_date()
    avg_humid = influxHelper.get_humid_average()
    avg_temp = influxHelper.get_temp_average()
    matplot_helper.map_light_intensity()
    matplot_helper.map_temp_humid()
    gcp_bucket_helper.upload_graphs_to_bucket()
    light_graph_url = gcp_bucket_helper.get_light_graph_url()
    temp_humid_graph_url = gcp_bucket_helper.get_temp_humid_url()

    # firestore schedule
    firestoreHelper = FirestoreHelper()
    next_water_schedule = firestoreHelper.get_doc_content()

    
    #with open('report_test.html') as fp:
    #     html_content = fp.read()
    
    # construct html
    html_content = """
<html>
    <head>
        <meta http-equiv="Content-Type" content="text/html; charset=utf-8">
        <style type="text/css">
            @media only screen and (max-width:450px) {
                .outershell {
                    width: 100% !important;
                }
            }
        </style>
    </head>
    <body>
        <table align='center' bgcolor="#ECF4D6" role="presentation" width="50%" border="0" cellspacing="9px" class='outershell'>
            <!-- Generals -->
            <tr>
                <td align="center" colspan="2">
                    <h2 style="margin-bottom: 20px;"> Generals </h2>
                </td>
            </tr>
            <tr>
                <td bgcolor="#265073" width="180px" height="87px">
                    <p style="color: #6D6D6D; font-size: 13px; margin: 0 0 0 1rem"> <img src="https://iili.io/JTbUf87.png" width="11px" height="11px"/>  Source </p>
                    <p style="color: #ECF4D6; font-weight: 700; font-size: 15px; margin: 23px 0 0 1rem"> $mcu </p>
                </td>
                <td bgcolor="#265073" width="180px" height="87px">
                    <p style="color: #6D6D6D; font-size: 13px; margin: 0 0 0 1rem"> <img src="https://iili.io/JTbUGPR.png" width="11px" height="11px"/>  Report Date </p>
                    <p style="color: #ECF4D6; font-weight: 700; font-size: 15px; margin: 23px 0 0 1rem"> $date </p>
                </td>
            </tr>

            <!-- Atmospheric -->
            <tr>
                <td align="center" colspan="2">
                    <h2 style="margin: 55px 0 20px 0;"> Atmospheric </h2>
                </td>
            </tr>
            <tr>
                <td bgcolor="#265073" width="180px" height="87px">
                    <p style="color: #6D6D6D; font-size: 13px; margin: 0 0 0 1rem"> <img src="https://iili.io/JufWZxt.png" width="11px" height="11px"/> Average Humid </p>
                    <p style="color: #ECF4D6; font-weight: 700; font-size: 15px; margin: 23px 0 0 1rem"> $humid % </p>
                </td>
                <td bgcolor="#265073" width="180px" height="87px">
                    <p style="color: #6D6D6D; font-size: 13px; margin: 0 0 0 1rem"> <img src="https://iili.io/Jufwe2a.png" width="5.2px" height="11px"/> Average Temp </p>
                    <p style="color: #ECF4D6; font-weight: 700; font-size: 15px; margin: 23px 0 0 1rem"> $temp Celsius </p>
                </td>
            </tr>
            <tr>
                <td bgcolor="#265073" width="180px" height="87px">
                    <p style="color: #6D6D6D; font-size: 13px; margin: 0 0 0 1rem"> <img src="https://iili.io/JufOqSs.png" width="11px" height="11px"/> Dark Exposure </p>
                    <p style="color: #ECF4D6; font-weight: 700; font-size: 15px; margin: 23px 0 0 1rem"> $dark </p>
                </td>
                <td bgcolor="#265073" width="180px" height="87px">
                    <p style="color: #6D6D6D; font-size: 13px; margin: 0 0 0 1rem"> <img src="https://iili.io/JufOTVS.png" width="11px" height="11px"/> Light Exposure </p>
                    <p style="color: #ECF4D6; font-weight: 700; font-size: 15px; margin: 23px 0 0 1rem"> $light </p>
                </td>
            </tr>

            <!-- Watering Module -->
            <tr>
                <td align="center" colspan="2">
                    <h2 style="margin: 55px 0 20px 0;"> Watering Module </h2>
                </td>
            </tr>
            <tr>
                <td bgcolor="#265073" width="400px" height="87px" colspan="2">
                    <p style="color: #6D6D6D; text-align: center; font-size: 11px; margin: 0 0 0 0"> <img src="https://iili.io/JufkRyb.png" width="13px" height="13px"/> Liquid Pumped </p>
                    <p style="color: #ECF4D6; font-weight: 700; text-align: center; font-size: 15px; margin: 23px 0 0 0"> 11.5 hours </p>
                </td>
            </tr>
            <tr>
                <td align="center" colspan="2">
                    <h3 style="margin: 23px 0 23px 0;"> Upcoming Watering Schedule </h3>
                    <p style="text-align: center; font-size: 15px; margin: 10px 0 0 0"> $next_schedule </p>
                </td>
            </tr>

            <!-- Graphical Data (from yesterday) -->
            <tr>
                <td align="center" colspan="2">
                    <h2 style="margin: 55px 0 20px 0;"> Graphical Data </h2>
                </td>          
            </tr> 
            <tr>
                <td align='center' colspan='2'>
                    <img src=$temp_humid_url width="75%" height="50%"/>
                </td> 
            </tr>
            <tr>
                <td align='center' colspan='2'>
                    <img src=$light_url width="75%" height="50%"/>
                </td>   
            </tr>
        </table>
</body>
</html>"""

    formatted_content = Template(html_content).substitute(mcu=mcu, date=report_date, humid=avg_humid, temp=avg_temp, dark=dark_exposure, light=light_exposure, next_schedule=next_water_schedule, light_url=light_graph_url, temp_humid_url=temp_humid_graph_url)

    # construct mail signature
    message['Subject'] = 'This is a test mail'
    message['From'] = mail
    message['To'] = "marcodsang@gmail.com"
    message['Cc'] = ""
    message['Bcc'] = ""

    message.set_content("This is a test email", "plain")

    message.add_alternative(formatted_content, subtype='html')

    with smtplib.SMTP_SSL("smtp.gmail.com", port, context=context) as server:
        server.login(mail, password)
        with open('userList.json', "r") as data:
                user_list = json.load(data)
                for data in user_list.values():
                    for user in data:
                        server.send_message(message) 
                        
    
        
                        

if __name__ == "__main__":
    main()