from magic import *
import json
import numpy as np





def test_greedy_algorithms():
    (Fluent()
    .from_params()
    .create_random_players(1)
    .first_against_himself(1,2,4)
    .on_maps('sample.json')
    .experiment('Greedy')
    .preview()
    .run()
    .dump('greedy_results')
     )

def test_historical_algorithms():
    (Fluent()
     .create_historical_players(5)
     .battling_in_pairs()
     .on_maps('sample.json')
     .experiment('Historical')
     .preview())

test_historical_algorithms()

#test_greedy_algorithms()

#Fluent().restore_dump('greedy_results').store_pointwise('test.csv')

#empty_queue()