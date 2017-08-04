from icfpc import *
import json


results = []
with open('sample.json') as f:
    for string in f:
        results.append(json.loads(string))

pairwise_output(results)