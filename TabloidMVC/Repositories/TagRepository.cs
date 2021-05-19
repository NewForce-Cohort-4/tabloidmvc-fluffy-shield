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
