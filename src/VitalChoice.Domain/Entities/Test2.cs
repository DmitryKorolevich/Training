using System;

namespace VitalChoice.Domain.Entities
{
	public class Test2 : Entity
	{
        public int TestId { get; set; }

        public string Name { get; set; }

        public virtual Test Test { get; set; }
	}
}
