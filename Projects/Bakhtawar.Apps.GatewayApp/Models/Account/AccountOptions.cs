using System;

namespace Bakhtawar.Apps.GatewayApp.Models.Account
{
    public static class AccountOptions
    {
        public static bool AllowLocalLogin = true;

        public static bool AllowRememberLogin = true;

        public static TimeSpan RememberMeLoginDuration = TimeSpan.FromDays(30);

        public static bool ShowLogoutPrompt = true;

        public static bool AutomaticRedirectAfterSignOut = true;

        public static string InvalidCredentialsErrorMessage = "Invalid username or password";
    }
}
