// switch the ground pin with power pin connection
// pins
int photoresistorPin = A0;
int photoresistorVal = 0;

// the setup function runs once when you press reset or power the board
void setup() {
  Serial.begin(9600);
}

// the loop function runs over and over again forever
void loop() {
  photoresistorVal = analogRead(photoresistorPin);
  Serial.println(photoresistorVal, DEC);
  delay(1000);
}
