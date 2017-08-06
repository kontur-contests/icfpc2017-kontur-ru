import magic
import json
import numpy as np

def test_historical_2():
    maps = {
        'sample.json': 2,
        'Sierpinski-triangle.json': 3,
        'randomSparse.json': 4,
        'lambda.json': 4,
        'circle.json': 4,
        'randomMedium.json': 4,
        'boston-sparse.json': 8,
        'tube.json': 8,
        'edinburgh-sparse.json': 16,
        'oxford-sparse.json': 16,
        'gothenburg-sparse.json': 16,
    }
    (magic.Fluent()
         .create_historical_players(6)
         .battles_on_map_set(maps,200)
         .experiment('Historical')
         .run().dump().store_pointwise('historical_4')
    )


def test_parameter():
    runner = magic.Fluent()
    (runner
    .from_params(MineWeight = magic.Param(10,300))
    .create_random_players(100)
    .battles_on_map('Sierpinski-triangle.json', 3, 100)
    .experiment('MRVW')
    #.preview()
    .run().dump().store_pointwise('exp.csv')
     )
    print(runner.token)




test_historical_2()

