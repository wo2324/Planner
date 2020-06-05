﻿using System;
using System.Data;

namespace Planner.Utils
{
    public class Planner
    {
        public Participant Participant { get; }
        public string Name { get; }
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
    }
}
