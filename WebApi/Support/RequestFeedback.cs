namespace WebApi.Support
{
    public class RequestView
    {
        public string Text { get; set; }
        public string Title { get; set; }
        public bool Success { get; set; }
    }

    public class RequestFeedback<T> : RequestView
    {
        public T Data { get; set; }

        public RequestFeedback(T Data, string Title = "There was a problem resolving your request", string Text = "", bool Success = false)
        {
            this.Title = Title;
            this.Text = Text;
            this.Success = Success;
            this.Data = Data;
        }

        public RequestFeedback()
        {
            this.Data = default;
            this.Success = false;
            this.Text = string.Empty;
            this.Title = string.Empty;
        }
    }

    public class RequestFeedback : RequestFeedback<string>
    {
        public RequestFeedback(string Data = "", string Title = "", string Text = "", bool Success = false) : base(Data, Title, Text, Success)
        {
            this.Title = Title;
            this.Text = Text;
            this.Success = Success;
            this.Data = Data;
        }

        public RequestFeedback()
        {
            this.Data = string.Empty;
            this.Success = false;
            this.Text = string.Empty;
            this.Title = "There was a problem resolving your request";
        }
    }
}