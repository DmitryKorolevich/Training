using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;
using VitalChoice.Business.Mail;
using VitalChoice.Data.Context;
using VitalChoice.Data.Extensions;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Data.Transaction;
using VitalChoice.Ecommerce.Domain.Entities.Users;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Permissions;
using VitalChoice.Infrastructure.Domain.Entities.Roles;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Identity;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Users;

namespace VitalChoice.Business.Services.Users
{
	public abstract class UserService : IUserService
	{
		private readonly SignInManager<ApplicationUser> _signInManager;

		private readonly IEcommerceRepositoryAsync<User> _ecommerceRepositoryAsync;
		private readonly IUserValidator<ApplicationUser> _userValidator;
	    private readonly ITransactionAccessor<VitalChoiceContext> _transactionAccessor;
	    private readonly ILogger _logger;

	    protected IAppInfrastructureService AppInfrastructureService { get; }

		protected UserManager<ApplicationUser> UserManager { get; }

		protected RoleManager<ApplicationRole> RoleManager { get; }

		protected INotificationService NotificationService { get; }

		protected AppOptions Options { get; }

		protected IDataContextAsync Context { get; }

	    protected UserService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, VitalChoiceContext context,
	        SignInManager<ApplicationUser> signInManager, IAppInfrastructureService appInfrastructureService,
	        INotificationService notificationService, IOptions<AppOptions> options, IEcommerceRepositoryAsync<User> ecommerceRepositoryAsync,
	        IUserValidator<ApplicationUser> userValidator, ITransactionAccessor<VitalChoiceContext> transactionAccessor, ILoggerProviderExtended loggerProvider)
	    {
	        UserManager = userManager;
	        RoleManager = roleManager;
	        Context = context;
	        this._signInManager = signInManager;
	        AppInfrastructureService = appInfrastructureService;
	        NotificationService = notificationService;
	        this._ecommerceRepositoryAsync = ecommerceRepositoryAsync;
	        this._userValidator = userValidator;
	        _transactionAccessor = transactionAccessor;
	        Options = options.Value;
	        _logger = loggerProvider.CreateLoggerDefault();

	    }

	    protected abstract Task SendActivationInternalAsync(ApplicationUser dbUser);

		protected abstract Task SendResetPasswordInternalAsync(ApplicationUser dbUser, string token);

        protected abstract Task SendForgotPasswordInternalAsync(ApplicationUser dbUser, string token);

        protected abstract void ValidateRoleAssignments(ApplicationUser dbUser, IList<RoleType> roles);

		protected virtual async Task ValidateUserInternalAsync(ApplicationUser user)
		{
			var validateResult = await _userValidator.ValidateAsync(UserManager, user);
			if (!validateResult.Succeeded)
			{
				throw new AppValidationException(AggregateIdentityErrors(validateResult.Errors));
			}
		}

