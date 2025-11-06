using StretchFitnessHub.Models;

namespace StretchFitnessHub.ViewModels
{
    public class RegistrationPageViewModel
    {
        public RegistrationRequest Request { get; set; } = new RegistrationRequest();

        public List<ClassSchedule> ClassList { get; set; } = new List<ClassSchedule>();
    }
}
