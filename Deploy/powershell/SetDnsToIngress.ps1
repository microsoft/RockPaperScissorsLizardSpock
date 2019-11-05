Param(
    [parameter(Mandatory=$true)][string]$resourceGroupAks,
    [parameter(Mandatory=$true)][string]$resourceGroupDns,
    [parameter(Mandatory=$true)][string]$dnsPrefix,
    [parameter(Mandatory=$false)][string]$dnsZoneName="",
    [parameter(Mandatory=$false)][string]$ip="",
    [parameter(Mandatory=$false)][bool]$byPassContextCheck=$false,
    [parameter(Mandatory=$false)][string]$dnsSubscription="",
    [parameter(Mandatory=$false)][string]$aksSubscription=""
)

if ([String]::IsNullOrEmpty($dnsSubscription)) {
    $dnsSubscription=$(az account show -o json | ConvertFrom-Json).id
}

if ([String]::IsNullOrEmpty($aksSubscription)) {
    $aksSubscription=$(az account show -o json | ConvertFrom-Json).id
}

Write-Host "DNS subs: $dnsSubscription / AKS subs: $aksSubscription"

if ([String]::IsNullOrEmpty($dnsZoneName)) {
    $dns=$(az network dns zone list -g $resourceGroupDns --query "[0]" -o json --subscription $dnsSubscription | ConvertFrom-Json)
}
else {
    $dns=$(az network dns zone show -g $resourceGroupDns -n $dnsZoneName -o json --subscription $dnsSubscription | ConvertFrom-Json)
}

if (-not $dns) {
    Write-Error "DNS zone not found in RG $dnsSubscription/$resourceGroupDns"
    exit 1
}
$dnsZoneName=$dns.name

$aks=$(az aks list -g $resourceGroupAks --query "[0]" -o json --subscription $aksSubscription | ConvertFrom-Json )
if (-not $aks) {
    Write-Error "No AKS found in RG $aksSubscription/$resourceGroupAks"
    exit 1
}
$aksName=$aks.name

if (-not $byPassContextCheck) {
    $k8sctx=$(kubectl config current-context)
    if ($k8sctx -ne $aksName) {
        Write-Error "Current K8S context is '$k8sctx'. Do not match AKS name ('$aksName'). If this is OK and set byPassContextCheck to false to bypass this check"
        exit 1
    }
}


if (-not $ip) {
    # We get the first loadBalancer IP we find
    $ip=$(kubectl get svc --all-namespaces -o jsonpath="{.items[*].status.loadBalancer.ingress[0].ip}").Split(' ')[0]
}

Write-Host "Creating prefix $dnsPrefix to dnsZone $dnsZOneName to IP $ip"
az network dns record-set a add-record -g $resourceGroupDns -z $dnsZoneName -n $dnsPrefix -a $ip --subscription $dnsSubscription