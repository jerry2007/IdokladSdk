﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Doklad.Shared.Enums.Api;
using IdokladSdk.Enums;
using IdokladSdk.IntegrationTests.Core.Extensions;
using IdokladSdk.Models.ProformaInvoice;
using IdokladSdk.Requests.Core.Extensions;
using NUnit.Framework;

namespace IdokladSdk.IntegrationTests.Tests.Clients.ProformaInvoice
{
    /// <summary>
    /// ProformaInvoiceTests.
    /// </summary>
    public partial class ProformaInvoiceTests
    {
        private int _proformaInvoiceIdAsync;

        [Test]
        [Order(9)]
        public async Task PostAsync_SuccessfullyCreated()
        {
            // Arrange
            var proformaInvoicePostModelAsync = _proformaInvoiceClient.Default().AssertResult();
            proformaInvoicePostModelAsync.PartnerId = PartnerId;
            proformaInvoicePostModelAsync.Description = "Test: PostAsync_SuccessfullyCreated";
            proformaInvoicePostModelAsync.DateOfPayment = DateTime.UtcNow.SetKindUtc();
            proformaInvoicePostModelAsync.IsEet = false;
            proformaInvoicePostModelAsync.Items.Clear();
            proformaInvoicePostModelAsync.Items.Add(new ProformaInvoiceItemPostModel
            {
                Name = "Test",
                UnitPrice = 100,
                Amount = 1
            });

            // Act
            var data = (await _proformaInvoiceClient.PostAsync(proformaInvoicePostModelAsync)).AssertResult();
            _proformaInvoiceIdAsync = data.Id;

            // Assert
            Assert.Greater(data.Id, 0);
            Assert.AreEqual(proformaInvoicePostModelAsync.DateOfIssue, data.DateOfIssue);
            Assert.AreEqual(PartnerId, data.PartnerId);
            Assert.Greater(data.Items.Count, 0);
        }

        [Test]
        [Order(10)]
        public async Task UpdateAsync_SuccessfullyUpdated()
        {
            var model = new ProformaInvoicePatchModel
            {
                Id = _proformaInvoiceIdAsync,
                Description = "Test: UpdateAsync_SuccessfullyUpdated"
            };

            // Act
            var data = (await _proformaInvoiceClient.UpdateAsync(model)).AssertResult();

            // Assert
            Assert.AreEqual(model.Description, data.Description);
        }

        [Test]
        [Order(11)]
        public async Task GetAsync_SuccessfullyGet()
        {
            // Act
            var data = (await _proformaInvoiceClient.Detail(_proformaInvoiceIdAsync).GetAsync()).AssertResult();

            // Assert
            Assert.AreEqual(_proformaInvoiceIdAsync, data.Id);
        }

        [Test]
        [Order(12)]
        public async Task CopyAsync_SuccessfullyGetPosModel()
        {
            // Arrange
            var invoiceToCopy = _proformaInvoiceClient.Detail(_proformaInvoiceIdAsync).Get().AssertResult();

            // Act
            var data = (await _proformaInvoiceClient.CopyAsync(_proformaInvoiceIdAsync)).AssertResult();

            // Assert
            Assert.AreEqual(invoiceToCopy.Description, data.Description);
            Assert.AreEqual(invoiceToCopy.PartnerId, data.PartnerId);
            Assert.AreEqual(invoiceToCopy.CurrencyId, data.CurrencyId);
        }

        [Test]
        [Order(13)]

        public async Task GetInvoiceForAccountAsync_SuccessfullyGetIssuedInvoiceAccountPostModel()
        {
            // Act
            var data = (await _proformaInvoiceClient.GetInvoiceForAccountAsync(_proformaInvoiceIdAsync)).AssertResult();

            // Assert
            var item = data.Items.FirstOrDefault(i => i.ItemType == PostIssuedInvoiceItemType.ItemTypeReduce);
            Assert.NotNull(item);
            Assert.AreEqual(-100, item.UnitPrice);
        }

        [Test]
        [Order(14)]
        public async Task AccountAsync_SuccessfullyAccounted()
        {
            // Act
            var data = (await _proformaInvoiceClient.AccountAsync(_proformaInvoiceIdAsync)).AssertResult();
            _issuedInvoiceToDeleteIds.Add(data.Id);

            // Assert
            var item = data.Items.Where(i => i.ItemType == IssuedInvoiceItemType.ItemTypeReduce);
            Assert.NotNull(item);
        }

        [Test]
        [Order(15)]
        public async Task DeleteAsync_SuccessfullyDeleted()
        {
            // Act
            var data = (await _proformaInvoiceClient.DeleteAsync(_proformaInvoiceIdAsync)).AssertResult();

            // Assert
            Assert.IsTrue(data);
        }

        [Test]
        public async Task GetRecurrenceFromInvoiceAsync_SuccessfullyReturned()
        {
            // Act
            var data = (await _proformaInvoiceClient.RecurrenceAsync(UnpaidProformaInvoiceId)).AssertResult();

            // Assert
            Assert.IsNotNull(data);
            Assert.IsNotNull(data.InvoiceTemplate);
            Assert.IsNotNull(data.RecurringSetting);
        }

        [Test]
        public async Task AccountMultipleProformasAsync_SuccessfullyAccounted()
        {
            // Arrange
            var proformaModel = CreateProformaInvoicePostModel();
            var proformaId1 = (await _proformaInvoiceClient.PostAsync(proformaModel)).AssertResult().Id;
            var proformaId2 = (await _proformaInvoiceClient.PostAsync(proformaModel)).AssertResult().Id;
            _proformaInvoiceToDeleteIds.AddRange(new[] { proformaId1, proformaId2 });
            var putModel = new AccountProformaInvoicesPutModel { ProformaIds = new[] { proformaId1, proformaId2 } };

            // Act
            var result = (await _proformaInvoiceClient.AccountMultipleProformaInvoicesAsync(putModel)).AssertResult();
            _issuedInvoiceToDeleteIds.Add(result.Id);

            // Assert
            Assert.NotNull(result);
        }

        [Test]
        public async Task GetListAsync_SuccessfullyReturned()
        {
            // Act
            var data = (await _proformaInvoiceClient.List().GetAsync()).AssertResult();

            // Assert
            Assert.Greater(data.TotalItems, 0);
            Assert.Greater(data.TotalPages, 0);
        }

        [Test]
        public async Task RecountAsync_SuccessfullyRecounted()
        {
            // Arrange
            var item = new ProformaInvoiceItemRecountPostModel
            {
                UnitPrice = 100,
                Amount = 2,
                Name = "Test",
                Id = 1,
                PriceType = PriceType.WithoutVat,
                VatRateType = VatRateType.Basic
            };
            var model = new ProformaInvoiceRecountPostModel
            {
                CurrencyId = 1,
                DateOfTaxing = DateTime.Today.SetKindUtc(),
                PaymentOptionId = 1,
                Items = new List<ProformaInvoiceItemRecountPostModel> { item }
            };

            // Act
            var data = (await _proformaInvoiceClient.RecountAsync(model)).AssertResult();

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
    }
}
