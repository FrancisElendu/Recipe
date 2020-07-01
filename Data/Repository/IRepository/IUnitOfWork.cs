using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Recipe.Data.Repository.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        //IRecipesRepository Recipes { get; set; 
        IRecipesRepository Recipes { get;}
        IUserRepository UserServices { get; set; }
        IMailRepository MailServices { get; set; }

        void Save();
    }
}
