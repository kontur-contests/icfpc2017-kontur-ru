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



def test_historical():

    (magic.Fluent()
         .create_historical_players(6)
         .battles_on_map_set(maps,10)
         .experiment('Historical')
         .run().dump()
    )

def assemble(fname,*args):
    magic.Fluent().restore_dump(*args).store_pointwise(fname)
    pass

#test_historical()
#assemble('historical_4', 27656,35115,89180)


def test_parameter():
    (magic.Fluent()
     .create_random_players('Podnaserator2000Ai', 10, (0, 5), (0, 5), (0, 100))
     .battles_on_map_set(maps, 30)
     .experiment('Uber')
     .run().dump())

assemble('naserator.csv',45487)
#test_parameter()