import paho.mqtt.client as mqtt
import paho.mqtt.publish as publish
import time

class MQTTclient:

    def __init__(self, broker_address,topics = [], port:int = 1883,qos = 0):
        self.topics = topics
        self.client = mqtt.Client()
        self.client.on_connect = self.on_connect
        self.client.on_message = self.on_message
        self.client.on_disconnect = self.on_disconnect
        self.client.publish_multiple = self.publish_multiple

        self.client.will_set("error","pi_lost_connection",qos=2)
        self.client.connect_async(broker_address, 1883, 60)
        self.client.loop_start()

    def __del__(self):
        self.stop()
        self.disconnect()

    def on_connect(self,client, userdata, flags, rc):
        print("MQTT connection status: " + str(rc))
        if rc is not 0:
            self.EXTERNAL_FUNCTION_MOCK_on_failed_connection()
        else:
            for topic in self.topics:
                self.client.subscribe(topic)

    def on_message(self,client, userdata, msg):
        msg.payload = msg.payload.decode("utf-8")
        self.EXTERNAL_FUNCTION_MOCK_msg_interpreter(msg.topic,msg.payload)

    def on_disconnect(self):
        pass
    
    def publish_multiple(self, topics, msgs, broker_address):
        merged_list = [(topics[i], msgs[i]) for i in range(0, len(topics))]
        publish.multiple(merged_list, broker_address, 1883, keepalive=60)

    def start(self):
        self.client.loop_start()

    def stop(self):
        self.client.loop_stop()

    def reconnect(self):
        self.client.reconnect()

    def disconnect(self):
        self.client.disconnect()

    def EXTERNAL_FUNCTION_MOCK_msg_interpreter(self,topic,payload):
        print("Topic: " + topic + " | Payload: " + payload)

    def EXTERNAL_FUNCTION_MOCK_on_failed_connection(self):
        print("Doing stuff when failed connection")
        pass


mqtt_connection = MQTTclient("25.98.111.255",["error", "test"])
msgs = ["multiple 1","multiple 2","multiple 1"]
topics = ["test", "error", "test"]
while(True):
    time.sleep(1)
    print("loop")
    mqtt_connection.publish_multiple(topics, msgs, "25.98.111.255")
    pass