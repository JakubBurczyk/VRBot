import RPi.GPIO as GPIO
import time

GPIO.setmode(GPIO.BCM)
GPIO.setwarnings(False)

GPIO.setup(17, GPIO.OUT, initial=GPIO.LOW)

for i in range(20):
    GPIO.output(17, GPIO.HIGH)
    time.sleep(0.1)
    GPIO.output(17, GPIO.LOW)
    time.sleep(0.1)
