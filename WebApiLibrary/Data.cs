using WebApiLibrary.Models;

namespace WebApiLibrary
{
    public static class Data
    {
        public static readonly List<AllowedPartnerModel> AllowedPartners = new() {
            new()
            {
                PartnerKey = "FAKEGOOGLE",
                PartnerRefNo = "FG-00001",
                PartnerPassword = "FAKEPASSWORD1234"
            },
            new()
            {
                PartnerKey = "FAKEPEOPLE",
                PartnerRefNo = "FG-00002",
                PartnerPassword = "FAKEPASSWORD4578"
            }
        };

        public static bool IsAllowedPartner(string partnerKey, string partnerRefNo, string partnerPassword)
        {
            return AllowedPartners
                .Any(e => e.PartnerKey!.Equals(partnerKey)
                          && e.PartnerRefNo!.Equals(partnerRefNo)
                          && e.PartnerPassword!.Equals(partnerPassword));
        }
    }
}
