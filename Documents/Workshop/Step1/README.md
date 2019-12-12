# First step setup

After cloning the repository, you can either 

### Develop locally 
Local development is the same for all steps:
1. Open "StepN" folder
2. Run `npm install`
3. Run `npm start`
4. Open http://localhost:1337

### Deploy to Azure 
Azure deployment is the same for all steps:
1. Open "StepN" folder
2. Run `npm install`
3. Create zip archive with contents of "StepN" folder
4. Open <choose_unique_name>.scm.azurewebsites.net
5. navigate to Tools -> Zip Push Deploy
6. Drag-and-drop zip archive to /wwwroot
7. Open <choose_unique_name>.azurewebsites.net and verify that site is running