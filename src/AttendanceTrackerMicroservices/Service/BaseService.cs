using AttendanceTrackerMicroservices.Models;
using AttendanceTrackerMicroservices.Service.IService;
using static AttendanceTrackerMicroservices.Utility.SD;
using Newtonsoft.Json;
using System.Text;
using System.Net;
using System.Linq.Expressions;

namespace AttendanceTrackerMicroservices.Service
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenProvider _tokenProvider;

        public BaseService(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider)
        {
            _httpClientFactory = httpClientFactory;
            _tokenProvider = tokenProvider;
        }

        public async Task<ResponseDTO?> SendAsync(RequestDTO requestDTO, bool withBearer = true)
        {
            try
            {
                // Creating client with the name `AttendanceTrackerAPI`
                HttpClient client = _httpClientFactory.CreateClient("AttendanceTrackerAPI");

                HttpRequestMessage message = new();

                // configuring the header
                message.Headers.Add("Accept", "application/json");

                // if JWT is used, try to get the token
                if (withBearer)
                {
                    var token = _tokenProvider.GetToken();
                    message.Headers.Add("Authorization", $"Bearer {token}");
                }

                // configuring the URL
                message.RequestUri = new Uri(requestDTO.Url);

                // Serializing the data into JSON String format
                // Note that only POST and PUT will have data
                if (requestDTO.Data != null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(requestDTO.Data), Encoding.UTF8, "application/json");
                }

                // configuring the API method type
                switch (requestDTO.ApiType)
                {
                    case ApiType.POST:
                        message.Method = HttpMethod.Post;
                        break;

                    case ApiType.DELETE:
                        message.Method = HttpMethod.Delete;
                        break;

                    case ApiType.PUT:
                        message.Method = HttpMethod.Put;
                        break;

                    default:
                        message.Method = HttpMethod.Get;
                        break;
                }

                // configuring the response message
                HttpResponseMessage? apiResponse = null;

                // invoke the services API endpoint 
                apiResponse = await client.SendAsync(message);

                switch (apiResponse.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        return new() { IsSuccess = false, Message = "Not found" };

                    case HttpStatusCode.Forbidden:
                        return new() { IsSuccess = false, Message = "Access denied" };

                    case HttpStatusCode.Unauthorized:
                        return new() { IsSuccess = false, Message = "Unauthorized" };

                    case HttpStatusCode.InternalServerError:
                        return new() { IsSuccess = false, Message = "Internal Server Error" };

                    default:
                        var apiContent = await apiResponse.Content.ReadAsStringAsync();
                        var apiResponseDto = JsonConvert.DeserializeObject<ResponseDTO>(apiContent);
                        return apiResponseDto;
                }
            }
            catch (Exception ex)
            {
                var dto = new ResponseDTO()
                {
                    Message = ex.Message.ToString(),
                    IsSuccess = false
                };
                return dto;
            }
        }
    }
}
