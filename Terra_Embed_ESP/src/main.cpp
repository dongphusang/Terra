#include <Arduino.h>
#include <ArduinoJson.h>
#include <DHT.h>
#include <math.h>
#include <WiFiMulti.h>
#include <InfluxDbClient.h>
#include <InfluxDbCloud.h>
#include "secrets.hpp"
#include <Firebase_ESP_Client.h>
#include <chrono>
#include <unordered_map>
#include <string>
#include <time.h>

// date hashmap
enum wday { SUNDAY, MONDAY, TUESDAY, WEDNESDAY, THURSDAY, FRIDAY, SATURDAY };
std::unordered_map<std::string, wday> const week_days = {
    {"Sunday", SUNDAY},
    {"Monday", MONDAY},
    {"Tuesday", TUESDAY},
    {"Wednesday", WEDNESDAY},
    {"Thursday", THURSDAY},
    {"Friday", FRIDAY},
    {"Saturday", SATURDAY} };


// WIFI
Point esp("ESP32_1");
InfluxDBClient client(INFLUXDB_URL, INFLUXDB_ORG, INFLUXDB_BUCKET, INFLUXDB_TOKEN, InfluxDbCloud2CACert);

// NTP
const char* ntpServer = "pool.ntp.org";
const long gmtOffset_sec = -18000;
const int daylightOffset_sec = 3600;

/*  define variables */
// relay, water level sensor
#define relayPin 32      // blue
#define waterLvlPin 33   // green
int water_level_val = 0;
// KY-018 (photoresistor)
#define lightPin 34      // yellow
int light_val = 0;
// DHT22 (temp&humid)
#define dhtPin 14        // grey
#define Type DHT22
DHT HT(dhtPin, Type);
// Soil Moisture
#define sMoistPin 35     // brown
int soil_moist_val = 0;
// button (test)
#define buttonPin 23
int buttonStat;

/* json data */
const int sensor_json_capacity = JSON_OBJECT_SIZE(6);
const int care_json_capacity = JSON_OBJECT_SIZE(5);
StaticJsonDocument<sensor_json_capacity> measures;
DynamicJsonDocument doc(512);
String result;

/* FIRESTORE */
FirebaseData fbdo;
FirebaseAuth auth;
FirebaseConfig config;
std::vector<std::string> schedules; // array to contain individual String typed schedules from json
std::string path;
std::string mask;

// FUNCTION DECLARATION
int read_soil_moist();
int read_water_level();
int read_light_val();
int read_tempC();
int read_humidity();
int calculate_schedule_hash(std::string schedule);
int get_index(std::vector<std::string>&input, std::string searched);
String read_last_watered();
String read_watering_freq();

void setup() {
  // relay, water level sensor
  pinMode(relayPin, OUTPUT);
  pinMode(waterLvlPin, INPUT);

  // DHT22
  HT.begin();

  // setup wifi
  WiFi.begin(SSID, PASSWORD);

  // firebase
  config.api_key = API_KEY;
  auth.user.email = USER_EMAIL;
  auth.user.password = USER_PASSWORD;
  Firebase.reconnectNetwork(true);
  fbdo.setBSSLBufferSize(4096, 1024);
  fbdo.setResponseSize(2048);
  path = "Subscriptions/ESP32_1";

  Firebase.begin(&config, &auth);

  // setup time
  configTime(gmtOffset_sec, daylightOffset_sec, ntpServer);

  // start serial
  Serial.begin(9600);
  delay(1000);
}

