using System;
using System.Collections.Generic;

namespace VitalChoice.Domain.Entities
{
	public class Test : Entity
	{
		public string Text { get; set; }

        public virtual ICollection<Test2> Text2s { get; set; }
	}
}
