# GitHub workflow secrets:
# AZURE_CREDENTIALS
# RESOURCE_GROUP
# CLUSTER_NAME
# CONTAINER_REGISTRY
# HOST_SUFFIX
# IMAGE_PULL_SECRET
# MASTER_SPACE
# REGISTRY_USERNAME
# REGISTRY_PASSWORD

Param(
    [parameter(Mandatory=$true)][string]$resourceGroup,
    [parameter(Mandatory=$false)][string]$spId
)

if (-not $spId) {
    # Create a service principal
    Write-Output "Creating service principal..."
    $spInfo=$(az ad sp create-for-rbac --sdk-auth true -o json | ConvertFrom-Json)
}
else {
    # Get existing service principal
    Write-Output "Retrieving existing service principal... ***NOTE: this is for script testing purposes only, Azure_Credentials will NOT be correct."
    $spInfo=$(az ad sp show -o json --id $spId | ConvertFrom-Json)
}

# Get AKS info
Write-Output "Retrieving AKS info..."
$aks=$(az aks list -g $resourceGroup --query "[0]" -o json | ConvertFrom-Json)

# Get ACR info
Write-Output "Retrieving Container Registry info..."
$acr=$(az acr list -g $resourceGroup --query "[0]" -o json | ConvertFrom-Json)

# Get ingress info
Write-Output "Retrieving Kubernetes ingress info..."
$ingress=$(kubectl get ingress -o json | ConvertFrom-Json)

Write-Output '========================================================='
Write-Output 'GitHub secrets for configuring GitHub workflow'
Write-Output '========================================================='

Write-Output "AZURE_CREDENTIALS: $($spInfo | ConvertTo-Json)"
Write-Output "RESOURCE_GROUP: $resourceGroup"
Write-Output "CLUSTER_NAME: $($aks.name)"
Write-Output "CONTAINER_REGISTRY: $($acr.loginServer)"
Write-Output "HOST: $($ingress.items[0].spec.rules[0].host)"
Write-Output "IMAGE_PULL_SECRET: demo-secret"
Write-Output "MASTER_SPACE: default"
Write-Output "REGISTRY_USERNAME: $($spInfo.clientId)"
Write-Output "REGISTRY_PASSWORD: $($spInfo.clientSecret)"
Write-Output '========================================================='



