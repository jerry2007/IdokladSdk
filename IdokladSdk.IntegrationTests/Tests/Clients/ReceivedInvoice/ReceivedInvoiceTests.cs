﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using IdokladSdk.Clients;
using IdokladSdk.Enums;
using IdokladSdk.IntegrationTests.Core;
using IdokladSdk.IntegrationTests.Core.Extensions;
using IdokladSdk.Models.ReceivedInvoice;
using IdokladSdk.Requests.Core.Extensions;
using NUnit.Framework;

namespace IdokladSdk.IntegrationTests.Tests.Clients.ReceivedInvoice
{
    /// <summary>
    /// ReceivedInvoiceTests.
    /// </summary>
    public partial class ReceivedInvoiceTests : TestBase
    {
        private const int PartnerId = 323823;
        private ReceivedInvoiceClient _receivedInvoiceClient;
        private ReceivedInvoicePostModel _receivedInvoicePostModel;
        private int _receivedInvoiceId;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            InitDokladApi();
            _receivedInvoiceClient = DokladApi.ReceivedInvoiceClient;
        }

        [Test]
        [Order(1)]
        public void Post_SuccessfullyCreated()
        {
            var vatCodeId = 24;

            // Arrange
            _receivedInvoicePostModel = _receivedInvoiceClient.Default().AssertResult();
            _receivedInvoicePostModel.PartnerId = PartnerId;
            _receivedInvoicePostModel.Description = "Invoice";
            _receivedInvoicePostModel.Items.Clear();
            _receivedInvoicePostModel.Items.Add(new ReceivedInvoiceItemPostModel
            {
                Name = "Test",
                UnitPrice = 100,
                VatCodeId = vatCodeId
            });

            // Act
            var data = _receivedInvoiceClient.Post(_receivedInvoicePostModel).AssertResult();
            _receivedInvoiceId = data.Id;

            // Assert
            Assert.Greater(data.Id, 0);
            Assert.AreEqual(_receivedInvoicePostModel.DateOfIssue, data.DateOfIssue);
            Assert.AreEqual(PartnerId, data.PartnerId);
            Assert.Greater(data.Items.Count, 0);
            Assert.AreEqual(vatCodeId, data.Items.First(i => i.ItemType == IssuedInvoiceItemType.ItemTypeNormal).VatCodeId);
        }

        [Test]
        [Order(1)]
        public void Post_NotValidModel_ReceivedDocumentNumberTooLong_ThrowException()
        {
            // Arrange
            var receivedInvoicePostModel = _receivedInvoiceClient.Default().AssertResult();
            receivedInvoicePostModel.PartnerId = PartnerId;
            receivedInvoicePostModel.Description = "Invoice";
            receivedInvoicePostModel.ReceivedDocumentNumber = new string('A', 31);
            receivedInvoicePostModel.Items.Clear();
            receivedInvoicePostModel.Items.Add(new ReceivedInvoiceItemPostModel
            {
                Name = "Test",
                UnitPrice = 100,
            });

            // Act
            var exception = Assert.Throws<ValidationException>(() => _receivedInvoiceClient.Post(receivedInvoicePostModel));

            // Assert
            Assert.IsNotNull(exception.Message);
            Assert.IsNotEmpty(exception.Message);
        }

        [Test]
        [Order(4)]
        public void Update_NotValidModel_ReceivedDocumentNumberTooLong_ThrowException()
        {
            var model = new ReceivedInvoicePatchModel
            {
                Id = _receivedInvoiceId,
                ReceivedDocumentNumber = new string('A', 31)
            };

            // Act
            var exception = Assert.Throws<ValidationException>(() => _receivedInvoiceClient.Update(model));

            // Assert
            Assert.IsNotNull(exception.Message);
            Assert.IsNotEmpty(exception.Message);
        }

        [Test]
        [Order(2)]
        public void Get_SuccessfullyGet()
        {
            // Act
            var data = _receivedInvoiceClient.Detail(_receivedInvoiceId).Get().AssertResult();

            // Assert
            Assert.AreEqual(_receivedInvoiceId, data.Id);
        }

        [Test]
        [Order(3)]
        public void Get_Expand_SuccessfullyGet()
        {
            // Act
            var data = _receivedInvoiceClient.Detail(_receivedInvoiceId)
                .Include(s => s.Partner).Get().AssertResult();

            Assert.AreEqual(_receivedInvoiceId, data.Id);
            Assert.IsNotNull(data.Partner);
        }

        [Test]
        [Order(4)]
        public void Update_SuccessfullyUpdated()
        {
            var model = new ReceivedInvoicePatchModel
            {
                Id = _receivedInvoiceId,
                Description = "DescriptionUpdated"
            };

            // Act
            var data = _receivedInvoiceClient.Update(model).AssertResult();

            // Assert
            Assert.AreEqual(model.Description, data.Description);
        }

        [Test]
        [Order(5)]
        public void Update_SetCustomVatRateToNull_CustomVatRateIsNull()
        {
            var invoiceToUpdate = _receivedInvoiceClient.Detail(_receivedInvoiceId).Get().AssertResult();
            var model = CreatePatchModelWithCustomVatRate(invoiceToUpdate, 13);
            var data = _receivedInvoiceClient.Update(model).AssertResult();
            Assert.IsNotNull(data.Items.First().CustomVatRate);
            model = CreatePatchModelWithCustomVatRate(invoiceToUpdate, null);

            // Act
            data = _receivedInvoiceClient.Update(model).AssertResult();

            // Assert
            Assert.IsNull(data.Items.First().CustomVatRate);
        }

