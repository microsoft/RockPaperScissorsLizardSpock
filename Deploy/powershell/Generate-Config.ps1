Param (
    # Resource group where AKS is deployed
    [parameter(Mandatory=$true)][string]$resourceGroup,
    # Resource group where ACR is deployed (if not set, same RG as AKS is assumed)
    [parameter(Mandatory=$false)][string]$resourceGroupAcr="",
    # Subscription where ACR is deployed (if not set, current subscription is assumed)
    [parameter(Mandatory=$false)][string]$subscriptionAcr="",
    # Subscription where AKS is deployed (if not set, current subscription is assumed)
    [parameter(Mandatory=$false)][string]$subscription="",
    # Output file to generate
    [parameter(Mandatory=$True)][string]$outputFile,
    # Template to use
    [parameter(Mandatory=$false)][string]$gvaluesTemplate="gvalues.template",
    # Ingress class to generate
    [parameter(Mandatory=$false)][string]$ingressClass="addon-http-application-routing",
    # Twitter Key (needed for web)
    [parameter(Mandatory=$false)][string]$twitterKey,
    # Twitter secret (needed for web)
    [parameter(Mandatory=$false)][string]$twitterSecret,
    # AKS Host to use (for ingress). If not set, the value of Http Application Routing is assumed
    [parameter(Mandatory=$false)][string]$aksHost="",
    # SSL support to ad (prod, staging, custom, none)
    [parameter(Mandatory=$false)][string][ValidateSet('prod','staging','none','custom', IgnoreCase=$false)]$sslSupport = "none",
    # Client ID of the SP who was access to Key Vault. If not set, SP used to create AKS is assumed
    [parameter(Mandatory=$false)][string]$kvClientId="",
    # K8S secret name for storing the TLS certificate
    [parameter(Mandatory=$false)][string]$tlsSecretName="rpsls-tls-custom",
    # Set the docker registry login server to use a generic Docker registry (instead of ACR)
    [parameter(Mandatory=$false)][string]$registryLogin="",
    # Set the docker registry user to use a generic Docker registry (instead of ACR)
    [parameter(Mandatory=$false)][string]$registryUser="",
    # Set the docker registry password to use a generic Docker registry (instead of ACR)
    [parameter(Mandatory=$false)][string]$registryPassword="",
    # Set to $true to use a custom docker registry. False otherwise (default value)
    [parameter(Mandatory=$false)][bool]$useCustomRegistry=$false,
    #Set the Google Analytics Id if needed.
    [parameter(Mandatory=$false)][string]$googleanalytics=""
)

function EnsureAndReturnFistItem($arr, $restype) {
    if (-not $arr -or $arr.Length -ne 1) {
        Write-Host "Fatal: No $restype found (or found more than one)" -ForegroundColor Red
        exit 1
    }
    return $arr[0]
}

# Check the rg
$rg=$(az group show -n $resourceGroup -o json | ConvertFrom-Json)

if (-not $rg) {
    Write-Host "Fatal: Resource group '$resourceGroup' not found" -ForegroundColor Red
    exit 1
}

if ([String]::IsNullOrEmpty($resourceGroupAcr)) {
    $resourceGroupAcr = $resourceGroup
}

if ([String]::IsNullOrEmpty($subscription)) {
    $subscription=$(az account show -o json | ConvertFrom-Json).id
}


if (-not $useCustomRegistry) {
    $rgAcr=$(az group show -n $resourceGroup -o json | ConvertFrom-Json)
    if (-not $rgAcr) {
        Write-Host "Fatal: Resource group '$resourceGroupAcr' for ACR not found" -ForegroundColor Red
        exit 1
    }
    if ([String]::IsNullOrEmpty($subscriptionAcr)) {
        $subscriptionAcr=$(az account show -o json | ConvertFrom-Json).id
    }
}
else {
    if ([String]::IsNullOrEmpty($registryLogin) -or [String]::IsNullOrEmpty($registryUser) -or [String]::IsNullOrEmpty($registryUser)) {
        Write-Error "If useCustomRegistry is true, must set registryLogin, registryUser and registryUser"
        exit 1
    }
}

Write-Host "=======================================" -ForegroundColor Yellow
Write-Host "RG: $subscription/$resourceGroup" -ForegroundColor Yellow
if (-not $useCustomRegistry) {
    Write-Host "RG ACR: $subscriptionAcr/$resourceGroupAcr" -ForegroundColor Yellow
}
else {
    Write-Host "Using custom Docker registry $registryLogin" -ForegroundColor Yellow
}
Write-Host "aksHost (if empty will use Http App Routing value): $aksHost" -ForegroundColor Yellow
Write-Host "=======================================" -ForegroundColor Yellow

