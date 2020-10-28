using System;

namespace WebApi.Dtos
{
    public class LogDto
    {
        public string Username { get; set; }
        public string Useremail { get; set; }
        public DateTime Date { get; set; }
    }

    public class ErrorLogDto : LogDto
    {
        public string Log { get; set; }
        public string Method { get; set; }
        public string Path { get; set; }
    }
}