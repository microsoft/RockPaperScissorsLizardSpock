const express = require('express');
const pickController = require('../controllers/pick.controller');

const router = express.Router();

router.route('/')
    .get(pickController.pick);

module.exports = router;
