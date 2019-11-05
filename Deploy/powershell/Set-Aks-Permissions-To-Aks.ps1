Param(
    [parameter(Mandatory=$true)][string]$aksName,
    [parameter(Mandatory=$true)][string]$acrName,
    [parameter(Mandatory=$true)][string]$resourceGroup
)


# Get the id of the service principal configured for AKS
$clientId=$(az aks show --resource-group $resourceGroup --name $aksName --query "servicePrincipalProfile.clientId" --output tsv)

# Get the ACR registry resource id
$acrId=$(az acr show --name $acrName --resource-group $resourceGroup --query "id" --output tsv)

# Create role assignment
az role assignment create --assignee $clientId --role acrpull --scope $acrId