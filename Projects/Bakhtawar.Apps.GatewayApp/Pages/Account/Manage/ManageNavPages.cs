using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bakhtawar.Apps.GatewayApp.Pages.Account.Manage
{
    public static class ManageNavPages
    {
        public static string Index => "Index";

        public static string Email => "Email";

        public static string ChangePassword => "ChangePassword";

        public static string DownloadPersonalData => "DownloadPersonalData";

        public static string DeletePersonalData => "DeletePersonalData";

        public static string ExternalLogins => "ExternalLogins";

        public static string PersonalData => "PersonalData";

        public static string TwoFactorAuthentication => "TwoFactorAuthentication";

        public static string IndexNavClass(this ViewContext viewContext) => viewContext.PageNavClass(Index);

        public static string EmailNavClass(this ViewContext viewContext) => viewContext.PageNavClass(Email);

        public static string ChangePasswordNavClass(this ViewContext viewContext) => viewContext.PageNavClass(ChangePassword);

        public static string DownloadPersonalDataNavClass(this ViewContext viewContext) => viewContext.PageNavClass(DownloadPersonalData);

        public static string DeletePersonalDataNavClass(this ViewContext viewContext) => viewContext.PageNavClass(DeletePersonalData);

        public static string ExternalLoginsNavClass(this ViewContext viewContext) => viewContext.PageNavClass(ExternalLogins);

        public static string PersonalDataNavClass(this ViewContext viewContext) => viewContext.PageNavClass(PersonalData);

        public static string TwoFactorAuthenticationNavClass(this ViewContext viewContext) => viewContext.PageNavClass(TwoFactorAuthentication);

        private static string PageNavClass(this ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);

            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}
