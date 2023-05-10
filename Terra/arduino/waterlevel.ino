// sensor pins
#define sensorPower 7
#define sensorPin A0

// water level val
int val = 0;


void setup() {
  pinMode(sensorPower, OUTPUT);
  digitalWrite(sensorPower, LOW);
  Serial.begin(9600);
}

void loop() {
  // get reading
  int level = read_water_level();
  // eval reading
  if(level <= 157 && level > 145){
    Serial.println("Water level: 100%");  // water level at near 100%
  } else if(level <= 145 && level > 130) {
    Serial.println("Water level: 50%");   // water level at near 50%
  } else if(level <= 130 && level > 110) {
    Serial.println("Water level: 10%");   // water level below 10%
  } else {
    Serial.println("Water level: depleted");  // needs to be refilled
  }
  // delay 1 sec before reading again
  delay(2000);
}

// read input from water level sensor
int read_water_level() {
  digitalWrite(sensorPower, HIGH); // enable sensor
  val = analogRead(sensorPin);      // read analog value
  digitalWrite(sensorPower, LOW);   // disable sensor
  return val;
}
