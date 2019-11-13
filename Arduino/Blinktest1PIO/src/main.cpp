#include <Arduino.h>

#include <EEPROM.h>

#define COM0 0         // hardware Serial Port
#define COM1 1         // Software Serial Port
#define START_BYTE '[' // Start Byte for serial commands
#define END_BYTE ']'
#define PROCESS_PERIOD_uS 1000

long startTime ;                    // start time for stop watch
long elapsedTime ;

     
unsigned int CommsTimeout = 0; 

unsigned int RxByte[2] = {0};      // Current byte received from each of the two comm ports
int BufferEnd[2] = {-1};           // Rx Buffer end index for each of the two comm ports
unsigned int RxBuffer[5][2] = {0}; // 5 byte Rx Command Buffer for each of the two comm ports
byte errorcount = 0;
String inputString = ""; // a String to hold incoming data

int DeadZone1 = 65; //Starup Value
int DeadZone2 = 66;
unsigned long TimesUp;

void setup()
{
  // initialize serial:
  Serial.begin(19200);
  // reserve 200 bytes for the inputString:
  inputString.reserve(200);
  pinMode(LED_BUILTIN, OUTPUT);
  
  TimesUp = micros();
}

void SendTwoValues(int id, int v1, int v2, int ComPort)
{
    if (ComPort == 0)
    {
        Serial.write(START_BYTE);
        Serial.write(id);
        Serial.write(v1);
        Serial.write(v2);
        Serial.write(END_BYTE);
    }
}

void SendValue(int id, int value, int ComPort)
{
    int low, high;

    high = value / 256;
    low = value - (high * 256);

    if (ComPort == 0)
    {
        Serial.write(START_BYTE);
        Serial.write(id);
        Serial.write(high);
        Serial.write(low);
        Serial.write(END_BYTE);
    }
  }

void WriteEEProm()
{
  EEPROM.write(0, 116);
  EEPROM.write(1, DeadZone1);
  EEPROM.write(2, DeadZone2);
}

void ReadEEProm()
{
  int evalue = EEPROM.read(0);
  if (evalue != 116)
  {
    WriteEEProm();
    return;
  }
  DeadZone1 = EEPROM.read(1);
  DeadZone2 = EEPROM.read(2);
  
}

void ParseCommand(int ComPort)
{
  // print the string when a newline arrives:
  CommsTimeout = 0; 
  switch (RxBuffer[0][ComPort])
  { 
    case 'D':
      DeadZone1 = RxBuffer[1][ComPort];
      DeadZone2 = RxBuffer[2][ComPort];
      break;

    case 'r':
      if(RxBuffer[1][ComPort]=='d'){
        switch (RxBuffer[2][ComPort])
        {
        case 'D':
          SendTwoValues('D', DeadZone1, DeadZone2, ComPort);
          
          break;
        }
      }
    case 's':
      if(RxBuffer[1][ComPort]=='a' && RxBuffer[2][ComPort]=='v'){
        WriteEEProm();
        break;
      }

    


  }
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
  while(1==1){
    ReadEEProm();
    while ((micros() - TimesUp) < PROCESS_PERIOD_uS) { ; }
          TimesUp += PROCESS_PERIOD_uS;
    CheckSerial0();
    // delay(100);
    CommsTimeout++;
          if (CommsTimeout >= 60000)       // 15 second timeout ie 60000 * PROCESS_PERIOD_uS
          {
              CommsTimeout = 60000;        // So the counter doesn't overflow
          }

    //  elapsedTime =   micros() - startTime;   
    //   startTime = micros();
    //   Serial.println(elapsedTime);
      
  }
}