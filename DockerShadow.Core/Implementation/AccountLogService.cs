using AutoMapper;
using DockerShadow.Core.Contract;
using DockerShadow.Core.Contract.Repository;
using DockerShadow.Core.DTO.Request;
using DockerShadow.Core.DTO.Response;
using DockerShadow.Core.Exceptions;
using DockerShadow.Domain.Common;
using DockerShadow.Domain.Entities;
using DockerShadow.Domain.QueryParameters;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace DockerShadow.Core.Implementation
{
    public class AccountLogService : IAccountLogService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly ILogger<AccountLogService> _logger;
        private readonly IAccountLogRepository _AccountLogRepository;

        public AccountLogService(IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            ILogger<AccountLogService> logger,
            IAccountLogRepository AccountLogRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _logger = logger;
            _AccountLogRepository = AccountLogRepository;
        }

        public async Task<PagedResponse<List<AccountLogResponse>>> GetLogs(LogQueryParameters queryParameters, CancellationToken cancellationToken)
        {
            DateTime currentDay = DateTime.Now.Date;
            DateTime startDate = string.IsNullOrEmpty(queryParameters.StartDate) ? new DateTime(currentDay.Year, 1, 1) : DateTime.Parse(queryParameters.StartDate, null);
            DateTime endDate = string.IsNullOrEmpty(queryParameters.EndDate) ? DateTime.Now.Date : DateTime.Parse(queryParameters.EndDate, null);

            GetPagedTxnsByDateReq getLogDto = new()
            {
                StartDate = startDate,
                EndDate = endDate.AddDays(1),
                PageNumber = queryParameters.PageNumber,
                PageSize = queryParameters.PageSize
            };

            var logResponse = await _AccountLogRepository.GetPagedByDateRange(getLogDto, cancellationToken);

            if (logResponse.Items.Count <= 0)
            {
                throw new ApiException($"No logs found.");
            }

            var response = _mapper.Map<List<AccountLogResponse>>(logResponse.Items);

            return new PagedResponse<List<AccountLogResponse>>(response, queryParameters.PageNumber, queryParameters.PageSize, logResponse.TotalRecords, $"Successfully retrieved logs");
        }

        public async Task<Response<AccountLogResponse>> GetLogById(long id, CancellationToken cancellationToken)
        {
            BankToBrokerLog log = await _AccountLogRepository.GetByIdAsync(id, cancellationToken) ?? throw new ApiException($"No log found.");

            var response = _mapper.Map<AccountLogResponse>(log);

            return new Response<AccountLogResponse>(response, $"Successfully retrieved log details.");
        }

        public async Task<byte[]> DownloadLogs(DownloadLogsRequest request, CancellationToken cancellationToken)
        {
            DateTime currentDay = DateTime.Now.Date;
            DateTime startDate = string.IsNullOrEmpty(request.StartDate) ? new DateTime(currentDay.Year, 1, 1) : DateTime.Parse(request.StartDate, null);
            DateTime endDate = string.IsNullOrEmpty(request.EndDate) ? DateTime.Now.Date : DateTime.Parse(request.EndDate, null);

            GetLogsByDateReq getInflowDto = new()
            {
                StartDate = startDate,
                EndDate = endDate.AddDays(1),
            };

            var logResponse = await _AccountLogRepository.GetByDateRange(getInflowDto, cancellationToken);

            if (logResponse.Count <= 0)
            {
                throw new ApiException("No logs found.");
            }

            var response = _mapper.Map<List<AccountLogResponse>>(logResponse);

            return CoreHelpers.ExportCsv(response);
        }
    }
}
