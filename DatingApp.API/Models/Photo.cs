using System;

namespace DatingApp.API.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Descriptipn { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsMain { get; set; }
    }
}