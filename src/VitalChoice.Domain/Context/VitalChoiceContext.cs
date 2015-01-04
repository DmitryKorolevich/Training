using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using VitalChoice.Data.DataContext;
using VitalChoice.Domain.Entities;

namespace VitalChoice.Domain.Context
{
	public class VitalChoiceContext : DataContext
	{

		protected override void OnConfiguring(DbContextOptions builder)
		{
			builder.UseSqlServer(@"Server=(localdb)\v11.0;Database=Blogging;Trusted_Connection=True;");
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			/*builder.Entity<User>();*/
			/*OneToMany(b => b.Posts, p => p.Blog)
				.ForeignKey(p => p.BlogId)*/
		}
	}
}
