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

    def create_random_players(self, count, seed=42):
        players = []
        np.random.seed(seed)
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

    def run_experiment(self, experiment_name):
        self.tasks = [ {'Experiment' : experiment_name, 'Map' : map,  'Players' : battle } for battle, map in self.battles_on_maps]
        self.results = execute_tasks(self.tasks)
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


#k = Fluent().from_params().create_players_randomly(1).one_player_various_groupsize(1,2,4,8)
print (k.results)