using System;
using System.Data;

namespace Planner.Tools
{
    public class Planner
    {
        public Participant Participant { get; }
        public string Name { get; set;  }
        public DayOfWeek FirstDay { get; set; }
        public ClockTime StartTime { get; }
        public ClockTime StopTime { get; }
        public ClockTimeInterval Interval { get; }
        public DataTable Task { get; set; }

        public Planner(Participant Participant, string PlannerName, DayOfWeek FirstDay, ClockTime StartHour, ClockTime StopHour, ClockTimeInterval TimeSpan)
        {
            this.Participant = Participant;
            this.Name = PlannerName;
            this.FirstDay = FirstDay;
            this.StartTime = StartHour;
            this.StopTime = StopHour;
            this.Interval = TimeSpan;
        }

        public Planner(Participant Participant, string PlannerName, DayOfWeek FirstDay, ClockTime StartHour, ClockTime StopHour, ClockTimeInterval TimeSpan, DataTable Task)
        {
            this.Participant = Participant;
            this.Name = PlannerName;
            this.FirstDay = FirstDay;
            this.StartTime = StartHour;
            this.StopTime = StopHour;
            this.Interval = TimeSpan;
            this.Task = Task;
        }
    }
}
