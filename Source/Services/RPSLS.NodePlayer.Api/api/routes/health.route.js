const express = require('express');
const healthController = require('../controllers/health.controller');

const router = express.Router();

router.route('/')
    .get(healthController.health);

module.exports = router;