using System;

namespace WebApi.Authorization
{
    public static class PermissionConstants
    {
        public const string Prefix = "Permissions";
        public const string ClaimType = "Permissions";
        public const string RefreshClaimType = "PermissionRefresh";
        public const string SubscriptionClaimType = "SubscriptionRefresh";

        /// <summary>
        /// In Seconds
        /// </summary>
        public const int RefreshTime = 60 * 60;

        public const bool RefreshEnabled = false;

        public const bool SubscriptionEnabled = true && RefreshEnabled;
        public const int SubscriptionRefreshTime = RefreshTime * 24;

        public const bool SubscriptionInternalValidation = false;

        public const string ExternalValidationDomain = "localhost:44328";
        public const string ExternalValidationLink = "https://" + ExternalValidationDomain + "/Subscription/Validate";

        public const string ApplicationId = "b8b3971b-8794-4a7d-b6c6-f5aea3981faa";

        private static DateTime? SubscriptionDate = null;

        private static DateTime SubscriptionDateLastRefresh = DateTime.MinValue;

        public static DateTime? GetSubscriptionDate() => SubscriptionDate;

        public static void SetSubscriptionDate(DateTime date) => SubscriptionDate = date;

        public static DateTime GetSubscriptionDateLastRefresh() => SubscriptionDateLastRefresh;

        public static void RenewSubscriptionDateLastRefresh() => SubscriptionDateLastRefresh = DateTime.UtcNow;

        /// <summary>
        /// In Seconds
        /// </summary>
        public const int SubscriptionDateRefreshInterval = RefreshTime * 24;
    }
}