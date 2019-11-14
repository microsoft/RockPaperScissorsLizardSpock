import random
import socket
from flask import jsonify

from .rpsls import RPSLS
from .rpsls_dto import get_rpsls_dto_json

# Fixed pick Game Strategy
def fixed_strategy(pick_value):
    pick_RPSLS=pick_value
    def pick():
        return get_rpsls_dto_json(pick_RPSLS)
    return pick

# Random pick Game Strategy
def random_strategy():
    def pick():
        pick_RPSLS = random.choice(list(RPSLS))
        return get_rpsls_dto_json(pick_RPSLS)
    return pick

# Iterative pick Game Strategy
def iterative_generator(value):
    while True:
        yield value
        value += 1
        value = value % len(RPSLS)

def iterative_strategy():
    pick_generator = iterative_generator(0)
    def pick():
        pick_RPSLS = RPSLS(next(pick_generator))
        return get_rpsls_dto_json(pick_RPSLS)
    return pick

    