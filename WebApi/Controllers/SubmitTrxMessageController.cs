using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WebApiLibrary.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubmitTrxMessageController : ControllerBase
    {
        private readonly ILogger<SubmitTrxMessageController> _logger;

        public SubmitTrxMessageController(ILogger<SubmitTrxMessageController> logger)
        {
            _logger = logger;
        }

        // POST: api/<SubmitTrxMessageController>
        [HttpPost]
        public IResponseModel PostSubmitTrxMessage([FromBody] TransactionModel request)
        {
            _logger.LogInformation("Request: {request}", JsonSerializer.Serialize(request));

            var message = request.Validate();
            _logger.LogInformation("Message: {message}", message);

            if (string.IsNullOrEmpty(message) == false)
            {
                var response = new ResponseFailedModel()
                {
                    Result = 0,
                    ResultMessage = message
                };

                _logger.LogInformation("Failed Response: {response}", JsonSerializer.Serialize(response));

                return response;
            }
            else
            {
                var response = new ResponseSuccessModel()
                {
                    Result = 1,
                    TotalAmount = request.TotalAmount,
                    TotalDiscount = request.CalculateDiscount(),
                    FinalAmount = request.CalculateFinalAmount()
                };

                _logger.LogInformation("Success Response: {response}", JsonSerializer.Serialize(response));

                return response;
            }
        }
    }
}
