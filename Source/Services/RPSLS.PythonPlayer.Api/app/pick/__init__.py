from flask.views import View
from flask import request, current_app as app
import os

from .rpsls import RPSLS
from .rpsls_dto import get_rpsls_dto_json
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
                predicted_result = get_pick_predicted(username)
                app.logger.info(f'Against user [{username}] predictor played {predicted_result.name}')
                return get_rpsls_dto_json(predicted_result)
            except Exception as ex:
                app.logger.error(ex)

        strategy = self.get_strategy()
        pick = strategy_map[strategy]
        result = pick()
        app.logger.info(f'Against some user, strategy {strategy} played {result.name}')
        return get_rpsls_dto_json(result)

    @staticmethod
    def get_strategy():
        default_value = 'random'
        return os.getenv('PICK_STRATEGY', default_value)