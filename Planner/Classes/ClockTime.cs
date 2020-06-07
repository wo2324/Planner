﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planner.Classes
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

        public static bool operator ==(ClockTime clockTimeModel, ClockTime clockTimeSample)
        {
            if (clockTimeModel.Hour == clockTimeSample.Hour && clockTimeModel.Minute == clockTimeSample.Minute)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool operator !=(ClockTime clockTimeModel, ClockTime clockTimeSample)
        {
            if (clockTimeModel.Hour != clockTimeSample.Hour || clockTimeModel.Minute != clockTimeSample.Minute)
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
            ClockTime result = new ClockTime();
            result.Hour = (clockTime.Hour + clockTimeInterval.Hour) % 24;
            result.Minute = (clockTime.Minute + clockTimeInterval.Minute) % 60;
            if ((clockTime.Minute + clockTimeInterval.Minute) / 60 != 0)
            {
                result.Hour++;
            }
            if (result.Hour == 24)
            {
                result.Hour = 0;
            }
            return result;
        }

        #endregion 

        public override string ToString()
        {
            return $"{this.Hour.ToString("D2")}:{this.Minute.ToString("D2")}";
        }

        public void AddInterval(ClockTimeInterval interval)
        {
            this.Hour = (this.Hour + interval.Hour) % 24;
            this.Minute = (this.Minute + interval.Minute) % 60;
            if ((this.Minute + interval.Minute) / 60 != 0)
            {
                this.Hour++;
            }
            if (this.Hour == 24)
            {
                this.Hour = 0;
            }
        }

        public void SubtractInterval(ClockTimeInterval interval)
        {
            if (this.Hour < interval.Hour || this.Hour == interval.Hour && this.Minute < interval.Minute)
            {
                throw new NegativeIntervalException();
            }
            this.Hour = this.Hour - interval.Hour;
            if (this.Minute - interval.Minute >= 0)
            {
                this.Minute = this.Minute - interval.Minute;
            }
            else
            {
                this.Minute = (this.Minute - interval.Minute) + 60;
                this.Hour--;
            }
        }
    }

    public class NegativeIntervalException : ApplicationException
    {
        public NegativeIntervalException()
        {
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
            ClockTimeInterval result = new ClockTimeInterval();
            result.Hour = clockTimeInterval.Hour * number;
            result.Hour = result.Hour + clockTimeInterval.Minute * number / 60;
            result.Minute = (clockTimeInterval.Minute * number) % 60;
            return result;
        }

        #endregion 

        public static ClockTimeInterval GetInterval(ClockTime clockStartTime, ClockTime clockStopTime)
        {
            int hour;
            if (clockStopTime.Hour - clockStartTime.Hour >= 0)
            {
                hour = clockStopTime.Hour - clockStartTime.Hour;
            }
            else
            {
                hour = (clockStopTime.Hour - clockStartTime.Hour) + 24;
            }

            int minute;
            if (clockStopTime.Minute - clockStartTime.Minute >= 0)
            {
                minute = clockStopTime.Minute - clockStartTime.Minute;
            }
            else
            {
                minute = (clockStopTime.Minute - clockStartTime.Minute) + 60;
                hour--;
            }

            return new ClockTimeInterval(hour, minute);
        }
    }
}