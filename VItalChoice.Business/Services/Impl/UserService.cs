﻿using System.Collections.Generic;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Services;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.Entities.Domain;
using VItalChoice.Business.Queries.User;
using VItalChoice.Business.Services.Contracts;

namespace VItalChoice.Business.Services.Impl
{
	public class UserService: GenericService<User>, IUserService
	{
		public UserService(IUnitOfWorkAsync unitOfWork) : base(unitOfWork)
		{
		}

		public IEnumerable<User> QueryByName(string name)
		{
			var query = new UserQuery();

			return Repository.Query(query.UserNameStartsWith(name)).Select();
		}
	}
}
