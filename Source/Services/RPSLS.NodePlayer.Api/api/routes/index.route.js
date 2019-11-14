const express = require('express');
const pick = require('./pick.route');
const health = require('./health.route');

const router = express.Router();

router.use('/pick', pick);
router.use('/health', health);

module.exports = router;