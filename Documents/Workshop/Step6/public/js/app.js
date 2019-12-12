function init() {
    // Global variables
    let webcamStream;
    const appContainer = document.getElementById('appContainer');
    const restartButtonElement = document.querySelector('.restart-button');
    const startButtonElement = appContainer.querySelector('.start-button');
    const picks = ["rock", "paper", "scissors", "lizard", "spock"];
    const resultText = {
        "winner": "you win!!",
        "loser": "you lose!!",
        "draw": "tie!!"
    };

    const getEnginePick = () => picks[Math.floor(Math.random() * picks.length)];
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

    const setLayout = (phase) => {
        const elements = [
            "video", "canvas", ".upload-photo-container", ".user-pick", ".pick-result",
            ".bot-player", ".bot-pick",
            ".start-button", ".app-counter", ".restart-button"
        ];

        const visibleElements = {
            "start": ["video", ".start-button", ".bot-player"],
            "countdown": ["video", ".app-counter", ".bot-player"],
            "predicting": ["canvas", ".app-counter", ".bot-player"],
            "results": ["canvas", ".user-pick", ".pick-result", ".app-counter", ".bot-pick", ".restart-button"],
            "uploadfile": [".upload-photo-container", ".app-counter", ".bot-player"]
        };

        for (let i = 0; i < elements.length; i++) {
            const el = appContainer.querySelector(elements[i]);
            if (visibleElements[phase].indexOf(elements[i]) == -1) {
                el.classList.add('hide');
            } else {
                el.classList.remove('hide');
            }
        }
    }

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

    const showResults = (prediction, enginePick) => {
        const userPickElement = appContainer.querySelector('.user-pick');
        const enginePickElement = appContainer.querySelector('.bot-pick-img');
        const resultsElement = appContainer.querySelector('.pick-result');

        userPickElement.src = 'img/user/' + prediction + '.png';
        enginePickElement.src = 'img/bot/' + enginePick + '.png';

        // Update results
        const result = getWinner(prediction, enginePick);
        const resultTextElement = resultsElement.firstElementChild;
        resultTextElement.className = result;
        resultTextElement.innerText = resultText[result];
        setLayout("results");
    };

    const submitImageFromCanvas = (canvasElement) => {
        const request = new XMLHttpRequest();
        request.open('POST', "/predict", true);
        request.setRequestHeader('Content-Type', 'application/octet-stream');
        request.onload = function() {
            if (request.status >= 200 && request.status < 400) {
                // Success!
                const prediction = JSON.parse(request.responseText).prediction.toLowerCase();
                const enginePick = getEnginePick();
                showResults(prediction, enginePick);
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

    const takePhoto = (videoElement, canvasElement) => {
        const canvasContext = canvasElement.getContext('2d');
        const videoSettings = webcamStream.getVideoTracks()[0].getSettings();
        canvasContext.drawImage(videoElement,
            0, 0, videoSettings.width, videoSettings.height,
            0, 0, canvasElement.width, canvasElement.height);
    };

    // Initialize camera
    function bindCamera() {
        const videoElement = document.querySelector('video');

        // getMedia polyfill
        navigator.getUserMedia = (navigator.getUserMedia ||
            navigator.webkitGetUserMedia ||
            navigator.mozGetUserMedia ||
            navigator.msGetUserMedia);

        // Check that getUserMedia is supported
        if (navigator.getUserMedia) {
            restartButtonElement.addEventListener("click", () => setLayout("start"));
            navigator.getUserMedia(
                // constraints
                {
                    video: { facingMode: 'environment' },
                    audio: false
                },
                // successCallback
                function(localMediaStream) {
                    try {
                        videoElement.srcObject = localMediaStream;
                    } catch (error) {
                        videoElement.src = window.URL.createObjectURL(localMediaStream);
                    }
                    webcamStream = localMediaStream;
                },
                // errorCallback
                function(err) {
                    console.log("The following error occured: " + err);
                }
            );
        } else {
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
        }
    }

    setLayout("start")
    bindCamera();
    startButtonElement.addEventListener("click", startBattle);
}

function onDocumentReady(fn) {
    if (document.attachEvent ? document.readyState === "complete" : document.readyState !== "loading") {
        fn();
    } else {
        document.addEventListener('DOMContentLoaded', fn);
    }
}

onDocumentReady(init);