using Ecommerce.DataAccess.Data;
using Ecommerce.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext db;
        public ICategoryRepository Category { get; private set; }
        public IProductRepository Product { get; private set; }

        public UnitOfWork(ApplicationDbContext db) 
        {
            this.db = db;
            Category = new CategoryRepository(this.db);
            Product = new ProductRepository(this.db);
        }

        public void Save()
        {
            db.SaveChanges();
        }
    }
}
