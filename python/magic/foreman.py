from pykafka import KafkaClient
import json

def execute_tasks(tasks_to_do, consumer_group):
    client = KafkaClient(hosts="icfpc-broker.dev.kontur.ru:9092")
    tasks = client.topics[b'tasks']
    results = client.topics[b'results']
    num_tasks = len(tasks_to_do)

    with tasks.get_sync_producer() as producer:
        for task in tasks_to_do:
            producer.produce(json.dumps(task).encode('utf-8'))

    consumer = results.get_balanced_consumer(
        consumer_group=consumer_group,
        auto_commit_enable=True,
        zookeeper_connect='icfpc-broker.dev.kontur.ru:2181'
    )

    results = []

    remaining_answers = num_tasks
    for message in consumer:
        remaining_answers -= 1
        if message is not None:
            results.append(json.loads(message.value.decode('utf-8')))
        if not remaining_answers:
            break

    return results


def empty_queue():
    client = KafkaClient(hosts="icfpc-broker.dev.kontur.ru:9092")
    results = client.topics[b'results']

    consumer = results.get_balanced_consumer(
        consumer_group=b'icfpc2017-foreman',
        auto_commit_enable=True,
        zookeeper_connect='icfpc-broker.dev.kontur.ru:2181'
    )

    for message in consumer:
        if message is not None:
            print(message.value)

    return results