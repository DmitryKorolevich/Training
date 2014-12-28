using System.Data.Entity.ModelConfiguration;
using VitalChoice.Entities.Domain;

namespace VitalChoice.Entities.Mapping
{
	public class UserMap: EntityTypeConfiguration<User>
	{
		public UserMap()
		{
			HasKey(x=>x.Id);
			Property(x => x.UserName).IsRequired();
		}
	}
}
