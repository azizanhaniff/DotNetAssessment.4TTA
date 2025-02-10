namespace WebApiLibrary.Models
{
    public class ResponseSuccessModel : IResponseModel
    {
        public int Result { get; set; }
        public long? TotalAmount { get; set; }
        public long? TotalDiscount { get; set; }
        public long? FinalAmount { get; set; }
    }
}
