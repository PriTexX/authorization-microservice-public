using authorization_microservice.Database;
using authorization_microservice.DatabaseModels;
using authorization_microservice.Repositories.Interfaces;

namespace authorization_microservice.Repositories.Implementations;

public class BaseRepository<TDbModel> : IBaseRepository<TDbModel> where TDbModel : ABCBaseModel
{
    private readonly ApplicationContext _context;
    
    public BaseRepository(ApplicationContext context)
    {
        _context = context;
    }

    public List<TDbModel> GetAll()
    {
        return _context.Set<TDbModel>().ToList();
    }

    public TDbModel? Get(string id)
    {
        return _context.Set<TDbModel>().FirstOrDefault(m => m.Guid == id);
    }

    public TDbModel Create(TDbModel model)
    {
        _context.Set<TDbModel>().Add(model);
        _context.SaveChanges();
        return model;
    }

    public TDbModel Update(TDbModel model)
    {
        var toUpdate = _context.Set<TDbModel>().FirstOrDefault(m => m.Guid == model.Guid);
        
        if (toUpdate is null)
        {
            throw new ArgumentException($"Model with guid: {model.Guid} doesn't exist");
        }
        
        toUpdate = model;

        _context.Set<TDbModel>().Update(toUpdate);
        _context.SaveChanges();
        
        return toUpdate;
    }

    public void Delete(string guid)
    {
        var toDelete = _context.Set<TDbModel>().FirstOrDefault(m => m.Guid == guid);

        if (toDelete is null)
        {
            throw new ArgumentException($"Model with guid: {guid} doesn't exist");
        }
        
        _context.Set<TDbModel>().Remove(toDelete);
        _context.SaveChanges();
    }
}