        [Test]
        [Order(6)]
        public void Copy_SuccessfullyGetPosModel()
        {
            // Arrange
            var invoiceToCopy = _receivedInvoiceClient.Detail(_receivedInvoiceId).Get().AssertResult();

            // Act
            var data = _receivedInvoiceClient.Copy(_receivedInvoiceId).AssertResult();

            // Assert
            Assert.AreEqual(invoiceToCopy.Description, data.Description);
            Assert.AreEqual(invoiceToCopy.PartnerId, data.PartnerId);
            Assert.AreEqual(invoiceToCopy.CurrencyId, data.CurrencyId);
        }

        [Test]
        [Order(7)]
        public void Update_AddNewItems_SucessfullyUpdated()
        {
            // Arrange
            var invoiceToUpdate = _receivedInvoiceClient.Detail(_receivedInvoiceId).Get().AssertResult();
            var itemName2 = "Test2Test";
            var itemName3 = "Test3Test";
            var model = new ReceivedInvoicePatchModel
            {
                Id = invoiceToUpdate.Id,
                Items = new List<ReceivedInvoiceItemPatchModel>
                {
                    new ReceivedInvoiceItemPatchModel
                    {
                        Id = 1,
                        Name = itemName2,
                        UnitPrice = 100
                    },
                    new ReceivedInvoiceItemPatchModel
                    {
                        Name = itemName3,
                        UnitPrice = 100
                    }
                }
            };

            // Act
            var data = _receivedInvoiceClient.Update(model).AssertResult();

            // Assert
            Assert.GreaterOrEqual(2, data.Items.Count);
            Assert.That(data.Items.Any(x => x.Name == itemName2));
            Assert.That(data.Items.Any(x => x.Name == itemName3));
        }

        [Test]
        [Order(8)]
        public void Delete_SuccessfullyDeleted()
        {
            // Act
            var data = _receivedInvoiceClient.Delete(_receivedInvoiceId).AssertResult();

            // Assert
            Assert.IsTrue(data);
        }

        [Test]
        public void Recount_SuccessfullyRecounted()
        {
            // Arrange
            var item = new ReceivedInvoiceItemRecountPostModel
            {
                UnitPrice = 100,
                Amount = 2,
                Name = "Test",
                Id = 1,
                PriceType = PriceType.WithoutVat,
                VatRateType = VatRateType.Basic
            };
            var model = new ReceivedInvoiceRecountPostModel
            {
                CurrencyId = 1,
                DateOfTaxing = DateTime.Today.SetKindUtc(),
                Items = new List<ReceivedInvoiceItemRecountPostModel> { item }
            };

            // Act
            var data = _receivedInvoiceClient.Recount(model).AssertResult();

            // Assert
            var recountedItem = data.Items.First(x => x.ItemType == IssuedInvoiceItemType.ItemTypeNormal);
            Assert.AreEqual(item.Id, recountedItem.Id);
            Assert.AreEqual(item.Name, recountedItem.Name);
            Assert.AreEqual(242, recountedItem.Prices.TotalWithVat);
            Assert.AreEqual(242, recountedItem.Prices.TotalWithVatHc);
            Assert.AreEqual(42, recountedItem.Prices.TotalVat);
            Assert.AreEqual(42, recountedItem.Prices.TotalVatHc);
            Assert.AreEqual(200, recountedItem.Prices.TotalWithoutVat);
            Assert.AreEqual(200, recountedItem.Prices.TotalWithoutVatHc);
        }

        [Test]
        public void Recount_ForeignCurrency_SuccessfullyRecounted()
        {
            // Arrange
            var item = new ReceivedInvoiceItemRecountPostModel
            {
                UnitPrice = 100,
                Amount = 1,
                Name = "Test",
                Id = 1,
                PriceType = PriceType.WithoutVat,
                VatRateType = VatRateType.Basic
            };
            var model = new ReceivedInvoiceRecountPostModel
            {
                CurrencyId = 2,
                ExchangeRate = 20,
                ExchangeRateAmount = 1,
                DateOfTaxing = DateTime.Today.SetKindUtc(),
                Items = new List<ReceivedInvoiceItemRecountPostModel> { item }
            };

            // Act
            var result = _receivedInvoiceClient.Recount(model).AssertResult();

            // Assert
            var recountedItem = result.Items.First(x => x.ItemType == IssuedInvoiceItemType.ItemTypeNormal);
            Assert.AreEqual(1, result.ExchangeRateAmount);
            Assert.AreEqual(20, result.ExchangeRate);
            Assert.AreEqual(2, result.CurrencyId);
            Assert.AreEqual(121, recountedItem.Prices.TotalWithVat);
            Assert.AreEqual(2420, recountedItem.Prices.TotalWithVatHc);
        }

        [Test]
        public void GetList_SuccessfullyReturned()
        {
            // Act
            var data = _receivedInvoiceClient.List().Get().AssertResult();

            // Assert
            Assert.Greater(data.TotalItems, 0);
            Assert.Greater(data.TotalPages, 0);
        }

        private ReceivedInvoicePatchModel CreatePatchModelWithCustomVatRate(ReceivedInvoiceGetModel invoiceToUpdate, decimal? customVatRate)
        {
            return new ReceivedInvoicePatchModel
            {
                Id = invoiceToUpdate.Id,
                Items = new List<ReceivedInvoiceItemPatchModel>
                {
                    new ReceivedInvoiceItemPatchModel
                    {
                        Id = invoiceToUpdate.Items.First().Id,
                        CustomVatRate = customVatRate,
                        Name = invoiceToUpdate.Items.First().Name
                    }
                }
            };
        }
    }
}
