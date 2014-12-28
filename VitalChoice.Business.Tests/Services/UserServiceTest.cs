using System;
using NUnit.Framework;
using VitalChoice.Data.DataContext;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.Entities.Context;
using VitalChoice.Entities.Domain;
using VItalChoice.Business.Services.Contracts;
using VItalChoice.Business.Services.Impl;

namespace VitalChoice.Business.Tests.Services
{
    public class UserServiceTest
    {
	    private IUserService userService;
	    private IUnitOfWorkAsync unitOfWork;

		[SetUp]
	    public void SetUp()
		{
			var dataContext = new VitalChoiceContext("VitalChoice");
			unitOfWork = new UnitOfWork(dataContext);

			userService = new UserService(unitOfWork);
	    }

		[Test]
	    public void AddUser()
		{
			var userName = Guid.NewGuid().ToString();

			var user = new User()
			{
				UserName = userName
			};

			userService.Insert(user);

			var dbUser = userService.QueryByName(userName);

			Assert.NotNull(dbUser);
		}

	    [TearDown]
		public void TearDown()
	    {
			unitOfWork.Dispose();
			//dataContext.Dispose();
	    }
    }
}