void loop() {    
  // create and init sensor data. Exporting payload to InfluxDB Cloud
  JsonObject sensor_json_object = measures.to<JsonObject>();
  sensor_json_object["SoilMoisture"] = read_soil_moist();
  sensor_json_object["Temperature"] = read_tempC();
  sensor_json_object["Humidity"] = read_humidity();
  sensor_json_object["WaterLevel"] = read_water_level();
  sensor_json_object["Light"] = read_light_val();
  
  // serialize to json string
  serializeJson(measures, result);

  // clear fields for reusing point
  esp.clearFields();
  // Store measured value into point
  // Add json string to field
  esp.addField("sensors", result);
  // Write point to Influx
  if (!client.writePoint(esp)) {
    Serial.print("InfluxDB write failed: ");
    Serial.println(client.getLastErrorMessage());
  }

  result.clear();

  // note: to get value from json here use measures["SoilMoisture"].as<int>()
  /* Watering Module */
  if (WiFi.status() == WL_CONNECTED && Firebase.ready()) {
    FirebaseJson content;     // firestore json
    FirebaseJsonArray array;  // array to contain individual FirebaseJsonArray typed schedules
    FirebaseJsonData jsonData;// contains jsondata from firestore json
    mask = "WateringModule";
    int operating_mode;
    /* retrieve operating mode from firestore */
    Firebase.Firestore.getDocument(&fbdo, FIRESTORE_ID, "", path.c_str());
    content.setJsonData(fbdo.payload().c_str());
    content.get(jsonData, "fields/WateringModule/booleanValue");
    operating_mode = jsonData.boolValue;

    // scheduled mode
    if (operating_mode == 1) {
      // retrieve data from firestore path
      mask = "Schedule";
      //Firebase.Firestore.getDocument(&fbdo, FIRESTORE_ID, "", path.c_str(), mask.c_str());
      // parse retrieved data
      schedules.clear();
      content.setJsonData(fbdo.payload().c_str());
      content.get(jsonData, "fields/Schedule/arrayValue/values", true);
      jsonData.get<FirebaseJsonArray>(array);

      // iterate through array to get all schedules 
      for (size_t i = 0; i < array.size(); i++){
        content.get(jsonData, "fields/Schedule/arrayValue/values/["+std::to_string(i)+"]/stringValue", true);
        schedules.push_back(jsonData.to<std::string>());
      }

      /* get current time GMT -5 */
      tm current_time;
      if (!getLocalTime(&current_time)) {
        Serial.println("unable to retrieve time");
      }

      /* iterate schedule to check their due */
      for (std::string schedule : schedules) {
        // calculate hash of current time and scheduled time by converting them to seconds
        int current_time_hash = 24*current_time.tm_wday*60*60 + current_time.tm_hour*60*60 + current_time.tm_min*60;
        int schedule_hash = calculate_schedule_hash(schedule);
        Serial.print("current: ");
        Serial.println(current_time_hash);
        Serial.print("scheduled: ");
        Serial.println(schedule_hash);

        // check if it's time to water
        if (current_time_hash == schedule_hash) {

          /// retrieve last watered information
          mask = "LastWatered";
          content.clear();
          //Firebase.Firestore.getDocument(&fbdo, FIRESTORE_ID, "", path.c_str(), mask.c_str());
          content.setJsonData(fbdo.payload().c_str());
          content.get(jsonData, "fields/LastWatered/stringValue");
          std::string last_schedule = jsonData.stringValue.c_str();
          std::string current_schedule = schedules[get_index(schedules, schedule)];
          current_schedule.erase(std::remove(current_schedule.begin(), current_schedule.end(), '.'), current_schedule.end());
          Serial.print("Last Schedule: ");
          Serial.println(last_schedule.c_str());
          Serial.print("Current Schedule: ");
          Serial.println(current_schedule.c_str());
          
          /// check if last_watered time hasn't been set to current time, which means plant isn't watered
          if (last_schedule.compare(current_schedule) != 0) {
            // if water soil is above 1200 (dry), don't save current time and continue to water
            //while (read_soil_moist() > 1200) {
              digitalWrite(relayPin, HIGH);
              delay(3000);
              digitalWrite(relayPin, LOW);         
              delay(5000);   
            //}
            // save current time as last_watered
            mask = "LastWatered";
            char last_watered[80];
            strftime(last_watered, 80, "%A, %I:%M %P", &current_time);
            content.clear();
            content.set("fields/LastWatered/stringValue", std::string(last_watered).c_str());
            if (Firebase.Firestore.patchDocument(&fbdo, FIRESTORE_ID, "", path.c_str(), content.raw(), mask.c_str()))
              Serial.printf("ok\n%s\n\n", fbdo.payload().c_str());
            else
              Serial.println(fbdo.errorReason());

            // update next schedule (assume schedules is ordered by day of week)
            int schedule_index = get_index(schedules, schedule);
            std::string next_schedule;
            Serial.print("Index: " + schedule_index);
            mask = "NextWateringSchedule";
            content.clear();
            // check if current schedule exists
            if (schedule_index != 0) {
              // get next_schedule (schedule at next index)
              if (schedule_index == schedules.size() - 1) // if current schedule is at the end, grab the first element
                next_schedule = schedules[0];
              else // if current schedule isn't at the end, grab the next element
                next_schedule = schedules[schedule_index + 1]; 
              // include next_schedule in content
              content.set("fields/NextWateringSchedule/stringValue", std::string(next_schedule).c_str());
              // upload content to firestore
              if (Firebase.Firestore.patchDocument(&fbdo, FIRESTORE_ID, "", path.c_str(), content.raw(), mask.c_str()))
                Serial.printf("ok\n%s\n\n", fbdo.payload().c_str());
              else
                Serial.println(fbdo.errorReason());
            }
          } else {
            Serial.print("THE SAME");
          }
        }
        else {
          Serial.println("not time yet");
        }
      }
    }
    else // auto mode
    { }

    delay(1000);
  }
  else {
    Serial.print("WiFi or Firebase not read: ");
    Serial.println(fbdo.errorReason());
  }
}

