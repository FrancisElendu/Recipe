using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Recipe.Models
{
    public class Recipes
    {
        public int RecipesId { get; set; }
        public string Ingredients { get; set; }
        public string Quantities { get; set; }
        public string Methods { get; set; }
    }
}
