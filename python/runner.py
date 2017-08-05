from magic import *
import json
import numpy as np


class Param:
    def __init__(self, _min=0, _max=1, _count=5):
        self.min=_min
        self.max=_max
        self.count = _count

class Fluent:

    def from_params(self, **kwargs):
        self.params = kwargs
        return self

    def create_random_players(self, count):
        players = []
        for i in range(count):
            player = dict()
            for key in self.params:
                value = np.random.random_sample(1)[0]
                value = value * (self.params[key].max-self.params[key].min)+self.params[key].min
                player[key]=value
            players.append({'Name' : str(i), 'Params' : player})
        self.players = players
        return self

    def battling_in_pairs(self):
        self.battles = [[self.players[first], self.players[second]]
                        for second in range(len(self.players))
                        for first in range(second - 1)]
        return self

    def first_against_himself(self, *args):
        self.battles = [[self.players[0] for _ in range(size)] for size in args]
        return self

    def on_maps(self, *args):
        self.battles_on_maps = [(battle,map) for battle in self.battles for map in args]
        return self



    def experiment(self, experiment_name):
        experiment_token = np.random.randint(1,100000)
        self.tasks = [{'Experiment': experiment_name, 'Token' : experiment_token, 'Part' : index, 'Map': map, 'Players': battle}
                      for index, (battle, map) in enumerate(self.battles_on_maps)]
        return self

    def preview(self):
        print(json.dumps(self.tasks,indent=2))
        return self

    def run(self):
        self.results = execute_tasks(self.tasks)
        return self

    def dump(self,dump_file):
        with open(dump_file,'w') as file:
            file.write(json.dumps(self.results,indent=2))
        return self

    def store_pointwise(self, filename):
        keys = list(self.params)
        with open(filename,'w') as file:
            file.write('rank,num_players,name,')
            file.write(",".join(keys))
            file.write('\n')
            for game in self.results:
                for player in game['Players']:
                    file.write(','.join([str(player['Rank']),str(len(game['Players'])), game['Map'], player['Name'] ]))
                    file.write(',')
                    file.write(','.join([str(player['Params'][key]) for key in keys]))
                    file.write('\n')
        return self



def test_greedy_algorithms():
    (Fluent()
    .from_params()
    .create_random_players(1)
    .first_against_himself(1,2,4)
    .on_maps('sample.json')
    .experiment('Greedy')
    .preview()
    #.run()
    #.dump('greedy_results')
     )

test_greedy_algorithms()

#empty_queue()