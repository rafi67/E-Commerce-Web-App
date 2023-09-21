using Ecommerce.DataAccess.Data;
using Ecommerce.DataAccess.Repository.IRepository;
using Ecommerce.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.DataAccess.Repository
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private ApplicationDbContext db;
        public CompanyRepository(ApplicationDbContext db) : base(db)
        {
            this.db = db;
        }

        public void Update(Company obj)
        {
            db.companies.Update(obj);
        }
    }
}
