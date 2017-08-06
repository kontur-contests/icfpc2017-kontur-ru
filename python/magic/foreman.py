from pykafka import KafkaClient
from pykafka.balancedconsumer import OffsetType
from uuid import uuid4
import json

def execute_tasks(tasks_to_do, token):
    client = KafkaClient(hosts="icfpc-broker.dev.kontur.ru:9092")
    tasks = client.topics[b'tasks']
    results = client.topics[b'results']
    num_tasks = len(tasks_to_do)

    consumer = results.get_balanced_consumer(
        consumer_group=("icfpc2017-foreman-%s" % uuid4().hex).encode('ascii'),
        auto_commit_enable=True,
        auto_offset_reset=OffsetType.LATEST,
        zookeeper_connect='icfpc-broker.dev.kontur.ru:2181'
    )
    print(str(len(tasks_to_do))+' to go')

    with tasks.get_sync_producer() as producer:
        for index, task in enumerate(tasks_to_do):
            print('\rsending '+str(index), end='')
            producer.produce(json.dumps(task).encode('utf-8'))

    results = []

    remaining_answers = num_tasks
    print('\nremaining', end='')
    for message in consumer:
        if message is not None:
            data = json.loads(message.value.decode('utf-8'))
            #print(json.dumps(data))
            if ('Token' in data) and (data['Token'] == token):
                remaining_answers -= 1
                results.append(data)
        if not remaining_answers:
            break
        print('\rremaining ' + str(remaining_answers), end='')

    return results

