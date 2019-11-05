Param(
    [parameter(Mandatory=$true)][string]$resourceGroup,
    [parameter(Mandatory=$false)][string]$acrName,
    [parameter(Mandatory=$false)][bool]$dockerBuild=$true,
    [parameter(Mandatory=$false)][bool]$dockerPush=$true,
    [parameter(Mandatory=$false)][string]$dockerTag="latest"
)

Push-Location $($MyInvocation.InvocationName | Split-Path)
$sourceFolder=$(./Join-Path-Recursively.ps1 -pathParts ..,..,Source)

if ([String]::IsNullOrEmpty($acrName)) {
    $acr = $(az acr list -g $resourceGroup --query "[0]" -o json | ConvertFrom-Json)
    if (-not $acr) {
        Write-Error "Parameter acrName not passed and $acr do not found in RG $resourceGroup"
        exit 1
    }
    $acrName = $acr.name
}

Write-Host "---------------------------------------------------" -ForegroundColor Yellow
Write-Host "Getting info from ACR $resourceGroup/$acrName" -ForegroundColor Yellow
Write-Host "---------------------------------------------------" -ForegroundColor Yellow
$acrLoginServer=$(az acr show -g $resourceGroup -n $acrName -o json | ConvertFrom-Json).loginServer
$acrCredentials=$(az acr credential show -g $resourceGroup -n $acrName -o json | ConvertFrom-Json)
$acrPwd=$acrCredentials.passwords[0].value
$acrUser=$acrCredentials.username
$dockerComposeFile= "docker-compose.yml"

if ($dockerBuild) {
    Write-Host "---------------------------------------------------" -ForegroundColor Yellow
    Write-Host "Using docker compose to build & tag images." -ForegroundColor Yellow
    Write-Host "Images will be named as $acrLoginServer/imageName:$dockerTag" -ForegroundColor Yellow
    Write-Host "---------------------------------------------------" -ForegroundColor Yellow

    Push-Location $sourceFolder
    $env:TAG=$dockerTag
    $env:REGISTRY=$acrLoginServer 
    docker-compose -f $dockerComposeFile build
    if (-not $?) { Pop-Location; exit 1 }
    Pop-Location
}

if ($dockerPush) {
    Write-Host "---------------------------------------------------" -ForegroundColor Yellow
    Write-Host "Pushing images to $acrLoginServer" -ForegroundColor Yellow
    Write-Host "---------------------------------------------------" -ForegroundColor Yellow

    docker login -p $acrPwd -u $acrUser $acrLoginServer
    if (-not $?) { exit 1 }

    $env:TAG=$dockerTag
    $env:REGISTRY=$acrLoginServer 

    Push-Location $sourceFolder
    docker-compose -f $dockerComposeFile push
    if (-not $?) { Pop-Location; exit 1 }
    Pop-Location
}

Pop-Location

exit $LastExitCode