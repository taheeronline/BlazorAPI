using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using MovieManager.BlazorUI.DTOs.UserDTOs;
using System.Security.Claims;

namespace MovieManager.BlazorUI.Providers
{
    // Note: We are implementing IDisposable to clean up the timer
    public class CustomAuthStateProvider : AuthenticationStateProvider, IDisposable
    {
        private readonly ProtectedLocalStorage _localStorage;
        private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());

        // The background timer
        private System.Timers.Timer? _logoutTimer;

        public CustomAuthStateProvider(ProtectedLocalStorage localStorage)
        {
            _localStorage = localStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var userSessionResult = await _localStorage.GetAsync<UserDTO>("UserSession");
                var userSession = userSessionResult.Success ? userSessionResult.Value : null;

                if (userSession == null)
                    return new AuthenticationState(_anonymous);

                if (DateTime.UtcNow >= userSession.ExpirationTime)
                {
                    await _localStorage.DeleteAsync("UserSession");
                    return new AuthenticationState(_anonymous);
                }

                // If session is valid, start the background timer for the remaining time
                StartExpirationTimer(userSession.ExpirationTime);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, userSession.Id.ToString()),
                    new Claim(ClaimTypes.Name, userSession.Name),
                    new Claim(ClaimTypes.Email, userSession.Email),
                    new Claim(ClaimTypes.Role, userSession.Role)
                };

                var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "CustomAuth"));
                return new AuthenticationState(claimsPrincipal);
            }
            catch
            {
                return new AuthenticationState(_anonymous);
            }
        }

        public async Task UpdateAuthenticationState(UserDTO? userSession)
        {
            ClaimsPrincipal claimsPrincipal;

            if (userSession != null)
            {
                await _localStorage.SetAsync("UserSession", userSession);

                // Start the countdown timer when they log in
                StartExpirationTimer(userSession.ExpirationTime);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, userSession.Id.ToString()),
                    new Claim(ClaimTypes.Name, userSession.Name),
                    new Claim(ClaimTypes.Email, userSession.Email),
                    new Claim(ClaimTypes.Role, userSession.Role),
                    // ADD THIS LINE: Pass the exact expiration time to the UI
                    new Claim("Expiration", userSession.ExpirationTime.ToString("O"))
                };
                claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "CustomAuth"));
            }
            else
            {
                await _localStorage.DeleteAsync("UserSession");

                // Stop the timer if they are logging out
                ClearTimer();

                claimsPrincipal = _anonymous;
            }

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }

        // --- Timer Logic Below ---

        private void StartExpirationTimer(DateTime expirationTime)
        {
            ClearTimer(); // Ensure no old timers are running in the background

            var timeRemaining = expirationTime - DateTime.UtcNow;

            if (timeRemaining.TotalMilliseconds <= 0)
            {
                // Time has already passed, trigger logout immediately
                _ = LogoutDueToExpiration();
                return;
            }

            // Create a timer for the exact amount of time left
            _logoutTimer = new System.Timers.Timer(timeRemaining.TotalMilliseconds);

            // When the timer finishes, trigger the logout method
            _logoutTimer.Elapsed += async (sender, e) => await LogoutDueToExpiration();

            // Only run once, do not repeat
            _logoutTimer.AutoReset = false;
            _logoutTimer.Start();
        }

        private async Task LogoutDueToExpiration()
        {
            ClearTimer();
            // Re-use your existing logic to wipe storage and update the UI
            await UpdateAuthenticationState(null);
        }

        private void ClearTimer()
        {
            if (_logoutTimer != null)
            {
                _logoutTimer.Stop();
                _logoutTimer.Dispose();
                _logoutTimer = null;
            }
        }

        // Fulfill the IDisposable contract
        public void Dispose()
        {
            ClearTimer();
        }
    }
}