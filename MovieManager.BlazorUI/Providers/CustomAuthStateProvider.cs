using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using MovieManager.BlazorUI.DTOs.UserDTOs;
using System.Security.Claims;

namespace MovieManager.BlazorUI.Providers
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ProtectedLocalStorage _localStorage;
        private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());

        public CustomAuthStateProvider(ProtectedLocalStorage localStorage)
        {
            _localStorage = localStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                // Retrieve the encrypted user session from the browser
                var userSessionResult = await _localStorage.GetAsync<UserDTO>("UserSession");
                var userSession = userSessionResult.Success ? userSessionResult.Value : null;

                if (userSession == null)
                    return new AuthenticationState(_anonymous);

                // Build the user's security claims based on the DTO
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, userSession.Id.ToString()),
                    new Claim(ClaimTypes.Name, userSession.Name),
                    new Claim(ClaimTypes.Email, userSession.Email),
                    new Claim(ClaimTypes.Role, userSession.Role) // Enables role-based access!
                };

                var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "CustomAuth"));
                return new AuthenticationState(claimsPrincipal);
            }
            catch
            {
                // If anything fails (like prerendering without JS access), return anonymous
                return new AuthenticationState(_anonymous);
            }
        }

        public async Task UpdateAuthenticationState(UserDTO? userSession)
        {
            ClaimsPrincipal claimsPrincipal;

            if (userSession != null)
            {
                // Save user to local storage and log them in
                await _localStorage.SetAsync("UserSession", userSession);
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, userSession.Id.ToString()),
                    new Claim(ClaimTypes.Name, userSession.Name),
                    new Claim(ClaimTypes.Email, userSession.Email),
                    new Claim(ClaimTypes.Role, userSession.Role)
                };
                claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "CustomAuth"));
            }
            else
            {
                // Clear the storage and log them out
                await _localStorage.DeleteAsync("UserSession");
                claimsPrincipal = _anonymous;
            }

            // Notify Blazor that the UI needs to update (e.g., hiding the login button)
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }
    }
}
