using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using WebApi.Data.Entities;

namespace WebApi.Services
{
    public partial class EmailSenderService : IEmailSenderService
    {
        private readonly IErrorLogService errorLogService;
        private readonly IEmailTemplateService emailTemplateService;
        private readonly IEmailConfigService emailConfigService;

        public EmailSenderService
        (
            IErrorLogService errorLogService,
            IEmailTemplateService emailTemplateService,
            IEmailConfigService emailConfigService
        )
        {
            this.errorLogService = errorLogService;
            this.emailTemplateService = emailTemplateService;
            this.emailConfigService = emailConfigService;
        }

        /// <summary>
        /// Sends an email
        /// </summary>
        /// <param name="emailAccount">Email account to use</param>
        /// <param name="subject">Subject</param>
        /// <param name="body">Body</param>
        /// <param name="toAddress">To address</param>
        /// <param name="replyTo">ReplyTo address</param>
        /// <param name="replyToName">ReplyTo display name</param>
        /// <param name="bcc">BCC addresses list</param>
        /// <param name="cc">CC addresses list</param>
        /// <param name="attachmentFilePath">Attachment file path</param>
        /// <param name="attachmentFileName">Attachment file name. If specified, then this file name will be sent to a recipient. Otherwise, "AttachmentFilePath" name will be used.</param>
        /// <param name="headers">Headers</param>
        public virtual async Task<bool> SendEmail
        (
            EmailAccount emailAccount,
            string subject,
            string body,
            string toAddress,
            string replyTo = null,
            string replyToName = null,
            IEnumerable<string> bcc = null,
            IEnumerable<string> cc = null,
            string attachmentFilePath = null,
            string attachmentFileName = null,
            IDictionary<string, string> headers = null
        )
        {
            MailMessage message = new MailMessage();
            try
            {
                if (emailAccount == null) throw new Exception("No email account configuration");
                message.From = new MailAddress(emailAccount.Email, emailAccount.DisplayName);

                //Send To
                if (toAddress != null) message.To.Add(toAddress.Trim());
                //Reply To
                if (!string.IsNullOrEmpty(replyTo)) message.ReplyToList.Add(new MailAddress(replyTo, replyToName));
                //BCC
                if (bcc != null) foreach (string address in bcc.Where(bccValue => !string.IsNullOrWhiteSpace(bccValue))) message.Bcc.Add(address.Trim());
                //CC
                if (cc != null) foreach (string address in cc.Where(ccValue => !string.IsNullOrWhiteSpace(ccValue))) message.CC.Add(address.Trim());

                //Content
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;

                //Headers
                if (headers != null) foreach (KeyValuePair<string, string> header in headers) message.Headers.Add(header.Key, header.Value);

                //Create file attachments for this e-mail message
                if (!string.IsNullOrEmpty(attachmentFilePath) && File.Exists(attachmentFilePath))
                {
                    Attachment attachment = new Attachment(attachmentFilePath);
                    attachment.ContentDisposition.CreationDate = File.GetCreationTime(attachmentFilePath);
                    attachment.ContentDisposition.ModificationDate = File.GetLastWriteTime(attachmentFilePath);
                    attachment.ContentDisposition.ReadDate = File.GetLastAccessTime(attachmentFilePath);
                    if (!string.IsNullOrEmpty(attachmentFileName)) attachment.Name = attachmentFileName;
                    message.Attachments.Add(attachment);
                }

                //Send email
                string userName = emailAccount.Username;
                string password = emailAccount.Password;
                SmtpClient client = new SmtpClient
                {
                    Host = emailAccount.Host,
                    Credentials = new System.Net.NetworkCredential(userName, password),
                    Port = emailAccount.Port,
                    EnableSsl = emailAccount.EnableSsl
                };
                client.Send(message);
                client.Dispose();
                message.Dispose();
                return true;
            }
            catch (Exception e)
            {
                message.Dispose();
                if (!await errorLogService.InsertException(e)) throw;
                return false;
            }
        }

        public async Task<bool> SendEmail(string Recipient, string Template, params string[] Parameters)
        {
            try
            {
                EmailTemplate EmailTemplate = await emailTemplateService.GetByName(Template);
                if (EmailTemplate == null) throw new Exception(Template + " is not a valid template");
                string EmailBody = string.Format(CultureInfo.InvariantCulture, EmailTemplate.Body, Parameters);
                EmailAccount SenderEmail = await emailConfigService.GetDefaultSender();
                if (SenderEmail == null) throw new Exception("No Default Email");
                return await SendEmail
                (
                    SenderEmail,
                    EmailTemplate.Subject,
                    EmailBody,
                    Recipient
                );
            }
            catch (Exception e)
            {
                await errorLogService.InsertException(e);
                throw;
            }
        }
    }
}