using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planner.Utils
{
    public class Planner
    {
        public int PlannerId { get; }
        public string PlannerName { get; }
        public DataTable Task { get; }
        public string FirstDay { get; set; }
        public string StartHour { get; set; }
        public string StopHour { get; set; }
        public string TimeSpan { get; set; }

        public Planner(int PlannerId, string PlannerName, DataTable Task, string FirstDay, string StartHour, string StopHour, string TimeSpan)
        {
            this.PlannerId = PlannerId;
            this.PlannerName = PlannerName;
            this.Task = Task;
            this.FirstDay = FirstDay;
            this.StartHour = StartHour;
            this.StopHour = StopHour;
            this.TimeSpan = TimeSpan;
        }
    }
}
