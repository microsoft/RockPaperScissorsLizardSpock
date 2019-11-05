Write-Host "------------------------------------------------------------" -ForegroundColor Yellow
Write-Host "Configuring RBAC for Tiller" -ForegroundColor Yellow
Write-Host "------------------------------------------------------------" -ForegroundColor Yellow
kubectl create serviceaccount --namespace kube-system tiller
kubectl create clusterrolebinding tiller-cluster-rule --clusterrole=cluster-admin --serviceaccount=kube-system:tiller
Write-Host "------------------------------------------------------------" -ForegroundColor Yellow
Write-Host "Installing Helm" -ForegroundColor Yellow
Write-Host "------------------------------------------------------------" -ForegroundColor Yellow
helm list -q  | Out-Null
if ($?) {
    helm init -c --service-account tiller --node-selectors "kubernetes.io/os=linux"
}
else {
    helm init --service-account tiller --node-selectors "kubernetes.io/os=linux" --wait
}

helm list -q  | Out-Null

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