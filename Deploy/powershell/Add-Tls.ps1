Param(
    [parameter(Mandatory=$false)][string][ValidateSet('prod','staging','none','custom', IgnoreCase=$false)]$sslSupport = "none",
    [parameter(Mandatory=$false)][string]$name="rpsls",
    [parameter(Mandatory=$true)][string]$resourceGroup,
    [parameter(Mandatory=$false)][string]$tlsCertFile="",
    [parameter(Mandatory=$false)][string]$tlsKeyFile="",
    [parameter(Mandatory=$false)][string]$domain="",
    [parameter(Mandatory=$false)][string]$tlsSecretName="rpsls-tls-custom",
    [parameter(Mandatory=$false)][bool]$byPassContextCheck=$false
)

$aks=$(az aks list -g $resourceGroup --query "[0]" | ConvertFrom-Json)
if (-not $aks) {
    Write-Error "No aks found in RG $resourceGroup"
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

function validate {
    $valid = $true

    if ($sslSupport -eq "custom")  {
        if ([string]::IsNullOrEmpty($domain)) {
            Write-Host "If sslSupport is 'custom' the domain parameter is mandatory." -ForegroundColor Red
            $valid=$false
        }
        if ([String]::IsNullOrEmpty($tlsCertFile)) {
            Write-Host "If sslSupport is 'custom' then need to pass the certificate file in tlsCertFile parameter" -ForegroundColor Red
            $valid=$false
        }
        if ([String]::IsNullOrEmpty($tlsKeyFile)) {
            Write-Host "If sslSupport is 'custom' then need to pass the certificate key file in tlsKeyFile parameter" -ForegroundColor Red
            $valid=$false
        }
        if ([String]::IsNullOrEmpty($tlsSecretName)) {
            Write-Host "If sslSupport is 'custom' then need to pass the Kubernetes secret name in tlsSecretName parameter" -ForegroundColor Red
            $valid=$false
        }        
    }
    
    if ($sslSupport -eq "none")  {
        Write-Host "sslSupport set to none. Nothing will be done. Use staging or prod to setup SSL/TLS" -ForegroundColor Yellow
        exit 0
    }

    if ($valid -eq $false) {
        exit 1
    }
}

validate

$certManager=$(helm ls cert-manager --output json | ConvertFrom-Json)
if (-not $certManager) {
    Write-Host "Installing Cert-Manager as is not installed..." -ForegroundColor Yellow
    cmd /c "helm install --name cert-manager --namespace kube-system  --version v0.4.1 stable/cert-manager" 
}

Write-Host "--------------------------------------------------------" -ForegroundColor Yellow
Write-Host " Enabling TLS support on cluster $aksName in RG $resourceGroup"  -ForegroundColor Yellow
Write-Host " --------------------------------------------------------" -ForegroundColor Yellow

if ([String]::IsNullOrEmpty($domain)) {
    $domain = $(az aks show -n $aksName -g $resourceGroup -o json --query addonProfiles.httpApplicationRouting.config.HTTPApplicationRoutingZoneName | ConvertFrom-Json)
    if ([String]::IsNullOrEmpty($domain)) {
        Write-Error "No domain found on AKS (is Http Application Routing installed?). Exiting"
        exit 1
    }
}

Write-Host "TLS/SSL will be bound to domain $domain" -ForegroundColor Yellow

Push-Location ..\helm

if ($sslSupport -eq "staging") {
    Write-Host "Adding TLS/SSL support using Let's Encrypt Staging environment" -ForegroundColor Yellow
    Write-Host "helm install --name $name-ssl -f tls-support\values-staging.yaml --set domain=$domain tls-support" -ForegroundColor Yellow
    cmd /c "helm install --name $name-ssl-staging -f tls-support\values-staging.yaml --set domain=$domain tls-support"
}
if ($sslSupport -eq "prod") {
    Write-Host "Adding TLS/SSL support using Let's Encrypt PRODUCTION environment" -ForegroundColor Yellow
    cmd /c "helm install --name $name-ssl-prod -f tls-support\values-prod.yaml --set domain=$domain tls-support"
}
if ($sslSupport -eq "custom") {
    Write-Host "TLS support is custom bound to domain $domain" -ForegroundColor Yellow
    Write-Host "Creating secret $tlsSecretName with TLS certificate from file $tlsCertFile and key from $tlsKeyFile"
    kubectl create secret tls $tlsSecretName --key $tlsKeyFile --cert $tlsCertFile
}

Pop-Location




