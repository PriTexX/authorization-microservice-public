using authorization_microservice.DatabaseModels;

namespace authorization_microservice.Repositories.Interfaces;

public interface IBaseRepository<TDbModel> where TDbModel : ABCBaseModel
{
    public List<TDbModel> GetAll();
    public TDbModel? Get(string id);
    public TDbModel Create(TDbModel model);
    public TDbModel Update(TDbModel model);
    public void Delete(string guid);
}