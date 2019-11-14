import socket
from flask import jsonify

def get_rpsls_dto_json(pick):
    return jsonify(text = pick.name, value = pick.value, player=socket.gethostname(), playerType="python")