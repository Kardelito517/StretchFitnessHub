namespace StretchFitnessHub.Models
{
    public class ClassSchedule
    {
        public int Id { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public string Days { get; set; } = string.Empty; 
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Coach { get; set; } = string.Empty;
    }
}
