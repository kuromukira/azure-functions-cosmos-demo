using Demo.Function.Models;

namespace Demo.Function.Domain.Interface;

public interface IPersonRepo
{
    Task<DomainResponse> Create(Person person);
    Task<DomainResponse> Modify(Person person);
    Task<DomainResponse> Remove(string id, string userName);
    Task<DomainResponse> Get(string userName);
    Task<DomainResponse> Get();
}