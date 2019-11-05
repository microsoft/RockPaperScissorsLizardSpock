# Scripts in this folder

## Deploy-Arm-Azure.ps1

Deploys the ARM template to a resource group. Following parameters are available:

- `resourceGroup` **Mandatory**: Resource group where to deploy. If not exists will be created
- `location`: Location where the resource group will be created. Only used if resource group do not exists. Ignored otherwise
- `clientId`: Client ID of the Service Principal to use when creating AKS. Only needed if AKS has to be created
- `password`: Password of the Service Principal to use when creating AKS. Only needed if AKS has to be created
- `deployKv`: If KeyVault has to bee deployed. Defaults to `true`.
- `aksVersion`: Specific AKS version to install. Defaults to latest available.
- `tryUseConsumptionPlan`: If `true` a consumption plan for function app will be deployed if location allows consumption plans
- `alternativeFuncSvcPlan`: Service Plan to use if consumption plan is not deployed. Mandatory if `tryUseConsumptionPlan` is `false`.

**Remmarks:**

1. If resource group do not exists, it will be created. 
2. If `clientId` or `password` are not provided and AKS has to be created, the script will use `az ad sp create-for-rbac` to create a SP. Note that this requires permissions on the Azure Directory. If AKS already exists is not created again.
3. To deploy the Key Vault the script must be able to obtain the `objectId` of the service principal that has granted access to Key Vault. This requires permissions to Azure Directory. If script can't obtain the objectId a warning is printed and key vault won't be deployed.
4. Script uses the same service principal for creating the AKS and giving access to Key Vault. If you need to use differnt service principals, need to deploy calling the script twice. The first time with `deployKv` to `$false` and using the `clientId` needed for AKS. Then call the script again, but pass the `clientId` that you want to grant access to Key Vault.
5. If the resource group location do not accept consumption plans (for any reason) an alternative app service plan must be provided using `alternativeFuncSvcPlan`. If not, ARM template won't be installed.
 
**Examples:**

Deploy ARM:

```powershell
.\Deploy-Arm-Azure.ps1 -resourceGroup MyRg
```

Deploy ARM using a `P1` app service for function app:

```powershell
.\Deploy-Arm-Azure.ps1 -resourceGroup MyRg -alternativeFuncSvcPlan P1 -tryUseConsumptionPlan $false
```

Deploy ARM using specified Service plan for AKS:

```powershell
.\Deploy-Arm-Azure.ps1 -resourceGroup MyRg -clientId xxxx-xxx-xxx -password yyyy-yyy-yyy
```

## <a name="Generate-Config"></a>Generate-Config.ps1

Helm charts require a YAML file with some values. Those values depends on the Azure resources. This file generates this file having a template and a resource group. Script parameters are:

* `resourceGroup`: Resource group where resources are deployed. **Mandatory**
* `resourceGroupAcr`: RG where ACR is stored. Defaults to the value of `resourceGroup`
* `subscription`: Azure subscription to use. Defaults to current one.
* `subscriptionAcr`: Azure subscription where ACR is stored. Defaults to current one
* `outputFile`: Full path of the YAML file to write. This file must be passed to Helm charts. **Mandatory**
* `gvaluesTemplate`: Full path of the template file. Defaults to `gvalues.template` which is the one provided. 
* `ingressClass`: Ingress class for all ingress resources generated. Defaults to `addon-http-application-routing`.
* `twitterKey`: Key of the twitter app. Needed to enable Login with Twitter in the web.
* `twitterSecret`: Secret of the twitter app. Needed to enable Login with Twitter in the web.
* `aksHost`: AKS public host value. Defaults to the value of the Http Application Routing. Is used to set the host value of ingress resources.
* `sslSupport`: TLS support to install and configure. Can be one of:
    * `none`: No TLS. Ingress resources will be generated without TLS info. **Default value**
    * `staging`: Ingresses are configured to use a TLS certificate auto-generated using Let's Encrypt staging server
    * `prod`: Ingresses are configured to use a TLS certificate auto-generated using Let's Encrypt production server
    * `custom`: Ingresses are configured to use a custom TLS certificate stored in a Kubernetes Secret
