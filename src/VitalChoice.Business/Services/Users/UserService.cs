using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.Data.Entity;
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
using VitalChoice.Interfaces.Services.Users;

namespace VitalChoice.Business.Services.Users
{
	public abstract class UserService : IUserService
	{
		private readonly SignInManager<ApplicationUser> signInManager;

		private readonly IEcommerceRepositoryAsync<User> ecommerceRepositoryAsync;
		private readonly IUserValidator<ApplicationUser> userValidator;

		protected IAppInfrastructureService AppInfrastructureService { get; }

		protected UserManager<ApplicationUser> UserManager { get; }

		protected RoleManager<ApplicationRole> RoleManager { get; }

		protected INotificationService NotificationService { get; }

		protected AppOptions Options { get; }

		protected IDataContextAsync Context { get; }

		protected UserService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IDataContextAsync context, SignInManager<ApplicationUser> signInManager, IAppInfrastructureService appInfrastructureService, INotificationService notificationService, IOptions<AppOptions> options, IEcommerceRepositoryAsync<User> ecommerceRepositoryAsync, IUserValidator<ApplicationUser> userValidator)
		{
			UserManager = userManager;
			RoleManager = roleManager;
			Context = context;
			this.signInManager = signInManager;
			AppInfrastructureService = appInfrastructureService;
			NotificationService = notificationService;
			this.ecommerceRepositoryAsync = ecommerceRepositoryAsync;
			this.userValidator = userValidator;
			Options = options.Options;
		}

		protected abstract Task SendActivationInternalAsync(ApplicationUser dbUser);

		protected abstract Task SendResetPasswordInternalAsync(ApplicationUser dbUser, string token);

		protected abstract void ValidateRoleAssignments(ApplicationUser dbUser, IList<RoleType> roles);

		protected virtual async Task ValidateUserInternalAsync(ApplicationUser user)
		{
			var validateResult = await userValidator.ValidateAsync(UserManager, user);
			if (!validateResult.Succeeded)
			{
				throw new AppValidationException(AggregateIdentityErrors(validateResult.Errors));
			}
		}

		protected virtual async Task PrepareForAdd(ApplicationUser user, IList<RoleType> roles)
		{
			user.CreateDate = DateTime.Now;
			user.UpdatedDate = DateTime.Now;
			user.Status = UserStatus.NotActive;
			user.LastLoginDate = null;
			user.PublicId = Guid.NewGuid();
			user.UserName = user.Email;

			await ValidateUserInternalAsync(user);

			ValidateRoleAssignments(user, roles);
		}

