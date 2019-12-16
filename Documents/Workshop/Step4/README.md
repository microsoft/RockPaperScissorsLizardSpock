# Step4
Goal:
* Return best match prediction to client-side
* Render result to HTML page

## Changes in index.js
```javascript
    const customVisionPostRequest = https.request(customVisionPostOptions, (predictionResponse) => {
        predictionResponse.on('data', function(data) {
            const customVisionResponse = JSON.parse(data);
            const predictions = customVisionResponse.predictions;
            console.log(predictions);
            const mostLikelyPrediction = predictions.sort((a, b) => {
                return (a.probability > b.probability) ? -1 :
                    (a.probability === b.probability ? 0 : 1);
            })[0].tagName;
            response.setHeader('Content-Type', 'text/json');
            response.end(`{ "prediction": "${mostLikelyPrediction}" }`);
        });
    });
```

## Changes in public/index.html

Complete the first console screen with user's pick and add opponent's console
```HTML
<div class="console-screen">
    <p class="console-text">User</p>
    <div class="cam-recorder-container">
        <video width="800" height="600" autoplay=""></video>
        <canvas width="800" height="600"></canvas>
        <img class="user-pick" src="img/user/rock.png" alt="user pick" />
    </div>
</div>
<div class="app-action">
    <div class="start-button">
        <p>start</p>
        <p>battle</p>
    </div>
    <div class="app-counter">
        <p class="app-counter-text">VS</p>
    </div>
</div>
<div class="console-screen">
    <p class="console-text">engine</p>
    <div class="opponent-container">
        <img class="bot-player" src="img/bot.png" alt="engine player">
    </div>
    <div class="restart-button">
        <span>play again</span>
    </div>
</div>
```

## changes in public/css/app.css

To add at the end
```CSS
.console-screen>div.restart-button {
    height: auto;
    position: relative;
    top: 30px;
}


/********** Actions And buttons **********/

.app-action {
    display: flex;
    flex: 2;
    justify-content: center;
    align-items: center;
}

.app-counter {
    display: flex;
    width: 10vw;
    height: 10vw;
    border-radius: 50%;
    box-shadow: inset 0px 0px 0px 10px #231547;
    align-items: center;
    justify-content: center;
    cursor: default;
    font-family: "Voyager";
    font-size: 3.5rem;
    color: #956dea;
    text-shadow: 0 2px 2px rgba(0, 0, 0, 0.9);
}


/********** Bot console **********/

.opponent-container {
    display: flex;
    justify-content: center;
    align-items: center;
}

.opponent-container>img {
    height: 100%;
    width: auto;
}
```

## Changes in public/js/app.js

At the beginning add global variables
```javascript
// Global variables
const restartButtonElement = document.querySelector('.restart-button');
const startButtonElement = appContainer.querySelector('.start-button');
```

Update setLayout elements arrays
```javascript
const elements = [
    "video", "canvas", ".user-pick",
    ".start-button", ".app-counter", ".restart-button"
];

const visibleElements = {
    "start": ["video", ".start-button"],
    "countdown": ["video", ".app-counter"],
    "predicting": ["canvas", ".app-counter"],
    "results": ["canvas", ".user-pick", ".app-counter", ".restart-button"]
};
```

Add new methods
```javascript
const startBattle = () => {
    if (!webcamStream) return;

    const videoElement = document.querySelector("video");
    const canvasElement = document.querySelector("canvas");
    const timerTick = 3000;

    setLayout("countdown");

    const counterTimerTick = function counterTimerTick() {
        takePhoto(videoElement, canvasElement);
        setLayout("predicting");
        submitImageFromCanvas(canvasElement);
    };

    setTimeout(counterTimerTick, timerTick);
};

const showResults = (prediction) => {
    const userPickElement = appContainer.querySelector('.user-pick');
    userPickElement.src = 'img/user/' + prediction + '.png';

    setLayout("results");
};
```

Inside submitImageFromCanvas from a success prediction
```javascript
    // Success!
    const prediction = JSON.parse(request.responseText).prediction.toLowerCase();
    showResults(prediction);
```

Instead of taking a picture after 3 seconds it will be triggered by pressing the start battle button, we need to remove the 3 seconds trigger at bindCamera function
```javascript
// TO REMOVE:
setTimeout(() => {
    const canvasElement = document.querySelector("canvas");
    takePhoto(videoElement, canvasElement);
    setLayout("predicting");
    submitImageFromCanvas(canvasElement);
}, 3000);
```
After bindCamera(); call
```javascript
    bindCamera();
    startButtonElement.addEventListener("click", startBattle);
    restartButtonElement.addEventListener("click", () => setLayout("start"));
```