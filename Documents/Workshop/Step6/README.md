# Step6
Goal:
* Visualize counter
* Show results

### Changes in public/index.html

Update camera container with results photo container
```HTML
<div class="cam-recorder-container">
    ...
    <div class="pick-result">
        <p>-</p>
    </div>
</div>
```

## Changes in public/css/app.css
```CSS
/********** Results **********/

.pick-result {
    background: #000000;
    color: #ffffff;
    width: 100%;
    height: 40px;
    position: absolute;
    bottom: 40px;
    font-family: Voyager;
    font-size: 2rem;
    margin: 0;
    text-align: center;
    text-shadow: 0 2px 2px rgba(0, 0, 0, 0.9);
}

.pick-result>p {
    margin: 5px;
}

.pick-result .loser {
    color: #cb0808;
}
```

## Changes in public/js/app.js
```javascript
    // Global variables
    const resultText = {
        "winner": "you win!!",
        "loser": "you lose!!",
        "draw": "tie!!"
    };
```

Add getWinner method with game logic
```javascript

    const getWinner = (userPick, enginePick) => {
        const winnerScheme = {
                "rock": ["scissors", "lizard"],
                "scissors": ["paper", "lizard"],
                "paper": ["rock", "spock"],
                "lizard": ["paper", "spock"],
                "spock": ["rock", "scissors"]
            },
            userPickValue = userPick.toLowerCase(),
            enginePickValue = enginePick.toLowerCase();

        if (userPick === enginePick) {
            return "draw";
        }

        if (winnerScheme[userPickValue].indexOf(enginePickValue) != -1) {
            return "winner";
        }

        return "loser";
    };
```

Update setLayout elements arrays with `pick-result` class
```javascript
    const elements = [
        "video", "canvas", ".user-pick", ".pick-result",
        ".bot-player", ".bot-pick",
        ".start-button", ".app-counter", ".restart-button"
    ];

    const visibleElements = {
        "start": ["video", ".start-button", ".bot-player"],
        "countdown": ["video", ".app-counter", ".bot-player"],
        "predicting": ["canvas", ".app-counter", ".bot-player"],
        "results": ["canvas", ".user-pick", ".pick-result", ".app-counter", ".bot-pick", ".restart-button"]
    };
```

Add counter display on start battle button function
```javascript
const startBattle = () => {
    if (!webcamStream) return;

    let counter = 0;
    const counterTextElement = appContainer.querySelector('.app-counter-text');
    const counterStart = 0;
    const counterStop = 3;
    const counterStep = 1;
    const timerTick = 1000;

    const videoElement = document.querySelector("video");
    const canvasElement = document.querySelector("canvas");

    let counterTimer;
    // set counter layout
    setLayout("countdown");

    const counterTimerTick = function counterTimerTick() {
        if (counterTimer) {
            clearTimeout(counterTimer);
        }
        counter += counterStep;
        if (counter < counterStop) {
            counterTextElement.innerHTML = counter;
            counterTimer = setTimeout(counterTimerTick, timerTick);
            return;
        }

        takePhoto(videoElement, canvasElement);
        setLayout("predicting");
        submitImageFromCanvas(canvasElement);
        counterTextElement.innerHTML = 'VS';
    };

    counter = counterStart;
    counterTimerTick();
};
```    

Add showResults results displaying if you won
```javascript
    const showResults = (prediction, enginePick) => {
        ...
        const resultsElement = appContainer.querySelector('.pick-result');
        ...
        // Update results
        const result = getWinner(prediction, enginePick);
        const resultTextElement = resultsElement.firstElementChild;
        resultTextElement.className = result;
        resultTextElement.innerText = resultText[result];
        setLayout("results");
    };
```

Add restart button action at the start of bindCamera function 
```javascript
    restartButtonElement.addEventListener("click", () => setLayout("start"));
```

## Workaround when getUserMedia is not supported

### Changes in public/index.html
```HTML
<div class="cam-recorder-container">
    <div class="upload-photo-container">
        <input id="upload-photo" type="file" class="upload-button" />
        <label for="upload-photo" class="custom-button">upload file</label>
    </div>
    ...
</div>
```

### Changes in public/js/app.js

Update setLayout elements arrays with `upload-photo-container` class
```javascript
    const elements = [".upload-photo-container", ... ];

    const visibleElements = {
        ...
        "uploadfile": [".upload-photo-container", ".app-counter", ".bot-player"]
    };
```
move restart action button to inside the getUserMedia found if clause
```javascript
    restartButtonElement.addEventListener("click", () => setLayout("start"));
```

Add the logic for missing getUserMedia logic.
```javascript
    ...
    console.log("getUserMedia not supported");
    setLayout("uploadfile");
    restartButtonElement.addEventListener("click", () => setLayout("uploadfile"));

    const canvasElement = document.querySelector("canvas");
    const canvasContext = canvasElement.getContext('2d');
    const image = new Image();
    image.onload = () => {
        canvasContext.drawImage(image,
            0, 0, image.width, image.height,
            0, 0, canvasElement.width, canvasElement.height);
        submitImageFromCanvas(canvasElement);
        URL.revokeObjectURL(image.src);
    };
    document.getElementById("upload-photo").addEventListener('change', (event) => {
        const file = event.target.files[0];
        image.src = URL.createObjectURL(file);
    });
```

