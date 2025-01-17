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
    public class FundLogService : IFundLogService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly ILogger<FundLogService> _logger;
        private readonly IFundLogRepository _fundLogRepository;

        public FundLogService(IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            ILogger<FundLogService> logger,
            IFundLogRepository fundLogRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _logger = logger;
            _fundLogRepository = fundLogRepository;
        }

        public async Task<PagedResponse<List<FundLogResponse>>> GetLogs(LogQueryParameters queryParameters, CancellationToken cancellationToken)
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

            var logResponse = await _fundLogRepository.GetPagedByDateRange(getLogDto, cancellationToken);

            if (logResponse.Items.Count <= 0)
            {
                throw new ApiException($"No logs found.");
            }

            var response = _mapper.Map<List<FundLogResponse>>(logResponse.Items);

            return new PagedResponse<List<FundLogResponse>>(response, queryParameters.PageNumber, queryParameters.PageSize, logResponse.TotalRecords, $"Successfully retrieved logs");
        }

        public async Task<Response<FundLogResponse>> GetLogById(long id, CancellationToken cancellationToken)
        {
            BankToBrokerFundWalletLog log = await _fundLogRepository.GetByIdAsync(id, cancellationToken) ?? throw new ApiException($"No log found.");

            var response = _mapper.Map<FundLogResponse>(log);

            return new Response<FundLogResponse>(response, $"Successfully retrieved log details.");
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

            var logResponse = await _fundLogRepository.GetByDateRange(getInflowDto, cancellationToken);

            if (logResponse.Count <= 0)
            {
                throw new ApiException("No logs found.");
            }

            var response = _mapper.Map<List<FundLogResponse>>(logResponse);

            return CoreHelpers.ExportCsv(response);
        }
    }
}
