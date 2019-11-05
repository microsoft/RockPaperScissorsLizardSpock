Param(
    [parameter(Mandatory=$false)][string]$chartsSourcePath="../helm",
    [parameter(Mandatory=$false)][string]$chartsDestPath="../helm/__charts",
    [parameter(Mandatory=$false)][string]$chartsToPackage="*",
    [parameter(Mandatory=$false)][string]$acrName="",
    [parameter(Mandatory=$false)][string]$chartVersion="1.0.0"
)

$charts=("java-player","php-player","python-player")
$acr=$null
$rg=$null

if (-not [String]::IsNullOrEmpty($acrName)) {
    $acr=$(az acr list --query "[?name=='$acrName'] | [0]" -o json | ConvertFrom-Json)
    if ($null -eq $acr) {
        Write-Error "ACR $acrName not found"
        exit 1
    }
    $rg=$acr.resourceGroup
}


foreach ($chart in $charts) {
    if ($chartsToPackage.Contains($chart) -or $chartsToPackage -eq "*") {
        Write-Output "Packaging helm chart $chart"
        $command = "helm package $chartsSourcePath/$chart --destination $chartsDestPath --version $chartVersion"  
        Invoke-Expression $command      
        if (-not $null -eq $acr)  {
            $chartPackedName="$chart-$chartVersion.tgz"
            Write-Output "Publishing $chartPackedName to ACR $rg/$acrName"
            $azCommand= "az acr helm push -n $acrName $chartsDestPath/$chartPackedName --force"
            Invoke-Expression $azCommand
        }
    }
}