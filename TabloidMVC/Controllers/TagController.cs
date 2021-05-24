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
    public class TagController : Controller
    {

        private readonly ITagRepository
           _tagRepositroy;

        public TagController(ITagRepository tagRepository)
        {
            _tagRepositroy = tagRepository;
        }

        // GET: TagController 
        public ActionResult Index()
        {
            var tags = _tagRepositroy.GetAllTags();
            return View(tags);
        }

        // GET: TagController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }
    }
}
