using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebApi.Data;
using WebApi.Data.Entities;

namespace WebApi.Authorization
{
    public class PermissionCalculator
    {
        private readonly DataContext context;

        public PermissionCalculator(DataContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Get all User Permissions and packages them into a string
        /// </summary>
        /// <param name="UserId">Id of the user</param>
        /// <returns></returns>
        public async Task<string> CalculatePermissions(Guid UserId)
        {
            ICollection<Permissions> userPermissions = await context.UserPermissions
                .Where(u => u.UserId == UserId)
                .Select(x => x.Permissions)
                .ToListAsync()
                ;

            IEnumerable<Permission> Permissions = userPermissions.Select(a => a.Enum).Where(b => b.HasValue).Select(a => a.Value);

            return Permissions.PackPermissionsIntoString();
        }

#pragma warning disable CS0162 // Part of the code is unreachable by design

        public async Task<DateTime> CalculateSubscription()
        {
            DateTime date = DateTime.MinValue;
            try
            {
                if (PermissionConstants.SubscriptionInternalValidation)
                {
                    return await context.Subscription.Select(a => a.Date).FirstOrDefaultAsync();
                }
                else
                {
                    DateTime Refresh = PermissionConstants.GetSubscriptionDateLastRefresh();
                    DateTime? SubscriptionDate = PermissionConstants.GetSubscriptionDate();
                    if (Refresh.AddSeconds(PermissionConstants.SubscriptionDateRefreshInterval) < DateTime.UtcNow || !SubscriptionDate.HasValue)
                    {
                        Uri uri = new Uri(PermissionConstants.ExternalValidationLink);
                        using HttpClient client = new HttpClient();
                        string Result = string.Empty;
                        ValidationData model = new ValidationData
                        {
                            ApplicationId = PermissionConstants.ApplicationId,
                        };
                        string json = JsonConvert.SerializeObject(model);
                        StringContent stringContent = new StringContent(json, Encoding.UTF8, "application/json");
                        HttpResponseMessage response = await client.PostAsync(uri, stringContent);

                        if (response.IsSuccessStatusCode)
                        {
                            Result = await response.Content.ReadAsStringAsync();
                            date = JsonConvert.DeserializeObject<DateTime>(Result);
                        }
                        stringContent.Dispose();
                        response.Dispose();
                        client.Dispose();
                        PermissionConstants.SetSubscriptionDate(date);
                        PermissionConstants.RenewSubscriptionDateLastRefresh();
                        return date;
                    }
                    else return SubscriptionDate.Value;
                }
            }
            catch (Exception e)
            {
                try
                {
                    context.ErrorLogs.Add(new ErrorLog { Disabled = false, Log = e.Message, Method = "PermissionCalculator", Path = "", RowDate = DateTime.UtcNow });
                    await context.SaveChangesAsync();
                }
                catch (Exception) { }
                return date;
            }
        }
    }
}