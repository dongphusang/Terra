#include <Arduino.h>
#include <ArduinoJson.h>
#include <DHT.h>
#include <math.h>
#include <WiFiMulti.h>
#include <InfluxDbClient.h>
#include <InfluxDbCloud.h>
#include "secrets.hpp"
#include <Firebase_ESP_Client.h>

// WIFI
Point esp("ESP32_1");
InfluxDBClient client(INFLUXDB_URL, INFLUXDB_ORG, INFLUXDB_BUCKET, INFLUXDB_TOKEN, InfluxDbCloud2CACert);

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
std::vector<String> list; // array to contain individual String typed schedules from json

// FUNCTION DECLARATION
int read_soil_moist();
int read_water_level();
int read_light_val();
int read_tempC();
int read_humidity();
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

  Firebase.begin(&config, &auth);

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
  /* Download watering module mode */
  if (WiFi.status() == WL_CONNECTED && Firebase.ready()) {
    /* retrieve watering schedules from firestore */
    String path = "Subscriptions/Schedule";
    String mask = "ESP32_1";
    Firebase.Firestore.getDocument(&fbdo, FIRESTORE_ID, "", path.c_str(), mask.c_str());
    /* parse firestore content */
    FirebaseJson content;     // firestore json
    FirebaseJsonArray array;  // array to contain individual FirebaseJsonArray typed schedules
    FirebaseJsonData jsonData;// contains jsondata from firestore json
    list.clear();
    content.setJsonData(fbdo.payload().c_str());
    content.get(jsonData, "fields/ESP32_1/arrayValue/values", true);
    /* parse firestore json to array to check for number of schedules */
    jsonData.get<FirebaseJsonArray>(array);
    /* based on result array, we have the size to iterate and get all schedules */
    for (size_t i = 0; i < array.size(); i++){
      content.get(jsonData, "fields/ESP32_1/arrayValue/values/["+std::to_string(i)+"]/stringValue", true);
      list.push_back(jsonData.to<String>());
    }
    for (size_t i = 0; i < list.size(); i++){
      Serial.print("index: ");
      Serial.println(i);
      Serial.print("value: ");
      Serial.println(list[i]);
    }
  }
  else
    Serial.print("hey bitch: ");
    Serial.println(fbdo.errorReason());
  // activate water module based on care settings
  
  delay(1000);
}

// read and return value of photoresistor
int read_light_val() {
  return map(analogRead(lightPin), 0, 4095, 100, 0);
}

// read and return temperature of DHT22
int read_tempC() {
  return HT.readTemperature();
}

// read and return humidity of DHT22
int read_humidity() {
  return HT.readHumidity();
}

// read and return value of soil moisture sensor
int read_soil_moist() {
  return map(analogRead(sMoistPin), 0, 4095, 0, 2001);
}

// read and return value of water level sensor
int read_water_level() {
  return map(analogRead(waterLvlPin), 0, 4095, 0, 100);
}

