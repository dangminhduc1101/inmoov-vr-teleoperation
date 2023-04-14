/* Libraries */
#include <Adafruit_PWMServoDriver.h>

/* Constants */
#define SERVO_FREQ 50
#define INPUT_MAX_LENGTH 50
#define SERVO_MAX_COUNT 16

#define LEFT_THUMB_PIN 0
#define LEFT_INDEX_PIN 1
#define LEFT_MIDDLE_PIN 2
#define LEFT_RING_PIN 3
#define LEFT_PINKY_PIN 4
#define RIGHT_THUMB_PIN 5
#define RIGHT_INDEX_PIN 6
#define RIGHT_MIDDLE_PIN 7
#define RIGHT_RING_PIN 8
#define RIGHT_PINKY_PIN 9
#define LEFT_PIVOT_PIN 10
#define LEFT_OMO_PIN 12
#define RIGHT_PIVOT_PIN 13
#define RIGHT_OMO_PIN 15


#define TP_MIN 750
#define TP_MAX 2250
#define RMI_MIN 500
#define RMI_MAX 2500
#define LEFT_PIVOT_MIN 600
#define LEFT_PIVOT_MAX 2400
#define RIGHT_PIVOT_MIN 1250
#define RIGHT_PIVOT_MAX 1750
#define LEFT_OMO_MIN 450
#define LEFT_OMO_MAX 1100
#define RIGHT_OMO_MIN 500
#define RIGHT_OMO_MAX 1250

/* Variables */
char s[INPUT_MAX_LENGTH];
bool ipt_read = false;

int lthumb, lindex, lmrp, lpivot, lomo;
int rthumb, rindex, rmrp, rpivot, romo;

Adafruit_PWMServoDriver pwm = Adafruit_PWMServoDriver();

/* Helper Functions */
void readInput() {
  static int i = 0;
  char c = Serial.read();

  if (c > 0 && ipt_read == false) {
    switch (c) {
      case '\r':
        break;
      case '\n':
        s[i] = '\0';
        i = 0;
        ipt_read = true;
        break;
      default:
        s[i] = c;
        i++;
    }
  }
}

const char* parseInput(const char* input, char delimiter, int index) {
  int idx_delimtr = 0;
  int idx_first = 0;
  int idx_last = -1;
  int ipt_length = strlen(input);

  for (int i = 0; i < ipt_length && idx_delimtr <= index; i++) {
    if (input[i] == delimiter || i == ipt_length - 1) {
      idx_delimtr++;
      idx_first = idx_last + 1;
      idx_last = (i == ipt_length - 1) ? i + 1 : i;
    }
  }  // find the indexes of the delimiter argument, and the first and last character of the string.

  if (idx_delimtr <= index) {
    return "N/A";
  }  // if index argument too high

  char* ipt_substr = malloc(idx_last - idx_first + 1);
  strncpy(ipt_substr, &input[idx_first], idx_last - idx_first);
  ipt_substr[idx_last - idx_first] = '\0';
  return ipt_substr;
}

int convertInput(const char* input, int index) {
  char* i = parseInput(input, '.', index);
  if (strcmp(i, "N/A") == 0) {
    return -2;
  }  // if arguument index too high, use for exception of inappropriate string length

  int ipt_converted = atoi(i);
  char str_parsed[INPUT_MAX_LENGTH];
  sprintf(str_parsed, "%d", ipt_converted);
  if (strcmp(i, str_parsed) != 0) {
    free(i);
    return -1;
  }  // exception handling for atoi()
  free(i);
  return ipt_converted;
}

int parseAngle(const char* input, int pos) {
  int p = convertInput(input, pos);
  return (p < 0 || p > 180) ? -1 : p;
}

bool excessInput(const char* input) {
  int p = convertInput(input, 10);
  return p != -2;
}

