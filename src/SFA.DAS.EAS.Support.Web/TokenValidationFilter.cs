using System;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.IdentityModel.Protocols;
using SFA.DAS.NLog.Logger;


namespace SFA.DAS.EAS.Support.Web
{
    public class TokenValidationFilter : FilterAttribute, IAuthorizationFilter
    {
        private readonly ILog _logger;
        private static string _audience = string.Empty;
        private readonly string _authority = string.Empty;

        private readonly string _tenant = string.Empty;

        private string _issuer = string.Empty;
        private List<SecurityToken> _signingTokens;
        private DateTime _stsMetadataRetrievalTime = DateTime.MinValue;
        private readonly string scopeClaimType = "http://schemas.microsoft.com/identity/claims/scope";
        private readonly string _scope = string.Empty;
        const string AuthorityBaseUrl = "https://login.microsoftonline.com/";
        public TokenValidationFilter(ISiteConnectorSettings settings, ILog logger)
        {
            _logger = logger;
            _tenant = settings.Tenant;
            _audience = settings.Audience;
            _authority = $"{AuthorityBaseUrl}{_tenant}";
            _scope = settings.Scope;
        }
        public void OnAuthorization(System.Web.Mvc.AuthorizationContext filterContext)
        {
            var request = filterContext.HttpContext.Request;

            var authHeader = request.Headers["Authorization"];
            if (authHeader == null)
            {
                _logger.Warn($"No Authorization header was provided by the caller");
                filterContext.Result = new HttpUnauthorizedResult();
                return;
            }
            var jwtToken = authHeader.Split(' ').LastOrDefault();
            if (jwtToken == null)
            {
                _logger.Warn($"No token was found in the Authorization header");
                filterContext.Result = new HttpUnauthorizedResult();
                return;
            }

            CancellationToken cancellationToken = new CancellationToken();
            if (
                DateTime.UtcNow.Subtract(_stsMetadataRetrievalTime).TotalHours > 24 ||
                string.IsNullOrEmpty(_issuer) ||
                _signingTokens == null
                )
            {
                try
                {

                    var stsDiscoveryEndpoint = $"{_authority}/.well-known/openid-configuration";
                    var configManager = new ConfigurationManager<OpenIdConnectConfiguration>(stsDiscoveryEndpoint);
                    var config = Task.Run(
                       async () => await configManager.GetConfigurationAsync(cancellationToken).ConfigureAwait(false), 
                                            cancellationToken).Result;

                    _issuer = config.Issuer;
                    _signingTokens = config.SigningTokens.ToList();



                    _stsMetadataRetrievalTime = DateTime.UtcNow;
                }
                catch (Exception e)
                {
                    _logger.Error(e, $"Exception occured obtaining configuration from authority");
                    filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                    return;
                }
            }
            var issuer = _issuer;
            var signingTokens = _signingTokens;

            var tokenHandler = new JwtSecurityTokenHandler();

            var validationParameters = new TokenValidationParameters
            {
                ValidAudience = _audience,
                ValidIssuer = issuer,
                IssuerSigningTokens = signingTokens,
                CertificateValidator = X509CertificateValidator.None
            };

            try
            {
                SecurityToken validatedToken = new JwtSecurityToken();
                var claimsPrincipal = tokenHandler.ValidateToken(jwtToken, validationParameters, out validatedToken);

                Thread.CurrentPrincipal = claimsPrincipal;

                if (HttpContext.Current != null) HttpContext.Current.User = claimsPrincipal;

                if (ClaimsPrincipal.Current.FindFirst(scopeClaimType) != null &&
                    ClaimsPrincipal.Current.FindFirst(scopeClaimType).Value != _scope)
                {
                    _logger.Warn($"The supplied token does not provide the required scope");
                    filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }
            }
            catch (SecurityTokenValidationException e)
            {
                _logger.Error(e, $"The supplied token is not valid. ");
                filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            catch (Exception e)
            {
                _logger.Error(e, $"An exception has occurred validating the supplied token");
                filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }
    }
}