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
        private readonly ICategoryRepository _categoryRepository;
        private readonly IPostRepository _postRepository;
								private readonly IUserProfileRepository _userRepository;
								private readonly ITagRepository _tagRepository;

								public PostController(
												ICategoryRepository categoryRepository, 
												IPostRepository postRepository, 
												IUserProfileRepository userProfileRepository,
												ITagRepository tagRepository)
								{
            _postRepository = postRepository;
            _categoryRepository = categoryRepository;
												_userRepository = userProfileRepository;
												_tagRepository = tagRepository;
        }

        public IActionResult Index(int UserId)
        {
												int activeUser = GetCurrentUserProfileId();
												if (activeUser == UserId)
												{
																var posts = _postRepository.GetPublishedByUser(activeUser);
																return View(posts);
												}
												else
												{
																var posts = _postRepository.GetAllPublishedPosts();
																return View(posts);
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
												var vm = new PostDetailViewModel();
            vm.Post = _postRepository.GetPublishedPostById(id);
												vm.AllTags = _tagRepository.GetAllTags();
            if (vm.Post == null)
            {
                int userId = GetCurrentUserProfileId();
                vm.Post = _postRepository.GetUserPostById(id, userId);
                if (vm.Post == null)
                {
                    return NotFound();
                }
            }
            return View(vm);
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
                // update the dogs OwnerId to the current user's Id 
                post.UserProfileId = GetCurrentUserProfileId();

                _postRepository.Delete(id);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View(post);
            }
        }


        private int GetCurrentUserProfileId()
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(id);
        }
    }
}