		protected virtual async Task PrepareForAdd(ApplicationUser user, IList<RoleType> roles)
		{
			user.CreateDate = DateTime.Now;
			user.UpdatedDate = DateTime.Now;
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

        public async Task SendForgotPasswordAsync(Guid publicId)
        {
            var user = await GetAsync(publicId);

            var token = await UserManager.GenerateUserTokenAsync(user, IdentityConstants.TokenProviderName,IdentityConstants.ForgotPasswordResetPurpose);

            await SendForgotPasswordInternalAsync(user, token);
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

		private IEnumerable<MessageInfo> AggregateIdentityErrors(IEnumerable<IdentityError> errors)
		{
		    return errors.Select(e => new MessageInfo
		    {
		        Field = e.Code,
		        Message = e.Description,
		        MessageType = MessageType.FieldAsCode
		    });
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

        protected virtual async Task DisabledValidateUserOnSignIn(string login)
        {
            var disabled = (await UserManager.Users.FirstOrDefaultAsync(x => x.Status == UserStatus.Disabled && x.Email.Equals(login))) != null;

            if (disabled)
            {
                throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.UserIsDisabled]);
            }
        }

        protected virtual async Task ValidateUserOnSignIn(string login)
        {
            await DisabledValidateUserOnSignIn(login);

            var user = await UserManager.Users.FirstOrDefaultAsync(x => x.Status == UserStatus.Active && x.Email.Equals(login));

			if (user!=null && !user.IsConfirmed)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.UserIsNotConfirmed]);
			}
		}

		public async Task<ApplicationUser> CreateAsync(ApplicationUser user, IList<RoleType> roles, bool sendActivation = true, bool createEcommerceUser = true, string password = null)
		{
			await PrepareForAdd(user, roles);

			using (var transaction = _transactionAccessor.BeginTransaction())
			{
				try
				{
					var createResult = await UserManager.CreateAsync(user);
					if (createResult.Succeeded)
					{
						if (!string.IsNullOrWhiteSpace(password))
						{
							var passwordResult = await UserManager.AddPasswordAsync(user, password);
							if (!passwordResult.Succeeded)
							{
								throw new AppValidationException(AggregateIdentityErrors(passwordResult.Errors));
							}
						}

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
							await _ecommerceRepositoryAsync.InsertAsync(new User() { Id = user.Id });

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
                catch (Exception e)
                {
                    _logger.LogError(e.Message, e);
                    user.Id = 0;
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

	    public async Task<IdentityResult> RemoveAsync(int idInternal)
	    {
	        return await UserManager.DeleteAsync(await GetAsync(idInternal));
	    }

	    public async Task<ApplicationUser> UpdateAsync(ApplicationUser user, IList<RoleType> roleIds = null, string password = null)
		{
			await ValidateUserInternalAsync(user);

			ValidateRoleAssignments(user, roleIds);

			using (var transaction = _transactionAccessor.BeginTransaction())
			{
				try
				{
					user = await UpdateInternalAsync(user, roleIds, password);

					transaction.Commit();

					return user;
				}
                catch (Exception e)
                {
                    _logger.LogError(e.Message, e);
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
			if (token == Guid.Empty)
			{
				throw new ArgumentException("Token can't be null or empty");
			}

			var user = await UserManager.Users.SingleOrDefaultAsync(x => x.ConfirmationToken == token);

			if (user == null)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUserByActivationToken]);
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

			await _signInManager.SignInAsync(user, false);

			user.LastLoginDate = DateTime.Now;
			user = await UpdateAsync(user);

			return user;
		}

	    public async Task<ApplicationUser> SignInNoStatusCheckingAsync(ApplicationUser user)
	    {
            await DisabledValidateUserOnSignIn(user.UserName);

		    await _signInManager.SignInAsync(user, false);

            user.LastLoginDate = DateTime.Now;
            user = await UpdateAsync(user);

            return user;
        }

	    public async Task<ApplicationUser> SignInAsync(string login, string password)
		{
			await ValidateUserOnSignIn(login);

			var result = await _signInManager.PasswordSignInAsync(login,password, false, true);
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
			await _signInManager.SignInAsync(user, false);

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
			await _signInManager.SignOutAsync();
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

			using (var transaction = _transactionAccessor.BeginTransaction())
			{
				try
				{
					user = await UpdateInternalAsync(user, roleIds);

					user = await ChangePasswordAsync(user, oldPassword, newPassword);

					transaction.Commit();

					return user;
				}
                catch (Exception e)
                {
                    _logger.LogError(e.Message, e);
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
									   (x.FirstName + " " + x.LastName).ToLower().Contains(keyword) ||
									   x.Email.ToLower().Contains(keyword));
			}

			var queryable = UserManager.Users.Include(p=>p.Profile).ThenInclude(p=>p.AdminTeam).AsNoTracking().Where(query);
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

		    List<ApplicationUser> items = null;

		    if (filter.Paging != null)
		    {
		        var pageIndex = filter.Paging.PageIndex;
		        var pageItemCount = filter.Paging.PageItemCount;

		        items = await queryable.Skip((pageIndex - 1)*pageItemCount).Take(pageItemCount).ToListAsync();
		    }
		    else
		    {
                items = await queryable.ToListAsync();
            }

		    return new PagedList<ApplicationUser>(items, overallCount);
		}

		public async Task<ApplicationUser> ResetPasswordAsync(string email, string token, string newPassword)
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
				user.ConfirmationToken = Guid.Empty;
				user.IsConfirmed = true;
				return await UpdateAsync(user);
			}
		}
	}
}