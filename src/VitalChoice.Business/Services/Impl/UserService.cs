using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using VitalChoice.Business.Services.Contracts;
using VitalChoice.Data.Extensions;
using VitalChoice.Data.Repositories;
using VitalChoice.Domain.Entities.Roles;
using VitalChoice.Domain.Entities.Users;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Domain.Transfer;
using VitalChoice.Domain.Transfer.Base;

namespace VitalChoice.Business.Services.Impl
{
	public class UserService : IUserService
	{
		private readonly UserManager<ApplicationUser> userManager;

		private readonly RoleManager<IdentityRole> roleManager;

		public UserService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
		{
			this.userManager = userManager;
			this.roleManager = roleManager;
		}

		private IList<string> GetRoleNamesByIds(IList<RoleType> roles)
		{
			return roleManager.Roles.Where(x => roles.Contains((RoleType)Enum.Parse(typeof(RoleType), x.Id)))
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

			var createResult = await userManager.CreateAsync(user);
			if (createResult.Succeeded)
			{
				var roleNames = GetRoleNamesByIds(roles);
                if (roleNames.Any())
				{
					var addToRoleResult = await userManager.AddToRolesAsync(user, roleNames);
					if (addToRoleResult.Succeeded)
					{
						return user;
					}
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

		public async Task<ApplicationUser> UpdateAsync(ApplicationUser user, IList<RoleType> roleIds = null)
		{
			user.UpdatedDate = DateTime.UtcNow;

			var updateResult = await userManager.UpdateAsync(user);
			if (updateResult.Succeeded)
			{
				if (roleIds != null)
				{
					var roleNames = GetRoleNamesByIds(roleIds);
					if (roleNames.Any())
					{
						user.Roles.Clear();
						var addToRoleResult = await userManager.AddToRolesAsync(user, roleNames);
						if (!addToRoleResult.Succeeded)
							throw new ApiException();
					}
				}
				return user;
			}

			throw new ApiException();
		}

		public async Task<ApplicationUser> GetAsync(Guid publicId)
		{
			return await userManager.Users.SingleOrDefaultAsync(x => !x.DeletedDate.HasValue && x.PublicId == publicId);
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

			Expression<Func<ApplicationUser, object>> sortExp = x => x.UpdatedDate;
			var sortOrder = SortOrder.Desc;
            if (filter.Sorting != null)
			{
				sortOrder = filter.Sorting.SortOrder;
				switch (filter.Sorting.Path)
				{
					case UserSortPath.AgentId: sortExp = x => x.Profile.AgentId; break;
					case UserSortPath.FullName: sortExp = x => x.FirstName + " " + x.LastName; break;
					case UserSortPath.Email: sortExp = x => x.Email; break;
					case UserSortPath.Status: sortExp = x => x.Status; break;
					case UserSortPath.LastLoginDate: sortExp = x => x.LastLoginDate; break;
				}
			}

			queryable = sortOrder == SortOrder.Asc ? queryable.OrderBy(sortExp) : queryable.OrderByDescending(sortExp);

			var pageIndex = filter.Paging.PageIndex;
			var pageItemCount = filter.Paging.PageItemCount;

			
            var items = await queryable.Skip((pageIndex - 1) * pageItemCount).Take(pageItemCount).ToListAsync();

			return new PagedList<ApplicationUser>(items, overallCount);
		}
	}
}