using System;

namespace StretchFitnessHub.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; } = false;
        public DateTime DateCreated { get; set; } = DateTime.Now;
    }
}
