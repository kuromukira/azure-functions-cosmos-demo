using Demo.Function.Domain.Interface;
using Demo.Function.Models;

namespace Demo.Function;

public class DemoFunction : BaseFunction
{
    private IPersonRepo IPersonRepo;

    public DemoFunction(IPersonRepo personRepo) => IPersonRepo = personRepo;

    [FunctionName("Person_Add")]
    public async Task<IActionResult> AddPerson([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "person/add")] HttpRequest request, ILogger log)
    {
        try
        {
            if (request.Body is null)
                throw new UnauthorizedAccessException("Request body is required.");
            else
            {
                string userAgent = request.Headers["user-agent"];
                string clientAddress = request.HttpContext.Connection.RemoteIpAddress.ToString();

                log.LogInformation($"User-Agent: {userAgent}");
                log.LogInformation($"Client-Address: {clientAddress}");

                string requestBody = await new StreamReader(request.Body).ReadToEndAsync();
                using Person person = JsonConvert.DeserializeObject<PersonViewModel>(requestBody).MapTo<Person>();

                person.Id = Guid.NewGuid().ToString();
                person.IsValid();

                DomainResponse dbResponse = await IPersonRepo.Create(person);
                if (dbResponse.Success)
                    return new OkObjectResult(new
                    {
                        Message = "Person created successfully.",
                        Data = dbResponse.Data
                    });
                else return new BadRequestObjectResult(dbResponse.Message);
            }
        }
        catch (Exception e)
        {
            return new BadRequestObjectResult(e.Message);
        }
    }

    [FunctionName("Person_Update")]
    public async Task<IActionResult> UpdatePerson([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "person/update/{id}")] HttpRequest request, string id, ILogger log)
    {
        try
        {
            if (request.Body is null || request.Query is null)
                throw new UnauthorizedAccessException("Request body and query parameters are required.");
            else
            {
                string userAgent = request.Headers["user-agent"];
                string clientAddress = request.HttpContext.Connection.RemoteIpAddress.ToString();

                log.LogInformation($"User-Agent: {userAgent}");
                log.LogInformation($"Client-Address: {clientAddress}");

                string requestBody = await new StreamReader(request.Body).ReadToEndAsync();

                using Person person = JsonConvert.DeserializeObject<PersonViewModel>(requestBody).MapTo<Person>();

                person.Id = id;
                person.IsValid();

                DomainResponse dbResponse = await IPersonRepo.Modify(person);
                if (dbResponse.Success)
                    return new OkObjectResult(new
                    {
                        Message = "Person updated successfully.",
                        Data = ((Person)dbResponse.Data).MapTo<PersonViewModel>()
                    });
                else return new BadRequestObjectResult(dbResponse.Message);
            }
        }
        catch (Exception e)
        {
            return new BadRequestObjectResult(e.Message);
        }
    }

    [FunctionName("Person_Delete")]
    public async Task<IActionResult> DeletePerson([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "person/delete")] HttpRequest request, ILogger log)
    {
        try
        {
            if (request.Query is null)
                throw new UnauthorizedAccessException("Query parameter is required.");
            else
            {
                string userAgent = request.Headers["user-agent"];
                string clientAddress = request.HttpContext.Connection.RemoteIpAddress.ToString();

                log.LogInformation($"User-Agent: {userAgent}");
                log.LogInformation($"Client-Address: {clientAddress}");

                DomainResponse dbResponse = await IPersonRepo.Remove(request.Query["id"], request.Query["userName"]);
                if (dbResponse.Success)
                    return new OkObjectResult(new
                    {
                        Message = "Person successfully deleted."
                    });
                else return new BadRequestObjectResult(dbResponse.Message);
            }
        }
        catch (Exception e)
        {
            return new BadRequestObjectResult(e.Message);
        }
    }

    [FunctionName("Person_Get")]
    public async Task<IActionResult> GetPerson([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "person/get/{username}")] HttpRequest request, string username, ILogger log)
    {
        try
        {
            if (request.Query is null)
                throw new UnauthorizedAccessException("Query parameter is required.");
            else
            {
                string userAgent = request.Headers["user-agent"];
                string clientAddress = request.HttpContext.Connection.RemoteIpAddress.ToString();

                log.LogInformation($"User-Agent: {userAgent}");
                log.LogInformation($"Client-Address: {clientAddress}");

                DomainResponse dbResponse = await IPersonRepo.Get(username);
                if (dbResponse.Success)
                    return new OkObjectResult(((Person)dbResponse.Data).MapTo<PersonViewModel>());
                else return new BadRequestObjectResult(dbResponse.Message);
            }
        }
        catch (Exception e)
        {
            return new BadRequestObjectResult(e.Message);
        }
    }

    [FunctionName("Persons_Get")]
    public async Task<IActionResult> GetPersons([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "person/get")] HttpRequest request, ILogger log)
    {
        try
        {
            if (request.Query is null)
                throw new UnauthorizedAccessException("Query parameter is required.");
            else
            {
                string userAgent = request.Headers["user-agent"];
                string clientAddress = request.HttpContext.Connection.RemoteIpAddress.ToString();

                log.LogInformation($"User-Agent: {userAgent}");
                log.LogInformation($"Client-Address: {clientAddress}");

                DomainResponse dbResponse = await IPersonRepo.Get();
                if (dbResponse.Success)
                    return new OkObjectResult(((List<Person>)dbResponse.Data).MapToList<PersonViewModel>());
                else return new BadRequestObjectResult(dbResponse.Message);
            }
        }
        catch (Exception e)
        {
            return new BadRequestObjectResult(e.Message);
        }
    }
}