The downloaded Custom vision model files go here.

An initial pre-trained version is in the folder "initialModel" of the model uploader (RPSLS.ModelUploader.Web project).
The entrypoint.sh of the model uploader copies the contents of "initialModel" to the folder "model" of the "model uploader. This folder "model" of the model uploader should be shared (via volumes) to this folder of the web.