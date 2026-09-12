using System;
using System.Threading.Tasks;
using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Storage;
using Azure.ResourceManager.Storage.Models;

class Program
{
    static async Task Main(string[] args)
    {
        // Replace with your Azure Subscription ID
        string subscriptionId = "84ab8ce8-efc6-427f-834f-00566c1bde0b";

        // Resource Group settings
        string resourceGroupName = "okok-rg";
        AzureLocation location = AzureLocation.EastUS;

        // Storage Account settings
        string storageAccountName = "okokstorage"; // Must be globally unique

        // Authenticate
        ArmClient armClient = new ArmClient(new DefaultAzureCredential());

        // Get subscription
        ResourceIdentifier subscriptionResourceId =
            SubscriptionResource.CreateResourceIdentifier(subscriptionId);

        SubscriptionResource subscription =
            armClient.GetSubscriptionResource(subscriptionResourceId);

        // Create Resource Group
        ResourceGroupCollection resourceGroups =
            subscription.GetResourceGroups();

        var rgData = new ResourceGroupData(location);

        ArmOperation<ResourceGroupResource> rgOperation =
            await resourceGroups.CreateOrUpdateAsync(
                WaitUntil.Completed,
                resourceGroupName,
                rgData);

        ResourceGroupResource resourceGroup = rgOperation.Value;

        Console.WriteLine($"Resource Group '{resourceGroupName}' created.");

        // Create Storage Account
        StorageAccountCollection storageAccounts =
            resourceGroup.GetStorageAccounts();

        var storageData = new StorageAccountCreateOrUpdateContent(
            new StorageSku(StorageSkuName.StandardLrs),
            StorageKind.StorageV2,
            location);

        ArmOperation<StorageAccountResource> storageOperation =
            await storageAccounts.CreateOrUpdateAsync(
                WaitUntil.Completed,
                storageAccountName,
                storageData);

        StorageAccountResource storageAccount = storageOperation.Value;

        Console.WriteLine($"Storage Account '{storageAccount.Data.Name}' created.");
    }
}