/*
  Serial Event example

  When new serial data arrives, this sketch adds it to a String.
  When a newline is received, the loop prints the string and clears it.

  A good test for this is to try it with a GPS receiver that sends out
  NMEA 0183 sentences.

  NOTE: The serialEvent() feature is not available on the Leonardo, Micro, or
  other ATmega32U4 based boards.

  created 9 May 2011
  by Tom Igoe

  This example code is in the public domain.

  http://www.arduino.cc/en/Tutorial/SerialEvent
*/
#include<EEPROM.h>
String inputString = "";         // a String to hold incoming data
bool stringComplete = false;  // whether the string is complete
int led13=0;  //Starup Value

void setup() {
  // initialize serial:
  Serial.begin(500000);
  // reserve 200 bytes for the inputString:
  inputString.reserve(200);
  pinMode(LED_BUILTIN, OUTPUT);
  
}

void WriteEEProm()
{
  EEPROM.write(0,114);
  EEPROM.write(1, led13);
}

void ReadEEProm()
{
  int evalue = EEPROM.read(0);
  if(evalue != 114){
   WriteEEProm(); 
  }
led13 = EEPROM.read(1);
  
  
}  
void loop() {
  ReadEEProm();
  digitalWrite(LED_BUILTIN, led13);
  
  // print the string when a newline arrives:
  if (stringComplete) {
    if(inputString.indexOf("START")!=-1){
      digitalWrite(LED_BUILTIN, led13);
      Serial.println(led13);
    }
    
//    Serial.println(inputString.indexOf("led13of"));
    if(inputString.indexOf("led13on")!=-1){
      digitalWrite(LED_BUILTIN, HIGH);
      
      led13=1;
      EEPROM.write(1, led13);
      Serial.println(led13);
    }

    if(inputString.indexOf("led13of")!=-1){
      digitalWrite(LED_BUILTIN, LOW);
      led13=0;
      EEPROM.write(1, led13);
      Serial.println(led13);
    }
    // clear the string:
    inputString = "";
    stringComplete = false;
  }
  
}

/*
  SerialEvent occurs whenever a new data comes in the hardware serial RX. This
  routine is run between each time loop() runs, so using delay inside loop can
  delay response. Multiple bytes of data may be available.
*/
void serialEvent() {
  while (Serial.available()) {
    // get the new byte:
    char inChar = (char)Serial.read();
    // add it to the inputString:
    inputString += inChar;
    // if the incoming character is a newline, set a flag so the main loop can
    // do something about it:
    if (inChar == '\n') {
      stringComplete = true;
    }
  }
}