		private async Task SendActivationAsync(ApplicationUser dbUser)
		{
			if (dbUser.Status != UserStatus.NotActive)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.UserAlreadyConfirmed]);
			}

			await SendActivationInternalAsync(dbUser);
		}

		public async Task<bool> ValidateEmailUniquenessAsync(string email)
		{
			var existing = await UserManager.Users.AnyAsync(x => x.Email.Equals(email));

			return !existing;
		}

		public async Task SendResetPasswordAsync(Guid publicId)
		{
			var user = await GetAsync(publicId);

			var token = await UserManager.GeneratePasswordResetTokenAsync(user);

			await SendResetPasswordInternalAsync(user, token);
		}

		private IList<string> GetRoleNamesByIds(IList<RoleType> roles)
		{
			return RoleManager.Roles.Where(x => roles.Contains((RoleType)x.Id))
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

		private async Task<ApplicationUser> UpdateInternalAsync(ApplicationUser user, IList<RoleType> roleIds = null,
			string password = null)
		{
			user.UpdatedDate = DateTime.Now;

			var updateResult = await UserManager.UpdateAsync(user);
			if (updateResult.Succeeded)
			{
				if (password != null)
				{
					var passwordResult = await UserManager.AddPasswordAsync(user, password);
					if (!passwordResult.Succeeded)
					{
						throw new AppValidationException(AggregateIdentityErrors(passwordResult.Errors));
					}
				}

				if (roleIds != null)
				{
					var oldRoleNames = await UserManager.GetRolesAsync(user);
					if (oldRoleNames.Any())
					{
						var deleteResult = await UserManager.RemoveFromRolesAsync(user, oldRoleNames);
						if (!deleteResult.Succeeded)
						{
							throw new AppValidationException(AggregateIdentityErrors(deleteResult.Errors));
						}
					}

					var roleNames = GetRoleNamesByIds(roleIds);
					if (roleNames.Any())
					{
						var addToRoleResult = await UserManager.AddToRolesAsync(user, roleNames);
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

		private async Task ValidateUserOnSignIn(string login)
		{
			var disabled = (await UserManager.Users.FirstOrDefaultAsync(x => x.Status == UserStatus.Disabled && x.Email.Equals(login))) != null;

			if (disabled)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.UserIsDisabled]);
			}

			//stupid ef
			var temp = await UserManager.Users.ToListAsync();
			var notConfirmed = temp.Any(x => !x.IsConfirmed && x.Email.Equals(login));

			if (notConfirmed)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.UserIsNotConfirmed]);
			}
		}

		public async Task<ApplicationUser> CreateAsync(ApplicationUser user, IList<RoleType> roles, bool sendActivation = true, bool createEcommerceUser = true)
		{
			await PrepareForAdd(user, roles);

			using (var transaction = new TransactionAccessor(Context).BeginTransaction())
			{
				try
				{
					var createResult = await UserManager.CreateAsync(user);
					if (createResult.Succeeded)
					{
						var roleNames = GetRoleNamesByIds(roles);
						if (roleNames.Any())
						{
							var addToRoleResult = await UserManager.AddToRolesAsync(user, roleNames);
							if (!addToRoleResult.Succeeded)
							{
								throw new AppValidationException(AggregateIdentityErrors(addToRoleResult.Errors));
							}
						}

						if (createEcommerceUser)
						{
							await ecommerceRepositoryAsync.InsertAsync(new User() { Id = user.Id });

						}

						transaction.Commit();

						if (sendActivation)
						{
							await SendActivationAsync(user.Email);
						}

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

		public async Task SendActivationAsync(string email)
		{
			var dbUser = await FindAsync(email);
			await SendActivationAsync(dbUser);
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
			await ValidateUserInternalAsync(user);

			ValidateRoleAssignments(user, roleIds);

			using (var transaction = new TransactionAccessor(Context).BeginTransaction())
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
			return await UserManager.Users.SingleOrDefaultAsync(x => x.PublicId == publicId);
		}

		public async Task<ApplicationUser> GetAsync(int internalId)
		{
			return await UserManager.Users.SingleOrDefaultAsync(x => x.Id == internalId);
		}

		public async Task<ApplicationUser> GetByTokenAsync(Guid token)
		{
			//var user = await UserManager.Users.Include(x => x.Profile).Include(x => x.Roles).SingleOrDefaultAsync(x => !x.DeletedDate.HasValue && x.Profile.ConfirmationToken == token);
			var temp = await UserManager.Users.ToListAsync();
			var user = temp.SingleOrDefault(x => x.ConfirmationToken == token);

			if (user == null)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUserByActivationToken]);
			}
			if (user.IsConfirmed)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.UserAlreadyConfirmed]);
			}
			if (user.TokenExpirationDate.Subtract(DateTime.Now).Days < 0)
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

			var roles = await UserManager.GetRolesAsync(user);
			foreach (var role in roles)
			{
				var roleClaims = await RoleManager.GetClaimsAsync(await RoleManager.FindByNameAsync(role));

				foreach (var permission in roleClaims.Where(roleClaim => roleClaim.Type == IdentityConstants.PermissionRoleClaimType).Select(roleClaim => RoleClaimValueToPermission(roleClaim.Value)).Where(permission => !permissions.Contains(permission)))
				{
					permissions.Add(permission);
				}
			}

			return permissions;
		}

		public async Task SignOutAsync(ApplicationUser user)
		{
			await signInManager.SignOutAsync();
		}

		public async Task ResendActivationAsync(Guid id)
		{
			var dbUser = await GetAsync(id);
			dbUser.ConfirmationToken = Guid.NewGuid();
			dbUser.TokenExpirationDate = DateTime.Now.AddDays(Options.ActivationTokenExpirationTermDays);

			await UpdateAsync(dbUser);

			await SendActivationAsync(dbUser);
		}

		public async Task<ApplicationUser> ChangePasswordAsync(ApplicationUser user, string oldPassword, string newPassword)
		{
			var result = await UserManager.ChangePasswordAsync(user, oldPassword, newPassword);
			if (!result.Succeeded)
			{
				throw new AppValidationException(AggregateIdentityErrors(result.Errors));
			}

			return user;
		}

		public async Task<ApplicationUser> UpdateWithPasswordChangeAsync(ApplicationUser user, string oldPassword,
			string newPassword, IList<RoleType> roleIds = null)
		{
			await ValidateUserInternalAsync(user);

			ValidateRoleAssignments(user, roleIds);

			using (var transaction = new TransactionAccessor(Context).BeginTransaction())
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
			return await UserManager.FindByEmailAsync(login);
		}

		public async Task<PagedList<ApplicationUser>> GetAsync(FilterBase filter)
		{
			Expression<Func<ApplicationUser, bool>> query = x => true;
			if (!string.IsNullOrWhiteSpace(filter.SearchText))
			{
				var keyword = filter.SearchText.ToLower();
				query = query.And(x => x.FirstName.ToLower().Contains(keyword) ||
										x.LastName.ToLower().Contains(keyword) ||
									   //(x.FirstName + " " + x.LastName).ToLower().Contains(keyword) || uncomment when stupid ef becomes stable
									   x.Email.ToLower().Contains(keyword));
			}

			IEnumerable<ApplicationUser> queryable = await UserManager.Users.AsNoTracking().Where(query).ToListAsync();// remove this bullshit when stupid ef starts working
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

			var result = await UserManager.ResetPasswordAsync(user, token, newPassword);
			if (!result.Succeeded)
			{
				throw new AppValidationException(AggregateIdentityErrors(result.Errors));
			}
			else
			{
				user.IsConfirmed = true;
				await UpdateAsync(user);
			}
		}
	}
}