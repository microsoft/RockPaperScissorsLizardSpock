Param(
    [parameter(Mandatory=$true)][string]$resourceGroup,
    [parameter(Mandatory=$false)][string]$aksHost="",
    [parameter(Mandatory=$false)][string]$funcappName="",
    [parameter(Mandatory=$false)][bool]$tlsEnabled=$true
)

$rg = $(az group show -n $resourceGroup -o json | ConvertFrom-Json)

if (-not $rg) {
    Write-Error ("RG $resourceGroup not found.")
    exit 1
}

if ([String]::IsNullOrEmpty($aksHost)) {
    $aks=$(az aks list -g $resourceGroup --query "[0]"  | ConvertFrom-Json)
    if (-not $aks) {
        Write-Error "No AKS found in RG $resourceGroup. If AKS is not deployed then aksHost must be specified"
        exit 1
    }
    $aksHost=$aks.addonProfiles.httpApplicationRouting.config.HTTPApplicationRoutingZoneName
}

if ([String]::IsNullOrEmpty($funcappName)) {
    $funcapp=$(az functionapp list -g $resourceGroup --query "[0]" -o json  | ConvertFrom-Json)

    if (-not $funcapp) {
        Write-Error "No Func App found in RG $resourceGroup"
        exit 1
    }
    else {
        Write-Host "Found funcapp $($funcapp.name) in RG $resourceGroup" -ForegroundColor Yellow
        $funcappName=$funcapp.name
    }
}
else {
    $funcapp=$(az functionapp show -g $resourceGroup -n $funcappName  -o json  | ConvertFrom-Json)
    if (-not $funcapp) {
        Write-Error "Funcapp $funcappName not found in RG $resourceGroup"
        exit 1
    }
}

$aksHost="https://$aksHost"
if (-not $tlsEnabled) {
    $aksHost = "http://$aksHost"
}

$command="az functionapp config appsettings set --name $($funcapp.name) --settings GAME_MANAGER_URI=$aksHost --resource-group $resourceGroup"

Invoke-Expression $command


