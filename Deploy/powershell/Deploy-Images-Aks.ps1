Param(
    [parameter(Mandatory=$true)][string]$aksName,
    [parameter(Mandatory=$false)][string]$aksHost="",
    [parameter(Mandatory=$true)][string]$resourceGroup,
    [parameter(Mandatory=$false)][string]$acrName,
    [parameter(Mandatory=$false)][string]$name = "rpsls",
    [parameter(Mandatory=$false)][string]$tag="latest",
    [parameter(Mandatory=$false)][string]$charts = "*",
    [parameter(Mandatory=$false)][string]$valuesFile = "gvalues.yaml",
    [parameter(Mandatory=$false)][bool]$kvDeploy=$true,
    [parameter(Mandatory=$false)][string]$kvPassword="",
    [parameter(Mandatory=$false)][bool]$deployKvSecret=$true,
    [parameter(Mandatory=$false)][string]$namespace=""
)
function validate {
    $valid = $true

    if ($kvDeploy) {
        if ([String]::IsNullOrEmpty($kvPassword) -and $deployKvSecret) {
            Write-Host "If Keyvault support is deployed and secret must be created, then kvPassword must be passed, with the pwd of SP with access to KeyVault"
            $valid = $false
        }
    }

    if ($valid -eq $false) {
        exit 1
    }
}

function createHelmCommand([string]$command) {
    $newcommand = $command

    if (-not [string]::IsNullOrEmpty($namespace)) {
        $newcommand = "$newcommand --namespace $namespace" 
    }

    return "$newcommand";
}

if ([String]::IsNullOrEmpty($acrName)) {
    $acr = $(az acr list -g $resourceGroup --query "[0]" -o json | ConvertFrom-Json)
    if (-not $acr) {
        Write-Error "Parameter acrName not passed and $acr do not found in RG $resourceGroup"
        exit 1
    }
    $acrName = $acr.name
}

Write-Host "--------------------------------------------------------" -ForegroundColor Yellow
Write-Host " Deploying images on cluster $aksName"  -ForegroundColor Yellow
Write-Host " "  -ForegroundColor Yellow
Write-Host " Additional parameters are:"  -ForegroundColor Yellow
Write-Host " Release Name: $name"  -ForegroundColor Yellow
Write-Host " AKS to use: $aksName in RG $resourceGroup and ACR $acrName"  -ForegroundColor Yellow
Write-Host " Images tag: $tag"  -ForegroundColor Yellow
Write-Host " Deploying Key Vault support:  $kvDeploy"
Write-Host " --------------------------------------------------------" 

if ($kvDeploy -and [String]::IsNullOrEmpty($kvName)) {
    Write-Host "Deploying for Key Vault but no Key Vault specified. Retrieving from RG $resourceGroup" -ForegroundColor Yellow
    $kvName=$(az keyvault list -g $resourceGroup --query "[0].name")
    Write-Host "KeyVault '$kvName' found in RG $resourceGroup" -ForegroundColor Yellow
}

validate

$acrLogin=$(az acr show -n $acrName -o json| ConvertFrom-Json).loginServer
$aks=$(az aks show -n $aksName -g $resourceGroup | ConvertFrom-Json)

if (-not $aks) {
    Write-Error "No AKS found in RG $resourceGroup"
    exit 1
}

if (-not $aksHost) {
    $aksHost=$aks.addonProfiles.httpApplicationRouting.config.HTTPApplicationRoutingZoneName
}


Push-Location $($MyInvocation.InvocationName | Split-Path)
Push-Location $(Join-Path .. helm)

Write-Host "Deploying charts $charts" -ForegroundColor Yellow
Write-Host "Configuration file used is $valuesFile" -ForegroundColor Yellow

if ($charts.Contains("cs") -or  $charts.Contains("*")) {
    Write-Host "C# chart - cs" -ForegroundColor Yellow
    $command = "helm upgrade --install $name-dotnet dotnet-player -f $valuesFile --set ingress.hosts='{$aksHost}' --set image.repository=$acrLogin/dotnet.player --set image.tag=$tag"
    $command = createHelmCommand $command 
    Invoke-Expression "$command"
    if (-not $?) { Pop-Location; Pop-Location; exit 1 }
}

if ($charts.Contains("nj") -or  $charts.Contains("*")) {
    Write-Host "NodeJS chart - nj" -ForegroundColor Yellow
    $command = "helm upgrade --install $name-node node-player -f $valuesFile --set ingress.hosts='{$aksHost}' --set image.repository=$acrLogin/node.player --set image.tag=$tag"
    $command = createHelmCommand $command 
    Invoke-Expression "$command"
    if (-not $?) { Pop-Location; Pop-Location; exit 1 }
}

if ($charts.Contains("jv") -or  $charts.Contains("*")) {
    Write-Host "Java chart - jv" -ForegroundColor Yellow
    $command = "helm upgrade --install $name-java java-player -f $valuesFile --set ingress.hosts='{$aksHost}' --set image.repository=$acrLogin/java.player --set image.tag=$tag"
    $command = createHelmCommand $command 
    Invoke-Expression "$command"
    if (-not $?) { Pop-Location; Pop-Location; exit 1 }
}

if ($charts.Contains("py") -or  $charts.Contains("*")) {
    Write-Host "Python chart - py" -ForegroundColor Yellow
    $command = "helm upgrade --install $name-python python-player -f $valuesFile --set ingress.hosts='{$aksHost}' --set image.repository=$acrLogin/python.player --set image.tag=$tag"
    $command = createHelmCommand $command 
    Invoke-Expression "$command"
    if (-not $?) { Pop-Location; Pop-Location; exit 1 }
}

if ($charts.Contains("php") -or  $charts.Contains("*")) {
    Write-Host "PHP chart - php" -ForegroundColor Yellow
    $command = "helm upgrade --install $name-php php-player -f $valuesFile --set ingress.hosts='{$aksHost}' --set image.repository=$acrLogin/php.player --set image.tag=$tag"
    $command = createHelmCommand $command 
    Invoke-Expression "$command"
    if (-not $?) { Pop-Location; Pop-Location; exit 1 }
}

if ($charts.Contains("gm") -or  $charts.Contains("*")) {
    Write-Host "Game Api chart - gm" -ForegroundColor Yellow
    $command = "helm upgrade --install $name-game game-api -f $valuesFile --set ingress.hosts='{$aksHost}' --set image.repository=$acrLogin/game.api --set image.tag=$tag "
    if ($kvDeploy) {
        $command="$command --set kv.deploySecret=$deployKvSecret --set kv.enabled=true --set kv.clientsecret=$kvPassword "
    }

    $command = createHelmCommand $command 
    Invoke-Expression "$command"
    if (-not $?) { Pop-Location; Pop-Location; exit 1 }
}

if ($charts.Contains("web") -or  $charts.Contains("*")) {
    Write-Host "Web chart - web" -ForegroundColor Yellow
    $command = "helm upgrade --install $name-web web -f $valuesFile --set ingress.hosts='{$aksHost}' --set image.repository=$acrLogin/web --set image.tag=$tag --set image.uploaderRepository=$acrLogin/modeluploader --set image.uploaderTag=$tag"
    $command = createHelmCommand $command 
    Invoke-Expression "$command"
    if (-not $?) { Pop-Location; Pop-Location; exit 1 }
}

Pop-Location
Pop-Location

Write-Host "Rock Paper Scissors Lizard Spock deployed on AKS" -ForegroundColor Yellow