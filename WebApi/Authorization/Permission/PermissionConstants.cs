using System;

namespace WebApi.Authorization
{
    public static class PermissionConstants
    {
        public const string ClaimType = "Permissions";
        public const string SubscriptionClaimType = "SubscriptionRefresh";

        public const bool SubscriptionEnabled = false;

        /// <summary>
        /// In Seconds
        /// </summary>
        public const int SubscriptionRefreshTime = 60 * 60 * 24;

        public const bool SubscriptionInternalValidation = false;

        public const string ExternalValidationDomain = "localhost:5001";
        public const string ExternalValidationLink = "https://" + ExternalValidationDomain + "/Subscription/Validate";

        public const string ApplicationId = "9775ba4f-5ac3-4dbe-ae04-1857ec29ad53";

        private static DateTime? SubscriptionDate = null;

        private static DateTime SubscriptionDateLastRefresh = DateTime.MinValue;

        public static DateTime? GetSubscriptionDate() => SubscriptionDate;

        public static void SetSubscriptionDate(DateTime date) => SubscriptionDate = date;

        public static DateTime GetSubscriptionDateLastRefresh() => SubscriptionDateLastRefresh;

        public static void RenewSubscriptionDateLastRefresh() => SubscriptionDateLastRefresh = DateTime.UtcNow;

        /// <summary>
        /// In Seconds
        /// </summary>
        public const int SubscriptionDateRefreshInterval = 60 * 60 * 24;
    }
}