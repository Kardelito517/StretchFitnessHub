using System;
using System.Collections.Generic;
using System.Linq;

namespace StretchFitnessHub.Models
{
    public class DashboardViewModel
    {
        public string CurrentUserRole { get; set; } = "Admin";
        public int TotalMembers { get; set; }
        public int ClassesToday { get; set; }
        public int ExpiringSoon { get; set; }
        public List<Member> Members { get; set; } = new List<Member>();
        public List<Member> ArchivedMembers { get; set; } = new List<Member>();
        public List<Notification> Notifications { get; set; } = new List<Notification>();
        public List<RegistrationRequest> RegistrationRequests { get; set; } = new List<RegistrationRequest>();
        public int NotificationCount { get; set; }
        public int TotalMembersCount => Members?.Count ?? 0;
        public HashSet<string> OngoingClasses { get; set; } = new HashSet<string>();
        public HashSet<string> UpcomingClasses { get; set; } = new HashSet<string>();
        public List<ClassSchedule> ClassSchedules { get; set; } = new List<ClassSchedule>();
        public List<WalkIn> WalkIns { get; set; } = new List<WalkIn>();
        public List<Staff> Staffs { get; set; } = new List<Staff>();
    }
}
