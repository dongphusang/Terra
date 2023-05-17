// 3-5V water pump. connect red wire to 3 or 5v, black wire to C slot of relay
// Relay. connect NO slot to 5V arduino


#define relayPin 2

int comdata = 0;

void setup() {
  pinMode(relayPin, OUTPUT);
  Serial.begin(9600);

}

void loop() {
  // put your main code here, to run repeatedly:
  digitalWrite(relayPin, HIGH);
  delay(500);
  digitalWrite(relayPin, LOW);
  delay(2000);
}
