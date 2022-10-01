using authorization_microservice.Services.Interfaces;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Runtime.Versioning;
using DirectoryManager;
using PolytechStudentsStaffManagerApi.services;

namespace authorization_microservice.Services.Implementations;

public static class DomainTypeExtensions
{
    public static string ToString(this DomainType domainType)
    {
        return domainType switch
        {
            DomainType.Stud => "stud",
            DomainType.Staff => "staff",
            _ => throw new ArgumentException("Wrong type of domain to connect")
        };
    }
}

public enum DomainType
{
    Stud,
    Staff,
}

public struct ADUser
{
    public string Guid { get; set; }
    public string Role { get; set; }
}

[SupportedOSPlatform("windows")]
public class ActiveDirectoryService : IActiveDirectoryService
{
    private class ActiveDirectoryConnections : IClosable, IConnectable
    {
        public PrincipalContext PrincipalContextStud { get; private set; }
        public PrincipalContext PrincipalContextStaff { get; private set; }
        
        public void Close(string type)
        {
            switch (type)
            {
                case "stud": PrincipalContextStud.Dispose(); break;
                case "staff": PrincipalContextStaff.Dispose(); break;
                default: throw new ArgumentException("Wrong type of domain to connect");
            }
        }

        public void Connect(string type)
        {
            switch (type)
            {
                case "stud": 
                    PrincipalContextStud = new PrincipalContext(
                        ContextType.Domain, "stud.test.ru", 
                        "readad@test.ru", "test"); 
                    break;
                
                case "staff": 
                    PrincipalContextStaff = new PrincipalContext(
                        ContextType.Domain, "staff.test.ru", 
                        "readad@test.ru", "test"); 
                    break;
                
                default: throw new ArgumentException("Wrong type of domain to connect");
            }
        }
    }

    private readonly IConnectionManagerService<ActiveDirectoryConnections> _cms;

    public ActiveDirectoryService()
    {
        _cms = new ConnectionManagerService<ActiveDirectoryConnections>(
            new []{"stud", "staff"}, 
            new ActiveDirectoryConnections());
    }
    
    public ADUser? AuthenticateUser(string login, string password)
    {
        var loginName = ResolveLoginName(login);
        var domainType = loginName.Contains("@stud") ? DomainType.Stud : DomainType.Staff;
        
        var guid = FindUserInDomain(loginName, password, domainType);

        if (guid is not null) return new ADUser{Guid = guid, Role = domainType.ToString()};

        guid = FindUserInDomain(loginName, password, domainType == DomainType.Stud ? DomainType.Staff : DomainType.Stud);
        
        if (guid is not null) return new ADUser{Guid = guid, Role = domainType.ToString()};

        return null;
    }

    private string? FindUserInDomain(string login, string password, DomainType domainType)
    {
        var userPrincipalName = ConvertLoginToUserPrincipalName(login);
        string? guid;
        try
        {
            var insPrincipalContext = domainType switch
            {
                DomainType.Stud => _cms.RequestConnection("stud").PrincipalContextStud,
                DomainType.Staff => _cms.RequestConnection("staff").PrincipalContextStaff,
                _ => throw new ArgumentException("Wrong type of domain")
            };

            if (!insPrincipalContext.ValidateCredentials(login, password)) return null;

            var user = new UserPrincipal(insPrincipalContext);
            var search = (DirectorySearcher)new PrincipalSearcher(user).GetUnderlyingSearcher();

            search.Filter = $"(&(sAMAccountType=805306368)(userPrincipalName={userPrincipalName}))";

            search.PropertiesToLoad.Clear();
            search.PropertiesToLoad.Add("employeenumber");

            var searchUser = search.FindOne();
            guid = searchUser?.Properties["employeenumber"][0].ToString();
        }
        finally
        {
            _cms.CloseConnection(domainType == DomainType.Stud ? "stud" : "staff");
        }

        return guid;
    }

    private string ConvertLoginToUserPrincipalName(string login)
    {
        return login[..(login.IndexOf('@') + 5)] + ".testdomain.ru";
    }

    private string ResolveLoginName(string login)
    {
        if (!login.Contains('@')) return login + "@stud.test.ru";
        
        if (login.Contains("@stud"))
        {
            return login[..login.IndexOf('@')] + "@stud.test.ru";
        }

        return login[..login.IndexOf('@')] + "@staff.test.ru";
    }
}