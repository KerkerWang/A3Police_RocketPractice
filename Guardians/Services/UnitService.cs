using Guardians.Data;
using Guardians.Models;

namespace Guardians.Services
{
    public interface IUnitService
    {
        List<Unit> GetAll();
    }
    public class UnitService : IUnitService
    {
        protected readonly DBContext _db;

        public UnitService(DBContext db)
        {
            _db = db;
        }

        public List<Unit> GetAll()
        {
            return _db.Set<Unit>().ToList();
        }
    }
}
