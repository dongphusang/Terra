/* Libraries */
#include <ArduinoJson.h>
#include <DHT.h>
#include <math.h>

/* Define variables */
// relay, water level sensor
#define relayPin 4
#define waterLvlPower 5
#define waterLvlPin A0
int water_level_val = 0;

// KY-018 (photoresistor)
#define lightPin A5
int light_val = 0;

// DHT22 (temp & humid)
#define dhtPin 3
#define Type DHT22
DHT HT(dhtPin, Type);

// json data
const int capacity = JSON_OBJECT_SIZE(5);
StaticJsonDocument<capacity> measurements;

void setup() {
  /* setups */
  // relay, water level sensor
  pinMode(relayPin, OUTPUT);
  pinMode(waterLvlPower, OUTPUT);
  digitalWrite(waterLvlPower, LOW);

  // light

  // DHT22
  HT.begin();
  /* setups end */

  Serial.begin(9600);
  delay(1000); // setup delay
}

void loop() {
  // create json for sending to Raspberry Pi
  JsonObject obj = measurements.to<JsonObject>();
  obj["SoilMoisture"] = 0;
  obj["Temperature"] = read_tempC();
  obj["Humidity"] = read_humidity();
  obj["WaterLevel"] = read_water_level();
  obj["Light"] = read_light_val();

  // send off data to RPi
  serializeJson(measurements, Serial);
  delay(1500);
}

int read_water_level() {
  digitalWrite(waterLvlPower, HIGH);
  water_level_val = analogRead(waterLvlPin);
  digitalWrite(waterLvlPower, LOW);

  return map(water_level_val, 0, 166, 0 , 100);
}

int read_light_val() {
  return analogRead(lightPin);
}

int read_tempC() {
  return HT.readTemperature();
}

int read_humidity() {
  return HT.readHumidity();
}
