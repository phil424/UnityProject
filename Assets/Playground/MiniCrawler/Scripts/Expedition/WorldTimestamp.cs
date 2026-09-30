using System;

namespace MiniCrawler.Expedition
{
    public readonly struct WorldTimestamp : IComparable<WorldTimestamp>
    {
        public const int MinutesPerHour = 60;
        public const int HoursPerDay = 24;
        public const int MinutesPerDay = MinutesPerHour * HoursPerDay;

        public double TotalMinutes { get; }

        public int DayNumber => (int)Math.Floor(TotalMinutes / MinutesPerDay) + 1;

        public int MinuteOfDay
        {
            get
            {
                int wholeMinutes = (int)Math.Floor(TotalMinutes);
                return wholeMinutes % MinutesPerDay;
            }
        }

        public int Hour => MinuteOfDay / MinutesPerHour;
        public int Minute => MinuteOfDay % MinutesPerHour;

        public string ClockText => $"{Hour:00}:{Minute:00}";
        public string DayClockText => $"Day {DayNumber} {ClockText}";

        public WorldTimestamp(double totalMinutes)
        {
            TotalMinutes = Math.Max(0d, totalMinutes);
        }

        public static WorldTimestamp FromDayAndTime(int dayNumber, int hour, int minute)
        {
            dayNumber = Math.Max(1, dayNumber);
            hour = Math.Max(0, Math.Min(HoursPerDay - 1, hour));
            minute = Math.Max(0, Math.Min(MinutesPerHour - 1, minute));

            double totalMinutes =
                ((dayNumber - 1) * MinutesPerDay) +
                (hour * MinutesPerHour) +
                minute;

            return new WorldTimestamp(totalMinutes);
        }

        public WorldTimestamp AddMinutes(double minutes)
        {
            return new WorldTimestamp(TotalMinutes + minutes);
        }

        public double MinutesUntil(WorldTimestamp other)
        {
            return other.TotalMinutes - TotalMinutes;
        }

        public int CompareTo(WorldTimestamp other)
        {
            return TotalMinutes.CompareTo(other.TotalMinutes);
        }

        public override string ToString()
        {
            return DayClockText;
        }
    }
}