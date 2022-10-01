using authorization_microservice.Services.Implementations;

namespace authorization_microservice.Services.Interfaces;

public interface IActiveDirectoryService
{
    public ADUser? AuthenticateUser(string login, string password);
}