#include <Arduino.h>
#include <ArduinoJson.h>
#include <DHT.h>
#include <math.h>

/*  define variables */
// relay, water level sensor
#define relayPin 4
#define waterLvlPower 5
#define waterLvlPin A0
int water_level_val = 0;
// KY-018 (photoresistor)
#define lightPin A5
int light_val = 0;
// DHT22 (temp&humid)
#define dhtPin 3
#define Type DHT22
DHT HT(dhtPin, Type);
// Soil Moisture
#define sMoistPin A4
int soil_moist_val = 0;

/* json data */
const int capacity = JSON_OBJECT_SIZE(5);
StaticJsonDocument<capacity> measures;

// function declaration
int read_soil_moist();
int read_water_level();
int read_light_val();
int read_tempC();
int read_humidity();

void setup() {
  // relay, water level sensor
  pinMode(relayPin, OUTPUT);
  pinMode(waterLvlPin, OUTPUT);
  digitalWrite(waterLvlPower, LOW);
  // DHT 22
  HT.begin();

  // start serial
  Serial.begin(9600);
  delay(1000);
}

void loop() {
  // create and init json data for payload to raspberry pi
  JsonObject obj = measures.to<JsonObject>();
  obj["SoilMoisture"] = read_soil_moist();
  obj["Temperature"] = read_tempC();
  obj["Humidity"] = read_humidity();
  obj["WaterLevel"] = read_water_level();
  obj["Light"] = read_light_val();

  // send off data to RPi
  serializeJson(measures, Serial);

  // start_watering_auto
  if (measures["SoilMoisture"].as<int>() > 450) {
    // if water level is permitable
    if (measures["WaterLevel"].as<int>() > 10) {
      // true: check plant's watering setting
        // retrieve dictionary from RPi that contains the information
          // if plant does need water now?
            // true: activate relay which runs pump
            // false: go back to checking soil (or do nothing)
      // false: go back to checking soil
    }
      
  }
  

  delay(1500);
}

// read and return value of soil moisture sensor
int read_soil_moist() {
  return analogRead(sMoistPin);
}

// read and return value of water level sensor
int read_water_level() {
  digitalWrite(waterLvlPower, HIGH);
  water_level_val = analogRead(waterLvlPin);
  digitalWrite(waterLvlPower, LOW);
  
  return map(water_level_val, 0, 166, 0, 100);
}

// read and return value of photoresistor
int read_light_val() {
  return analogRead(lightPin);
}

// read and return temperature of DHT22
int read_tempC() {
  return HT.readTemperature();
}

// read and return humidity of DHT22
int read_humidity() {
  return HT.readHumidity();
}