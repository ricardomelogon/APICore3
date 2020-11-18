namespace WebApi
{
    public static class EmailTemplates
    {
        public static class Account
        {
            public const string RegistrationCode = "Account.RegistrationCode";
            public const string ForgotPassword = "Account.ForgotPassword";
        }
    }

    public static class IdentitySettings
    {
        /// <summary>
        /// Minimum password length
        /// </summary>
        public const int PasswordLength = 8;

        /// <summary>
        /// Maximum number of auth tries before automatic lockout
        /// </summary>
        public const int LockoutTries = 10;

        /// <summary>
        /// Lockout period in minutes
        /// </summary>
        public const int LockoutTime = 60 * 24 * 30 * 12 * 10;
    }

    public static class JwtSettings
    {
        /// <summary>
        /// Expiration time in minutes
        /// </summary>
        public const int Expiration = 10;

        /// <summary>
        /// Threshold for token refresh.
        /// If the time to expiration is smaller the RefreshTime in minutes the token should be refreshed.
        /// </summary>
        public const int RefreshTime = 5;
    }

    public static class JwtClaimType
    {
        public const string UserId = "UserId";
        public const string Email = "Email";
        public const string RevokeCode = "RevokeCode";
        public const string Role = "Role";
        public const string FirstName = "FirstName";
        public const string LastName = "LastName";
    }

    public static class AuthStatus
    {
        /// <summary>
        /// Request success
        /// </summary>
        public const string Ok = "OK";

        /// <summary>
        /// Unidentified error
        /// </summary>
        public const string Error = "ERROR";

        /// <summary>
        /// No mandatory email provided to request
        /// </summary>
        public const string NoEmail = "NO_EMAIL";

        /// <summary>
        /// User e-mail was not yet confirmed
        /// </summary>
        public const string NoEmailConfirm = "NO_EMAIL_CONFIRM";

        /// <summary>
        /// Username is not in the appropriate format
        /// </summary>
        public const string UsernameNotValid = "USERNAME_NOT_VALID";

        /// <summary>
        /// User mandatory password was not provided to request
        /// </summary>
        public const string PasswordRequired = "PASSWORD_REQUIRED";

        /// <summary>
        /// The given username is already in use
        /// </summary>
        public const string UsernameTaken = "USERNAME_TAKEN";

        /// <summary>
        /// The given e-mail is already in use
        /// </summary>
        public const string EmailTaken = "EMAIL_TAKEN";

        /// <summary>
        /// User has been locked out of the application
        /// </summary>
        public const string UserLocked = "USER_LOCKED";

        /// <summary>
        /// Token provided is not in the correct format
        /// </summary>
        public const string TokenNotValid = "TOKEN_NOT_VALID";

        /// <summary>
        /// The given e-mail is not valid or does not correspond to the user's e-mail
        /// </summary>
        public const string LoginInvalid = "LOGIN_INVALID";

        /// <summary>
        /// The present token has been revoked
        /// </summary>
        public const string TokenRevoked = "TOKEN_REVOKED";

        /// <summary>
        /// The user details given do not have the required e-mail or username
        /// </summary>
        public const string NoUsernameOrEmail = "NO_USERNAME_OR_EMAIL";
    }

    public static class FacebookPermissions
    {
        public const string Email = "email";
        public const string PublicProfile = "public_profile";
        public const string GroupsAccessMemberInfo = "groups_access_member_info";
        public const string PublishToGroups = "publish_to_groups";
        public const string UserAgeRange = "user_age_range";
        public const string UserBirthday = "user_birthday";
        public const string UserEvents = "user_events";
        public const string UserFriends = "user_friends";
        public const string UserGender = "user_gender";
        public const string UserHometown = "user_hometown";
        public const string UserLikes = "user_likes";
        public const string UserLink = "user_link";
        public const string UserLocation = "user_location";
        public const string UserPhotos = "user_photos";
        public const string UserPosts = "user_posts";
        public const string UserTaggedPlaces = "user_tagged_places";
        public const string UserVideos = "user_videos";
    }
}