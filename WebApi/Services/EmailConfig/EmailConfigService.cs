using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Data;
using WebApi.Data.Entities;
using WebApi.Dtos;
using Z.EntityFramework.Plus;

namespace WebApi.Services
{
    public partial class EmailConfigService : IEmailConfigService
    {
        private readonly DataContext db;
        private readonly IErrorLogService errorLogService;

        public EmailConfigService(DataContext context, IErrorLogService errorLogService)
        {
            this.db = context;
            this.errorLogService = errorLogService;
        }

        public async Task<EmailConfig> GetById(int id)
        {
            try
            {
                return await db.EmailConfigs.Where(m => m.Id == id).FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                if (!await errorLogService.InsertException(e)) throw;
                return null;
            }
        }

        public async Task Update(EmailConfigItemDto model)
        {
            try
            {
                if (!model.Id.HasValue || !model.Port.HasValue || !model.EnableSSL.HasValue || string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Host) || string.IsNullOrWhiteSpace(model.UserName)) throw new Exception("One or more required fields are missing");
                EmailConfig record = await db.EmailConfigs.Where(q => q.Id == model.Id).SingleOrDefaultAsync();
                if (record == null) throw new Exception("Email config not found");
                if (!string.IsNullOrWhiteSpace(model.Password)) record.Password = model.Password;
                record.DisplayName = model.DisplayName;
                record.Email = model.Email;
                record.Host = model.Host;
                record.Port = model.Port.Value;
                record.UserName = model.UserName;
                record.EnableSSL = model.EnableSSL.Value;
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                await errorLogService.InsertException(e);
                throw;
            }
        }

        public async Task<EmailAccount> GetDefaultSender()
        {
            try
            {
                EmailConfig emailconfig = await db.EmailConfigs.Where(i => i.IsDefaultSender == true).FirstOrDefaultAsync();
                EmailAccount emailAccount = new EmailAccount();

                if (emailconfig != null)
                {
                    emailAccount.DisplayName = emailconfig.DisplayName;
                    emailAccount.Email = emailconfig.Email;
                    emailAccount.Password = emailconfig.Password;
                    emailAccount.Host = emailconfig.Host;
                    emailAccount.Port = emailconfig.Port;
                    emailAccount.Username = emailconfig.UserName;
                    emailAccount.EnableSsl = emailconfig.EnableSSL;
                }

                return emailAccount;
            }
            catch (Exception e)
            {
                if (!await errorLogService.InsertException(e)) throw;
                return null;
            }
        }

        public async Task<EmailAccount> GetDefaultReceiver()
        {
            try
            {
                EmailConfig emailconfig = await db.EmailConfigs.Where(i => i.Active.HasValue && i.Active.Value && i.IsDefaultReceiver).FirstOrDefaultAsync();
                if (emailconfig == null) return null;
                EmailAccount emailAccount = new EmailAccount();

                if (emailconfig != null)
                {
                    emailAccount.DisplayName = emailconfig.DisplayName;
                    emailAccount.Email = emailconfig.Email;
                    emailAccount.Password = emailconfig.Password;
                    emailAccount.Host = emailconfig.Host;
                    emailAccount.Port = emailconfig.Port;
                    emailAccount.Username = emailconfig.UserName;
                    emailAccount.EnableSsl = emailconfig.EnableSSL;
                }

                return emailAccount;
            }
            catch (Exception e)
            {
                if (!await errorLogService.InsertException(e)) throw;
                return null;
            }
        }

        public async Task<EmailConfigDto> GetConfiguration()
        {
            try
            {
                QueryFutureValue<EmailConfigItemDto> Sender_F = db.EmailConfigs.Where(a => a.IsDefaultSender).Select(a => new EmailConfigItemDto
                {
                    DisplayName = a.DisplayName,
                    Email = a.Email,
                    EnableSSL = a.EnableSSL,
                    Host = a.Host,
                    Id = a.Id,
                    Port = a.Port,
                    UserName = a.UserName
                }).DeferredSingleOrDefault().FutureValue();
                QueryFutureValue<EmailConfigItemDto> Receiver_F = db.EmailConfigs.Where(a => a.IsDefaultReceiver).Select(a => new EmailConfigItemDto
                {
                    DisplayName = a.DisplayName,
                    Email = a.Email,
                    EnableSSL = a.EnableSSL,
                    Host = a.Host,
                    Id = a.Id,
                    Port = a.Port,
                    UserName = a.UserName
                }).DeferredSingleOrDefault().FutureValue();

                return new EmailConfigDto
                {
                    Receiver = await Receiver_F.ValueAsync(),
                    Sender = await Sender_F.ValueAsync()
                };
            }
            catch (Exception e)
            {
                await errorLogService.InsertException(e); throw;
            }
        }
    }
}