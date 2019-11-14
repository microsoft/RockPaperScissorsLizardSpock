import os
import json
import urllib.request

from .rpsls import RPSLS
from .rpsls_dto import get_rpsls_dto_json

def get_pick_predicted(user_name):
    queried_url = _get_queried_url(user_name)
    response = _get_response_from_predictor(queried_url)
    predicted_pick = RPSLS[response['prediction'].lower()]
    return get_rpsls_dto_json(predicted_pick)

def _get_queried_url(user_name):
    predictor_url = os.getenv('PREDICTOR_URL')
    return f'{predictor_url}&humanPlayerName={user_name}'

def _get_response_from_predictor(queried_url):    
    req = urllib.request.urlopen(queried_url)
    encoding = req.info().get_content_charset('utf-8')
    data = req.read()
    return json.loads(data.decode(encoding))