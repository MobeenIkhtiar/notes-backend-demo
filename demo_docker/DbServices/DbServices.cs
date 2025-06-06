using demo_docker.Models;

namespace demo_docker.DbServices
{
    public class DbServices 

    {
        AppDbContext _dbContext;

        public DbServices(AppDbContext appDbContext) { _dbContext = appDbContext; }
        
        public void RemoveDataFromDb(Guid Id)
        {
            var res = _dbContext.Notes.Where(x => x.Id == Id).FirstOrDefault();
            _dbContext.Notes.Remove(res);
                _dbContext.SaveChanges();



        }

        public Notes GetFromDb(Guid Id)
        {
            var res = _dbContext.Notes.Where(x => x.Id == Id).FirstOrDefault();
            return res;



        }
    }
}
