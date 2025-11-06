using StretchFitnessHub.Models;
using System.Collections.Generic;

namespace StretchFitnessHub.ViewModels
{
    public class WalkinViewModel
    {
        public List<ClassSchedule> ClassList { get; set; } = new List<ClassSchedule>();

        public int SelectedPlanId { get; set; }
        public int SelectedClassId { get; set; }
    }
}
