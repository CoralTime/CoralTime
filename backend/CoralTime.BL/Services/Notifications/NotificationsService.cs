using AutoMapper;
using CoralTime.BL.Interfaces;
using CoralTime.Common.Helpers;
using CoralTime.DAL.Repositories;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System.Threading.Tasks;
using CoralTime.BL.Interfaces.Reports;

namespace CoralTime.BL.Services
{
    public partial class NotificationsService : BaseService, INotificationService
    {
        private readonly IConfiguration _configuration;

        private readonly IReportExportService _reportExportService;

        public NotificationsService(UnitOfWork uow, IMapper mapper, IConfiguration configuration, IReportExportService reportExportService)
            : base(uow, mapper)
        {
            _reportExportService = reportExportService;
            _configuration = configuration;
        }

        private class EmailSenderSimpleModel
        {
            public string Subject { get; set; }

            public string ToEmail { get; set; }

            public string EmailText { get; set; }
        }

        private async Task CreateAndSendEmailNotificationForUserAsync(string emailText, string emailMember, string subject)
        {
            var emailSenderSimpleModel = new EmailSenderSimpleModel
            {
                Subject = subject,
                ToEmail = emailMember,
                EmailText = emailText
            };

            await EmailSenderSimple(_configuration, emailSenderSimpleModel);
        }

        private async Task EmailSenderSimple(IConfiguration configuration, EmailSenderSimpleModel emailSenderSimpleModel)
        {
            var body = new TextPart("html")
            {
                Text = emailSenderSimpleModel.EmailText
            };
            
            var multipart = new Multipart { body };

            var emailSender = new EmailSender(configuration);
            emailSender.CreateSimpleMessage(emailSenderSimpleModel.ToEmail, multipart, emailSenderSimpleModel.Subject);

            await emailSender.SendMessageAsync();
        }
    }
} 
