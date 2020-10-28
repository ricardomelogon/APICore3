using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Data.Entities;

namespace WebApi.Services
{
    public partial interface IEmailSenderService
    {
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
        /// <param name="cc">CC addresses ist</param>
        /// <param name="attachmentFilePath">Attachment file path</param>
        /// <param name="attachmentFileName">Attachment file name. If specified, then this file name will be sent to a recipient. Otherwise, "AttachmentFilePath" name will be used.</param>
        /// <param name="headers">Headers</param>
        Task<bool> SendEmail
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
        );

        Task<bool> SendEmail(string Recipient, string Template, params string[] Parameters);
    }
}