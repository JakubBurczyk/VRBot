# MQTT Publish Demo
# Publish two messages, to two different topics

import paho.mqtt.publish as publish

publish.single("test", "test message", hostname="25.98.111.255")
publish.single("error", "error message", hostname="25.98.111.255")
print("Done")