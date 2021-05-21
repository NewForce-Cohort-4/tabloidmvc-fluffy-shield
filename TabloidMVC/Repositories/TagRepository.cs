using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TabloidMVC.Models;
using TabloidMVC.Utils;

namespace TabloidMVC.Repositories
{
				public class TagRepository : BaseRepository, ITagRepository
				{
								public TagRepository(IConfiguration config) : base (config) { }
								
								public List<Tag> GetAllTags()
								{
												using (var conn = Connection)
												{
																conn.Open();
																using (var cmd = conn.CreateCommand())
																{
																				cmd.CommandText = @"
																								SELECT		Id, 
																																[Name]
																								FROM
																																Tag
																				";
																				SqlDataReader reader = cmd.ExecuteReader();

																				List<Tag> tags = new List<Tag>();

																				while (reader.Read())
																				{
																								tags.Add(NewTagFromReader(reader));
																				}

																				reader.Close();

																				return tags;
																}
												}
								}
								/// <summary>
								///					Ticket # 17 - Add a Tag to a Post
								///					GetTagByPostId finds all Tags in the PostTag table,
								///					where the Post.Id matches a selected Post.
								/// </summary>

								public List<Tag> GetTagByPostId(int id)
								{
												using (SqlConnection conn = Connection)
												{
																conn.Open();
																using (SqlCommand cmd = conn.CreateCommand())
																{
																				cmd.CommandText = @"
																								SELECT		t.Id AS Id,
																																t.Name as Name
																								FROM PostTag pt
																								LEFT JOIN Post p ON pt.PostId = p.Id
																								LEFT JOIN Tag t ON pt.TagId = t.Id
																								WHERE PostId = @postId																			
																				";

																				cmd.Parameters.AddWithValue("@postId", id);
																				
																				SqlDataReader reader = cmd.ExecuteReader();

																				List<Tag> tags = new List<Tag>();

																				while (reader.Read())
																				{
																								tags.Add(NewTagFromReader(reader));
																				}

																				reader.Close();

																				return tags;
																}
												}
								}

								/// <summary>
								///					Ticket # 17 - Add a Tag to a Post
								///					NewTagFromReader is a private reader function,
								///					to build object properties from DB returned data.
								/// </summary>

								private Tag NewTagFromReader(SqlDataReader reader)
								{
												return new Tag()
												{
																Id = reader.GetInt32(reader.GetOrdinal("Id")),
																Name = reader.GetString(reader.GetOrdinal("Name"))
												};
								}
				}
}
