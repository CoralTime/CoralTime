using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System.Net;
using System.Threading.Tasks;

namespace CoralTime.Common.Helpers
{
    public class EmailSender
    {
        private MimeMessage _message;

        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void CreateSimpleMessage(string emailTo, MimeEntity messageBody, string subject, string[] CcEmails = null, string[] BccEmails = null)
        {
            _message = new MimeMessage
            {
                Subject = subject,
                Body = messageBody
            };

            _message.From.Add(new MailboxAddress(_configuration["Email:From"]));
            _message.To.Add(new MailboxAddress(emailTo));

            if (CcEmails != null)
            {
                foreach (var email in CcEmails)
                {
                    _message.Cc.Add(new MailboxAddress(email));
                }
            }

            if (BccEmails != null)
            {
                foreach (var email in BccEmails)
                {
                    _message.Bcc.Add(new MailboxAddress(email));
                }
            }
        }

        public async Task SendMessageAsync()
        {
            using (var client = new SmtpClient())
            {
                client.Connect(_configuration["Email:Host"], int.Parse(_configuration["Email:Port"]), SecureSocketOptions.Auto);

                bool.TryParse(_configuration["Email:EnableAuthetication"] ?? "false", out bool enableAuthetication);

                if (enableAuthetication)
                {
                    client.Authenticate(new NetworkCredential(_configuration["Email:Login"], _configuration["Email:Password"]));
                }

                await client.SendAsync(_message);
                client.Disconnect(true);
            }
        }
    }
}