import magic
import json
import numpy as np






def test_historical_algorithms_1():
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

def test_historical_2():
    runner = magic.Fluent()
    (runner
    .create_historical_players(5)
    .battles_on_map('sample.json', 2, 3)
    .battles_on_map('Sierpinski-triangle.json', 3, 4)
    .battles_on_map('gothenburg-sparse.json', 8, 6)
    .experiment('Historical')
    .preview())

test_historical_2()


#test_historical_algorithms()

#magic.Fluent().restore_dump('dumps\\result_dump_33915.json').store_pointwise('test.csv')

#Fluent().restore_dump('result_dump_93839.json').store_pointwise('test.csv')


#test_greedy_algorithms()

#Fluent().restore_dump('greedy_results').store_pointwise('test.csv')


