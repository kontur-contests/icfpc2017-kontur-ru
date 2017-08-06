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

dummies = [
    { 'Name' : 'LochKillerAi', 'ClassName' : 'LochKillerAi', 'Params' : {} },
    { 'Name' : 'ConnectClosestMinesAi', 'ClassName' : 'ConnectClosestMinesAi', 'Params' : {} },
#    { 'Name' : 'LochMaxVertexWeighterKillerAi', 'ClassName': 'LochMaxVertexWeighterKillerAi', 'Params': {}}
]

def test_historical():

    (magic.Fluent()
         .create_historical_players(7)
         .battles_on_map_set(maps,50)
         .experiment('Historical')
         .run().dump()
    )


def test_parameter_future():
    (magic.Fluent()
     .create_random_players('FutureIsNow', 10, (0.3,5))
     .battles_on_map_set(maps, 50)
     .add_dummies(dummies)
     .experiment('Uber')
     #.preview()
     .run().dump()
     )



def assemble(fname,*args):
    magic.Fluent().restore_dump(*args).store_pointwise(fname)
    pass

#assemble('future.csv',45820,41308,35379,31603,48105,16426)
#test_parameter_future()

test_historical();
#assemble('hist',46229)
#assemble('future',14139,58033,61678,93832);test_parameter_future()

#assemble('historical_4')
#assemble('naserator.csv',45487)
#test_parameter()