from icfpc import *
import json


def print_task(lst):
    return "\n".join(str(i) for i in lst)

def repeat(lst,count):
    return [item for item in lst for i in range(count)]

with open('tasks.json','w') as f:
    f.write(print_task(repeat(one_out(get_dummy_ai(5)),10)))

def get_result():
    result = []
    with open('result.jsonline') as f:
        for line in f:
            result.append(json.loads(line))
    return result

pairwise_output(get_result())