### Getting Resources
$tokens=@{}

# Standard fixed tokens
$tokens.ingressclass=$ingressClass

$appinsights=$(az monitor app-insights component show --app rpsls-app-insights -g $resourceGroup -o json --subscription $subscription | ConvertFrom-Json)
if ($appinsights) {
    Write-Host "App Insights Instrumentation Key: $($appinsights)" -ForegroundColor Yellow
    $tokens.appinsightsik=$appinsights.instrumentationKey
}
else {
    $tokens.appinsightsik=""
}

#Google Analytics
$tokens.googleanalyticsid=$googleanalytics

if (-not $useCustomRegistry) {
    # get the acr name, username, and password
    $acrName=$(az acr list -g $resourceGroupAcr -o json --subscription $subscriptionAcr  | ConvertFrom-Json).name
    $acrCredentials = $(az acr credential show -n $acrName -g $resourceGroupAcr -o json --subscription $subscriptionAcr  | ConvertFrom-Json)
    if ($acrCredentials) {
        Write-Host "Setting ACR Credentials..." -ForegroundColor Yellow
        $acrPwd=$acrCredentials.passwords[0].value
        $acrUser=$acrCredentials.username
        $tokens.acrName=$acrName
        $tokens.acrLogin=$acrUser
        $tokens.acrPassword=$acrPwd
    }
}
else {
    Write-Host "Setting Docker registry credentials..."
    $otkens.acrName=$registryLogin
    $tokens.acrLogin=$registryUser
    $tokens.acrPassword=$registryPassword
}

$aks=$(az aks list -g $resourceGroup --query "[0]" --subscription $subscription -o json | ConvertFrom-Json)

if ([String]::IsNullOrEmpty($aksHost)) {
    # get the AKS hostname
    $aksHost=$aks.addonProfiles.httpApplicationRouting.config.HTTPApplicationRoutingZoneName
}
$tokens.aksHost=$aksHost
# Enable or disable the TLS ingress endpoints support based on sslSupport parameter
if ($sslSupport -eq "none") {
    $tokens.tlsEnabled="false"
}
else {
    $tokens.tlsEnabled="true"
    if ($sslSupport -eq "staging") {
        $tlsSecretName="rpsls-letsencrypt-staging"
    }
    elseif ($sslSupport -eq "prod") {
        $tlsSecretName="rpsls-letsencrypt-prod"
    }
}
$tokens.tlsSecretName=$tlsSecretName

# get the Key Vayult name, tenant and clientid
$kv=$(az keyvault list -g $resourceGroup --query "[0]" --subscription $subscription -o json | ConvertFrom-Json)
$tokens.kvName = $kv.name
$tokens.kvTenant = $kv.properties.tenantId
if (-not $kvClientId) {
    $tokens.kvClientId = $aks.servicePrincipalProfile.clientId
}
else {
    $tokens.kvClientId=$kvClientId
}

$tokens.twitterKey = $twitterKey
$tokens.twitterSecret = $twitterSecret

# get the function access key
$funcapp=$(az functionapp list -g $resourceGroup --query "[0]" -o json --subscription $subscription | ConvertFrom-Json)
$funcname="NextMove"

Write-Host "Found funcapp $($funcapp.name) in RG $resourceGroup"

# bug in code - https://github.com/microsoft/RockPaperScissorsLizardSpock/issues/3
# $funckeys=$(az rest --method post --uri "https://management.azure.com$($funcapp.id)/functions/$funcname/listKeys?api-version=2018-02-01" -o json --subscription $subscription | ConvertFrom-Json)
$funckeys=$(az rest --method post --uri "https://management.azure.com$($funcapp.id)/host/default/listKeys?api-version=2018-02-01" -o json --subscription $subscription | ConvertFrom-Json)

$tokens.predictorbaseurl="https://$($funcapp.defaultHostName)/api/challenger/move?code=$($funckeys.default)"

Write-Host "===========================================================" -ForegroundColor Yellow
Write-Host "gvalues file will be generated with values:"
Write-Host ($tokens | ConvertTo-Json) -ForegroundColor Yellow
Write-Host "===========================================================" -ForegroundColor Yellow

Push-Location $($MyInvocation.InvocationName | Split-Path)
Write-Host "Saving the values yaml at $outputFile (template is $gvaluesTemplate)" -ForegroundColor Yellow
& .\Token-Replace.ps1 -inputFile $gvaluesTemplate -outputFile $outputFile -tokens $tokens
Pop-Location