// read and return value of photoresistor
int read_light_val() {
  return map(analogRead(lightPin), 0, 4095, 100, 0);
}

// read and return temperature of DHT22
int read_tempC() {
  int result = HT.readTemperature();
  if (result > 100) 
    return 0;
  return result;
}

// read and return humidity of DHT22
int read_humidity() {
  int result = HT.readHumidity();
  if (result > 100)
    return 0;
  return result;
}

// read and return value of soil moisture sensor
int read_soil_moist() {
  return map(analogRead(sMoistPin), 0, 4095, 0, 2001);
}

// read and return value of water level sensor
int read_water_level() {
  return map(analogRead(waterLvlPin), 0, 4095, 0, 100);
}

int calculate_schedule_hash(std::string schedule) {
  int index_start;
  int length;
  std::string day_of_week;
  std::string hour;
  std::string minute;

  /* parse date elements for hash calculation */
  // parse day of week
  index_start = 0, length = schedule.find_first_of(',');
  day_of_week = schedule.substr(index_start, length);
  // parse hour
  index_start = schedule.find_first_of(' ')+1, length = schedule.find_first_of(":") - index_start;
  hour = schedule.substr(index_start, length);
  // parse minute
  index_start = schedule.find_first_of(":") + 1, length = 2;
  minute = schedule.substr(index_start, length);

  Serial.println("---------");
  Serial.println(schedule.c_str());
  Serial.println(24*week_days.at(day_of_week)*60*60);
  Serial.println(day_of_week.c_str());
  Serial.println(stoi(hour)*60*60);
  Serial.println(hour.c_str());
  Serial.println(stoi(minute)*60);
  Serial.println(minute.c_str());
  Serial.println("---------");

  /* calculate hash */
  int hash = 24*week_days.at(day_of_week)*60*60 + stoi(hour)*60*60 + stoi(minute)*60;
  // add worth of 12 hours in seconds to current hash if schedule has p.m., remove same amount if a.m.
  if (schedule.find("p.m.") != std::string::npos) 
    hash += 43200;  
  else if (schedule.find("a.m.") != std::string::npos && hour.compare("12") == 0)
    hash -= 43200;

  return hash;
}

  /* return index of target in a vector */
int get_index(std::vector<std::string>&input, std::string searched) {
  for (int i = 0; i < input.size(); i++) {
      if (input[i] == searched) {
          return i;
      }
  }

  return 0;
}

