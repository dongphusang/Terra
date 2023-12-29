import influxdb_client
from influxdb_client.client.write_api import SYNCHRONOUS
import json
from zoneinfo import ZoneInfo, TZPATH
from datetime import timedelta, datetime
        
class InfluxHelper():

    def __init__(self) -> None:
        # init access attrs
        self._token = ""
        self._org = ""
        self._host = ""
        with open('config.json', 'r') as data:
            config = json.load(data)
            self._token = config["InfluxDB"]["TOKEN"]
            self._org = config["InfluxDB"]["ORG"]
            self._host = config["InfluxDB"]["HOST"]
        # init influx client
        _client = influxdb_client.InfluxDBClient(
            url=self._host,
            token=self._token,
            org=self._org
        )
        self._query_api = _client.query_api()

        # construct query
        self._query = 'from(bucket:"Terra")\
        |> range(start: -24h)\
        |> filter(fn:(r) => r._measurement == "ESP32_1")\
        |> filter(fn:(r) => r._field == "sensors")'

    # get temperatures from InfluxDB and calculate average
    def get_temp_average(self) -> int:
        result = self._query_api.query(org=self._org, query=self._query)
        for table in result:
            count = 0
            sum = 0
            for record in table.records:
                count+=1
                #print("raw json: "+record.get_value())
                entry = json.loads(record.get_value())
                sum+=entry["Temperature"]
                #print(entry["Temperature"])
                #print("\n")

            return str(round(sum/count, 1))

    # get temperature points from InfluxDB within 24 hours
    def get_temps_of_day(self) -> list[list[int], list[int]]:
        result = self._query_api.query(org=self._org, query=self._query)
        for table in result:
            tuple = []
            temps = []
            time = []
            for record in table.records:
                entry = json.loads(record.get_value())
                entry_time = record.get_time().astimezone(ZoneInfo("US/Eastern")).strftime("%H")
                temp_elem = entry["Temperature"]
                temps.append(round(temp_elem, 1))
                time.append(str(entry_time))

            tuple.append(temps)
            tuple.append(time)

            return tuple

    # get humidity from InfluxDB and calculate average 
    def get_humid_average(self) -> int: 
        result = self._query_api.query(org=self._org, query=self._query)
        for table in result:
            count = 0
            sum = 0
            for record in table.records:
                count+=1
                #print("raw json: "+record.get_value())
                #print("Iteration #: " + str(count))
                entry = json.loads(record.get_value())
                sum+=entry["Humidity"]
                #print(entry["Humidity"])
                #print("\n")

            return str(round(sum/count, 1))

    # get humid points from influxDB within 24 hours
    def get_humids_of_day(self) -> list[list[object], list[object]]:
        result = self._query_api.query(org=self._org, query=self._query)
        for table in result:
            humids = []
            tuple = []
            time = []
            for record in table.records:
                entry = json.loads(record.get_value())
                entry_time = record.get_time().astimezone(ZoneInfo("US/Eastern")).strftime("%H")
                humid_elem = entry["Humidity"]
                humids.append(round(humid_elem, 1))
                time.append(str(entry_time))

            tuple.append(humids)
            tuple.append(time)

            return tuple

    # get light intensity by time from influxDB within 24 hours
    def get_lights_of_day(self) -> list[list[object], list[object]]:
        result = self._query_api.query(org=self._org, query=self._query)
        for table in result:
            tuple = []
            light = []
            time = []
            for record in table.records:
                entry = json.loads(record.get_value())
                entry_time = record.get_time().astimezone(ZoneInfo("US/Eastern")).strftime("%H")
                light_elem = entry["Light"]
                light.append(round(light_elem, 1))
                time.append(str(entry_time))

            tuple.append(light)
            tuple.append(time)

            return tuple
    
    # get the compute source that an email subscribes to
    def get_data_source(self) -> str:
        result = self._query_api.query(org=self._org, query=self._query)
        for table in result:
            for record in table.records:
                try:
                    return record.get_measurement()
                except NameError:
                    print("---record variable is empty---")
                    return "empty"
                
    # get report date
    def get_report_date(self) -> str:
        result = self._query_api.query(org=self._org, query=self._query)
        for table in result:
            for record in table.records:
                try:
                    # convert result time to local time
                    localizedTime = record.get_start().astimezone(ZoneInfo("US/Eastern"))
                    return localizedTime.strftime('%d %b %Y')
                except NameError:
                    print("---record variable is empty---")
                    return "empty"
                
    # get amount of time plant exposed to dark
    def get_time_darkexp(self) -> str:
        result = self._query_api.query(org=self._org, query=self._query)
        for table in result:
            anchor: datetime = table.records[0].get_time().astimezone(ZoneInfo("US/Eastern"))
            dark_duration = 0
            for record in table.records:
                entry = json.loads(record.get_value())
                if (entry["Light"] < 30) :
                    time_portion_of_effect = record.get_time().astimezone(ZoneInfo("US/Eastern")) - anchor
                    dark_duration += (time_portion_of_effect.total_seconds() / 2)
                    anchor = record.get_time().astimezone(ZoneInfo("US/Eastern"))       
            # calculate output (hour/minute/second)
            if (dark_duration < 60):
                # return seconds
                return str(round(dark_duration, 1)) + " seconds"
            elif (dark_duration > 60):
                # convert to minute
                dark_duration /= 60
                if (dark_duration > 60):
                    # convert to hour
                    dark_duration /= 60
                    # return hours
                    return str(round(dark_duration, 1)) + " hours"
                # return minutes
                return str(round(dark_duration, 1)) + " minutes"
    
    #get amount of time plant exposed to light
    def get_time_lightexp(self) -> str:
        result = self._query_api.query(org=self._org, query=self._query)
        for table in result:
            anchor: datetime = table.records[0].get_time().astimezone(ZoneInfo("US/Eastern"))
            light_duration = 0
            for record in table.records:
                entry = json.loads(record.get_value())
                if (entry["Light"] > 30) :
                    time_portion_of_effect = record.get_time().astimezone(ZoneInfo("US/Eastern")) - anchor
                    light_duration += (time_portion_of_effect.total_seconds() / 2)
                    anchor = record.get_time().astimezone(ZoneInfo("US/Eastern"))
            # calculate output (hour/minute/second)
            if (light_duration < 60):
                # return seconds
                return str(round(light_duration, 1)) + " seconds"
            elif (light_duration > 60):
                # convert to minute
                light_duration /= 60
                if (light_duration > 60):
                    # convert to hour
                    light_duration /= 60
                    # return hours
                    return str(round(light_duration, 1)) + " hours"
                # return minutes
                return str(round(light_duration, 1)) + " minutes"
                    
    
   


