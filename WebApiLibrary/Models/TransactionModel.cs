using System.Globalization;
using WebApiLibrary.Utilities;

namespace WebApiLibrary.Models
{
    public class TransactionModel
    {
        public string? PartnerKey { get; set; }
        public string? PartnerRefNo { get; set; }
        public string? PartnerPassword { get; set; }
        public long? TotalAmount { get; set; }
        public List<ItemModel> Items { get; set; } = new();
        public string? Timestamp { get; set; }
        public string? Sig { get; set; }

        public long CalculateDiscount()
        {
            if (string.IsNullOrEmpty(Validate()) == false) { return 0; }

            double discount = TotalAmount switch
            {
                long totalAmount when totalAmount <= 200 => 0,
                long totalAmount when totalAmount <= 500 => 0.05,
                long totalAmount when totalAmount <= 800 => 0.07,
                long totalAmount when totalAmount <= 1200 => 0.1,
                _ => 0.15,
            };

            if (PrimeNumber.IsPrimeNumber((long)TotalAmount!) && TotalAmount! > 500)
            {
                discount += 0.08;
            }

            if (((long)TotalAmount).ToString().EndsWith("5") && TotalAmount! > 900)
            {
                discount += 0.1;
            }

            if (discount > 0.2)
            {
                discount = 0.2;
            }

            return (long)(TotalAmount * discount);
        }

        public long CalculateFinalAmount()
        {
            if (string.IsNullOrEmpty(Validate()) == false) { return 0; }

            return (long)(TotalAmount! - CalculateDiscount());
        }

        public string GenerateSignature()
        {
            return Base64.Encode(
                SHA256.Encode(
                    string.Concat(
                        Timestamp == null ? "" : DateTime.ParseExact(Timestamp, "o", CultureInfo.InvariantCulture).ToUniversalTime().ToString("yyyyMMddHHmmss"),
                        PartnerKey,
                        PartnerRefNo,
                        TotalAmount,
                        PartnerPassword
                    )
                )
            );
        }

        public string? Validate()
        {
            var message = ValidatePartnerKey();
            if (string.IsNullOrEmpty(message) == false) { return message; }

            message = ValidatePartnerRefNo();
            if (string.IsNullOrEmpty(message) == false) { return message; }

            message = ValidatePartnerPassword();
            if (string.IsNullOrEmpty(message) == false) { return message; }

            message = ValidateTimestamp();
            if (string.IsNullOrEmpty(message) == false) { return message; }

            foreach (var item in Items)
            {
                message = item.Validate();
                if (string.IsNullOrEmpty(message) == false) { return message; }
            }

            message = ValidateSig();
            if (string.IsNullOrEmpty(message) == false) { return message; }

            message = ValidateTotalAmount();
            if (string.IsNullOrEmpty(message) == false) { return message; }

            if (Data.IsAllowedPartner(PartnerKey!, PartnerRefNo!, Base64.Decode(PartnerPassword!)) == false)
            {
                return "Access Denied!";
            }

            return message;
        }

        private string? ValidatePartnerKey()
        {
            if (string.IsNullOrEmpty(PartnerKey))
            {
                return ValidationMessage.Required("PartnerKey");
            }
            else if (PartnerKey.Length > 50)
            {
                return ValidationMessage.MaxLength("PartnerKey", 50);
            }

            return null;
        }

        private string? ValidatePartnerRefNo()
        {
            if (string.IsNullOrEmpty(PartnerRefNo))
            {
                return ValidationMessage.Required("PartnerRefNo");
            }
            else if (PartnerRefNo.Length > 50)
            {
                return ValidationMessage.MaxLength("PartnerRefNo", 50);
            }

            return null;
        }

        private string? ValidatePartnerPassword()
        {
            if (string.IsNullOrEmpty(PartnerPassword))
            {
                return ValidationMessage.Required("PartnerPassword");
            }
            else if (PartnerPassword.Length > 50)
            {
                return ValidationMessage.MaxLength("PartnerPassword", 50);
            }

            return null;
        }

        private string? ValidateTotalAmount()
        {
            if (TotalAmount == null)
            {
                return ValidationMessage.Required("TotalAmount");
            }
            else if (TotalAmount < 0)
            {
                return ValidationMessage.PositiveValue("TotalAmount");
            }
            if (TotalAmount != Items.Select(e => e.Qty * e.UnitPrice).Sum())
            {
                return "Invalid Total Amount.";
            }

            return null;
        }

        private string? ValidateTimestamp()
        {
            if (string.IsNullOrEmpty(Timestamp))
            {
                return ValidationMessage.Required("Timestamp");
            }
            else if (TimeDifference() > 300)
            {
                return "Expired.";
            }

            return null;
        }

        private double TimeDifference()
        {
            var now = DateTime.UtcNow;
            var timestamp = DateTime.ParseExact(Timestamp!, "o", CultureInfo.InvariantCulture).ToUniversalTime();

            TimeSpan ts = now - timestamp;

            return Math.Abs(ts.TotalSeconds);
        }

        private string? ValidateSig()
        {
            if (string.IsNullOrEmpty(Sig))
            {
                return ValidationMessage.Required("Sig");
            }
            else if (VerifySignatureHash() == false)
            {
                // Signature Mismatch
                return "Access Denied!";
            }

            return null;
        }

        private bool VerifySignatureHash()
        {
            return StringComparer.OrdinalIgnoreCase.Compare(Sig, GenerateSignature()) == 0;
        }
    }
}
