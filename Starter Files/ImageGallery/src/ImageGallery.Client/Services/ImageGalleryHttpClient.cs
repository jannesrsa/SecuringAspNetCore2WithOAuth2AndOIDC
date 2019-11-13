using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ImageGallery.Client.Services
{
    public class ImageGalleryHttpClient : IImageGalleryHttpClient
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private HttpClient _httpClient = new HttpClient();

        public ImageGalleryHttpClient(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<HttpClient> GetClient()
        {
            var currentContext = _httpContextAccessor.HttpContext;
            string accessToken;

            var expiresAtToken = await currentContext.GetTokenAsync("expires_at");
            if (string.IsNullOrEmpty(expiresAtToken) ||
                (DateTime.TryParse(expiresAtToken, out DateTime expires_at) &&
                expires_at.AddSeconds(-60).ToUniversalTime() < DateTime.UtcNow))
            {
                accessToken = await RenewTokens();
            }
            else
            {
                accessToken = await currentContext.GetTokenAsync(
                   OpenIdConnectParameterNames.AccessToken);
            }

            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                _httpClient.SetBearerToken(accessToken);
            }

            _httpClient.BaseAddress = new Uri("https://localhost:1601/");
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            return await Task.FromResult(_httpClient);
        }

        private async Task<string> RenewTokens()
        {
            var currentContext = _httpContextAccessor.HttpContext;

            var discoveryClient = new DiscoveryClient("https://localhost:44379/");
            var metaDataResponse = await discoveryClient.GetAsync();

            var tokenClient = new TokenClient(metaDataResponse.TokenEndpoint, "imagegalleryclient", "secret");

            var currentRefreshToken = await currentContext
                .GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);

            var tokenResult = await tokenClient.RequestRefreshTokenAsync(currentRefreshToken);

            if (!tokenResult.IsError)
            {
                var updatedTokens = new List<AuthenticationToken>{
                    new AuthenticationToken
                    {
                        Name = OpenIdConnectParameterNames.IdToken,
                        Value= tokenResult.IdentityToken
                    },
                    new AuthenticationToken
                    {
                        Name = OpenIdConnectParameterNames.AccessToken,
                        Value= tokenResult.AccessToken
                    },
                    new AuthenticationToken
                    {
                        Name = OpenIdConnectParameterNames.RefreshToken,
                        Value= tokenResult.RefreshToken
                    }
                };

                var expiresAt = DateTime.UtcNow + TimeSpan.FromSeconds(tokenResult.ExpiresIn);
                updatedTokens.Add(new AuthenticationToken
                {
                    Name = "expires_at",
                    Value = expiresAt.ToString("o", CultureInfo.InvariantCulture)
                });

                var currentAuthenticateResult = await currentContext.AuthenticateAsync("Cookies");

                currentAuthenticateResult.Properties.StoreTokens(updatedTokens);

                await currentContext.SignInAsync("Cookies",
                    currentAuthenticateResult.Principal,
                    currentAuthenticateResult.Properties);

                return tokenResult.AccessToken;
            }
            else
            {
                throw new Exception();
            }
        }
    }
}