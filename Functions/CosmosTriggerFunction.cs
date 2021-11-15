using Microsoft.Azure.Documents;

namespace Demo.Function;

public class CosmosTriggerFunction : BaseFunction
{
    [FunctionName("Persons_Trigger")]
    public void PersonsAlertTrigger([CosmosDBTrigger(
        databaseName: "demo-pl-db",
        collectionName: "persons",
        ConnectionStringSetting = "CosmosDbConnection",
        CreateLeaseCollectionIfNotExists = true,
        LeaseCollectionPrefix = "persons_")]IReadOnlyList<Document> input, ILogger log)
    {
        try
        {
            if (input != null && input.Any())
            {
                foreach (Document document in input)
                    log.LogWarning($"Triggered for Person Id {document.Id}");
            }
        }
        catch (Exception ex)
        {
            log.LogError(ex.Message);
        }
    }
}