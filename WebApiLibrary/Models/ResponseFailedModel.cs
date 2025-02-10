namespace WebApiLibrary.Models
{
    public class ResponseFailedModel : IResponseModel
    {
        public int Result { get; set; }
        public string? ResultMessage { get; set; }
    }
}
