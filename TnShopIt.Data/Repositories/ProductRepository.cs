using System.Collections.Generic;
using System.Linq;
using TnShopIt.Data.Infrastructure;
using TnShopIt.Model.Models;

namespace TnShopIt.Data.Repositories
{
    //Để định nghĩa các phương thức mà k có sẵn trong Repository
    public interface IProductRepository : IRepository<Product>
    {
        IEnumerable<Product> GetAlias(string alias);
    }

    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        public ProductRepository(DbFactory dbFactory) : base(dbFactory)
        {
        }

        public IEnumerable<Product> GetAlias(string alias)
        {
            return this.DbContext.Products.Where(x => x.Alias == alias);
        }
    }
}