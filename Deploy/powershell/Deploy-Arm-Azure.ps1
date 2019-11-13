Param(
    [parameter(Mandatory=$true)][string]$resourceGroup,
    [parameter(Mandatory=$false)][string]$location,
    [parameter(Mandatory=$false)][string]$clientId="",
    [parameter(Mandatory=$false)][string]$password="",
    [parameter(Mandatory=$false)][string]$objectId="",
    [parameter(Mandatory=$false)][bool]$deployKv=$true,
    [parameter(Mandatory=$false)][string]$aksVersion="",
    [parameter(Mandatory=$false)] 
        [ValidateSet('','S1','S2','S3','B1','B2','B3','P1','P2','P3','P1V1','P1V2','P1V3','F1')][string]$alternativeFuncSvcPlan='',
    [parameter(Mandatory=$false)][bool]$tryUseConsumptionPlan=$true
)

function SkuNameToTier($skuName) {
    if ($skuName -eq 'S1' -or $skuName -eq 'S2' -or $skuName -eq 'S3') {
        return "Standard"
    }
    if ($skuName -eq 'B1' -or $skuName -eq 'B2' -or $skuName -eq 'B3') {
        return "Basic"
    }
    if ($skuName -eq 'P1' -or $skuName -eq 'P2' -or $skuName -eq 'P3') {
        return "Premium"
    }
    if ($skuName -eq 'P1V1' -or $skuName -eq 'P1V2' -or $skuName -eq 'P1V3') {
        return "PremiumV2"
    }    
    if ($skuName -eq 'F1') {
        return "Free"
    }    
    if ($skuName -eq 'Y1') {
        return "Dynamic"
    }   
    return ""
}

$sourceFolder=$(Join-Path -Path .. -ChildPath arm)

$spCreated=$false
$script="deployment.json"

Write-Host "--------------------------------------------------------" -ForegroundColor Yellow
Write-Host "Deploying ARM script $script" -ForegroundColor Yellow
Write-Host "-------------------------------------------------------- " -ForegroundColor Yellow

$rg = $(az group show -n $resourceGroup -o json | ConvertFrom-Json)

if (-not $rg) {
    if (-not $location) {
        Write-Host "Location is mandatory if resource group does not exist" -ForegroundColor Yellow
        exit 1
    }

    Write-Host "Creating resource group $resourceGroup in $location" -ForegroundColor Yellow
    az group create -n $resourceGroup -l $location
    $rg = $(az group create -n $resourceGroup -l $location -o json | ConvertFrom-Json)
}
else 
{
    $location = $rg.location
    Write-Host "Resource group inferred location is: $location" -ForegroundColor Yellow
}


if ($tryUseConsumptionPlan) {
    $funcSvcPlan="Y1"
    $hasConsumptionAvailable=$(az functionapp list-consumption-locations --query "[?name=='$location'].name" -o json | ConvertFrom-Json)[0]
    if (-not $hasConsumptionAvailable) {
        Write-Host "Location $location does NOT support consumption plan." -ForegroundColor Yellow
        if (-not [String]::IsNullOrEmpty($alternativeFuncSvcPlan)) {
            Write-Host "Deploying using App Service Plan $alternativeFuncSvcPlan for FuncApp"
            $funcSvcPlan=$alternativeFuncSvcPlan
        }
        else {
            Write-Host "Can't deploy in location $location because Consumption Plan is not supported and no alternative function svc plan has been specified" -ForegroundColor Yellow
            exit 1
        }
    } 
}
else {
    if ([String]::IsNullOrEmpty($alternativeFuncSvcPlan)) {
        Write-Host "Can't deploy in location $location because no svc plan type has been specified" -ForegroundColor Yellow
        exit 1
    }
    $funcSvcPlan=$alternativeFuncSvcPlan
}


$aks = $(az aks list -g $resourceGroup --query "[0]" | ConvertFrom-Json)
if (-not $aks) {
    Write-Host "AKS not present in rg $resourceGroup. It will be created" -ForegroundColor Yellow
    Write-Host "Getting last AKS version in location $location" -ForegroundColor Yellow
    if ([String]::IsNullOrEmpty($aksVersion)) {
        $aksVersions=$(az aks get-versions -l $location --query  orchestrators[].orchestratorVersion -o json | ConvertFrom-Json)
        $aksVersion=$aksVersions[$aksVersions.Length-1]
        Write-Host "AKS last version is $aksVersion. Will use that" -ForegroundColor Yellow
    }
    else {
        Write-Host "AKS version provided is $aksVersion"  -ForegroundColor Yellow
    }

    if (-not $clientId -or -not $password) {
        Write-Host "Service principal will be created..." -ForegroundColor Yellow
        $sp = $(az ad sp create-for-rbac -o json | ConvertFrom-Json)
        $clientId = $sp.appId
        $password = $sp.password
        $spCreated=$true
    }
}
else {
    Write-Host "AKS already present" -ForegroundColor Yellow
    if (-not $clientId) {
        Write-Host "Retrieving AKS service principal details to use with Vault..." -ForegroundColor Yellow
        $clientId=$aks.servicePrincipalProfile.clientId
    }
}

if ($deployKv) {
    if ([String]::IsNullOrEmpty($objectId)) {
        $objectId = $(az ad sp show --id $clientId --query "objectId" -o tsv)
        if (-not $objectId) {
            Write-Host "Not enough permissions to get the ObjectId of AKS SP $clientId." -ForegroundColor Yellow
            $deployKv=$false
        }
        else {
            Write-Host "ObjectId: $objectId (belongs to clientId: $clientId) - used in Key Vault" -ForegroundColor Yellow
        }
    }
    else {
        Write-Host "WARNING: Will use specified objectid ($objectId) for Key Vault. If this ObjectId do not belong to clientid ($clientId) RPSLS won't work as expected." -ForegroundColor Yellow
        Write-Host "This is because deployment assumes that the ClientId used to create the AKS is the one that have granted permissions to KeyVault" -ForegroundColor Yellow
    }
}

if (-not $deployKv) {
    Write-Host "WARNING: Key vault won't be deployed." -ForegroundColor Yellow
}
$funcSvcPlanTier = SkuNameToTier($funcSvcPlan)
Write-Host "Function App will be deployed under a Service Plan of type $funcSvcPlan ($funcSvcPlanTier)" -ForegroundColor yellow

Push-Location $($MyInvocation.InvocationName | Split-Path)
Push-Location $sourceFolder
Write-Host "Begining the ARM deployment..." -ForegroundColor Yellow
az group deployment create -g $resourceGroup --template-file $script --parameters servicePrincipalId=$clientId --parameters servicePrincipalSecret=$password --parameters aksVersion=$aksVersion --parameters kv_objectId=$objectId --parameters funcAppSkuName=$funcSvcPlan --parameters funcAppSkuTier=$funcSvcPlanTier
$armExitCode=$LastExitCode

Pop-Location
Pop-Location

if ($spCreated) {
    Write-Host "-----------------------------------------" -ForegroundColor Yellow
    Write-Host "Details of the Service Principal Created:" -ForegroundColor Yellow
    Write-Host ($sp | ConvertTo-Json) -ForegroundColor Yellow
    Write-Host "-----------------------------------------" -ForegroundColor Yellow
}

exit $armExitCode
