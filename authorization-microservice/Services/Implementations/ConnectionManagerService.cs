using authorization_microservice.Services.Interfaces;
using DirectoryManager;

namespace PolytechStudentsStaffManagerApi.services;

public enum ConnectionState
{
    Connected,
    Disconnected,
    Closing,
    Working,
}

public class ConnectionManagerService<T> : IConnectionManagerService<T> where T : IClosable, IConnectable
{
    private readonly T _connectionObject;
    // private readonly Dictionary<string, ConnectionState> _connectionsStates;

    public ConnectionManagerService(IEnumerable<string> types, T connectionService)
    {
        _connectionObject = connectionService;
        // _connectionsStates = new Dictionary<string, ConnectionState>();
        
        // foreach (var type in types)
        // {
        //     _connectionsStates.Add(type, ConnectionState.Disconnected);    
        // }
        
    }

    public T RequestConnection(string type)
    {
        _connectionObject.Connect(type);
        return _connectionObject;
    }

    public void CloseConnection(string type, TimeSpan? delay = null)
    {
        _connectionObject.Close(type);
    }
}