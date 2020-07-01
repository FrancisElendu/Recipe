using Recipe.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Recipe.Data.Repository.IRepository
{
    public interface IRecipesRepository : IRepository<Recipes>
    {
        void Update(Recipes Recipes);
    }
}
