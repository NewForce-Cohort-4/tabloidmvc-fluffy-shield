using System.Collections.Generic;

namespace TabloidMVC.Models.ViewModels
{
				public class PostDetailViewModel				
				{
								public Post Post { get; set; }
								public List<Category> CategoryOptions { get; set; }
								public List<Tag> AllTags { get; set; }
				}
}
