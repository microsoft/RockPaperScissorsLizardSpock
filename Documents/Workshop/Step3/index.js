const express = require('express');
const app = express();
const PORT = process.env.PORT || 1337;
const bodyParser = require('body-parser')
const PredictionConfig = require("./config");

app.use(bodyParser.urlencoded({ extended: false }))
app.use(bodyParser.raw({ limit: '10MB' }));
app.use(express.static('public'));

app.use(function(req, res, next) {
    res.header("Access-Control-Allow-Origin", "*");
    res.header("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept");
    next();
});

app.post('/predict', (request, response) => {
    const imageData = request.body;
    const fs = require("fs");
    fs.writeFileSync("test.png", imageData);

    const customVisionPostOptions = {
        hostname: PredictionConfig.ServerHost,
        port: 443,
        path: PredictionConfig.ModelPath,
        method: 'POST',
        headers: {
            'Content-Type': 'application/octet-stream',
            'Prediction-key': PredictionConfig.Key
        }
    };
    // Set up the request
    const https = require('https');
    const customVisionPostRequest = https.request(customVisionPostOptions, (predictionResponse) => {
        predictionResponse.on('data', function(data) {
            const customVisionResponse = JSON.parse(data);
            const predictions = customVisionResponse.predictions;
            console.log(predictions);
        });
    });

    // post the data
    customVisionPostRequest.write(imageData);
    customVisionPostRequest.end();
});

app.listen(PORT, () => console.log(`Listening on ${ PORT }`));