void updateOutput() {
  if (ipt_read == true) {
    lthumb = parseAngle(s, 0);
    lindex = parseAngle(s, 1);
    lmrp = parseAngle(s, 2);
    rthumb = parseAngle(s, 3);
    rindex = parseAngle(s, 4);
    rmrp = parseAngle(s, 5);
    lpivot = parseAngle(s, 6);
    lomo = parseAngle(s, 7);
    rpivot = parseAngle(s, 8);
    romo = parseAngle(s, 9);

    /*
    Serial.println(lthumb);
    Serial.println(lindex);
    Serial.println(lmrp);
    Serial.println(lpivot);
    Serial.println(lomo);
    Serial.println(rthumb);
    Serial.println(rindex);
    Serial.println(rmrp);
    Serial.println(rpivot);
    Serial.println(romo);        // use only for debugging, else put as comment, since Serial.println() adds unread strings into the buffer and slows down the parser
    */
    if (excessInput(s) || lthumb == -1 || lindex == -1 || lmrp == -1 || lpivot == -1 || lomo == -1
        || rthumb == -1 || rindex == -1 || rmrp == -1 || rpivot == -1 || romo == -1) {
      /*
        Serial.println("Invalid input");      // use only for debugging, else put as comment
        */
      for (int i = 0; i < SERVO_MAX_COUNT; i++) {
        pwm.setPWM(i, 0, 4096);
      }
    } else {
      pwm.setPWM(LEFT_THUMB_PIN, 0, int(float(map(lthumb, 0, 180, TP_MIN, TP_MAX)) / 1000000 * SERVO_FREQ * 4096));
      pwm.setPWM(LEFT_INDEX_PIN, 0, int(float(map(lindex, 0, 180, RMI_MIN, RMI_MAX)) / 1000000 * SERVO_FREQ * 4096));
      pwm.setPWM(LEFT_MIDDLE_PIN, 0, int(float(map(lmrp, 0, 180, RMI_MIN, RMI_MAX)) / 1000000 * SERVO_FREQ * 4096));
      pwm.setPWM(LEFT_RING_PIN, 0, int(float(map(lmrp, 0, 180, RMI_MIN, RMI_MAX)) / 1000000 * SERVO_FREQ * 4096));
      pwm.setPWM(LEFT_PINKY_PIN, 0, int(float(map(lmrp, 0, 180, TP_MIN, TP_MAX)) / 1000000 * SERVO_FREQ * 4096));
      pwm.setPWM(LEFT_PIVOT_PIN, 0, int(float(map(lpivot, 0, 180, LEFT_PIVOT_MIN, LEFT_PIVOT_MAX)) / 1000000 * SERVO_FREQ * 4096));
      pwm.setPWM(LEFT_OMO_PIN, 0, int(float(map(lomo, 0, 180, LEFT_OMO_MIN, LEFT_OMO_MAX)) / 1000000 * SERVO_FREQ * 4096));
      pwm.setPWM(RIGHT_THUMB_PIN, 0, int(float(map(rthumb, 0, 180, TP_MIN, TP_MAX)) / 1000000 * SERVO_FREQ * 4096));
      pwm.setPWM(RIGHT_INDEX_PIN, 0, int(float(map(rindex, 0, 180, RMI_MIN, RMI_MAX)) / 1000000 * SERVO_FREQ * 4096));
      pwm.setPWM(RIGHT_MIDDLE_PIN, 0, int(float(map(rmrp, 0, 180, RMI_MIN, RMI_MAX)) / 1000000 * SERVO_FREQ * 4096));
      pwm.setPWM(RIGHT_RING_PIN, 0, int(float(map(rmrp, 0, 180, RMI_MIN, RMI_MAX)) / 1000000 * SERVO_FREQ * 4096));
      pwm.setPWM(RIGHT_PINKY_PIN, 0, int(float(map(rmrp, 0, 180, TP_MIN, TP_MAX)) / 1000000 * SERVO_FREQ * 4096));
      pwm.setPWM(RIGHT_PIVOT_PIN, 0, int(float(map(rpivot, 0, 180, RIGHT_PIVOT_MIN, RIGHT_PIVOT_MAX)) / 1000000 * SERVO_FREQ * 4096));
      pwm.setPWM(RIGHT_OMO_PIN, 0, int(float(map(romo, 0, 180, RIGHT_OMO_MIN, RIGHT_OMO_MAX)) / 1000000 * SERVO_FREQ * 4096));
    }
    ipt_read = false;
  }
}

/* Main functions */
void setup() {
  Serial.begin(115200);

  pwm.begin();
  pwm.setPWMFreq(SERVO_FREQ);
}

void loop() {
  readInput();
  updateOutput();
}