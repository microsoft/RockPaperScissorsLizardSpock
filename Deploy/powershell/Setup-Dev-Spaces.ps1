Param(
    [parameter(Mandatory=$true)][string]$resourceGroup,
    [parameter(Mandatory=$true)][string]$rootSpace,
    [parameter(Mandatory=$true)][string]$aksName,
    [parameter(Mandatory=$true)][string[]]$childSpaces 
)

# Create Dev Spaces controller with some child spaces
az aks use-dev-spaces -g $resourceGroup -n $aksName --space $rootSpace -y

foreach($child in $childSpaces)
{
    az aks use-dev-spaces -g $resourceGroup -n $aksName --space $rootSpace/$child -y
}
