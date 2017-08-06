import magic
import json
import numpy as np

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

def test_historical_2():

    (magic.Fluent()
         .create_historical_players(6)
         .battles_on_map_set(maps,1)
         .experiment('Historical')
         .run().dump().store_pointwise('historical_4')
    )


def test_parameter():
    (magic.Fluent()
     .create_random_players('Podnaserator2000Ai', 1, (0, 1), (0, 1), (0, 1))
     .create_random_players('MaxReachableVertexWeightAi', 1, (0, 1))
     .battles_on_map('sample.json',2,5)
     .experiment('Uber')
     .run().dump())

test_parameter()
