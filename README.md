# Kusto Client Example

This example does some simple Kusto setup, write and read to the same Table. Easily extensible to a wider use, but keeping it simple to just show how the basic use case works.

The steps walk you through creating a Kusto Cluster (ADX) and configuring it for streaming. Then you use the app in this folder to upload records to the table you created, then you can also read the data in that table. 

## Requirements

- Azure Subscription
- Azure CLI installed

## Steps

When instructed to go to the Azure Portal, these are the steps

- Go to https://ms.portal.azure.com
- Go to your subscription/resource group from above


### Create a Resource Group and ADX Cluster

Using the command prompt run these commands in order to create the Azure Resources, replacing only your subscription ID where needed. 

```bash
az account set -s [_YOUR_SUBSCRIPTION_ID_]

az group create --location EastUS --name grecoeexample

az extension add -n kusto

az kusto cluster create --name Example123ADX --resource-group grecoeexample  --sku name="Dev(No SLA)_Standard_D11_v2" capacity=1 tier="Basic" --location EastUS
```

### Create the Database and Table and configure them. 

1. Go to the Azure Portal and find the Example123ADX or whatever name you ended up with. 
1. In the menu choose Databases and create a new one called **TestADX**
1. In the menu choose Configuration and change this setting
    - *Streaming ingestion -> enable -> Save*
    - This will take 5-10 minutes before you can continue
1. In the menu choose Query and run these commands one at a time (note: do not change the following two lines as the code expects this table with this format)

``` bash
.create table EventLog ( Timestamp:datetime, Message:string)

.alter table EventLog policy streamingingestion enable
```

### Run the application

1. Go to the Azure Portal and get your endpoint for your ADX.
1. Update the KustSettings endpoints with your ADX name as a replacement.
1. Run the application. 