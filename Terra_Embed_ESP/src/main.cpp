#include <Arduino.h>
#include <ArduinoJson.h>
#include <DHT.h>
#include <math.h>
#include <WiFiMulti.h>
#include <InfluxDbClient.h>
#include <InfluxDbCloud.h>

#define WIFI_SSID "NaUOa"
#define WIFI_PASSWORD "hetthuocchua@60"
#define INFLUXDB_URL "https://us-east-1-1.aws.cloud2.influxdata.com"
#define INFLUXDB_TOKEN "gkSWc1Vzwfq8nr3vKRhklT_ebchZakq1rzF4dRpAgJdaBp1orj5G6kZVeflHZgzf2TdWrB75vKwYpjwhfK_htg=="
#define INFLUXDB_ORG "marcodsang@gmail.com"
#define INFLUXDB_BUCKET "Terra"
//#define TZ_INFO "UTC-4"

// WIFI
Point esp("ESP32");
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
StaticJsonDocument<care_json_capacity> care_config;
String result;

// function declaration
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
  WiFi.begin(WIFI_SSID, WIFI_PASSWORD);

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
  // Write point
  if (!client.writePoint(esp)) {
    Serial.print("InfluxDB write failed: ");
    Serial.println(client.getLastErrorMessage());
  }

  result.clear();

  // start_watering_auto
  if (measures["SoilMoisture"].as<int>() > 450) {
    // if water level is permitable
    if (measures["WaterLevel"].as<int>() > 10) {
      // true: check plant's watering setting
          // if plant does need water now?
            // true: activate relay which runs pump
            // false: go back to checking soil (or do nothing)
      // false: go back to checking soil
    }
      
  }
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

