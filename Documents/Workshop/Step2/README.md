# Step2 changes
Goal:
* Initalize camera
* Take a picture
* Store it in canvas element

## index.html changes
```HTML
<div id="appContainer" class="app-content">
    <div class="console-controls"><img src="img/controls_left.svg" alt="left panel console" /></div>
    <div class="console-screen">
        <p class="console-text">User</p>
        <div class="cam-recorder-container">
            <video width="800" height="600" autoplay=""></video>
            <canvas width="800" height="600"></canvas>
        </div>
    </div>
    <div class="console-controls"><img src="img/controls_right.svg" alt="right panel console" /></div>
</div>
```

## app.css changes
```CSS
/********** Layout **********/

.app {
    background-size: 100%;
    flex-direction: column;
    display: flex;
    width: 100%;
    background-image: url(../img/screens_panel.svg), url(../img/platform_opponents.svg);
    background-repeat: no-repeat;
    background-position-y: 4vh, top;
}

.app-content {
    display: flex;
    flex-direction: row;
    margin-top: 10vh;
    width: 100%;
    justify-content: space-around;
}

.console-controls {
    padding-top: 2vw;
    height: 27.4vw;
    display: flex;
    align-items: center;
    justify-content: center;
    flex: 2;
}

.console-controls>img {
    max-height: 100%;
}


/********** Consoles layout **********/

.console-screen {
    height: 25vw;
    flex: 4;
    margin: 10px;
    display: flex;
    justify-content: flex-start;
    align-items: center;
    flex-direction: column;
    background-image: url(../img/console_background.svg);
    background-repeat: no-repeat;
    background-size: contain;
    background-position: center top;
}

.console-screen>div {
    height: 20vw;
}

.console-text {
    font-family: Voyager;
    color: #ffffff;
    display: flex;
    height: 1.8vw;
    font-size: 1rem;
    margin: 5px auto;
    text-shadow: 0 2px 2px rgba(0, 0, 0, 0.9);
    box-shadow: 0px -8px 12px -1px rgba(72, 52, 133, 0.56);
    text-align: center;
    vertical-align: middle;
}


/********** User console **********/

.cam-recorder-container {
    position: relative;
    margin: 25px 40px;
    border-radius: 10px;
}

.cam-recorder-container img {
    position: absolute;
    width: 5vw;
    top: 1.7vw;
    right: 1vw;
    opacity: 1;
    background: #53398d;
    border-radius: 100px;
}

.console-screen video,
.console-screen canvas {
    height: auto;
    width: 100%;
    max-width: 24vw
}
```

## app.js changes, inside init() method
``` javascript

    // Global variables
    let webcamStream;
    const appContainer = document.getElementById('appContainer');

    const setLayout = (phase) => {
        const elements = ["video", "canvas"];
        const visibleElements = {
            "start": ["video"],
            "predicting": ["canvas"],
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
                    setTimeout(() => {
                        const canvasElement = document.querySelector("canvas");
                        takePhoto(videoElement, canvasElement);
                        setLayout("predicting");
                    }, 3000);
                },
                // errorCallback
                function(err) {
                    console.log("The following error occured: " + err);
                }
            );
        } else {
            console.log("getUserMedia not supported");
        }
    }

    setLayout("start");
    bindCamera();
```