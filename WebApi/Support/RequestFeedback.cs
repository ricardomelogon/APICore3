namespace WebApi.Support
{
    public class RequestView
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
    }

    public class RequestFeedback<T> : RequestView
    {
        public T Data { get; set; }

        public RequestFeedback(T Data, string Title = "There was a problem resolving your request", string Text = "", bool Success = false)
        {
            this.Message = Title;
            this.Status = Text;
            this.Success = Success;
            this.Data = Data;
        }

        public RequestFeedback()
        {
            this.Data = default;
            this.Success = false;
            this.Status = string.Empty;
            this.Message = string.Empty;
        }
    }

    public class RequestFeedback : RequestFeedback<string>
    {
        public RequestFeedback(string Data = "", string Title = "", string Text = "", bool Success = false) : base(Data, Title, Text, Success)
        {
            this.Message = Title;
            this.Status = Text;
            this.Success = Success;
            this.Data = Data;
        }

        public RequestFeedback()
        {
            this.Data = string.Empty;
            this.Success = false;
            this.Status = string.Empty;
            this.Message = "There was a problem resolving your request";
        }
    }
}