# Step3 changes
Goal:
* Submit picture to index.js
* Receive picture on index.js
* Submit picture from index.js to Custom Vision API
* Receive predictions from Custom Vision

## Changes to public/js/app.js

``` javascript
    const submitImageFromCanvas = (canvasElement) => {
        const request = new XMLHttpRequest();
        request.open('POST', "/predict", true);
        request.setRequestHeader('Content-Type', 'application/octet-stream');
        request.onload = function() {
            if (request.status >= 200 && request.status < 400) {
                // Success!
                console.log(request.responseText);
            } else {
                console.error(request);
            }
        };

        request.onerror = function(error) {
            console.error(error);
        };

        canvasElement.toBlob(function(blob) {
            request.send(blob);
        });
    };
```

And call it after store it in canvas.
``` javascript
    const canvasElement = document.querySelector("canvas");
    takePhoto(videoElement, canvasElement);
    setLayout("predicting");
    submitImageFromCanvas(canvasElement);
```

## Create config.js
```javascript
require('dotenv').config();

const defaultConfig = {
    "Key": "<PUT_YOUR_KEY_HERE>",
    "ModelPath": "<PUT_PATH_TO_ITERATION_HERE>", // example path - /customvision/v3.0/Prediction/b99e831f-1d60-4e36-91d3-05eac0d2e03c/classify/iterations/Iteration2/image 
    "ServerHost": "<PUT_HOST_SERVER_HERE>" // example host - westeurope.api.cognitive.microsoft.com
};

module.exports = {
    Key: process.env.CV_KEY || defaultConfig.Key,
    ModelPath: process.env.CV_MODEL_PATH || defaultConfig.ModelPath,
    ServerHost: process.env.CV_SERVER_HOST || defaultConfig.ServerHost
};
```

You can either set the variables as default configuration or set as environment variables in a ".env" file (which will be ignored)
```
CV_KEY=XXXXXXXXXXXXXXXXX
CV_MODEL_PATH=XXXXXXXXXXXXXXXXX
CV_SERVER_HOST=XXXXXXXXXXXXXXXXX
```

## Changes to index.js
```javascript
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
```

# Troubleshooting
In case you are getting errors about body size on POST, replace the following code in public/js/app.js:

```javascript
    canvasElement.toBlob(function(blob) {
        request.send(blob);
    });

```
with resizing via a new canvas

```javascript
    const resizeCanvas = document.createElement('canvas'),
    resizeCanvasContext = resizeCanvas.getContext('2d'),
    resizeRatio = 0.5; // Set appropriate resize ratio

    resizeCanvas.width = canvasElement.width * resizeRatio;
    resizeCanvas.height = canvasElement.height * resizeRatio;
    resizeCanvasContext.drawImage(canvasElement, 0, 0, resizeCanvas.width, resizeCanvas.height);

    resizeCanvas.toBlob(function(blob) {
        request.send(blob);
    });
```