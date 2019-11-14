function setVideoSource(width, height, videoParentElement) {
    const modelLabel = document.querySelector("#model-label");
    modelLabel.textContent = window.tensorflow.modelLabel;
    return navigator.mediaDevices.getUserMedia({ video: { width, height } })
        .catch(() => navigator.mediaDevices.getUserMedia({ video: { width: { exact: width }, height: { exact: height } } }))
        .then(stream => {
            const video = document.querySelector(videoParentElement + ' video');
            video.srcObject = stream;
        });
}

async function predict(width, height, threshold, videoParentElement) {
    const video = document.querySelector(videoParentElement + ' video');
    const canvas = document.createElement('canvas');
    canvas.width = 224;
    canvas.height = 224;
    const distx = (width - height) / 2;
    const ctx = canvas.getContext('2d');
    ctx.drawImage(video, distx, 0, width, height, 0, 0, 224, 224);
    const img = ctx.getImageData(0, 0, 224, 224).data;
    const imagedata = [];
    for (let i = 0; i < img.length; i += 4) {
        imagedata.push(img[i + 2], img[i + 1], img[i]);
    }

    const tensorflow = window.tensorflow;
    const tensor = tf.tensor1d(imagedata).reshape([1, 224, 224, 3]);
    const pred = await tensorflow.model.execute({ 'Placeholder': tensor }).reshape([tensorflow.labels.length]).data();
    if (Math.max(...pred) < threshold) return '-1';
    const prediction = tensorflow.labels[pred.indexOf(Math.max(...pred))];
    return String(prediction);
}

window.onload = async function () {
    const model = await tf.loadGraphModel('/model/model.json');
    const metaResponse = await fetch('/model/cvexport.manifest');
    const modelmeta = await metaResponse.json();
    const labelsResponse = await fetch('/model/labels.txt');
    const l = await labelsResponse.text();
    const labels = l
        .split(/\s/g)
        .filter(l => l !== '');
    const modelLabel = new Date(modelmeta.ExportedDate).toLocaleString();
    window.tensorflow = { model, modelmeta, labels, modelLabel };
};