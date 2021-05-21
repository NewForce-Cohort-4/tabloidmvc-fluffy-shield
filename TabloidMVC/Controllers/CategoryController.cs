using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TabloidMVC.Models;
using TabloidMVC.Repositories;

namespace TabloidMVC.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {

        private readonly ICategoryRepository
           _categoryRepositroy;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepositroy = categoryRepository;
        }

        // GET: CategoryController 
        public ActionResult Index()
        {
            var categories =
                _categoryRepositroy.GetAll();
            return View(categories);
        }

        // GET: CategoryController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: CategoryController/Create
        public ActionResult Create()
        {
            //new instance of a category to be created
            var cm = new Category();
            return View(cm);
        }

        // POST: CategoryController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Category cm)
        {
            try
            {
                //adding a category to the database and redirecting the user back to the index page of category
                _categoryRepositroy.Add(cm);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(cm);
            }
        }

        // GET: CategoryController/Edit/5
        public ActionResult Edit(int id)
        {
            //grabs the category clicked with by it's id that is stored in the database
            Category category = _categoryRepositroy.GetCategoryById(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: CategoryController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Category category)
        {
            try
            {
                // runs the method UpdateCategory saving the changes made within the edit functionality in the browser 
                _categoryRepositroy.UpdateCategory(category);

                // return will take you back to the index view listing all of the categories with any edits made
                return RedirectToAction("Index");
            }
            catch
            {
                //this will keep you in the same view of editing a category and keep you there signaling there is most likely an issue with the SQL Query
                return View(category);
            }
        }

        // GET: CategoryController/Delete/5
        public ActionResult Delete(int id)
        {
            Category category = _categoryRepositroy.GetCategoryById(id);
            return View(category);
        }

        // POST: CategoryController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Category category)
        {
            try
            {
                _categoryRepositroy.Delete(id);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View(category);
            }
        }
    }
}
