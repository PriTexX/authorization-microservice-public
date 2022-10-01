using authorization_microservice.Services.Interfaces;
using DirectoryManager;

namespace PolytechStudentsStaffManagerApi.services;

public interface IConnectionManagerService<T> where T : IClosable, IConnectable
{
    public T RequestConnection(string conn);
    public void CloseConnection(string type, TimeSpan? delay = null);
}