using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
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
												var vm = new PostDetailViewModel();
            vm.Post = _postRepository.GetPublishedPostById(id);
												vm.AllTags = _tagRepository.GetTagByPostId(id);
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

								/// <summary>
								///					Ticket # 17 - Add a Tag to a Post
								/// </summary>

								// TagDetails route creates a GET view to display all tags
								// The TagRoute view receives a list of Tags and related Post.Id
								public IActionResult TagDetails(int id)
								{
												PostDetailViewModel vm = new PostDetailViewModel();
												vm.AllTags = _tagRepository.GetTagByPostId(id);
												vm.TagsByPost = _tagRepository.GetAllTags();
												foreach (Tag tag in vm.TagsByPost)
												{
																if(vm.AllTags.Exists(t => t.Name == tag.Name))
																{
																				tag.Selected = true;
																}
												}
												vm.Post = new Post() { 
												Id = id};
												return View(vm);
								}

								/// <summary>
								///					Ticket # 17 - Add a Tag to a Post
								/// </summary>

								// TagDetails POST route receives all tags from Form POST
								// SelectedTags is called from the view model to store all tags from DB
								// POST method receives tag.Name [String] and Tag.Selected [Boolean],
								// Find method is envoked on SelectedTags to match Tag Name and 
								// pass the new object to tagRepository, which contains Tag.Id

								/// <summary>
								///					Ticket # 18 - Add a Tag to a Post
								/// </summary>

								// Refactored TagDetails POST method with oldTag variable
								// which contains an Exists method called
								// on the List returned by GetTagByPostId, if the oldTag bool
								// is false, the tag is added to the database,
								// otherwise the tag is ignored. If the user deselects a
								// tag, where tag.Selected is false, the PostDeleteTag		
								// is envoked in a try/catch to circumvent DB errors.

								[HttpPost]
								public IActionResult TagDetails(PostDetailViewModel vm, int id)
								{
												vm.SelectedTags = _tagRepository.GetAllTags();
												vm.AllTags = _tagRepository.GetTagByPostId(id);
												try
												{
																foreach (Tag tag in vm.TagsByPost)
																{
																				bool oldTag = vm.AllTags.Exists(t => t.Name == tag.Name);
																				if (tag.Selected)
																				{
																								if (!oldTag)
																								{
																												Tag fTag = vm.SelectedTags.Find(t => t.Name == tag.Name);
																												_postRepository.PostAddTag(fTag, id);
																								}
																								else
																								{
																												continue;
																								}
																				}
																				else
																				{
																								try
																								{
																												Tag fTag = vm.SelectedTags.Find(t => t.Name == tag.Name);
																												_postRepository.PostDeleteTag(fTag, id);
																								}
																								catch (Exception)
																								{
																												continue;
																								}
																				}
																}
																return RedirectToAction("Details", new { id = id });
												}
												catch (Exception ex)
												{
																return View();
												}

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
