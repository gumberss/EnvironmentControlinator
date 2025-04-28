#include <Servo.h>
#define SERVO 8
Servo broom;


int redCommand = 0;
int greenCommand = 0;
int blueCommand = 0;
bool inCommand = false;

int r = 0;
int g = 0;
int b = 0;

typedef struct {
  int pinR;
  int pinG;
  int pinB;
} Led;

Led leds[2];
int ledsCount;

String input = "";
int currentCommand = 3;
int lastCollorCommand = currentCommand; 

int broomPos = 0;
void setup() {
  broom.attach(SERVO);
  Serial.begin(9600);

  //top left
  leds[0].pinR = 2;
  leds[0].pinG = 3;
  leds[0].pinB = 4;

  //top right
  leds[1].pinR = 5;
  leds[1].pinG = 6;
  leds[1].pinB = 7;

  ledsCount = 2;//Definir quantidade de leds
  
   for(int i = 0; i < ledsCount; i++){
    pinMode(leds[i].pinR, OUTPUT);
    pinMode(leds[i].pinG, OUTPUT);
    pinMode(leds[i].pinB, OUTPUT);
   }

    pinMode(13, OUTPUT);

    digitalWrite(13, HIGH);
   

  while(broomPos < 90){
    broom.write(++broomPos);  
    delay(20);
    
  }
  
  
}

bool inShot = false;
bool walking = false;

bool isBroomDown = false;
bool broomInExecution = false;

void loop() { 

  if(Serial.available() > 0){
    Serial.println("reading");
    int valueRead = Serial.read() - '0';
    if(valueRead != -35)
      currentCommand = valueRead;  
    
    
  }
  if (currentCommand == 9){
    //broom.write(90);
    //delay(300);
    //broom.write(0);
    //delay(30);
    isBroomDown = true;
    broomInExecution = true;
  }
  if(currentCommand ==  4){
    inShot = true;
  } else if(currentCommand ==  5){
    inShot = false;
  } else if(currentCommand == 6){
    walking = true;
  } else if(currentCommand == 7){
    walking = false;
  }

  if(broomInExecution){
    if(broomPos == 0){
      isBroomDown = false;
    }

    if(isBroomDown){
      broomPos--;
    }else{
      broomPos++;
    }

    broom.write(broomPos);
    
    delay(8);
    if(broomPos == 90) broomInExecution = false;

     Serial.print(broomInExecution);
     Serial.print(broomPos);
     Serial.println(isBroomDown);
  }
  
  if(inShot){
    //setColorNow(0, 0, 0);    

    //delay(10);

    if(currentCommand != 4)
      lastCollorCommand = currentCommand;
      
    setColorNow(255, 255, 255);
    delay(100);

    setColorNow(100, 255, 255);
    delay(100);
  }
  else if(walking){
    
    setColor(255, 110, 0);
  }
  else if (currentCommand == 8){

    for(int i = 0; i < 5; i++){
        setColorNow(110, 255, 110);
        delay(100);
    
        setColorNow(255, 255, 255);
        delay(100);
    }

    currentCommand = lastCollorCommand;
  }else if (currentCommand == 9){
    //broom.write(90);
    //delay(300);
    //broom.write(0);
    //delay(30);
    isBroomDown = true;
    broomInExecution = true;
  
    if(broomPos == 90)
      isBroomDown = false;

     currentCommand = lastCollorCommand;
   
   
    //
    Serial.println("read");
  }
  else 
  { 
    if(currentCommand == 5 || currentCommand == 7) 
      currentCommand =  lastCollorCommand;
    
    if(currentCommand == 0){//danger
      Serial.println("red");
      setColor(255, 0, 0);    // red 
      lastCollorCommand = 0;
    }else if(currentCommand == 1){//warning
      Serial.println("orange");
      setColor(255, 110, 0);    // orange
      lastCollorCommand = 1;
    }else if(currentCommand == 2){ // ok
      Serial.println("green");
      setColor(0, 255, 0);    // green
      lastCollorCommand = 2;
    }else if(currentCommand == 3){
      rainbow();
      lastCollorCommand = 3;
    } 
  }

    delay(5);
}
int currentRainbow = 0;
void rainbow(){
  int oldRed = r, oldGreen = g, oldBlue = b;
  
  if(currentRainbow == 0){
    setColor(255, 0, 0);    // red
    if(oldRed == r && oldGreen == g && oldBlue == b) currentRainbow = 1;
  }
  if(currentRainbow == 1){
    setColor(0, 0, 255);    // blue
    if(oldRed == r && oldGreen == g && oldBlue == b) currentRainbow = 2;
  }
  if(currentRainbow == 2){
    setColor(255, 255, 0);  // yellow
    if(oldRed == r && oldGreen == g && oldBlue == b) currentRainbow = 3;
  }
  if(currentRainbow == 3){
    setColor(80, 0, 80);    // purple
    if(oldRed == r && oldGreen == g && oldBlue == b) currentRainbow = 4;
  }
  if(currentRainbow == 4){
    setColor(0, 255, 255);  // aqua
    if(oldRed == r && oldGreen == g && oldBlue == b) currentRainbow = 5;
  }
  if(currentRainbow == 5){
    setColor(0, 255, 0);  // aqua
    if(oldRed == r && oldGreen == g && oldBlue == b) currentRainbow = 0;
  }
}

void setColor(int red, int green, int blue) {
    red = 255 - red;
    green = 255 - green;
    blue = 255 - blue;
    
    if ( r < red ) r += 1;
    if ( r > red ) r -= 1;

    if ( g < green ) g += 1;
    if ( g > green ) g -= 1;

    if ( b < blue ) b += 1;
    if ( b > blue ) b -= 1;

    _setColor();
}

void setColorNow(int red, int green, int blue){
    for(int i = 0; i< ledsCount; i++){
    analogWrite(leds[i].pinR, red);
    analogWrite(leds[i].pinG, green);
    analogWrite(leds[i].pinB, blue);   
  }
}

void _setColor() {

  for(int i = 0; i< ledsCount; i++){
    setColorNow(r,g,b);
  }
}