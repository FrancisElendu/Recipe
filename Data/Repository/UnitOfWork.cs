using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Recipe.Data.Repository.IRepository;
using Recipe.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Recipe.Data.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _config;
        private IRecipesRepository _recipes;

        public UnitOfWork(ApplicationDbContext db, UserManager<ApplicationUser> userManager, 
                    SignInManager<ApplicationUser> signInManager, IConfiguration config) 
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;

            //Recipes = new RecipesRepository(_db);
            UserServices = new UserRepository(_db, _userManager, _signInManager, _config); 
            MailServices = new MailRepository(_db, _config);
        }

        public IRecipesRepository Recipes
        { 
            get 
            { 
                if (_recipes == null)
                _recipes = new RecipesRepository(_db); 
                return _recipes; 
            } 
        }
        //public IRecipesRepository Recipes { get; set; }

        public IUserRepository UserServices { get; set; }

        public IMailRepository MailServices { get; set; }

        public void Dispose()
        {
            _db.Dispose();
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
