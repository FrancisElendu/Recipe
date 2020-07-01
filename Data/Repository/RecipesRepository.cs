using Recipe.Data.Repository.IRepository;
using Recipe.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Recipe.Data.Repository
{
    public class RecipesRepository : Repository<Recipes>, IRecipesRepository
    {
        private readonly ApplicationDbContext _db;

        public RecipesRepository(ApplicationDbContext db): base(db)
        {
            _db = db;
        }
        public void Update(Recipes Recipes)
        {
            var objFromDb = _db.Recipes.FirstOrDefault(x => x.RecipesId == Recipes.RecipesId);
            objFromDb.Ingredients = Recipes.Ingredients;
            objFromDb.Quantities = Recipes.Quantities;
            objFromDb.Methods = Recipes.Methods;
            _db.SaveChanges();
        }
    }
}
