Param(
    [parameter(Mandatory=$true)][string]$resourceGroup,
    [parameter(Mandatory=$true)][string]$rootSpace,
    [parameter(Mandatory=$true)][string]$aksName
)

$azdsVersion=$(azds --version)

Write-Host "Your Azure DevSpaces client version is: $azdsVersion"
Write-Host "Updating Azure DevSpaces client version to latest."

# Update Dev Spaces Client
wget https://aka.ms/azds-linux-unattended
sudo sh ./azds-linux-unattended

# Create Dev Spaces controller with some child spaces
azds use -g $resourceGroup -n $aksName --space $rootSpace -y
azds use -g $resourceGroup -n $aksName --space $rootSpace/abel -y
azds use -g $resourceGroup -n $aksName --space $rootSpace/jessica -y

# az aks use-dev-spaces -g $resourceGroup -n $aksName --space $rootSpace -y
# az aks use-dev-spaces -g $resourceGroup -n $aksName --space $rootSpace/abel -y
# az aks use-dev-spaces -g $resourceGroup -n $aksName --space $rootSpace/jessica -y