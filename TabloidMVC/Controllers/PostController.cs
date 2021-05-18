﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using System.Security.Claims;
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

        private int GetCurrentUserProfileId()
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(id);
        }
    }
}
