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

        public Planner(Participant participant, string plannerName, DayOfWeek firstDay, ClockTime startHour, ClockTime stopHour, ClockTimeInterval timeSpan)
        {
            this.Participant = participant;
            this.Name = plannerName;
            this.FirstDay = firstDay;
            this.StartTime = startHour;
            this.StopTime = stopHour;
            this.Interval = timeSpan;
        }

        public Planner(Participant participant, string plannerName, DayOfWeek firstDay, ClockTime startHour, ClockTime stopHour, ClockTimeInterval timeSpan, DataTable task)
        {
            this.Participant = participant;
            this.Name = plannerName;
            this.FirstDay = firstDay;
            this.StartTime = startHour;
            this.StopTime = stopHour;
            this.Interval = timeSpan;
            this.Task = task;
        }
    }
}
