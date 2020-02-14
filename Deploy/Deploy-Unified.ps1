Param(
    [parameter(Mandatory=$true)][string]$resourceGroup,
    [parameter(Mandatory=$true)][string]$location,
    [parameter(Mandatory=$false)][string]$subscription="",
    [parameter(Mandatory=$true)][string]$clientId,
    [parameter(Mandatory=$true)][string]$password,
    [parameter(Mandatory=$false)][string]$spObjectId,
    [parameter(Mandatory=$false)][string]$tag="latest",
    [parameter(Mandatory=$false)][bool]$deployGlobalSecret=$false
)

$gValuesFile="configFile.yaml"

Push-Location $($MyInvocation.InvocationName | Split-Path)

## Connecting kubectl to AKS
Write-Host "Login in your account" -ForegroundColor Yellow
# az login

if (-not [String]::IsNullOrEmpty($subscription)) {
    Write-Host "Choosing your subscription" -ForegroundColor Yellow
    az account set --subscription $subscription
}

Push-Location powershell

## Deploy ARM
if ($spObjectId) {
    & ./Deploy-Arm-Azure.ps1 -resourceGroup $resourceGroup -location $location -clientId $clientId -password $password -objectId $spObjectId
} else {
    & ./Deploy-Arm-Azure.ps1 -resourceGroup $resourceGroup -location $location -clientId $clientId -password $password
}
if (-not $?) { Pop-Location; Pop-Location; exit 1 }

Write-Host "Retrieving Aks Name" -ForegroundColor Yellow
$aksName = $(az aks list -g $resourceGroup -o json | ConvertFrom-Json).name
Write-Host "The name of your AKS: $aksName" -ForegroundColor Yellow

Write-Host "Retrieving credentials" -ForegroundColor Yellow
az aks get-credentials -n $aksName -g $resourceGroup --overwrite-existing

# Create KeyVault FlexVolume
& ./Create-Kv-FlexVolume.ps1
if (-not $?) { Pop-Location; Pop-Location; exit 1 }

if ($deployGlobalSecret) {
    # Deploys the global secret to access keyvault. Secret can also be installed
    # as part of game-api chart
    kubectl create secret generic game-api-kv --from-literal clientid=$clientId --from-literal clientsecret=$password --type=azure/kv
}

# Set up Dev Spaces
Write-Host "Setting up Azure Dev Spaces for AKS: $aksName"
& ./Setup-Dev-Spaces.ps1 -resourceGroup $resourceGroup -aksName $aksName -rootSpace default

# Deploy Azure function in order to have its key on helm values.
$funcapp=$(az functionapp list -g $resourceGroup --query "[0]" -o json | ConvertFrom-Json)
if ($funcapp) {
    $azFunctionLocation=$(./Join-Path-Recursively.ps1 "..","..","Source","Functions","RPSLS.Python.Api")
    Push-Location $azFunctionLocation
    func azure functionapp publish $($funcapp.name) --no-build
    Pop-Location
}

# Generate Config
$gValuesLocation=$(./Join-Path-Recursively.ps1 "..","helm","__values",$gValuesFile)
& ./Generate-Config.ps1 -resourceGroup $resourceGroup -outputFile $gValuesLocation
if (-not $?) { Pop-Location; Pop-Location; exit 1 }

# Build and Push
& ./Build-Push.ps1 -resourceGroup $resourceGroup -dockerTag $tag
if (-not $?) { Pop-Location; Pop-Location; exit 1 }

# Deploy images in AKS
$needToDeployKvSecret= -not $deployGlobalSecret
$gValuesLocation=$(./Join-Path-Recursively.ps1 "__values", $gValuesFile)
& ./Deploy-Images-Aks.ps1 -kvPassword $password -aksName $aksName -resourceGroup $resourceGroup -charts "*" -valuesFile $gValuesLocation -tag $tag -deployKvSecret $needToDeployKvSecret

Pop-Location
Pop-Location