from flask import Flask, request
from healthcheck import HealthCheck
import logging

from .pick import Picker

app = Flask(__name__)

health = HealthCheck()

app.add_url_rule("/healthcheck", "healthcheck", view_func=lambda: health.run())
app.add_url_rule('/pick', 'pick', view_func=Picker.as_view('picker'))


if __name__ == "__main__":
    app.run(threaded=True)
else:
    gunicorn_logger = logging.getLogger('gunicorn.error')
    app.logger.handlers = gunicorn_logger.handlers
    app.logger.setLevel(gunicorn_logger.level)

app.logger.info('Configured pick strategy with \'%s\'', Picker.get_strategy())
