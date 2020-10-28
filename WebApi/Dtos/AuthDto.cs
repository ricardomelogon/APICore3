using System.Collections.Generic;

#pragma warning disable IDE1006 // Naming Styles - Naming style defined by a third party and should not be changed
namespace WebApi.Dtos
{
    public class AuthStatusDto
    {
        public AuthDto AuthDto { get; set; }
        public string Status { get; set; }
    }

    public class AuthToken
    {
        public string idToken { get; set; }
    }

    public class FBTokenDetails
    {
        public FBTokenDetailsResponse data { get; set; }
        public FBTokenDetailsError error { get; set; }
    }

    public class FBTokenDetailsResponse
    {
        public string app_id { get; set; }
        public string type { get; set; }
        public string application { get; set; }
        public int? data_access_expires_at { get; set; }
        public int? expires_at { get; set; }
        public bool? is_valid { get; set; }
        public string user_id { get; set; }
        public ICollection<string> scopes { get; set; }
        public FBTokenDetailsError error { get; set; }
    }

    public class FBTokenDetailsError
    {
        public string message { get; set; }
        public string type { get; set; }
        public int? code { get; set; }
        public string fbtrace_id { get; set; }
    }

    public class FBUserInfo
    {
        public string id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public FBUserInfoPicture picture { get; set; }
    }

    public class FBUserInfoPicture
    {
        public int? height { get; set; }
        public bool? is_silhouette { get; set; }
        public string url { get; set; }
        public int width { get; set; }
    }
}