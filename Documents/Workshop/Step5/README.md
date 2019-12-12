# Step5
Goal:
* Add AI opponent
* Show opponents move

## Changes in public/index.html

Update opponents container with its pick
```HTML
<div class="opponent-container">
    <img class="bot-player" src="img/bot.png" alt="engine player">
    <div class="bot-pick">
        <img class="bot-pick-img" src="img/bot/rock.png" alt="engine pick">
    </div>
</div>
```

## Changes in public/css/app.css
``` CSS
.bot-pick {
    height: 10vw;
    width: 10vw;
    border-radius: 100%;
    background: #492f93;
    box-shadow: inset 0px 0px 0px 2px #492f93;
    box-sizing: border-box;
}

.bot-pick img {
    width: 100%;
    height: auto;
}
```

## Changes in public/js/app.js
Add picks and engine pick logic as global variables
```javascript
// Global variables
...
const picks = ["rock", "paper", "scissors", "lizard", "spock"];
const getEnginePick = () => picks[Math.floor(Math.random() * picks.length)];
```

Update setLayout elements arrays
```javascript
const elements = [
    "video", "canvas", ".user-pick",
    ".bot-player", ".bot-pick",
    ".start-button", ".app-counter", ".restart-button"
];

const visibleElements = {
    "start": ["video", ".start-button", ".bot-player"],
    "countdown": ["video", ".app-counter", ".bot-player"],
    "predicting": ["canvas", ".app-counter", ".bot-player"],
    "results": ["canvas", ".user-pick", ".app-counter", ".bot-pick", ".restart-button"]
};
```

Add engine pick logic to showResults method
```javascript

    const showResults = (prediction, enginePick) => {
        const userPickElement = appContainer.querySelector('.user-pick');
        const enginePickElement = appContainer.querySelector('.bot-pick-img');

        userPickElement.src = 'img/user/' + prediction + '.png';
        enginePickElement.src = 'img/bot/' + enginePick + '.png';

        setLayout("results");
    };
```

Inside submitImageFromCanvas method add engine pick and show results call
```javascript
    const enginePick = getEnginePick();
    showResults(prediction, enginePick);
```