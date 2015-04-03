﻿using System;
using System.Collections.Generic;
using VitalChoice.Domain;
using VitalChoice.Validation.Models;
using VitalChoice.Validators.UserManagement;

namespace VitalChoice.Models.UserManagement
{
    public class ManageUserModel : Model<ApplicationUser, UserManageSettings>
	{
		public string FirstName { get; set; }

		public string LastName { get; set; }

		public string Email { get; set; }

		public IList<string> RoleNames { get; set; }

		public string AgentId { get; set; }

	    public string Status { get; set; }

		public Guid PublicId { get; set; }
	}
}