* `tlsSecretName`: Name of the Kubernetes secret that contains the TLS certificate. Only used if `sslSupport` is `custom`. Defaults to `rpsls-tls-custom`
* `kvClientId`: Id of the Service Principal with read access to Key Vault. Defaults to the SP used to create the AKS
* `registryLogin`: Docker registry Login server if another Docker registry is used instead of ACR. Only used if `useCustomRegistry` is true.
* `registryUser`: Docker registry User  if another Docker registry is used instead of ACR. Only used if `useCustomRegistry` is true.
* `registryPassword`: Docker registry Password  if another Docker registry is used instead of ACR. Only used if `useCustomRegistry` is true.
* `useCustomRegistry`: Configure deployments to use another Docker registry instead of ACR. Defaults to false.
* `googleanalytics`: Google Analytics ID. If set, GA tracking code will be added to the web.

This script uses AZ CLI to retrieve some data needed to generate a YAML file using the specified template. If you customize the helm charts can create your own template file.

## Add-Tiller.ps1

Installs _Tiller_ and _keyvault-flexvol_ on the cluster. Both are global pre-requisites that need to be installed once. No parameters are allowed.

## Add-Tls.ps1

Adds TLS support to the cluster. This script does two things:

1. Install cert-manager on the cluster if needed with the correct CRDs (issuers and certificates)
2. Adds a secret to the cluster with the TLS certificate if needed

Script parameters are:

* `resourceGroup`: Resource Group where AKS is. **Mandatory**
* `name`: Prefix used for the secrets. Defaults to `rpsls`
* `sslSupport` TLS support to configure:
    * `none`: No TLS. Ingress resources will be generated without TLS info. **Default value**
    * `staging`: Ingresses are configured to use a TLS certificate auto-generated using Let's Encrypt staging server
    * `prod`: Ingresses are configured to use a TLS certificate auto-generated using Let's Encrypt production server
    * `custom`: Ingresses are configured to use a custom TLS certificate stored in a Kubernetes Secret
* `domain`: Domain to bind the TLS. Used if needed to create _certificate_ CRD for cert-manager. Defaults to the value of the Http Application Routing. Not used if `sslSupport` is `custom`
* `tlsCertFile`: TLS Certificate file in `.crt` format. Mandatory if `sslSupport` is `custom`. Ignored otherwise.
* `tlsKeyFile`: TLS Certificate public key file in `.key` format. Mandatory if `sslSupport` is `custom`. Ignored otherwise.
* `tlsSecretName`: Name of the k8s secret that will hold the TLS certificate. Used only if `sslSupport` is `custom`. If `sslSupport` is `staging` or `prod` the name of the k8s secret is fixed and can't be overriden.
* `byPassContextCheck`: If `true` the script checks that the current `kubectl` context has the same name of the AKS cluster. By default the `az aks get-credentials` creates contexts with the same name of the cluster, so this is a valid security check. If parameter is set to `false` this check is skipped. Defaults to `true`.

## Build-Push.ps1

Helper script that builds and push all or some of the docker images, using the compose files. It has the following parameters:

* `resourceGroup`: Resource group where ACR is deployed. **Mandatory**
* `acrName`: Name of the ACR. If not provided defaults to the first ACR found in the resource group.
* `dockerBuild`: If the docker images will be built. Build command is `docker compose build`. Defaults to `true`.
* `dockerPush`: If the docker images will be pushed to ACR. Push command is `docker compose push`. Defaults to `true`.
* `dockerTag`: Tag to use. Defaults to `latest`.

## Create-Secret.ps1

This script creates the docker registry secret to allow AKS pull images from ACR.

