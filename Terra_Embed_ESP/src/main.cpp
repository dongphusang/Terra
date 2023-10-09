#include <Arduino.h>
#include <ArduinoJson.h>
#include <DHT.h>
#include <math.h>

/*  define variables */
// KY-018 (photoresistor)
#define lightPin 34
int light_val = 0;


// function declaration
int read_soil_moist();
int read_water_level();
int read_light_val();
int read_tempC();
int read_humidity();
String read_last_watered();
String read_watering_freq();

void setup() {
  // start serial
  Serial.begin(115200);
  delay(1000);
}

void loop() {

      
  Serial.println(read_light_val());
  
  delay(1000);
}



// read and return value of photoresistor
int read_light_val() {
  return map(analogRead(lightPin), 0, 4095, 100, 0);
}

