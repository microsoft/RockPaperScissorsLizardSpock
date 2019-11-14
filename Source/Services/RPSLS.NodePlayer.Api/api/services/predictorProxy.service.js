const axios = require('axios');
const os = require("os");
require('dotenv').config();

const { RPSLSChoices } = require('../settings/rpslsOptions');


class PredictorProxyService {
    async getPickPredicted(username) {
        const urlQueried = `${process.env.PREDICTOR_URL}&humanPlayerName=${username}`;
        const response = await axios.get(urlQueried);
        const prediction = response.data.prediction;
        
        return {
            "playerType": "node",
            "player": os.hostname(),
            "text": prediction,
            "value": RPSLSChoices.indexOf(prediction.toLowerCase())
        };
    }
}

module.exports = PredictorProxyService;