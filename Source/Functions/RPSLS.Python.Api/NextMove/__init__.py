import logging
import os

import azure.functions as func

from .next_move import predict

def main(req: func.HttpRequest) -> func.HttpResponse:
    logging.info('Python HTTP trigger function processed a request.')
    # sample request url for testing in local
    # http://localhost:7071/api/challenger/move?humanPlayerName=john
    player_name = req.params.get('humanPlayerName', None)

    try:
        if player_name:
            next_move = predict(player_name)
            return func.HttpResponse(next_move)
        else:
            return func.HttpResponse(
                'Please enter the required fields',
                status_code=400
            )
    except Exception as ex:
        logging.error(ex)
        return func.HttpResponse('Error processing next move', status_code=500)

