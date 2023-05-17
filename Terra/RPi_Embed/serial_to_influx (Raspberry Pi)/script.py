
import serial
from datetime import datetime
import influxdb_client, os, time
from influxdb_client import InfluxDBClient, Point, WritePrecision
from influxdb_client.client.write_api import SYNCHRONOUS

# influx db access
bucket = "Terra" 
token = "gkSWc1Vzwfq8nr3vKRhklT_ebchZakq1rzF4dRpAgJdaBp1orj5G6kZVeflHZgzf2TdWrB75vKwYpjwhfK_htg=="
org = "marcodsang@gmail.com"
url = "https://us-east-1-1.aws.cloud2.influxdata.com"
write_client = influxdb_client.InfluxDBClient(url=url, token=token, org=org)
write_api = write_client.write_api(write_options=SYNCHRONOUS)

# getting data from arduino
if __name__ == '__main__':    
    # serial connection to RPi
    ser = serial.Serial('/dev/ttyACM0', 9600, timeout=1)
    ser.reset_input_buffer()
    while True:
        if ser.in_waiting > 0:
            line = (ser.readline().decode('utf-8').rstrip())

            point = (
                Point("Terra_Embed")
                .tag("host", "SangDong")  
                .field("status", line)
                .time(datetime.utcnow(), WritePrecision.MS))
            
            
            write_api.write(bucket=bucket, org=org, record=point)
            time.sleep(1)
            print(line)
            print()