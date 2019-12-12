--- 
description: "Code and tutorial for rock-paper-scissors-lizard-spock game used at Azure Custom Vision hands-on workshop"
languages: 
  - javascript
page_type: sample
products: 
  - azure
  - azure-cognitive-services
urlFragment: rock-paper-scissors-lizard-spock-customvision
---

# Rock-paper-scissors-lizard-spock
Code and tutorial for "rock-paper-scissors-lizard-spock" game used at Azure Custom Vision hands-on workshop.

## Contents
Outline the file contents of the repository. It helps users navigate the codebase, build configuration and any related assets.

| File/folder       | Description                                |
|-------------------|--------------------------------------------|
| `Step{1-6}`       | Sample source code.                        |
| `.gitignore`      | Define what to ignore at commit time.      |
| `README.md`       | This README file.                          |
| `LICENSE`         | The license for the sample.                |

# Prerequisites

1. Azure Subscribtion
2. Published Custom Vision iteration and access key
3. [NodeJS](https://nodejs.org/en/download/)

# Setup

## Azure Subscription

An ARM template is provided so you can create the whole infrastructure required for this training (an App Service with its Service Plan and two Cognitive Services).
Deploy pressing the following button and filling up the required parameters

<a href="https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FOriolBonjoch%2Frock-paper-scissors-customvision%2Fmaster%2Fdeployment.json"><img src="assets/deploy-to-azure.png" alt="Deploy to Azure"/></a>

* Fill resource group name and its location.
  ![Resource deployment form filled](assets/screenshots/0_resource_deployment_form_filled.png)

* Open your resource group and verify that you have the following resources: App Service Plan, App Service and two Cognitive Services

  ![Open your resource group and verify that you have the following resources: App Service Plan, App Service and two Cognitive Services](assets/screenshots/0_resources_deployed.png)

## Custom Vision

### Sign in https://www.customvision.ai/ using created Azure account 
  ![Sign in https://www.customvision.ai/ using created Azure account](assets/screenshots/0_customvision_signin.JPG)
  
### Create a new Custom Vision project
* Click "New Project"

![Click "New Project"](assets/screenshots/0_customvision_projects_blank.JPG)

* Fill-in the form (Name - "RPSLS", Resource - "rpsls-customvision [F0]", Project Type - "Classification", Classification Types - "Multiclass", Domains - "General") and click "Create project"

![Fill-in the form and click "Create project"](assets/screenshots/0_customvision_projects_new.JPG)

### Upload and tag images
* In Custom Vision project click "Add images"

![In Custom Vision project click "Add images"](assets/screenshots/0_customvision_project_addimages.JPG)

* Add images, select appropriate tag (e.g. paper) and click "Upload files"

![Add images, select appropriate tag (e.g. paper) and click "Upload files"](assets/screenshots/0_customvision_project_addimages_paper.JPG)
* Wait until upload is finished

![Wait until upload is finished](assets/screenshots/0_customvision_project_addimages_uploading.JPG)
![Wait until upload is finished](assets/screenshots/0_customvision_project_addimages_uploaded.JPG)

* Repeat for the other folders, wait until all images are uploaded

### Train a model
* In Custom Vision project click "Train"

![In Custom Vision project click "Train"](assets/screenshots/0_customvision_project_train.JPG)

* Select "Fast Training" and click "Train"

![Select "Fast Training" and click "Train"](assets/screenshots/0_customvision_project_train_setup.JPG)

* Wait for training to finish

![Wait for training to finish](assets/screenshots/0_customvision_project_train_done.JPG)

### Manual validation

* Click on "Quick Test"

![Click on "Quick Test"](assets/screenshots/0_customvision_project_iteration_quicktest.JPG)

* Upload test image

![Upload test image](assets/screenshots/0_customvision_project_iteration_quicktest_results.JPG)

### Prediction correction

* Click on "Predictions"

![Click on "Predictions"](assets/screenshots/0_customvision_project_iteration_predictions.JPG)

* Select incorrectly predicted image

![Select incorrectly predicted image](assets/screenshots/0_customvision_project_iteration_predictions_list.JPG)

* Assign correct tag and click "Save and close"

![Select incorrectly predicted image](assets/screenshots/0_customvision_project_iteration_predictions_detail.JPG)

### (Optional) Advanced training
* In Custom Vision project click "Train"

![In Custom Vision project click "Train"](assets/screenshots/0_customvision_project_train.JPG)

* Select "Advanced Training" for 1 hour and click "Train"

![Select "Advanced Training" for 1 hour and click "Train"](assets/screenshots/0_customvision_project_train_setup_adv.JPG)

* Wait for training to finish

![Wait for training to finish](assets/screenshots/0_customvision_project_train_done.JPG)

### Publish iteration
* Open "Performance" tab, select finished iteration and click "Publish"

![Wait for training to finish](assets/screenshots/0_customvision_project_iteration_publish.JPG)

* Fill-in model name, select "RPSCustomVision_Prediction" as prediction resource and click "Publish"

![Fill-in model name, select "RPSCustomVision_Prediction" as prediction resource and click "Publish"](assets/screenshots/0_customvision_project_iteration_publish_setup_1.JPG)
![Fill-in model name, select "RPSCustomVision_Prediction" as prediction resource and click "Publish"](assets/screenshots/0_customvision_project_iteration_publish_setup_2.JPG)

* Click on "Prediction URL" to see your credentials

![Wait for training to finish](assets/screenshots/0_customvision_project_iteration_prediction.JPG)


## Web application code
Application based on code from [NodeJS app on Azure](https://docs.microsoft.com/en-us/azure/app-service/app-service-web-get-started-nodejs) example.

### Structure

* index.js - server-side logic
* public/index.html - WebApp HTML template
* public/css/app.css - WebApp CSS styles
* public/js/app.js - WebApp Javascript logic

### Local development 
Local development is the same for all the steps:
1. Open "StepN" folder
2. Run `npm install`
3. Run `npm start`
4. Open http://localhost:1337

### Azure deployment 
Azure deployment is the same for all the steps:
1. Open "StepN" folder
2. Create zip archive with contents of "StepN" folder, except for node_modules folder
3. Open <choose_unique_name>.scm.azurewebsites.net
4. navigate to Tools -> Zip Push Deploy
5. Drag-and-drop zip archive to /wwwroot
6. Open <choose_unique_name>.azurewebsites.net and verify that site is running

Code changes are described in respective steps:

[Step 1](Step1/README.md)

[Step 2](Step2/README.md)

[Step 3](Step3/README.md)

[Step 4](Step4/README.md)

[Step 5](Step5/README.md)

[Step 6 (Final)](Step6/README.md)


# Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.opensource.microsoft.com.

When you submit a pull request, a CLA bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
