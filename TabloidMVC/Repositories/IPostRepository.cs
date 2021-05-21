using System.Collections.Generic;
using TabloidMVC.Models;

namespace TabloidMVC.Repositories
{
    public interface IPostRepository
    {
        void Add(Post post);
        List<Post> GetAllPublishedPosts();
								List<Post> GetPublishedByUser(int userId);
        Post GetUserPostById(int id, int userProfileId);
								Post GetPublishedPostById(int id);
        void Delete(int postId);
								void PostAddTag(Tag tag, int postId);
    }
}