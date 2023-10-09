
import serial
import json
from datetime import datetime
import influxdb_client, os, time
from influxdb_client import InfluxDBClient, Point, WritePrecision
from influxdb_client.client.write_api import SYNCHRONOUS
import firebase_admin
from firebase_admin import credentials
from firebase_admin import firestore


# influx db access
bucket = "Terra" 
token = "gkSWc1Vzwfq8nr3vKRhklT_ebchZakq1rzF4dRpAgJdaBp1orj5G6kZVeflHZgzf2TdWrB75vKwYpjwhfK_htg=="
org = "marcodsang@gmail.com"
url = "https://us-east-1-1.aws.cloud2.influxdata.com"
write_client = influxdb_client.InfluxDBClient(url=url, token=token, org=org)
write_api = write_client.write_api(write_options=SYNCHRONOUS)

# firestore access
cred = credentials.Certificate('C:\\Users\\sangs\\Desktop\\Terra\\Terra\\RPi_Py_Scripts\\serial_to_influx\\acc.json')
app = firebase_admin.initialize_app(cred)
db = firestore.client()

# get caresettings from firestore
doc_ref = db.collection("Subscriptions").document("CareSettings")


# getting data from arduino
if __name__ == '__main__':    
    #serial connection to RPi
    ser = serial.Serial('COM6', 9600, timeout=1)
    ser.reset_input_buffer()
    while True:
        if ser.in_waiting > 0:
            

            ###
            # step 2: upload sensor data from arduino to influxdb
            ###
            line = (ser.readline().decode('utf-8').rstrip())
            point = (
                Point("Terra_Embed")
                .tag("host", "SangDong")  
                .field("status", line)
                .time(datetime.utcnow(), WritePrecision.MS))                     
            write_api.write(bucket=bucket, org=org, record=point)   
            
            ###
            # step 1: download caresettings from firestore and send to arduino   
            ###         
            caresettings = doc_ref.get().to_dict()
            caresettings_json = json.dumps(caresettings)
            ser.write("haha".encode('utf-8'))
            print(line)
            print()
            
            
            


