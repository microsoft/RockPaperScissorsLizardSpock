const express = require('express');
const cors = require('cors');
require('dotenv').config();

const routes = require('./api/routes/index.route');

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