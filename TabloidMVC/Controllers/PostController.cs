using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using System;
using System.Security.Claims;
using TabloidMVC.Models;
using TabloidMVC.Models.ViewModels;
using TabloidMVC.Repositories;

namespace TabloidMVC.Controllers
{
    [Authorize]
    public class PostController : Controller
    {
        private readonly IPostRepository _postRepository;
        private readonly ICategoryRepository _categoryRepository;
		private readonly IUserProfileRepository _userRepository;

        public PostController(
			IPostRepository postRepository, 
			ICategoryRepository categoryRepository, 
			IUserProfileRepository userProfileRepository)
        {
            _postRepository = postRepository;
            _categoryRepository = categoryRepository;
			_userRepository = userProfileRepository;

        }

        public IActionResult Index(int UserId)
        {
            UserViewModel vm = new UserViewModel();
            vm.activeUser = GetCurrentUserProfileId();
			    if (vm.activeUser == UserId)
				{
					vm.Posts = _postRepository.GetPublishedByUser(vm.activeUser);
					return View(vm);
				}
			    else
				{
					vm.Posts = _postRepository.GetAllPublishedPosts();
				    return View(vm);
				}
        }

		public IActionResult Myposts(int UserId)
		{
			int activeUser = GetCurrentUserProfileId();
			var posts = _postRepository.GetPublishedByUser(activeUser);
		    return View(posts);

		}

        public IActionResult Details(int id)
        {
            var post = _postRepository.GetPublishedPostById(id);
            if (post == null)
            {
                int userId = GetCurrentUserProfileId();
                post = _postRepository.GetUserPostById(id, userId);
                if (post == null)
                {
                    return NotFound();
                }
            }
            return View(post);
        }

        public IActionResult Create()
        {
            var vm = new PostCreateViewModel();
            vm.CategoryOptions = _categoryRepository.GetAll();
            return View(vm);
        }

        [HttpPost]
        public IActionResult Create(PostCreateViewModel vm)
        {
            try
            {
                vm.Post.CreateDateTime = DateAndTime.Now;
                vm.Post.IsApproved = true;
                vm.Post.UserProfileId = GetCurrentUserProfileId();

                _postRepository.Add(vm.Post);

                return RedirectToAction("Details", new { id = vm.Post.Id });
            } 
            catch
            {
                vm.CategoryOptions = _categoryRepository.GetAll();
                return View(vm);
            }
        }

        //Get: PostController/Delete
        public IActionResult Delete(int id)
        {
            
            Post post = _postRepository.GetPublishedPostById(id);

            return View(post);
        }

        // POST: PostController/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Post post)
        {
            try
            {
                // update to the current user's Id 
                post.UserProfileId = GetCurrentUserProfileId();

                _postRepository.Delete(id);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View(post);
            }
        }

        // GET: Post/Edit
        public IActionResult Edit(int id)
        {
            var vm = new PostEditViewModel();
            vm.CategoryOptions = _categoryRepository.GetAll();
            vm.Post = _postRepository.GetPublishedPostById(id);
            
            
            int activeUser = GetCurrentUserProfileId();
            if (vm.Post == null)
            {
                return NotFound();
            }
            else if (activeUser == vm.Post.UserProfileId)
            {
                return View(vm);
            }
            else
            {
                return NotFound();
            }
        }

        // POST: Owners/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Post post)
        {
            var vm = new PostEditViewModel();
            try
            {
                post.IsApproved = true;
                _postRepository.Update(post);

                    return RedirectToAction("Details", new { id = id });

            }
            catch (Exception ex)
            {
                return View(vm);
            }
        }

        private int GetCurrentUserProfileId()
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(id);
        }
    }
}
