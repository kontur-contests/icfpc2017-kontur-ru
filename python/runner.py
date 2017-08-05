import magic
import json
import numpy as np







def test_historical_2():
    runner = magic.Fluent()
    (runner
    .create_historical_players(5)
    .battles_on_map('sample.json', 2, 3)
    .battles_on_map('Sierpinski-triangle.json', 3, 4)
    .battles_on_map('gothenburg-sparse.json', 8, 6)
    .experiment('Historical')
    .preview())

def test_parameter():
    runner = magic.Fluent()
    (runner
    .from_params(MineWeight = magic.Param(10,300))
    .create_random_players(100)
    .battles_on_map('gothenburg-sparse.json', 8, 100)
    .experiment('MRVW')
    .preview()
     )

test_parameter()


#test_historical_2()

