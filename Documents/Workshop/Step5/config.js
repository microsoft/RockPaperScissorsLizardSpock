require('dotenv').config();

const defaultConfig = {
    "Key": "<PUT_YOUR_KEY_HERE>",
    "ModelPath": "<PUT_PATH_TO_ITERATION_HERE>",
    "ServerHost": "<PUT_HOST_SERVER_HERE>"
};

module.exports = {
    Key: process.env.CV_KEY || defaultConfig.Key,
    ModelPath: process.env.CV_MODEL_PATH || defaultConfig.ModelPath,
    ServerHost: process.env.CV_SERVER_HOST || defaultConfig.ServerHost
};