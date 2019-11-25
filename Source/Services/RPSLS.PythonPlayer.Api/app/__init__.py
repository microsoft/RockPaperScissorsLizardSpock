from flask import Flask, request
from healthcheck import HealthCheck
from applicationinsights.flask.ext import AppInsights

import logging
import os

from .pick import Picker

app = Flask(__name__)
appinsightskey = os.getenv('APPLICATION_INSIGHTS_IKEY', '')
if appinsightskey:
    app.config['APPINSIGHTS_INSTRUMENTATIONKEY'] = appinsightskey
    # log requests, traces and exceptions to the Application Insights service
    appinsights = AppInsights(app)

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
