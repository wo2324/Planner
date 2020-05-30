using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planner.Utils
{
    class ClockTime
    {
        public int Hour { get; set; }
        public int Minute { get; set; }

        public ClockTime(int hour, int minute)
        {
            this.Hour = hour;
            this.Minute = minute;
        }

        public override string ToString()
        {
            return $"{this.Hour.ToString("D2")}:{this.Minute.ToString("D2")}";
        }
    }
}
