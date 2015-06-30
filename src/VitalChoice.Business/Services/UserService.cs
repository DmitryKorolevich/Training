using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Framework.OptionsModel;
using VitalChoice.Business.Mail;
using VitalChoice.Data.DataContext;
using VitalChoice.Data.Extensions;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Data.Transaction;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities.eCommerce.Users;
using VitalChoice.Domain.Entities.Options;
using VitalChoice.Domain.Entities.Permissions;
using VitalChoice.Domain.Entities.Roles;
using VitalChoice.Domain.Entities.Users;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Domain.Mail;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Infrastructure.Identity;
using VitalChoice.Interfaces.Services;
using Microsoft.Data.Entity;

namespace VitalChoice.Business.Services
{
	public class UserService : IUserService
	{
		private readonly UserManager<ApplicationUser> userManager;

		private readonly RoleManager<IdentityRole<int>> roleManager;

		private readonly IDataContextAsync context;

		private readonly SignInManager<ApplicationUser> signInManager;

		private readonly IAppInfrastructureService appInfrastructureService;
		private readonly INotificationService notificationService;
		private readonly IEcommerceRepositoryAsync<User> ecommerceRepositoryAsync;
		private readonly AppOptions options;

		public UserService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<int>> roleManager, IDataContextAsync context, SignInManager<ApplicationUser> signInManager, IAppInfrastructureService appInfrastructureService, INotificationService notificationService, IOptions<AppOptions> options, IEcommerceRepositoryAsync<User> ecommerceRepositoryAsync)
		{
			this.userManager = userManager;
			this.roleManager = roleManager;
			this.context = context;
			this.signInManager = signInManager;
			this.appInfrastructureService = appInfrastructureService;
			this.notificationService = notificationService;
			this.ecommerceRepositoryAsync = ecommerceRepositoryAsync;
			this.options = options.Options;
		}

		private IList<string> GetRoleNamesByIds(IList<RoleType> roles)
		{
			return roleManager.Roles.Where(x => roles.Contains((RoleType)x.Id))
						.Select(x => x.Name)
						.ToList();
		}

		private PermissionType RoleClaimValueToPermission(string value)
		{
			return (PermissionType) Enum.Parse(typeof (PermissionType), value);
		}

		private string AggregateIdentityErrors(IEnumerable<IdentityError> errors)
		{
			return errors.First().Description.Trim();
		}

