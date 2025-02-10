using System.Text.Json;
using WebApiLibrary.Models;
using WebApiLibrary.Utilities;

namespace WebApiLibrary.Tests
{
    public class TransactionModelTests
    {
        [Fact]
        public void SampleRequest()
        {
            // Arrange

            var request = new TransactionModel()
            {
                PartnerKey = "FAKEGOOGLE",
                PartnerRefNo = "FG-00001",
                PartnerPassword = "RkFLRVBBU1NXT1JEMTIzNA==",
                TotalAmount = 1000,
                Items = new List<ItemModel> {
                    new()
                    {
                        PartnerItemRef = "i-00001",
                        Name = "Pen",
                        Qty = 4,
                        UnitPrice = 200
                    },
                    new()
                    {
                        PartnerItemRef = "i-00002",
                        Name = "Ruler",
                        Qty = 2,
                        UnitPrice = 100
                    }
                },
                Timestamp = "2024-08-15T02:11:22.0000000Z",
                Sig = "MDE3ZTBkODg4ZDNhYzU0ZDBlZWRmNmU2NmUyOWRhZWU4Y2M1NzQ1OTIzZGRjYTc1ZGNjOTkwYzg2MWJlMDExMw=="
            };

            string? expected = "Expired.";


            // Act

            string? actual = null;

            var message = request.Validate();

            if (string.IsNullOrEmpty(message) == false)
            {
                var response = new ResponseFailedModel()
                {
                    Result = 0,
                    ResultMessage = message
                };

                actual = response.ResultMessage;
            }

            // Assert

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SampleCalculation1()
        {
            // Arrange

            var request = new TransactionModel()
            {
                PartnerKey = "FAKEGOOGLE",
                PartnerRefNo = "FG-00001",
                PartnerPassword = "RkFLRVBBU1NXT1JEMTIzNA==",
                TotalAmount = 1000,
                Items = new List<ItemModel> {
                    new()
                    {
                        PartnerItemRef = "i-00001",
                        Name = "Pen",
                        Qty = 4,
                        UnitPrice = 200
                    },
                    new()
                    {
                        PartnerItemRef = "i-00002",
                        Name = "Ruler",
                        Qty = 2,
                        UnitPrice = 100
                    }
                },
                Timestamp = DateTime.UtcNow.ToString("o"),
            };
            request.Sig = request.GenerateSignature();

            string? expected = JsonSerializer.Serialize(new ResponseSuccessModel
            {
                Result = 1,
                TotalAmount = 1000,
                TotalDiscount = 100,
                FinalAmount = 900
            });

            // Act

            ResponseSuccessModel actual = new ResponseSuccessModel();

            var message = request.Validate();

            if (string.IsNullOrEmpty(message))
            {
                var response = new ResponseSuccessModel()
                {
                    Result = 1,
                    TotalAmount = request.TotalAmount,
                    TotalDiscount = request.CalculateDiscount(),
                    FinalAmount = request.CalculateFinalAmount()
                };

                actual = response;
            }

            // Assert

            Assert.Equal(expected, JsonSerializer.Serialize(actual));
        }

        [Fact]
        public void SampleCalculation2()
        {
            // Arrange

            var request = new TransactionModel()
            {
                PartnerKey = "FAKEGOOGLE",
                PartnerRefNo = "FG-00001",
                PartnerPassword = "RkFLRVBBU1NXT1JEMTIzNA==",
                TotalAmount = 1205,
                Items = new List<ItemModel> {
                    new()
                    {
                        PartnerItemRef = "i-00001",
                        Name = "Pen",
                        Qty = 5,
                        UnitPrice = 101
                    },
                    new()
                    {
                        PartnerItemRef = "i-00002",
                        Name = "Ruler",
                        Qty = 2,
                        UnitPrice = 350
                    }
                },
                Timestamp = DateTime.UtcNow.ToString("o"),
            };
            request.Sig = request.GenerateSignature();

            string? expected = JsonSerializer.Serialize(new ResponseSuccessModel
            {
                Result = 1,
                TotalAmount = 1205,
                TotalDiscount = 241,
                FinalAmount = 964
            });

            // Act

            ResponseSuccessModel actual = new ResponseSuccessModel();

            var message = request.Validate();

            if (string.IsNullOrEmpty(message))
            {
                var response = new ResponseSuccessModel()
                {
                    Result = 1,
                    TotalAmount = request.TotalAmount,
                    TotalDiscount = request.CalculateDiscount(),
                    FinalAmount = request.CalculateFinalAmount()
                };

                actual = response;
            }

            // Assert

            Assert.Equal(expected, JsonSerializer.Serialize(actual));
        }

        [Theory]
        [InlineData(301)]
        [InlineData(600)]
        [InlineData(-301)]
        [InlineData(-600)]
        public void ResponseShouldReturnMessageWhenTimestampExceedServerTimePlusMinusFiveMinutes(int seconds)
        {
            // Arrange

            var request = new TransactionModel()
            {
                PartnerKey = "FAKEGOOGLE",
                PartnerRefNo = "FG-00001",
                PartnerPassword = "RkFLRVBBU1NXT1JEMTIzNA==",
                TotalAmount = 1000,
                Items = new List<ItemModel> {
                    new()
                    {
                        PartnerItemRef = "i-00001",
                        Name = "Pen",
                        Qty = 4,
                        UnitPrice = 200
                    },
                    new()
                    {
                        PartnerItemRef = "i-00002",
                        Name = "Ruler",
                        Qty = 2,
                        UnitPrice = 100
                    }
                },
                Timestamp = DateTime.UtcNow.AddSeconds(seconds).ToString("o")
            };
            request.Sig = request.GenerateSignature();

            string? expected = "Expired.";


            // Act

            string? actual = null;

            var message = request.Validate();

            if (string.IsNullOrEmpty(message) == false)
            {
                var response = new ResponseFailedModel()
                {
                    Result = 0,
                    ResultMessage = message
                };

                actual = response.ResultMessage;
            }

            // Assert

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(299)]
        [InlineData(1)]
        [InlineData(-299)]
        [InlineData(-1)]
        public void ResponseShouldReturnNoMessageWhenTimestampNotExceedServerTimePlusMinusFiveMinutes(int seconds)
        {
            // Arrange

            var request = new TransactionModel()
            {
                PartnerKey = "FAKEGOOGLE",
                PartnerRefNo = "FG-00001",
                PartnerPassword = "RkFLRVBBU1NXT1JEMTIzNA==",
                TotalAmount = 1000,
                Items = new List<ItemModel> {
                    new()
                    {
                        PartnerItemRef = "i-00001",
                        Name = "Pen",
                        Qty = 4,
                        UnitPrice = 200
                    },
                    new()
                    {
                        PartnerItemRef = "i-00002",
                        Name = "Ruler",
                        Qty = 2,
                        UnitPrice = 100
                    }
                },
                Timestamp = DateTime.UtcNow.AddSeconds(seconds).ToUniversalTime().ToString("o")
            };

            request.Sig = request.GenerateSignature();

            string? expected = null;


            // Act

            string? actual = null;

            var message = request.Validate();

            if (string.IsNullOrEmpty(message) == false)
            {
                var response = new ResponseFailedModel()
                {
                    Result = 0,
                    ResultMessage = message
                };

                actual = response.ResultMessage;
            }

            // Assert

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("PartnerKey")]
        [InlineData("PartnerRefNo")]
        [InlineData("PartnerPassword")]
        [InlineData("TotalAmount")]
        [InlineData("PartnerItemRef")]
        [InlineData("Name")]
        [InlineData("Qty")]
        [InlineData("UnitPrice")]
        [InlineData("Timestamp")]
        [InlineData("Sig")]
        public void ResponseShouldReturnMessageWhenRequiredFieldIsNullOrEmpty(string requiredField)
        {
            // Arrange

            var request = new TransactionModel()
            {
                PartnerKey = requiredField == "PartnerKey" ? null : "FAKEGOOGLE",
                PartnerRefNo = requiredField == "PartnerRefNo" ? null : "FG-00001",
                PartnerPassword = requiredField == "PartnerPassword" ? null : "RkFLRVBBU1NXT1JEMTIzNA==",
                TotalAmount = requiredField == "TotalAmount" ? null : 1000,
                Items = new List<ItemModel> {
                    new()
                    {
                        PartnerItemRef = requiredField == "PartnerItemRef" ? null : "i-00001",
                        Name = requiredField == "Name" ? null : "Pen",
                        Qty = requiredField == "Qty" ? null : 4,
                        UnitPrice = requiredField == "UnitPrice" ? null : 200
                    },
                    new()
                    {
                        PartnerItemRef = requiredField == "PartnerItemRef" ? null : "i-00002",
                        Name = requiredField == "Name" ? null : "Ruler",
                        Qty = requiredField == "Qty" ? null : 2,
                        UnitPrice = requiredField == "UnitPrice" ? null : 100
                    }
                },
                Timestamp = requiredField == "Timestamp" ? null : DateTime.UtcNow.ToString("o"),
            };
            request.Sig = requiredField == "Sig" ? null : request.GenerateSignature();

            string? expected = ValidationMessage.Required(requiredField);


            // Act

            string? actual = null;

            var message = request.Validate();

            if (string.IsNullOrEmpty(message) == false)
            {
                var response = new ResponseFailedModel()
                {
                    Result = 0,
                    ResultMessage = message
                };

                actual = response.ResultMessage;
            }

            // Assert

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("PartnerKey", 50)]
        [InlineData("PartnerRefNo", 50)]
        [InlineData("PartnerPassword", 50)]
        [InlineData("PartnerItemRef", 50)]
        [InlineData("Name", 100)]
        public void ResponseShouldReturnErrorMessageWhenFieldExceedsMaxLength(string field, int maxLength)
        {
            // Arrange

            var request = new TransactionModel()
            {
                PartnerKey = field == "PartnerKey" ? "WALLOFTEXTWALLOFTEXTWALLOFTEXTWALLOFTEXTWALLOFTEXTWALLOFTEXTWALLOFTEXTWALLOFTEXTWALLOFTEXTWALLOFTEXTWALLOFTEXT" : "FAKEGOOGLE",
                PartnerRefNo = field == "PartnerRefNo" ? "WALLOFTEXTWALLOFTEXTWALLOFTEXTWALLOFTEXTWALLOFTEXTWALLOFTEXTWALLOFTEXTWALLOFTEXTWALLOFTEXTWALLOFTEXTWALLOFTEXT" : "FG-00001",
                PartnerPassword = field == "PartnerPassword" ? "WALLOFTEXTWALLOFTEXTWALLOFTEXTWALLOFTEXTWALLOFTEXTWALLOFTEXTWALLOFTEXTWALLOFTEXTWALLOFTEXTWALLOFTEXTWALLOFTEXT" : "RkFLRVBBU1NXT1JEMTIzNA==",
                TotalAmount = 1000,
                Items = new List<ItemModel> {
                    new()
                    {
                        PartnerItemRef = field == "PartnerItemRef" ? "WALLOFTEXTWALLOFTEXTWALLOFTEXTWALLOFTEXTWALLOFTEXTWALLOFTEXTWALLOFTEXTWALLOFTEXTWALLOFTEXTWALLOFTEXTWALLOFTEXT" : "i-00001",
                        Name = field == "Name" ? "WALLOFTEXTWALLOFTEXTWALLOFTEXTWALLOFTEXTWALLOFTEXTWALLOFTEXTWALLOFTEXTWALLOFTEXTWALLOFTEXTWALLOFTEXTWALLOFTEXT" : "Pen",
                        Qty = 4,
                        UnitPrice = 200
                    },
                    new()
                    {
                        PartnerItemRef = field == "PartnerItemRef" ? "WALLOFTEXTWALLOFTEXTWALLOFTEXTWALLOFTEXTWALLOFTEXTWALLOFTEXTWALLOFTEXTWALLOFTEXTWALLOFTEXTWALLOFTEXTWALLOFTEXT" : "i-00002",
                        Name = field == "Name" ? "WALLOFTEXTWALLOFTEXTWALLOFTEXTWALLOFTEXTWALLOFTEXTWALLOFTEXTWALLOFTEXTWALLOFTEXTWALLOFTEXTWALLOFTEXTWALLOFTEXT" : "Ruler",
                        Qty = 2,
                        UnitPrice = 100
                    }
                },
                Timestamp = DateTime.UtcNow.ToString("o"),
            };
            request.Sig = request.GenerateSignature();

            string? expected = ValidationMessage.MaxLength(field, maxLength);


            // Act

            string? actual = null;

            var message = request.Validate();

            if (string.IsNullOrEmpty(message) == false)
            {
                var response = new ResponseFailedModel()
                {
                    Result = 0,
                    ResultMessage = message
                };

                actual = response.ResultMessage;
            }

            // Assert

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("TotalAmount")]
        [InlineData("UnitPrice")]
        public void ResponseShouldReturnMessageWhenFieldHasNegativeValue(string field)
        {
            // Arrange

            var request = new TransactionModel()
            {
                PartnerKey = "FAKEGOOGLE",
                PartnerRefNo = "FG-00001",
                PartnerPassword = "RkFLRVBBU1NXT1JEMTIzNA==",
                TotalAmount = field == "TotalAmount" ? -1000 : 1000,
                Items = new List<ItemModel> {
                    new()
                    {
                        PartnerItemRef = "i-00001",
                        Name = "Pen",
                        Qty = 4,
                        UnitPrice = field == "UnitPrice" ? -200 : 200
                    },
                    new()
                    {
                        PartnerItemRef = "i-00002",
                        Name = "Ruler",
                        Qty = 2,
                        UnitPrice = field == "UnitPrice" ? -100 : 100
                    }
                },
                Timestamp = DateTime.UtcNow.ToString("o"),
            };
            request.Sig = request.GenerateSignature();

            string? expected = ValidationMessage.PositiveValue(field);


            // Act

            string? actual = null;

            var message = request.Validate();

            if (string.IsNullOrEmpty(message) == false)
            {
                var response = new ResponseFailedModel()
                {
                    Result = 0,
                    ResultMessage = message
                };

                actual = response.ResultMessage;
            }

            // Assert

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("Qty", 1)]
        public void ResponseShouldReturnMessageWhenFieldIsNotMoreThanValue(string field, int value)
        {
            // Arrange

            var request = new TransactionModel()
            {
                PartnerKey = "FAKEGOOGLE",
                PartnerRefNo = "FG-00001",
                PartnerPassword = "RkFLRVBBU1NXT1JEMTIzNA==",
                TotalAmount = 1000,
                Items = new List<ItemModel> {
                    new()
                    {
                        PartnerItemRef = "i-00001",
                        Name = "Pen",
                        Qty = field == "Qty" ? value : 4,
                        UnitPrice = 200
                    },
                    new()
                    {
                        PartnerItemRef = "i-00002",
                        Name = "Ruler",
                        Qty = field == "Qty" ? value : 2,
                        UnitPrice = 100
                    }
                },
                Timestamp = DateTime.UtcNow.ToString("o"),
            };
            request.Sig = request.GenerateSignature();

            string? expected = ValidationMessage.MustMoreThanValue(field, value);


            // Act

            string? actual = null;

            var message = request.Validate();

            if (string.IsNullOrEmpty(message) == false)
            {
                var response = new ResponseFailedModel()
                {
                    Result = 0,
                    ResultMessage = message
                };

                actual = response.ResultMessage;
            }

            // Assert

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("Qty", 5)]
        public void ResponseShouldReturnMessageWhenFieldExceedValue(string field, int value)
        {
            // Arrange

            var request = new TransactionModel()
            {
                PartnerKey = "FAKEGOOGLE",
                PartnerRefNo = "FG-00001",
                PartnerPassword = "RkFLRVBBU1NXT1JEMTIzNA==",
                TotalAmount = 1000,
                Items = new List<ItemModel> {
                    new()
                    {
                        PartnerItemRef = "i-00001",
                        Name = "Pen",
                        Qty = field == "Qty" ? (value + 1) : 4,
                        UnitPrice = 200
                    },
                    new()
                    {
                        PartnerItemRef = "i-00002",
                        Name = "Ruler",
                        Qty = field == "Qty" ? (value + 1) : 2,
                        UnitPrice = 100
                    }
                },
                Timestamp = DateTime.UtcNow.ToString("o"),
            };
            request.Sig = request.GenerateSignature();

            string? expected = ValidationMessage.NotExceedValue(field, value);


            // Act

            string? actual = null;

            var message = request.Validate();

            if (string.IsNullOrEmpty(message) == false)
            {
                var response = new ResponseFailedModel()
                {
                    Result = 0,
                    ResultMessage = message
                };

                actual = response.ResultMessage;
            }

            // Assert

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ResponseShouldReturnAccessDeniedWhenSignatureMismatched()
        {
            // Arrange

            var request = new TransactionModel()
            {
                PartnerKey = "FAKEGOOGLE",
                PartnerRefNo = "FG-00001",
                PartnerPassword = "RkFLRVBBU1NXT1JEMTIzNA==",
                TotalAmount = 1000,
                Items = new List<ItemModel> {
                    new()
                    {
                        PartnerItemRef = "i-00001",
                        Name = "Pen",
                        Qty = 4,
                        UnitPrice = 200
                    },
                    new()
                    {
                        PartnerItemRef = "i-00002",
                        Name = "Ruler",
                        Qty = 2,
                        UnitPrice = 100
                    }
                },
                Timestamp = DateTime.UtcNow.ToString("o"),
            };
            request.Sig = request.GenerateSignature();
            request.TotalAmount = 1001;

            string? expected = "Access Denied!";


            // Act

            string? actual = null;

            var message = request.Validate();

            if (string.IsNullOrEmpty(message) == false)
            {
                var response = new ResponseFailedModel()
                {
                    Result = 0,
                    ResultMessage = message
                };

                actual = response.ResultMessage;
            }

            // Assert

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ResponseShouldReturnNoMessageWhenSignatureMatched()
        {
            // Arrange

            var request = new TransactionModel()
            {
                PartnerKey = "FAKEGOOGLE",
                PartnerRefNo = "FG-00001",
                PartnerPassword = "RkFLRVBBU1NXT1JEMTIzNA==",
                TotalAmount = 1000,
                Items = new List<ItemModel> {
                    new()
                    {
                        PartnerItemRef = "i-00001",
                        Name = "Pen",
                        Qty = 4,
                        UnitPrice = 200
                    },
                    new()
                    {
                        PartnerItemRef = "i-00002",
                        Name = "Ruler",
                        Qty = 2,
                        UnitPrice = 100
                    }
                },
                Timestamp = DateTime.UtcNow.ToString("o"),
            };
            request.Sig = request.GenerateSignature();

            string? expected = null;


            // Act

            string? actual = null;

            var message = request.Validate();

            if (string.IsNullOrEmpty(message) == false)
            {
                var response = new ResponseFailedModel()
                {
                    Result = 0,
                    ResultMessage = message
                };

                actual = response.ResultMessage;
            }

            // Assert

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("FAKEGOOGLZ", "FG-00001", "FAKEPASSWORD1234")]
        [InlineData("FAKEGOOGLE", "FG-00002", "FAKEPASSWORD1234")]
        [InlineData("FAKEGOOGLE", "FG-00001", "FAKEPASSWORD1235")]
        [InlineData("FAKEPE0PLE", "FG-00002", "FAKEPASSWORD4578")]
        [InlineData("FAKEPEOPLE", "FG-00001", "FAKEPASSWORD4578")]
        [InlineData("FAKEPEOPLE", "FG-00002", "FAKEPASSWORD457X")]
        public void ResponseShouldReturnAccessDeniedWhenNotAllowedPartner(string partnerKey, string partnerRefNo, string partnerPassword)
        {
            // Arrange

            var request = new TransactionModel()
            {
                PartnerKey = partnerKey,
                PartnerRefNo = partnerRefNo,
                PartnerPassword = Base64.Encode(partnerPassword),
                TotalAmount = 1000,
                Items = new List<ItemModel> {
                    new()
                    {
                        PartnerItemRef = "i-00001",
                        Name = "Pen",
                        Qty = 4,
                        UnitPrice = 200
                    },
                    new()
                    {
                        PartnerItemRef = "i-00002",
                        Name = "Ruler",
                        Qty = 2,
                        UnitPrice = 100
                    }
                },
                Timestamp = DateTime.UtcNow.ToString("o"),
            };
            request.Sig = request.GenerateSignature();

            string? expected = "Access Denied!";


            // Act

            string? actual = null;

            var message = request.Validate();

            if (string.IsNullOrEmpty(message) == false)
            {
                var response = new ResponseFailedModel()
                {
                    Result = 0,
                    ResultMessage = message
                };

                actual = response.ResultMessage;
            }

            // Assert

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("FAKEGOOGLE", "FG-00001", "FAKEPASSWORD1234")]
        [InlineData("FAKEPEOPLE", "FG-00002", "FAKEPASSWORD4578")]
        public void ResponseShouldReturnNoMessageNotAllowedPartner(string partnerKey, string partnerRefNo, string partnerPassword)
        {
            // Arrange

            var request = new TransactionModel()
            {
                PartnerKey = partnerKey,
                PartnerRefNo = partnerRefNo,
                PartnerPassword = Base64.Encode(partnerPassword),
                TotalAmount = 1000,
                Items = new List<ItemModel> {
                    new()
                    {
                        PartnerItemRef = "i-00001",
                        Name = "Pen",
                        Qty = 4,
                        UnitPrice = 200
                    },
                    new()
                    {
                        PartnerItemRef = "i-00002",
                        Name = "Ruler",
                        Qty = 2,
                        UnitPrice = 100
                    }
                },
                Timestamp = DateTime.UtcNow.ToString("o"),
            };
            request.Sig = request.GenerateSignature();

            string? expected = null;


            // Act

            string? actual = null;

            var message = request.Validate();

            if (string.IsNullOrEmpty(message) == false)
            {
                var response = new ResponseFailedModel()
                {
                    Result = 0,
                    ResultMessage = message
                };

                actual = response.ResultMessage;
            }

            // Assert

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ResponseShouldReturnMessageWhenInvalidTotalAmount()
        {
            // Arrange

            var request = new TransactionModel()
            {
                PartnerKey = "FAKEGOOGLE",
                PartnerRefNo = "FG-00001",
                PartnerPassword = "RkFLRVBBU1NXT1JEMTIzNA==",
                TotalAmount = 1000,
                Items = new List<ItemModel> {
                    new()
                    {
                        PartnerItemRef = "i-00001",
                        Name = "Pen",
                        Qty = 4,
                        UnitPrice = 400
                    },
                    new()
                    {
                        PartnerItemRef = "i-00002",
                        Name = "Ruler",
                        Qty = 2,
                        UnitPrice = 1000
                    }
                },
                Timestamp = DateTime.UtcNow.ToString("o"),
            };
            request.Sig = request.GenerateSignature();

            string? expected = "Invalid Total Amount.";


            // Act

            string? actual = null;

            var message = request.Validate();

            if (string.IsNullOrEmpty(message) == false)
            {
                var response = new ResponseFailedModel()
                {
                    Result = 0,
                    ResultMessage = message
                };

                actual = response.ResultMessage;
            }

            // Assert

            Assert.Equal(expected, actual);
        }
    }
}
