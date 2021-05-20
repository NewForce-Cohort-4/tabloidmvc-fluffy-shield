using System.Collections.Generic;
using TabloidMVC.Models;

namespace TabloidMVC.Repositories
{
    public interface ICategoryRepository
    {
        //interface to get all of the categories from the database
        List<Category> GetAll();
        // interface to add a category
        void Add(Category category);

        //interface to delete a category from the categories ID
        void Delete(int categoryId);
        Category GetCategoryById(int id);
    }
}