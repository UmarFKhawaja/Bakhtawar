using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Slugify;

namespace Bakhtawar.Apps.GatewayApp.Services
{
    public class FileSystemEmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        private readonly SlugHelper _slugHelper;
        
        public FileSystemEmailSender(IConfiguration configuration, SlugHelper slugHelper)
            : base()
        {
            _configuration = configuration;
            _slugHelper = slugHelper;
        }
        
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var emailSenderFolder = _configuration["EmailSender:Folder"];
            var emailSubject = _slugHelper.GenerateSlug(subject);
            var emailTimestamp = $"{DateTime.UtcNow:yyyy-MM-dd-HH-mm-ss}";
            var emailFile = Path.Combine(emailSenderFolder, email, emailTimestamp, $"{emailSubject}.txt");
            var emailFolder = Path.GetDirectoryName(emailFile);

            if (!Directory.Exists(emailFolder))
            {
                Directory.CreateDirectory(emailFolder);
            }
            
            await File.WriteAllTextAsync(emailFile, htmlMessage);
        }
    }
}