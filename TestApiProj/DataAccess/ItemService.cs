using TestApiProj.MainEntity;
using TestApiProj.Services;

namespace TestApiProj.DataAccess
{
    public class ItemService:IItemService
    {
        private readonly MyDbContext _dbContext;

        public ItemService(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public string AddItem (Items items)
        {
         var response = _dbContext.Items.Add (items);
          _dbContext.SaveChanges();
            return "Item Added Succesfully";
        }
    }
}
