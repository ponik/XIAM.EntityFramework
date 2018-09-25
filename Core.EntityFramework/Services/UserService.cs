using IdentityServer3.Core;
using IdentityServer3.Core.Extensions;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services.Default;
using IdentityServer3.Core.Services.InMemory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityModel;
using System.Security.Claims;
using IdentityServer3.EntityFramework.Entities;

namespace IdentityServer3.EntityFramework.Services
{
    public class UserService : UserServiceBase
    {
        private readonly IUserConfigurationDbContext dbContext;
        private readonly EntityFrameworkServiceOptions options;

        public UserService(IUserConfigurationDbContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            this.dbContext = context;
        }

        public UserService(EntityFrameworkServiceOptions options, IUserConfigurationDbContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            this.options = options;
            this.dbContext = context;
        }



        /// <summary>
        /// This methods gets called for local authentication (whenever the user uses the username and password dialog).
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public override Task AuthenticateLocalAsync(LocalAuthenticationContext context)
        {
            var query =
                from u in dbContext.Users
                where u.Username == context.UserName && u.Password == context.Password
                select u;

            var user = query.SingleOrDefault()?.ToModel();
            if (user != null)
            {
                context.AuthenticateResult = new AuthenticateResult(user.Subject, GetDisplayName(user));
            }

            return Task.FromResult(0);
        }

        /// <summary>
        /// This method gets called when the user uses an external identity provider to authenticate.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public override Task AuthenticateExternalAsync(ExternalAuthenticationContext context)
        {
            var query =
                from u in dbContext.Users
                where
                    u.Provider == context.ExternalIdentity.Provider &&
                    u.ProviderId == context.ExternalIdentity.ProviderId
                select u;

            var user = query.SingleOrDefault()?.ToModel();
            if (user == null)
            {
                string displayName;

                var name = context.ExternalIdentity.Claims.FirstOrDefault(x => x.Type == Constants.ClaimTypes.Name);
                if (name == null)
                {
                    displayName = context.ExternalIdentity.ProviderId;
                }
                else
                {
                    displayName = name.Value;
                }

                user = new InMemoryUser
                {
                    Subject = CryptoRandom.CreateUniqueId(),
                    Provider = context.ExternalIdentity.Provider,
                    ProviderId = context.ExternalIdentity.ProviderId,
                    Username = displayName,
                    Claims = context.ExternalIdentity.Claims
                };
                dbContext.Users.Add(user.ToEntity());
            }

            context.AuthenticateResult = new AuthenticateResult(user.Subject, GetDisplayName(user), identityProvider: context.ExternalIdentity.Provider);

            return Task.FromResult(0);
        }

        /// <summary>
        /// This method is called whenever claims about the user are requested (e.g. during token creation or via the userinfo endpoint)
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public override Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var query =
                from u in dbContext.Users
                where u.Subject == context.Subject.GetSubjectId()
                select u;
            var user = query.Single()?.ToModel();

            if (user != null)
            {
                var claims = new List<Claim>{
                    new Claim(Constants.ClaimTypes.Subject, user.Subject),
                };

                claims.AddRange(user.Claims);
                if (!context.AllClaimsRequested)
                {
                    claims = claims.Where(x => context.RequestedClaimTypes.Contains(x.Type)).ToList();
                }

                context.IssuedClaims = claims;
            }
            return Task.FromResult(0);
        }

        /// <summary>
        /// This method gets called whenever identity server needs to determine if the user is valid or active (e.g. during token issuance or validation)
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">subject</exception>
        public override Task IsActiveAsync(IsActiveContext context)
        {
            if (context.Subject == null) throw new ArgumentNullException("subject");

            var query =
                from u in dbContext.Users
                where
                    u.Subject == context.Subject.GetSubjectId()
                select u;

            var user = query.SingleOrDefault();

            context.IsActive = (user != null) && user.Enabled;

            return Task.FromResult(0);
        }

        /// <summary>
        /// Retrieves the display name.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        protected virtual string GetDisplayName(InMemoryUser user)
        {
            var nameClaim = user.Claims.FirstOrDefault(x => x.Type == Constants.ClaimTypes.Name);
            if (nameClaim != null)
            {
                return nameClaim.Value;
            }

            return user.Username;
        }
    }
}
