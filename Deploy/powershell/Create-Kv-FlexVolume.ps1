Write-Host "------------------------------------------------------------" -ForegroundColor Yellow
Write-Host "Installing Key Vault FlexVolume" -ForegroundColor Yellow
Write-Host "------------------------------------------------------------" -ForegroundColor Yellow

$kv=$(kubectl get ns kv --no-headers --ignore-not-found)

if (-not [String]::IsNullOrEmpty($kv)) {
    Write-Host "Namespace kv found. Assuming Key Vault FlexVolume is already installed" -ForegroundColor Yellow
}
else {
    kubectl create -f https://raw.githubusercontent.com/Azure/kubernetes-keyvault-flexvol/master/deployment/kv-flexvol-installer.yaml
}
exit $LastExitCode