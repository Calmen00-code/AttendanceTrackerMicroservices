namespace AttendanceTrackerMicroservices.Models.ViewModels
{
    public class DailyAttendanceRecordVM
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public DateTime Date { get; set; }
        public string CheckIn { get; set; }
        public string CheckOut { get; set; }
    }
}
