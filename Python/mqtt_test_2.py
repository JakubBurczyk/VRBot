import paho.mqtt.publish as publish

publish.single("test/debug", "Hello World!", hostname="25.98.111.255")