If the ACR has administrative credentials enabled, those credentials are used to create the secret. Otherwise you have to pass a Service Principal Id and Password and this SP will be granted access permisions to the ACR and the secret will be created using these SP credentials.

* `resourceGroup`: Resource group where AKS is deployed. **Mandatory**
* `acrName`: Name of the ACR. Defaults to the first ACR found in the Resource Group
* `clientId`: ClientId of the service principal to use if ACR has no admin credentials enabled.
* `password`: Password of the SP referred by `clientId`

## Deploy-Images-Aks.ps1

This script deploys Docker images on the AKS by running the Helm Charts.

Helm charts are located in `/Deploy/helm` folder.

`resourceGroup`: Resource group where AKS is.
`aksName`: Name of the AKS **Mandatory**
`aksHost`: Host bound to public IP of ingress controller. Defaults to URL set by HTTP Application routing.
`acrName`: Name of the ACR where images are pushed. Defaults to the first ACR found in the resource group.
`name`: Base name for Helm releases. Defaults to `rpsls`.
`tag`: Tag of the Docker images to install. Defaults to `latest`
`charts`: Comma-separated list with the names of charts to install. Default to all.
`valuesFile`: Configuration file to use. This file is generated by calling `Generate-Config.ps1`. Defaults to `gvalues.yaml`.
`kvDeploy`: If needs to deploy the key vault support. Defaults to `true`.
`kvPassword`: Password of the service principal that has granted access to Key Vault.
`deployKvSecret`: If the secret for accessing the Key Vault has to be deployed or not. Defaults to `true`.

**Remmarks**

The KeyVault support is implemented through [FlexVol](https://github.com/Azure/kubernetes-keyvault-flexvol). It requires a Kubernetes secret with the service principal id and password that can access to the keyvault. This secret can be created by this script (if `deployKvSecret` is `true`) or can be deployed for some administrator (only need to do this once). The secret must be named `game-api-kv` and its type is `azure/kv`. Following `kubectl` command can be typed to create it:

```
kubectl create secret generic game-api-kv --from-literal clientid=$clientId --from-literal clientsecret=$password --type=azure/kv
``` 

## Helm-Package.ps1

This script is used to package the Helm charts. It packages the charts using helm, and uploads to an ACR.

## Set-Predictor-Conf.ps1

The predictor is an Azure Function in Python. This script setups its configuration

* `resourceGroup`: Resource group **Mandatory**
* `aksHost`: Host bound to public IP of ingress controller. Defaults to URL set by HTTP Application routing.
* `funcappName`: Name of the function app. Defaults to first function app in the resource group
* `tlsEnabled`: If Predictor must access Game API using HTTPS. Defaults to `true`.

**Remmarks** If TLS is installed using non-trusted certificates (like the one generated by LE staging), then you cannot use HTTPS (`tlsEnabled` should be false)

## SetDnsToIngress.ps1

This script sets specific DNS bound to the IP of the ingress controller of the given AKS. It is a helper script that you can use to set custom domains to your AKS, given that a "DNS Zone" is already created in Azure.

* `resourceGroupAks`: Resource Group where AKS is. **Mandatory**
* `resourceGroupDns`: Resource Group where DNS Zone is. **Mandatory**
* `dnsPrefix`: DNS prefix (will be prefixed to the domain specified by the DNS)
* `dnsZoneName`: Name of the DNS Zone to use. Defaults to the first found in `resourceGroupDns`
* `ip`: Ip to bound to the DNS. Defaults to the ip of the first ingress controller found.
* `byPassContextCheck`: : If `true` the script checks that the current `kubectl` context has the same name of the AKS cluster. By default the `az aks get-credentials` creates contexts with the same name of the cluster, so this is a valid security check. If parameter is set to `false` this check is skipped. Defaults to `true`.
* `dnsSubscription`: Subscription where DNS Zone is. Defaults to current subscription.
* `aksSubscription`: Subscription where AKS is. Defaults to current subscription.

