using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Dtos;
using WebApi.Data.Entities;
using WebApi.Helpers;
using WebApi.Services.Extensions;
using WebApi.Data;

namespace WebApi.Services
{
    public partial class EmailTemplateService : IEmailTemplateService
    {
        private readonly DataContext db;
        private readonly IErrorLogService errorLogService;

        public EmailTemplateService(DataContext context, IErrorLogService errorLogService)
        {
            this.db = context;
            this.errorLogService = errorLogService;
        }

        public async Task Delete(EmailTemplate emailTemplate)
        {
            try
            {
                EmailTemplate record = db.EmailTemplates.Where(q => q.Id == emailTemplate.Id).FirstOrDefault();
                db.EmailTemplates.Remove(record);
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                await errorLogService.InsertException(e);
                throw;
            }
        }

        public async Task<ICollection<EmailDto>> EmailDto()
        {
            try
            {
                return await db.EmailTemplates.Select(a => new EmailDto { Name = a.Name, Subject = a.Subject, Id = a.Id }).ToListAsync();
            }
            catch (Exception e)
            {
                await errorLogService.InsertException(e); throw;
            }
        }

        public async Task<IQueryable<EmailTemplate>> GetAllQ()
        {
            try
            {
                return db.EmailTemplates.OrderByDescending(q => q.RowDate);
            }
            catch (Exception e)
            {
                if (!await errorLogService.InsertException(e)) throw;
                return null;
            }
        }

        public async Task<EmailTemplate> GetById(int id)
        {
            try
            {
                return await db.EmailTemplates.Where(m => m.Id == id).FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                if (!await errorLogService.InsertException(e)) throw;
                return null;
            }
        }

        public async Task<EmailTemplate> GetByName(string name)
        {
            try
            {
                return await db.EmailTemplates.Where(e => e.Name.ToUpper() == name.ToUpper()).SingleOrDefaultAsync();
            }
            catch (Exception e)
            {
                if (!await errorLogService.InsertException(e)) throw;
                else return null;
            }
        }

        public async Task Insert(EmailTemplate emailTemplate)
        {
            try
            {
                if (emailTemplate != null)
                {
                    emailTemplate.RowDate = DateTime.UtcNow;
                    await db.EmailTemplates.AddAsync(emailTemplate);
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                await errorLogService.InsertException(e);
                throw;
            }
        }

        public async Task<ICollection<EmailTemplate>> SearchByName(string name)
        {
            try
            {
                return await db.EmailTemplates.Where(e => e.Name.Contains(name, StringComparison.InvariantCulture)).OrderBy(q => q.Name).ToListAsync();
            }
            catch (Exception e)
            {
                if (!await errorLogService.InsertException(e)) throw;
                else return null;
            }
        }

        public async Task Update(EmailTemplate emailTemplate)
        {
            try
            {
                EmailTemplate record = db.EmailTemplates.Where(q => q.Id == emailTemplate.Id).FirstOrDefault();
                record.Body = emailTemplate.Body;
                record.Subject = emailTemplate.Subject;
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                await errorLogService.InsertException(e);
                throw;
            }
        }

        public async Task<EmailEditDto> GetEditById(int id)
        {
            try
            {
                return await db.EmailTemplates.Where(m => m.Id == id).Select(a => new EmailEditDto { Body = a.Body, Id = a.Id, Name = a.Name, Subject = a.Subject }).SingleOrDefaultAsync();
            }
            catch (Exception e)
            {
                await errorLogService.InsertException(e); throw;
            }
        }
    }
}