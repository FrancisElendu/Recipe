using Microsoft.Extensions.Configuration;
using Recipe.Data.Repository.IRepository;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Recipe.Data.Repository
{
    public class MailRepository : IMailRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _config;
        public MailRepository(ApplicationDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }
        public async Task SendEmailAsync(string toEmail, string subject, string content)
        {
           // _db.Log = Response.Output;

            var apiKey = _config["SendGridAPIKey"];
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("test@example.com", "JWT Auth Demo");
            var to = new EmailAddress(toEmail);
            //var plainTextContent = "and easy to do anywhere, even with C#";
            //var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, content, content);
            var response = await client.SendEmailAsync(msg);
        }
    }
}
