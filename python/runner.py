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
     .create_random_players('FutureIsNow', 10, (0.2,2))
     .battles_on_map_set(maps, 200)
     .add_dummies(dummies)
     .experiment('Uber')
     #.preview()
     .run().dump()
     )


def test_uber():
    (magic.Fluent()
     .create_random_players('UberAi', 100, (0,1), (0,1), (0,1), (0,1) )
     .battles_on_map_set(maps, 50)
     .experiment('Uber')
     #.preview()
     .run().dump()
     )


def assemble(fname,*args):
    if len(args)==0: return
    magic.Fluent().restore_dump(*args).store_pointwise(fname)

#assemble('future.csv',11080,80290,73859,65026,31822,43116,72148)
test_parameter_future()

#assemble('uber.csv',1274,40991,81735,79724,11270,71580,57526)
#test_uber()

assemble('hist',27146,56663,47969,17158)
#test_historical();
