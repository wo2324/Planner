using System;

namespace Planner.Tools
{
    public class ClockTime
    {
        public int Hour { get; set; }
        public int Minute { get; set; }

        public ClockTime()
        {

        }

        public ClockTime(int hour, int minute)
        {
            this.Hour = hour;
            this.Minute = minute;
        }

        #region Operators overloading 

        public static bool operator ==(ClockTime clockTime, ClockTime clockTimeSample)
        {
            if (clockTime.Hour == clockTimeSample.Hour && clockTime.Minute == clockTimeSample.Minute)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool operator !=(ClockTime clockTime, ClockTime clockTimeSample)
        {
            if (clockTime.Hour != clockTimeSample.Hour || clockTime.Minute != clockTimeSample.Minute)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static ClockTime operator +(ClockTime clockTime, ClockTimeInterval clockTimeInterval)
        {
            int hour, minute;
            hour = (clockTime.Hour + clockTimeInterval.Hour) % 24;
            if ((clockTime.Minute + clockTimeInterval.Minute) / 60 != 0)
            {
                hour++;
            }
            if (hour == 24)
            {
                hour = 0;
            }
            minute = (clockTime.Minute + clockTimeInterval.Minute) % 60;
            return new ClockTime(hour, minute);
        }

        #endregion 

        public override string ToString()
        {
            return $"{this.Hour.ToString("D2")}:{this.Minute.ToString("D2")}";
        }

        public void AddInterval(ClockTimeInterval interval)
        {
            this.Hour = (this.Hour + interval.Hour) % 24;
            if ((this.Minute + interval.Minute) / 60 != 0)
            {
                this.Hour++;
            }
            if (this.Hour == 24)
            {
                this.Hour = 0;
            }
            this.Minute = (this.Minute + interval.Minute) % 60;
        }

        public void SubtractInterval(ClockTimeInterval interval)
        {
            this.Hour = this.Hour - interval.Hour;
            if (this.Minute - interval.Minute >= 0)
            {
                this.Minute = this.Minute - interval.Minute;
            }
            else
            {
                this.Hour--;
                this.Minute = (this.Minute - interval.Minute) + 60;
            }
        }
    }

    public class ClockTimeInterval : ClockTime
    {
        public ClockTimeInterval() : base()
        {
        }

        public ClockTimeInterval(int hour, int minute) : base(hour, minute)
        {
        }

        #region Operators overloading 

        public static ClockTimeInterval operator *(ClockTimeInterval clockTimeInterval, int number)
        {
            int hour, minute;
            hour = clockTimeInterval.Hour * number;
            hour = hour + clockTimeInterval.Minute * number / 60;
            minute = (clockTimeInterval.Minute * number) % 60;
            return new ClockTimeInterval(hour, minute);
        }

        #endregion 

        public static ClockTimeInterval GetInterval(ClockTime clockStartTime, ClockTime clockStopTime)
        {
            int hour, minute;
            if (clockStopTime.Hour - clockStartTime.Hour >= 0)
            {
                hour = clockStopTime.Hour - clockStartTime.Hour;
            }
            else
            {
                hour = (clockStopTime.Hour - clockStartTime.Hour) + 24;
            }
            if (clockStopTime.Minute - clockStartTime.Minute >= 0)
            {
                minute = clockStopTime.Minute - clockStartTime.Minute;
            }
            else
            {
                hour--;
                minute = (clockStopTime.Minute - clockStartTime.Minute) + 60;
            }
            return new ClockTimeInterval(hour, minute);
        }
    }
}
