import numpy as np
import json
from magic.foreman import *
import os


class Fluent:

    def __init__(self):
        self.param_names = list()
        self.battles_on_maps = []
        self.players = []

    def create_random_players(self, class_name, count, *limits):
        for i in range(count):
            player = dict()
            for number,limit in enumerate(limits):
                key = 'param'+str(number+1)
                if key not in self.param_names:
                    self.param_names.append(key)
                value = np.random.random_sample(1)[0]
                value = value * (limit[1] - limit[0]) + limit[0]
                player[key] = value
            self.players.append({'Name': uuid4().hex, 'Params': player, 'ClassName' : class_name})
        return self


    def create_historical_players(self, history_length):
        self.players = [ { 'Name' : 'Age'+str(i), 'Params' : {'Age' : i}} for i in range(history_length)]
        self.param_names = ['Age']
        return self;

    def create_nigga_players(self, mine_weight, players_number, default_mine_weight=100):
        self.players = [{ 'Name' : player_index, 'Params': {'MineWeight': default_mine_weight}}
                        for player_index
                        in range(players_number)]
        self.players[0]['Params']['MineWeight'] = mine_weight
        return self

    def battling_in_pairs(self):
        self.battles = [[self.players[first], self.players[second]]
                        for second in range(len(self.players))
                        for first in range(len(self.players))
                        if first != second]
        return self

    def first_against_himself(self, *args):
        self.battles = [[self.players[0] for _ in range(size)] for size in args]
        return self

    def on_maps(self, *args):
        self.battles_on_maps = [(battle,map) for battle in self.battles for map in args]
        return self

    def add_dummies(self, dummies):
        bs = []
        for battle, map in self.battles_on_maps:
            place = np.random.randint(0,len(battle))
            for i in range(len(battle)):
                if i!=place:
                    battle[i] = np.random.choice(dummies.values)


            bs.append((battle,map))
        self.battles_on_maps = bs
        return self

    def repeating(self, count):
        self.battles_on_maps = [x for x in self.battles_on_maps for _ in range(count)]
        return self


    def battles_on_map(self, map, player_count, battles_count):
        for _ in range(battles_count):
            players = [np.random.choice(self.players) for _ in range(player_count)]
            self.battles_on_maps.append((players,map))
        return self

    def battles_on_map_set(self, maps, battles_count):
        for map in maps:
            self.battles_on_map(map,maps[map],battles_count)
        return self


    def experiment(self, experiment_name):
        self.token = np.random.randint(1,100000)
        self.tasks = [{'Experiment': experiment_name, 'Token' : self.token, 'Part' : index, 'Map': map, 'Players': battle}
                      for index, (battle, map) in enumerate(self.battles_on_maps)]
        return self

    def preview(self):
        print(json.dumps(self.tasks,indent=2))
        return self

    def run(self):
        self.results = execute_tasks(self.tasks,self.token)
        return self



    def dump(self,dump_file = None):
        self.result_dump_file = dump_file or os.path.join('dumps',
                                                     'result_dump_' + str(self.token) + '.json')
        with open(self.result_dump_file,'w') as file:
            file.write(json.dumps(self.results,indent=2))
        return self

    def restore_dump(self, *tokens):
        self.results=[]
        for token in tokens:
            dump_file = os.path.join('dumps','result_dump_' + str(token) + '.json')
            with open(dump_file,'r') as file:
                results = json.loads(file.read())
                for r in results:
                    self.results.append(r)
        self.param_names = []
        for result in self.results:
            for player in result['Task']['Players']:
                for key in player['Params']:
                    if key not in self.param_names:
                        self.param_names.append(key)
        return self

    def store_pointwise(self, filename, mode='w', header=True):
        keys = self.param_names
        print(keys)
        with open(filename, mode) as file:
            if header:
                file.write(','.join([
                    'game_number',
                    'server_name',
                    'scores',
                    'ranking',
                    'tournament_scores',
                    'num_players',
                    'map',
                    'map_rivers_count',
                    'map_sites_count',
                    'map_mines_count',
                    'name'] + keys
                ))
            file.write('\n')
            for game_number, game in enumerate(self.results):
                for player_index in range(len(game['Results'])):
                    player = game['Task']['Players'][player_index]
                    result = game['Results'][player_index]
                    file.write(','.join([str(x) for x in [
                        game_number,
                        result['ServerName'],
                        result['Scores'],
                        result['Ranking'],
                        result['TournamentScore'],
                        len(game['Results']),
                        game['Task']['Map'],
                        game['RiversCount'],
                        game['SitesCount'],
                        game['MinesCount'],
                        player['Name']
                    ]]))
                    file.write(',')
                    file.write(','.join([
                        str(player['Params'][key]) if key in player['Params'] else 'NA' for key in keys
                    ]))
                    file.write('\n')
        return self
