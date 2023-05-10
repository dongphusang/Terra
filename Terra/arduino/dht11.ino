//delay read to 2000 since this sensor doesn't change value frequently

#include <DHT.h>
#define Type DHT11

int sensePin = 2;
DHT HT(sensePin, Type);
float humidity;
float tempC;
float tempF;

void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);
  HT.begin();
  delay(1000);
}

void loop() {
  // put your main code here, to run repeatedly:
  humidity = HT.readHumidity();
  Serial.print("Humidity: ");
  Serial.println(humidity);
  delay(1000);
}
