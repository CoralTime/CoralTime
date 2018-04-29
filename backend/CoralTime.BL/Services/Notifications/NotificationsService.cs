using AutoMapper;
using CoralTime.BL.Interfaces;
using CoralTime.Common.Helpers;
using CoralTime.DAL.Repositories;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System.Threading.Tasks;

namespace CoralTime.BL.Services
{
    public partial class NotificationsService : BaseService, INotificationService
    {
        private readonly IConfiguration _configuration;

        public NotificationsService(UnitOfWork uow, IMapper mapper, IConfiguration configuration)
            : base(uow, mapper)
        {
            _configuration = configuration;
        }

        private class EmailSenderSimpleModel
        {
            public string Subject { get; set; }

            public string ToEmail { get; set; }

            public string EmailText { get; set; }
        }

        private async Task EmailSenderSimple(IConfiguration configuration, EmailSenderSimpleModel emailSenderSimpleModel)
        {
            var body = new TextPart("html")
            {
                Text = emailSenderSimpleModel.EmailText
            };

            var emailSender = new EmailSender(configuration);

            emailSender.CreateSimpleMessage(emailSenderSimpleModel.ToEmail, new Multipart {body}, emailSenderSimpleModel.Subject);

            await emailSender.SendMessageAsync();
        }
    }
} 
