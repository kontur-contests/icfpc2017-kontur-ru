from kafka import KafkaProducer, KafkaConsumer
import time
import json

def send_and_receive(data, output_queue = 'test', input_queue = 'test'):

    broker = 'icfpc-broker.dev.kontur.ru'

    consumer = KafkaConsumer('qwerty', group_id=None, bootstrap_servers=broker)

    producer = KafkaProducer(
        bootstrap_servers=broker,
    )

    producer.send('qwerty',b'1X')

    print("sending complete")


    for message in consumer:
        print("%s:%d:%d: key=%s value=%s" % (message.topic, message.partition,
                                             message.offset, message.key,
                                             message.value))
