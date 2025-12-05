using AttendanceTrackerMicroservices.Models;
using AttendanceTrackerMicroservices.Service.IService;
using static AttendanceTrackerMicroservices.Utility.SD;

namespace AttendanceTrackerMicroservices.Service
{
    public class TrackerService : ITrackerService
    {
        private readonly IBaseService _baseService;

        public TrackerService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDTO?> ShouldUserCheckIn(string userId)
        {
            RequestDTO requestDTO = new RequestDTO()
            {
                ApiType = ApiType.GET,
                Data = userId,
                Url = $"{TrackerAPIBase}/api/tracker/should-user-check-in/{userId}"
            };

            return await _baseService.SendAsync(requestDTO);
        }
    }
}
