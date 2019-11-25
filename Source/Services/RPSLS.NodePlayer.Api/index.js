const express = require('express');
const cors = require('cors');
const appInsights = require("applicationinsights");

const routes = require('./api/routes/index.route');

require('dotenv').config();

// Start application insights
const applicationInsightsIK = process.env.APPLICATION_INSIGHTS_IKEY;
if (applicationInsightsIK) {
  appInsights.setup(applicationInsightsIK).start();
}

const app = express();

// Application-Level Middleware
app.use(cors());

// Routes
app.use(routes);

// Start
app.listen(process.env.PORT, () =>
  console.log(`App listening on port ${process.env.PORT}!
Configured pick strategy with '${process.env.PICK_STRATEGY}'`)
);