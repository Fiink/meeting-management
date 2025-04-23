namespace MeetingManagementSystem.Models
{
    /// <summary>
    /// Denotes a range of time, typically used for meeting durations.
    /// Note that the StartTime must be before EndTime
    /// </summary>
    public class TimeRange
    {
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }

        public TimeRange(DateTimeOffset startTime, DateTimeOffset endTime)
        {
            if (startTime > endTime)
            {
                throw new ArgumentException("End time cannot be before start time");
            } else if (startTime == endTime)
            {
                throw new ArgumentException("Start time and end time cannot be identical");
            }
            StartTime = startTime;
            EndTime = endTime;
        }

        // Optional: Add a method to check if a DateTimeOffset falls within the range
        public bool Contains(DateTimeOffset dateTime)
        {
            return dateTime >= StartTime && dateTime <= EndTime;
        }

        public override string ToString()
        {
            return $"Start: {StartTime}, End: {EndTime}";
        }
    }
}
