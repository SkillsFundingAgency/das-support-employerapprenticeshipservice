using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.IdentityModel.Protocols;

namespace SFA.DAS.EAS.Support.Web
{
    [ExcludeFromCodeCoverage]
    public class TokenValidationHandler : DelegatingHandler
    {
        private static string _audience = string.Empty;
        private readonly string _authority = string.Empty;

        private readonly string _tenant = string.Empty;

        private string _issuer = string.Empty;
        private List<SecurityToken> _signingTokens;
        private DateTime _stsMetadataRetrievalTime = DateTime.MinValue;
        private readonly string scopeClaimType = "http://schemas.microsoft.com/identity/claims/scope";
        private readonly string _scope = string.Empty;
        const string AuthorityBaseUrl = "https://login.microsoftonline.com/";
            
        public TokenValidationHandler()
        {

            var settings = DependencyResolver.Current.GetService<ISiteConnectorSettings>();
            _tenant = settings.Tenant;
            _audience = settings.Audience;
            _authority = $"{AuthorityBaseUrl}{_tenant}";
            _scope = settings.Scope;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            string jwtToken = null;
            var authHeader = request.Headers.Authorization;
            if (authHeader != null) jwtToken = authHeader.Parameter;

            if (jwtToken == null)
            {
                var response = BuildResponseErrorMessage(HttpStatusCode.Unauthorized);
                return response;
            }

            string issuer;
            List<SecurityToken> signingTokens;

            try
            {
                if (DateTime.UtcNow.Subtract(_stsMetadataRetrievalTime).TotalHours > 24
                    || string.IsNullOrEmpty(_issuer)
                    || _signingTokens == null)
                {
                    var stsDiscoveryEndpoint = $"{_authority}/.well-known/openid-configuration";
                    var configManager = new ConfigurationManager<OpenIdConnectConfiguration>(stsDiscoveryEndpoint);
                    var config = await configManager.GetConfigurationAsync(cancellationToken);
                    _issuer = config.Issuer;
                    _signingTokens = config.SigningTokens.ToList();
                    _stsMetadataRetrievalTime = DateTime.UtcNow;
                }
                issuer = _issuer;
                signingTokens = _signingTokens;
            }
            catch (Exception)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }

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
                    var response = BuildResponseErrorMessage(HttpStatusCode.Forbidden);
                    return response;
                }

                return await base.SendAsync(request, cancellationToken);
            }
            catch (SecurityTokenValidationException)
            {
                var response = BuildResponseErrorMessage(HttpStatusCode.Unauthorized);
                return response;
            }
            catch (Exception)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        private HttpResponseMessage BuildResponseErrorMessage(HttpStatusCode statusCode)
        {
            var response = new HttpResponseMessage(statusCode);
            var parameter = "authorization_uri=\"" + _authority + "\"" + "," + "resource_id=" + _audience;
            var authenticateHeader = new AuthenticationHeaderValue("Bearer",parameter);
            response.Headers.WwwAuthenticate.Add(authenticateHeader);
            return response;
        }
    }
}