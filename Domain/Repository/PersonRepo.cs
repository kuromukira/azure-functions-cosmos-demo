using System.Net;
using Demo.Function.Domain.Interface;
using Demo.Function.Models;

namespace Demo.Function.Domain.Repository;

public class PersonRepo : IPersonRepo
{
    protected readonly Container Container;

    public PersonRepo(Database db)
        => Container = db.CreateContainerIfNotExistsAsync("persons", "/userName").GetAwaiter().GetResult();

    async Task<DomainResponse> IPersonRepo.Create(Person person)
    {
        try
        {
            QueryDefinition query = new QueryDefinition("SELECT * FROM c WHERE c.userName = @userName")
                .WithParameter("@userName", person.Username);

            using FeedIterator<Person> queryResponse = Container.GetItemQueryIterator<Person>(query);
            if (queryResponse.HasMoreResults)
            {
                FeedResponse<Person> next = await queryResponse.ReadNextAsync();
                if (next.Count > 0)
                    return new(Success: false, RU: next.RequestCharge, Message: $"{person.Username} already exists.");
            }

            person.AddedOn = DateTime.Now;

            ItemResponse<Person> response = await this.Container.CreateItemAsync<Person>(person, new PartitionKey(person.Username));
            return new(Success: true, RU: response.RequestCharge, Data: new { id = person.Id, username = person.Username });
        }
        catch (CosmosException)
        {
            throw;
        }
        catch (Exception)
        {
            throw;
        }
    }

    async Task<DomainResponse> IPersonRepo.Get(string userName)
    {
        try
        {
            QueryDefinition query = new QueryDefinition("SELECT * FROM c WHERE c.userName = @userName")
                .WithParameter("@userName", userName);

            using FeedIterator<Person> queryResponse = Container.GetItemQueryIterator<Person>(query);
            if (queryResponse.HasMoreResults)
            {
                FeedResponse<Person> next = await queryResponse.ReadNextAsync();
                if (next.Any())
                    return new(Success: true, next.RequestCharge, Data: next.Resource.FirstOrDefault());
                else return new(Success: false, Message: $"{userName} does not exist.");
            }
            else return new(Success: false, RU: (await queryResponse.ReadNextAsync()).RequestCharge, Message: $"{userName} does not exist.");
        }
        catch (CosmosException)
        {
            throw;
        }
        catch (Exception)
        {
            throw;
        }
    }

    async Task<DomainResponse> IPersonRepo.Get()
    {
        try
        {
            QueryDefinition query = new QueryDefinition("SELECT * FROM c");

            double requestUnit = 0;
            List<Person> persons = new();
            using FeedIterator<Person> queryResponse = Container.GetItemQueryIterator<Person>(
                query,
                requestOptions: new() { MaxItemCount = 50 }
            );
            while (queryResponse.HasMoreResults)
            {
                FeedResponse<Person> next = await queryResponse.ReadNextAsync();
                persons.AddRange(next);
                requestUnit += next.RequestCharge;
            }
            return new(Success: true, RU: requestUnit, Data: persons);
        }
        catch (CosmosException)
        {
            throw;
        }
        catch (Exception)
        {
            throw;
        }
    }

    async Task<DomainResponse> IPersonRepo.Modify(Person person)
    {
        ItemResponse<Person> response = null;
        try
        {
            response = await Container.ReadItemAsync<Person>(person.Id, new PartitionKey(person.Username));

            using Person existing = response.Resource;
            existing.FirstName = person.FirstName;
            existing.LastName = person.LastName;
            existing.Address = person.Address;
            existing.Phone = person.Phone;
            existing.Email = person.Email;
            existing.ModifiedOn = DateTime.Now;

            response = await Container.ReplaceItemAsync<Person>(existing, person.Id, new PartitionKey(person.Username));
            return new(Success: true, response.RequestCharge, Data: response.Resource);
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return new(Success: false, Message: $"{person.Username} does not exist.");
        }
        catch (CosmosException ex) when (ex.StatusCode != HttpStatusCode.NotFound)
        {
            throw;
        }
        catch (Exception ex)
        {
            return new(Success: false, RU: response.RequestCharge, Message: ex.Message);
        }
    }

    async Task<DomainResponse> IPersonRepo.Remove(string id, string userName)
    {
        ItemResponse<Person> response = null;
        try
        {
            response = await Container.DeleteItemAsync<Person>(id, new PartitionKey(userName));
            return new(Success: true, RU: response.RequestCharge);
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return new(Success: false, Message: $"{userName} does not exist.");
        }
        catch (CosmosException ex) when (ex.StatusCode != HttpStatusCode.NotFound)
        {
            throw;
        }
        catch (Exception ex)
        {
            return new(Success: false, RU: response.RequestCharge, Message: ex.Message);
        }
    }
}