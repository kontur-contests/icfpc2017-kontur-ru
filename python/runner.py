import magic
import json
import numpy as np






def test_historical_algorithms():
    runner = magic.Fluent()
    (runner
     .create_historical_players(2)
     .battling_in_pairs()
     .on_maps('sample.json', 'Sierpinski-triangle.json')
     .repeating(1)
     .experiment('Historical')
     # .preview()
     .run().dump()
     #.store_pointwise('historical.csv')
     )
    print (runner.token)

test_historical_algorithms()

#Fluent().restore_dump('result_dump_93839.json').store_pointwise('test.csv')


#test_greedy_algorithms()

#Fluent().restore_dump('greedy_results').store_pointwise('test.csv')


