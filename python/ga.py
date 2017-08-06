import numpy as np
import magic
length = 4
from magic.foreman import *

def init_genes(count):
    pool = [
        [
            np.random.rand(1)[0] for _ in range(length)
        ]
        for __ in range(count)
    ]
    return pool

def mutate(pool):
    for g in pool:
        c = np.random.randint(length)
        g[c] = np.random.rand(1)[0]

def evaluate(pool):
    players = [
        {
            'Name' : uuid4().hex,
            'ClassName': 'UberAi',
            'Params' : { 'param'+str(i+1) : pool[i] for i in range(length) }
    }]

    r=(magic.Fluent()
        .set_players(players)
        .battles_on_map_set(magic.get_maps(),5)
        .experiment('Uber')
        .run()
        .dump())





pool = init_genes(10);pool = pool + pool;evaluate(pool)