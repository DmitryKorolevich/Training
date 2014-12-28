using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VitalChoice.Data.DataContext;
using VitalChoice.Entities.Domain;
using VitalChoice.Entities.Mapping;

namespace VitalChoice.Entities.Context
{
	public class VitalChoiceContext : DataContext
	{
		public VitalChoiceContext(string nameOrConnectionString) : base(nameOrConnectionString)
		{
		}

		public DbSet<User> Users { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Configurations.Add(new UserMap());
		}
	}
}
