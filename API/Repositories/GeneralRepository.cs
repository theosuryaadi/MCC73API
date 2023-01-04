using API.Contexts;
using API.Entity;
using API.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories;

public class GeneralRepository<Entity, T> : IRepository<Entity, T> where Entity : class
{
    private MyContext _context;
    private DbSet<Entity> _repo;
    public GeneralRepository(MyContext context)
    {
        _context = context;
        _repo = context.Set<Entity>();
    }

    public int Delete(T id)
    {
        var data = _repo.Find(id);
        if (data == null)
        {
            return 0;
        }

        _repo.Remove(data);
        var result = _context.SaveChanges();
        return result;
    }

    public IEnumerable<Entity> Get()
    {
        return _repo.ToList();
    }

    public Entity Get(T id)
    {
        return _repo.Find(id);
    }

    public int Insert(Entity entity)
    {
        _repo.Add(entity);
        var result = _context.SaveChanges();
        return result;
    }

    public int Update(Entity entity)
    {
        _repo.Entry(entity).State = EntityState.Modified;
        var result = _context.SaveChanges();
        return result;
    }
}
