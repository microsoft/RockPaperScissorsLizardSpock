Param(
    [parameter(Mandatory=$true)][string]$resourceGroup,
    [parameter(Mandatory=$true)][string]$rootSpace,
    [parameter(Mandatory=$true)][string]$aksName
)

# Create Dev Spaces controller with some child spaces
az aks use-dev-spaces -g $resourceGroup -n $aksName --space $rootSpace -y
az aks use-dev-spaces -g $resourceGroup -n $aksName --space $rootSpace/scott -y
az aks use-dev-spaces -g $resourceGroup -n $aksName --space $rootSpace/jessica -y