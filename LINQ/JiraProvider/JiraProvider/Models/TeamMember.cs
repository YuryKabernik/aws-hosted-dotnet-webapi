using System.ComponentModel.DataAnnotations.Schema;

namespace JiraProvider.Models
{
	public class TeamMember
	{
		public string Name { get; set; }
		
		[Column("Last Name")]
		public string LastName { get; set; }

		public string Title { get; set; }
		
		public short Age { get; set; }
	}
}
