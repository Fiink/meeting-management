using MeetingManagementSystem.Data.Models;

namespace MeetingManagementSystem.Models
{
    /// <summary>
    /// Denotes a range of time, typically used for meeting durations.
    /// Note that the StartTime must be before EndTime
    /// </summary>
    public class TimeRange
    {
        public DateTimeOffset StartTime { get; }
        public DateTimeOffset EndTime { get; }

        public TimeRange(DateTimeOffset startTime, DateTimeOffset endTime)
        {
            if (startTime > endTime)
            {
                throw new ArgumentException("End time cannot be before start time");
            }
            else if (startTime == endTime)
            {
                throw new ArgumentException("Start time and end time cannot be identical");
            }
            StartTime = startTime;
            EndTime = endTime;
        }

        public bool Contains(DateTimeOffset dateTime)
        {
            return dateTime >= StartTime && dateTime <= EndTime;
        }

        public bool DoesOverlapWith(DateTimeOffset startTime, DateTimeOffset endTime)
        {
            return startTime < EndTime && StartTime < endTime;
        }

        public override string ToString()
        {
            return $"Start: {StartTime}, End: {EndTime}";
        }
    }
}
