using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using VitalChoice.Business.Services.Contracts;
using VitalChoice.Data.DataContext;
using VitalChoice.Data.Extensions;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Transaction;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.Domain.Entities.Roles;
using VitalChoice.Domain.Entities.Users;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Domain.Transfer;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Infrastructure.UnitOfWork;

namespace VitalChoice.Business.Services.Impl
{
	public class UserService : IUserService
	{
		private readonly UserManager<ApplicationUser> userManager;

		private readonly RoleManager<IdentityRole<int>> roleManager;

		private readonly IDataContextAsync context;

		private readonly SignInManager<ApplicationUser> signInManager;

		public UserService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<int>> roleManager, IDataContextAsync context, SignInManager<ApplicationUser> signInManager)
		{
			this.userManager = userManager;
			this.roleManager = roleManager;
			this.context = context;
			this.signInManager = signInManager;
		}

		private IList<string> GetRoleNamesByIds(IList<RoleType> roles)
		{
			return roleManager.Roles.Where(x => roles.Contains((RoleType)x.Id))
						.Select(x => x.Name)
						.ToList();
		}

		public async Task<ApplicationUser> CreateAsync(ApplicationUser user, IList<RoleType> roles)
		{
			user.CreateDate = DateTime.UtcNow;
			user.UpdatedDate = DateTime.UtcNow;
			user.Status = UserStatus.NotActive;
			user.LastLoginDate = null;
			user.PublicId = Guid.NewGuid();
			user.UserName = user.Email;

			using (var transaction = new TransactionManager(context).BeginTransaction())
			{
				try
				{
					var createResult = await userManager.CreateAsync(user);
					if (createResult.Succeeded)
					{
						var roleNames = GetRoleNamesByIds(roles);
						if (roleNames.Any())
						{
							var addToRoleResult = await userManager.AddToRolesAsync(user, roleNames);
							if (!addToRoleResult.Succeeded)
							{
								throw new ApiException();
							}
						}
						transaction.Commit();

						return user;
					}
				}
				catch (Exception)
				{
					transaction.Rollback();
					throw;
				}
			}

			throw new ApiException();
		}

		public async Task DeleteAsync(ApplicationUser user)
		{
			user.DeletedDate = DateTime.UtcNow;
			user.Status = UserStatus.Disabled;

			var result = await UpdateAsync(user);
			if (result != null)
			{
				return;
			}

			throw new ApiException();
		}

		public async Task<ApplicationUser> UpdateAsync(ApplicationUser user, IList<RoleType> roleIds = null, string password = null)
		{
			user.UpdatedDate = DateTime.UtcNow;

			using (var transaction = new TransactionManager(context).BeginTransaction())
			{
				try
				{
					var updateResult = await userManager.UpdateAsync(user);
					if (updateResult.Succeeded)
					{
						if (password != null)
						{
							var passwordResult = await userManager.AddPasswordAsync(user, password);
							if (!passwordResult.Succeeded)
							{
								throw new ApiException();
							}
						}

						if (roleIds != null)
						{
							var oldRoleNames = await userManager.GetRolesAsync(user);
							if (oldRoleNames.Any())
							{
								var deleteResult = await userManager.RemoveFromRolesAsync(user, oldRoleNames);
								if (!deleteResult.Succeeded)
								{
									throw new ApiException();
								}
							}

							var roleNames = GetRoleNamesByIds(roleIds);
							if (roleNames.Any())
							{
								var addToRoleResult = await userManager.AddToRolesAsync(user, roleNames);
								if (!addToRoleResult.Succeeded)
									throw new ApiException();
							}
						}

						transaction.Commit();

						return user;
					}
				}
				catch (Exception)
				{
					transaction.Rollback();
					throw;
				}
			}

			throw new ApiException();
		}

		public async Task<ApplicationUser> GetAsync(Guid publicId)
		{
			return await userManager.Users.Include(x => x.Profile).Include(x => x.Roles).SingleOrDefaultAsync(x => !x.DeletedDate.HasValue && x.PublicId == publicId);
		}

		public async Task<ApplicationUser> GetByTokenAsync(Guid token)
		{
			//var user = await userManager.Users.Include(x => x.Profile).Include(x => x.Roles).SingleOrDefaultAsync(x => !x.DeletedDate.HasValue && x.Profile.ConfirmationToken == token);
			var temp = await userManager.Users.Include(x => x.Profile).Include(x => x.Roles).ToListAsync();
			var user = temp.SingleOrDefault(x => !x.DeletedDate.HasValue && x.Profile.ConfirmationToken == token);

			if (user == null)
			{
				throw new AppValidationException("key"); //can't find user;
			}
			if (user.Profile.IsConfirmed)
			{
				throw new AppValidationException("key"); //already confirmed
			}
			if (user.Profile.TokenExpirationDate.Subtract(DateTime.UtcNow).Days < 0)
			{
				throw new AppValidationException("key"); // expired;
			}
			 
			return user;
		}

		public async Task SignInAsync(ApplicationUser user)
		{
			await signInManager.SignInAsync(user, false);
		}

		public async Task<PagedList<ApplicationUser>> GetAsync(FilterBase filter)
		{
			Expression<Func<ApplicationUser, bool>> query = x => !x.DeletedDate.HasValue;
			if (!string.IsNullOrWhiteSpace(filter.SearchText))
			{
				var keyword = filter.SearchText.ToLower();
				query = query.And(x => (x.FirstName + " " + x.LastName).ToLower().Contains(keyword) ||
				                       x.Email.ToLower().Contains(keyword));
			}

			var queryable = userManager.Users.AsNoTracking().Where(query);
			var overallCount = await queryable.CountAsync();

			var sortOrder = SortOrder.Desc;
			if (filter.Sorting != null)
			{
				sortOrder = filter.Sorting.SortOrder;
				switch (filter.Sorting.Path)
				{
					case UserSortPath.AgentId: queryable = sortOrder == SortOrder.Asc ? queryable.OrderBy(x => x.Profile.AgentId) : queryable.OrderByDescending(x => x.Profile.AgentId); break;
					case UserSortPath.FullName: queryable = sortOrder == SortOrder.Asc ? queryable.OrderBy(x => x.FirstName + " " + x.LastName) : queryable.OrderByDescending(x => x.FirstName + " " + x.LastName); break;
					case UserSortPath.Email: queryable = sortOrder == SortOrder.Asc ? queryable.OrderBy(x => x.Email) : queryable.OrderByDescending(x => x.Email); break;
					case UserSortPath.Status: queryable = sortOrder == SortOrder.Asc ? queryable.OrderBy(x => x.Status) : queryable.OrderByDescending(x => x.Status); break;
					case UserSortPath.LastLoginDate: queryable = sortOrder == SortOrder.Asc ? queryable.OrderBy(x => x.LastLoginDate) : queryable.OrderByDescending(x => x.LastLoginDate); break;
				}
			}

            queryable = sortOrder == SortOrder.Asc ? queryable.OrderBy(x => x.UpdatedDate) : queryable.OrderByDescending(x => x.UpdatedDate);

			var pageIndex = filter.Paging.PageIndex;
			var pageItemCount = filter.Paging.PageItemCount;

			queryable = queryable.Include(x => x.Roles).Include(x => x.Profile);
            if (pageIndex > 1)//stupid ef doesn't work without this
            {
	            queryable = queryable.Skip((pageIndex - 1)*pageItemCount);
            }

			var items = await queryable.Take(pageItemCount).ToListAsync();

			return new PagedList<ApplicationUser>(items, overallCount);
		}
	}
}