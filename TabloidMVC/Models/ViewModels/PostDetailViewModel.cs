using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace TabloidMVC.Models.ViewModels
{
				public class PostDetailViewModel				
				{
								public Post Post { get; set; }
								public Tag Tag { get; set; }
								public List<Category> CategoryOptions { get; set; }
								public List<Tag> AllTags { get; set; }
								[BindProperty]
								public List<Tag> TagsByPost { get; set; }
								public List<Tag> SelectedTags { get; set; } = new List<Tag>();
				}
}
