#include <Arduino.h>

#include <EEPROM.h>

#define COM0 0         // hardware Serial Port
#define COM1 1         // Software Serial Port
#define START_BYTE '[' // Start Byte for serial commands
#define END_BYTE ']'

unsigned int RxByte[2] = {0};      // Current byte received from each of the two comm ports
int BufferEnd[2] = {-1};           // Rx Buffer end index for each of the two comm ports
unsigned int RxBuffer[5][2] = {0}; // 5 byte Rx Command Buffer for each of the two comm ports
byte errorcount =0;
String inputString = ""; // a String to hold incoming data

int led13 = 0; //Starup Value

void setup()
{
  // initialize serial:
  Serial.begin(500000);
  // reserve 200 bytes for the inputString:
  inputString.reserve(200);
  pinMode(LED_BUILTIN, OUTPUT);
}

void WriteEEProm()
{
  EEPROM.write(0, 114);
  EEPROM.write(1, led13);
}

void ReadEEProm()
{
  int evalue = EEPROM.read(0);
  if (evalue != 114)
  {
    WriteEEProm();
  }
  led13 = EEPROM.read(1);
}

void ParseCommand(int comPort)
{
  // print the string when a newline arrives:

  switch (RxBuffer[0] [comPort])
  {
    
  }
  // if (stringComplete)
  // {
  //   if (inputString.indexOf("START") != -1)
  //   {
  //     digitalWrite(LED_BUILTIN, led13);
  //     Serial.println(led13);
  //   }

  //   if (inputString.indexOf("led13on") != -1)
  //   {
  //     digitalWrite(LED_BUILTIN, HIGH);

  //     led13 = 1;
  //     EEPROM.write(1, led13);
  //     Serial.println(led13);
  //   }

  //   if (inputString.indexOf("led13of") != -1)
  //   {
  //     digitalWrite(LED_BUILTIN, LOW);
  //     led13 = 0;
  //     EEPROM.write(1, led13);
  //     Serial.println(led13);
  //   }
  //   // clear the string:
  // }
}

void CheckSerial0()
{
  while (Serial.available())
  {
    if (BufferEnd[COM0] == -1)
    {
      RxByte[COM0] = Serial.read();
      if (RxByte[COM0] != START_BYTE)
      {
        BufferEnd[COM0] = -1;
        errorcount++;
      }
      else
      {
        BufferEnd[COM0] = 0;
      }
    }
    else
    {
      RxByte[COM0] = Serial.read();
      RxBuffer[BufferEnd[COM0]][COM0] = RxByte[COM0];
      BufferEnd[COM0]++;
      if (BufferEnd[COM0] > 3)
      {
        if (RxBuffer[3][COM0] == END_BYTE)
        {
          ParseCommand(COM0);
        }
        else
        {
          errorcount++;
        }
        BufferEnd[COM0] = -1;
      }
    }
  }
}

void loop()
{
  ReadEEProm();
  digitalWrite(LED_BUILTIN, led13);

  CheckSerial0();
}

/*
  SerialEvent occurs whenever a new data comes in the hardware serial RX. This
  routine is run between each time loop() runs, so using delay inside loop can
  delay response. Multiple bytes of data may be available.
*/
