Param(
    [parameter(Mandatory=$true)][string]$resourceGroup,
    [parameter(Mandatory=$false)][string]$resourceGroupAcr="",
    [parameter(Mandatory=$false)][string]$subscription="",
    [parameter(Mandatory=$false)][string]$subscriptionAcr="",
    [parameter(Mandatory=$false)][string]$setVariables="y",
    [parameter(Mandatory=$false)][string]$varsToSet="*",
    [parameter(Mandatory=$false)][string]$aksHost="",
    [parameter(Mandatory=$false)][string]$acrName=""
)

if ($setVariables -eq "true" -or $setVariables -eq "y") {

    if ([String]::IsNullOrEmpty($subscription)) {
        $subscription=$(az account show -o json | ConvertFrom-Json).id
    }    

    if ([String]::IsNullOrEmpty($subscriptionAcr)) {
        $subscriptionAcr=$(az account show -o json | ConvertFrom-Json).id
    }

    if ([String]::IsNullOrEmpty($resourceGroupAcr)) {
        $resourceGroupAcr = $resourceGroup
    }    


    Write-Host "======================================" -ForegroundColor Yellow
    Write-Host "AKS RG: $subscription/$resourceGroup" -ForegroundColor Yellow
    if ([String]::IsNullOrEmpty($acrName))  {
        Write-Host "ACR RG: $subscriptionAcr/$resourceGroupAcr" -ForegroundColor Yellow
    }
    Write-Host "======================================" -ForegroundColor Yellow

    $aks=$(az aks list -g $resourceGroup --query "[0]" --subscription $subscription | ConvertFrom-Json)
    if ($null -eq $aks) {
        Write-Error "No aks found in RG $resourceGroup"
        exit 1
    }
    else {
        Write-Host "Found AKS $($aks.name) in RG $resourceGroup" -ForegroundColor Yellow
    }

    if ([String]::IsNullOrEmpty($acrName)) {
        $acr=$(az acr list -g $resourceGroupAcr --query "[0]" --subscription $subscriptionAcr | ConvertFrom-Json)
        if ($null -eq $acr) {
            Write-Error "No acr found in RG $resourceGroupAcr"
            exit 1
        }
        else {
            Write-Host "Found ACR $($acr.name) in RG $resourceGroupAcr" -ForegroundColor Yellow
            $acrName=$acr.name
        }    
    }

    $keyvaultname=$(az keyvault list -g $resourceGroup --query "[0]" --subscription $subscription | ConvertFrom-Json).name
    if (-not $keyvaultname) {
        Write-Error "No keyvault in RG $resourceGroup"
        exit 1
    }

    $keyvault=$(az keyvault show -g $resourceGroup -n $keyvaultname --subscription $subscription | ConvertFrom-Json)

    if ([String]::IsNullOrEmpty($aksHost)) {
        $aksHost=$aks.addonProfiles.httpApplicationRouting.config.HTTPApplicationRoutingZoneName
    }

    $values=@{
        "aksName"="$($aks.name)"
        "aksHost"="$aksHost"
        "tenant"="$($keyvault.properties.tenantId)"
        "kvName"="$($keyvault.name)"
        "acrName"="$acrName"
        "clientid"="$($aks.servicePrincipalProfile.clientId)"
    }

    foreach ($var in $values.Keys) {        
        if ($varsToSet -eq "*" -or $varsToSet.Contains($var)) {
            Write-Output "Setting $var to $($values[$var])"
            Write-Host "##vso[task.setvariable variable=$var;]$($values[$var])"
        }
        else {
            Write-Output "Variable $var skipped due to varsToSet=0$varsToSet'"
        }
    }
}

