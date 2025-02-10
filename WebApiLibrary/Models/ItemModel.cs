using WebApiLibrary.Utilities;

namespace WebApiLibrary.Models
{
    public class ItemModel
    {
        public string? PartnerItemRef { get; set; }
        public string? Name { get; set; }
        public int? Qty { get; set; }
        public long? UnitPrice { get; set; }

        public string? Validate()
        {
            var message = ValidatePartnerItemRef();
            if (string.IsNullOrEmpty(message) == false) { return message; }

            message = ValidateName();
            if (string.IsNullOrEmpty(message) == false) { return message; }

            message = ValidateQty();
            if (string.IsNullOrEmpty(message) == false) { return message; }

            message = ValidateUnitPrice();
            if (string.IsNullOrEmpty(message) == false) { return message; }

            return message;
        }

        private string? ValidatePartnerItemRef()
        {
            if (string.IsNullOrEmpty(PartnerItemRef))
            {
                return ValidationMessage.Required("PartnerItemRef");
            }
            else if (PartnerItemRef.Length > 50)
            {
                return ValidationMessage.MaxLength("PartnerItemRef", 50);
            }

            return null;
        }

        private string? ValidateName()
        {
            if (string.IsNullOrEmpty(Name))
            {
                return ValidationMessage.Required("Name");
            }
            else if (Name.Length > 100)
            {
                return ValidationMessage.MaxLength("Name", 100);
            }

            return null;
        }

        private string? ValidateQty()
        {
            if (Qty == null)
            {
                return ValidationMessage.Required("Qty");
            }
            else if (Qty <= 1)
            {
                return ValidationMessage.MustMoreThanValue("Qty", 1);
            }
            else if (Qty > 5)
            {
                return ValidationMessage.NotExceedValue("Qty", 5);
            }

            return null;
        }

        private string? ValidateUnitPrice()
        {
            if (UnitPrice == null)
            {
                return ValidationMessage.Required("UnitPrice");
            }
            else if (UnitPrice < 0)
            {
                return ValidationMessage.PositiveValue("UnitPrice");
            }

            return null;
        }
    }
}
