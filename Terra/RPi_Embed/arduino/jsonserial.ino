#include <ArduinoJson.h>

const int capacity = JSON_OBJECT_SIZE(5);
StaticJsonDocument<capacity> measurements;


void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);
}

void loop() {
  // put your main code here, to run repeatedly:
  JsonObject obj = measurements.to<JsonObject>();
  obj["soil_moisture"] = 1;
  obj["temperature"] = 2;
  obj["humidity"] = 3;
  obj["water_level"] = 4;
  obj["light"] = 5;

  serializeJson(measurements, Serial);
  delay(2000);
}
