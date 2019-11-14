from flask.views import View
from flask import request
import os
import logging

from .rpsls import RPSLS
from .strategies import fixed_strategy, random_strategy, iterative_strategy
from .proxy_predictor import get_pick_predicted

strategy_map = {
    'rock': fixed_strategy(RPSLS.rock),
    'paper': fixed_strategy(RPSLS.paper),
    'scissors': fixed_strategy(RPSLS.scissors),
    'lizard': fixed_strategy(RPSLS.lizard),
    'spock': fixed_strategy(RPSLS.spock),
    'random': random_strategy(),
    'iterative': iterative_strategy()
}

class Picker(View):
    def dispatch_request(self):
        username = request.args.get('username', '')

        if(username != ''):
            try:
                return get_pick_predicted(username)
            except Exception as ex:
                logging.error(ex)

        strategy = self.get_strategy()
        pick = strategy_map[strategy]        
        return pick()

    @staticmethod
    def get_strategy():
        default_value = 'random'
        return os.getenv('PICK_STRATEGY', default_value)