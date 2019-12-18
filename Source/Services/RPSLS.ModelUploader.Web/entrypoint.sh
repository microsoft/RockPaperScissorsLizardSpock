#!/bin/bash
mkdir -p /app/${UploadPath:-model} 
if [ -d "/app/initialModel" ]; then
    cp /app/initialModel/* /app/${UploadPath:-model}  
fi
dotnet RPSLS.ModelUploader.Web.dll