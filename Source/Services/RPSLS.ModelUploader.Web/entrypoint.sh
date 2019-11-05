#!/bin/bash
mkdir -p /app/${UploadPath:-model} 
if [ -d "/initialModel" ]; then
    cp /initialModel/* /app/${UploadPath:-model}  
fi
dotnet RPSLS.ModelUploader.Web.dll