		private async Task SendActivationAsync(ApplicationUser dbUser)
		{
			if (dbUser.Status != UserStatus.NotActive)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.UserAlreadyConfirmed]);
			}

			await notificationService.SendUserActivationAsync(dbUser.Email, new UserActivation()
			{
				FirstName = dbUser.FirstName,
				LastName = dbUser.LastName,
				Link = $"{options.AdminHost}#/authentication/activate/{dbUser.Profile.ConfirmationToken}"
			});
		}

		private async Task<ApplicationUser> UpdateInternalAsync(ApplicationUser user, IList<RoleType> roleIds = null,
			string password = null)
		{
			user.UpdatedDate = DateTime.Now;

			var updateResult = await userManager.UpdateAsync(user);
			if (updateResult.Succeeded)
			{
				if (password != null)
				{
					var passwordResult = await userManager.AddPasswordAsync(user, password);
					if (!passwordResult.Succeeded)
					{
						throw new AppValidationException(AggregateIdentityErrors(passwordResult.Errors));
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
							throw new AppValidationException(AggregateIdentityErrors(deleteResult.Errors));
						}
					}

					var roleNames = GetRoleNamesByIds(roleIds);
					if (roleNames.Any())
					{
						var addToRoleResult = await userManager.AddToRolesAsync(user, roleNames);
						if (!addToRoleResult.Succeeded)
							throw new AppValidationException(AggregateIdentityErrors(addToRoleResult.Errors));
					}
				}

				return user;
			}
			else
			{
				throw new AppValidationException(AggregateIdentityErrors(updateResult.Errors));
			}
		}

		public async Task ValidateUserOnSignIn(string login)
		{
			var disabled = await userManager.Users.AnyAsync(x => x.Status == UserStatus.Disabled && x.Email.Equals(login) && !x.DeletedDate.HasValue);

			if (disabled)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.UserIsDisabled]);
			}

			//stupid ef
			var temp = await userManager.Users.Include(x => x.Profile).ToListAsync();
			var notConfirmed = temp.Any(x => !x.Profile.IsConfirmed && x.Email.Equals(login) && !x.DeletedDate.HasValue);

			if (notConfirmed)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.UserIsNotConfirmed]);
			}
		}

		public async Task<ApplicationUser> CreateAsync(ApplicationUser user, IList<RoleType> roles, bool sendActivation = true)
		{
			user.CreateDate = DateTime.Now;
			user.UpdatedDate = DateTime.Now;
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
								throw new AppValidationException(AggregateIdentityErrors(addToRoleResult.Errors));
							}
						}

						await ecommerceRepositoryAsync.InsertAsync(new User() {Id = user.Id});

						if (sendActivation)
						{
							var dbUser = await FindAsync(user.Email);
							await SendActivationAsync(dbUser);
						}

						transaction.Commit();

						return user;
					}
					else
					{
						throw new AppValidationException(AggregateIdentityErrors(createResult.Errors));
					}
				}
				catch (Exception)
				{
					transaction.Rollback();
					throw;
				}
			}
		}

		public async Task DeleteAsync(ApplicationUser user)
		{
			user.DeletedDate = DateTime.Now;
			user.Status = UserStatus.Disabled;

			var result = await UpdateAsync(user);
			if (result != null)
			{
				return;
			}

			throw new ApiException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.UpdateUserGeneral]);
		}

		public async Task<ApplicationUser> UpdateAsync(ApplicationUser user, IList<RoleType> roleIds = null, string password = null)
		{
			using (var transaction = new TransactionManager(context).BeginTransaction())
			{
				try
				{
					user = await UpdateInternalAsync(user, roleIds, password);

					transaction.Commit();

					return user;
				}
				catch (Exception)
				{
					transaction.Rollback();
					throw;
				}
			}
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
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUserByActivationToken]);
			}
			if (user.Profile.IsConfirmed)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.UserAlreadyConfirmed]);
			}
			if (user.Profile.TokenExpirationDate.Subtract(DateTime.Now).Days < 0)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.ActivationTokenExpired]);
			}
			 
			return user;
		}

		public async Task<ApplicationUser> SignInAsync(ApplicationUser user)
		{
			await ValidateUserOnSignIn(user.UserName);

			await signInManager.SignInAsync(user, false);

			user.LastLoginDate = DateTime.Now;
			user = await UpdateAsync(user);

			return user;
		}

		public async Task<ApplicationUser> SignInAsync(string login, string password)
		{
			await ValidateUserOnSignIn(login);

			var result = await signInManager.PasswordSignInAsync(login,password, false, true);
			if (!result.Succeeded)
			{
				throw new AppValidationException(result.IsLockedOut ? ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.UserLockedOut] : ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.IncorrectUserPassword]);
			}

			var user = await FindAsync(login);

			user.LastLoginDate = DateTime.Now;
			user = await UpdateAsync(user);

			return user;
		}

		public async Task<ApplicationUser> RefreshSignInAsync(ApplicationUser user)
		{
			await signInManager.SignInAsync(user, false);

			return user;
		}

		public async Task<IList<PermissionType>> GetUserPermissions(ApplicationUser user)
		{
			var permissions = new List<PermissionType>();

			var roles = await userManager.GetRolesAsync(user);
			foreach (var role in roles)
			{
				var roleClaims = await roleManager.GetClaimsAsync(await roleManager.FindByNameAsync(role));

				foreach (var permission in roleClaims.Where(roleClaim => roleClaim.Type == IdentityConstants.PermissionRoleClaimType).Select(roleClaim => RoleClaimValueToPermission(roleClaim.Value)).Where(permission => !permissions.Contains(permission)))
				{
					permissions.Add(permission);
				}
			}

			return permissions;
		}

		public async Task<bool> IsSuperAdmin(ApplicationUser user)
		{
			return await userManager.IsInRoleAsync(user, appInfrastructureService.Get()
				.Roles.Single(x => x.Key == (int) RoleType.SuperAdminUser)
				.Text.Normalize());
		}

		public void SignOut(ApplicationUser user)
		{
			signInManager.SignOut();
		}

		public async Task SendActivationAsync(Guid id)
		{
			var dbUser = await GetAsync(id);
			dbUser.Profile.ConfirmationToken = Guid.NewGuid();
			dbUser.Profile.TokenExpirationDate = DateTime.Now.AddDays(options.ActivationTokenExpirationTermDays);

			await UpdateAsync(dbUser);

			await SendActivationAsync(dbUser);

		}

		public async Task<ApplicationUser> ChangePasswordAsync(ApplicationUser user, string oldPassword, string newPassword)
		{
			var result = await userManager.ChangePasswordAsync(user, oldPassword, newPassword);
			if (!result.Succeeded)
			{
				throw new AppValidationException(AggregateIdentityErrors(result.Errors));
			}

			return user;
		}

		public async Task<ApplicationUser> UpdateWithPasswordChangeAsync(ApplicationUser user, string oldPassword,
			string newPassword, IList<RoleType> roleIds = null)
		{
			using (var transaction = new TransactionManager(context).BeginTransaction())
			{
				try
				{
					user = await UpdateInternalAsync(user, roleIds);

					user = await ChangePasswordAsync(user, oldPassword, newPassword);

					transaction.Commit();

					return user;
				}
				catch (Exception)
				{
					transaction.Rollback();
					throw;
				}
			}
		}

		public async Task<ApplicationUser> FindAsync(string login)
		{
			return await userManager.FindByEmailAsync(login);
		}

		public async Task<PagedList<ApplicationUser>> GetAsync(FilterBase filter)
		{
			Expression<Func<ApplicationUser, bool>> query = x => !x.DeletedDate.HasValue;
			if (!string.IsNullOrWhiteSpace(filter.SearchText))
			{
				var keyword = filter.SearchText.ToLower();
				query = query.And(x => x.FirstName.ToLower().Contains(keyword) ||
										x.LastName.ToLower().Contains(keyword) ||
									   //(x.FirstName + " " + x.LastName).ToLower().Contains(keyword) || uncomment when stupid ef becomes stable
									   x.Email.ToLower().Contains(keyword));
			}

			IEnumerable<ApplicationUser> queryable = await userManager.Users.Include(x => x.Profile).Include(x => x.Roles).AsNoTracking().Where(query).ToListAsync();// remove this bullshit when stupid ef starts working
			var overallCount = queryable.Count();

			var sortOrder = filter.Sorting.SortOrder;
			switch (filter.Sorting.Path)
			{
				case UserSortPath.AgentId:
					queryable = sortOrder == SortOrder.Asc
						? queryable.OrderBy(x => x.Profile.AgentId)
						: queryable.OrderByDescending(x => x.Profile.AgentId);
					break;
				case UserSortPath.FullName:
					queryable = sortOrder == SortOrder.Asc
						? queryable.OrderBy(x => x.FirstName + " " + x.LastName)
						: queryable.OrderByDescending(x => x.FirstName + " " + x.LastName);
					break;
				case UserSortPath.Email:
					queryable = sortOrder == SortOrder.Asc
						? queryable.OrderBy(x => x.Email)
						: queryable.OrderByDescending(x => x.Email);
					break;
				case UserSortPath.Status:
					queryable = sortOrder == SortOrder.Asc
						? queryable.OrderBy(x => x.Status)
						: queryable.OrderByDescending(x => x.Status);
					break;
				case UserSortPath.LastLoginDate:
					queryable = sortOrder == SortOrder.Asc
						? queryable.OrderBy(x => x.LastLoginDate)
						: queryable.OrderByDescending(x => x.LastLoginDate);
					break;
				default:
					queryable = queryable.OrderByDescending(x => x.UpdatedDate);
					break;
			}

			var pageIndex = filter.Paging.PageIndex;
			var pageItemCount = filter.Paging.PageItemCount;

			var items = queryable.Skip((pageIndex - 1)*pageItemCount).Take(pageItemCount).ToList();

			return new PagedList<ApplicationUser>(items, overallCount);
		}

		public async Task SendResetPasswordAsync(Guid publicId)
		{
			var user = await GetAsync(publicId);

			var token = await userManager.GeneratePasswordResetTokenAsync(user);

			await notificationService.SendPasswordResetAsync(user.Email, new PasswordReset()
			{
				FirstName = user.FirstName,
				LastName = user.LastName,
				Link = $"{options.AdminHost}#/authentication/passwordreset/{token}"
			});
		}

		public async Task ResetPasswordAsync(string email, string token, string newPassword)
		{
			var user = await FindAsync(email);
			if (user == null)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUser]);
			}

			if (user.Status == UserStatus.Disabled)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.UserIsDisabled]);
			}

			var result = await userManager.ResetPasswordAsync(user, token, newPassword);
			if (!result.Succeeded)
			{
				throw new AppValidationException(AggregateIdentityErrors(result.Errors));
			}
			else
			{
				user.Profile.IsConfirmed = true;
				await UpdateAsync(user);
			}
		}
	}
}