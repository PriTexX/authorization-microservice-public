using System.ComponentModel.DataAnnotations;

namespace authorization_microservice.DatabaseModels;

public abstract class ABCBaseModel
{ 
    public virtual string Guid { get; set; }
}