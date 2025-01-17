﻿using System;
using System.IO;
using System.Linq;
using System.Net;
using IdokladSdk.Clients;
using IdokladSdk.Enums;
using IdokladSdk.IntegrationTests.Core;
using IdokladSdk.IntegrationTests.Core.Extensions;
using IdokladSdk.Models.Account;
using NUnit.Framework;

namespace IdokladSdk.IntegrationTests.Tests.Clients.Account
{
    public partial class AccountTest : TestBase
    {
        private const int AgendaId = 187854;
        private const string LogoFileName = "Solitea.png";
        private const string LogoPath = "Tests/Clients/Account/Solitea.png";
        private const int UserId = 161205;
        private const string Street = "Drobného 555/49";

        private AccountClient _accountClient;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            InitDokladApi();
            _accountClient = DokladApi.AccountClient;
        }

        [Test]
        public void AgendaList_SuccessfullyGetAgendaList()
        {
            // Act
            var data = _accountClient.Agendas.List().Get().AssertResult();

            // Assert
            Assert.Greater(data.TotalItems, 0);
            var agenda = data.Items.FirstOrDefault();
            Assert.NotNull(agenda);
            Assert.AreEqual(AgendaId, agenda.Id);
            Assert.NotNull(agenda.Contact);
            Assert.AreEqual(Street, agenda.Contact.Street);
        }

        [Test]
        public void SubscriptionsList_FilteredByDateFromSuccessfullyGetSubscription()
        {
            // Act
            var data = _accountClient.Subscriptions.List()
                .Filter(f => f.DateFrom.IsGreaterThan(new DateTime(2020, 3, 1))).Get().AssertResult();

            // Assert
            Assert.That(data.TotalItems, Is.GreaterThan(2));
        }

        [Test]
        public void SubscriptionsList_FilteredByIsCanceledSuccessfullyGetSubscription()
        {
            // Act
            var data = _accountClient.Subscriptions.List()
                .Filter(f => f.IsCanceled.IsEqual(true)).Get().AssertResult();

            // Assert
            Assert.That(data.TotalItems, Is.EqualTo(0));
        }

        [Test]
        public void AgendaDetail_SuccessfullyGetAgendaDetail()
        {
            // Act
            var data = _accountClient.Agendas.Detail(AgendaId).Get().AssertResult();

            // Assert
            Assert.NotNull(data);
            Assert.AreEqual(AgendaId, data.Id);
            Assert.AreEqual("Solitea Česká republika, a.s.", data.Name);
            Assert.NotNull(data.Contact);
            Assert.AreEqual(Street, data.Contact.Street);
            Assert.IsNotEmpty(data.CswCustomerPin);
            Assert.AreNotEqual(Guid.Empty, data.CswCustomerGuid);
            Assert.That(data.HasVatRegimeOss, Is.True);
            Assert.That(data.DeleteStatus, Is.EqualTo(AgendaDeleteStatus.RequestSent));
            Assert.That(data.IsActiveStorePayment, Is.False);
        }

        [Test]
        public void AgendaCurrent_SuccessfullyGetAgendaCurrentDetail()
        {
            // Act
            var data = _accountClient.Agendas.Current().Get().AssertResult();

            // Assert
            Assert.NotNull(data);
            Assert.AreEqual(AgendaId, data.Id);
            Assert.AreEqual("Solitea Česká republika, a.s.", data.Name);
            Assert.NotNull(data.Contact);
            Assert.AreEqual(Street, data.Contact.Street);
            Assert.True(data.BankAccounts.Count(a => a.IsDefault) == 1);
        }

        [Test]
        public void AgendaDeleteRequest_DoesNotFail()
        {
            // Arrange
            var model = new AgendaDeleteRequestPostModel();

            // Act
            var data = _accountClient.Agendas.DeleteRequest(model).AssertResult();

            // Assert
            Assert.True(data);
        }

        [Test]
        public void AgendaUpdate_DoesNotFail()
        {
            // Arrange
            var model = new AgendaPatchModel();

            // Act
            var data = _accountClient.Agendas.Update(model).AssertResult();

            // Assert
            Assert.NotNull(data);
            Assert.NotNull(data.AutomaticPairPaymentsSettings);
            Assert.True(data.BankAccounts.Any());
            Assert.True(data.CashRegisters.Any());
            Assert.NotNull(data.Contact);
            Assert.NotNull(data.PurchaseSettings);
            Assert.NotNull(data.SalesSettings);
            Assert.NotNull(data.SendReminderSettings);
            Assert.NotNull(data.Subscription);
        }

        [Test]
        public void AgendaUpdate_ValidIdentification_DoesNotFail()
        {
            // Arrange
            var model = new AgendaPatchModel
            {
                Contact = new AgendaContactPatchModel
                {
                    IdentificationNumber = string.Empty,
                    HasNoIdentificationNumber = true
                }
            };

            // Act
            var hasIdentificationData = _accountClient.Agendas.Update(model).AssertResult().Contact;

            // Assert
            Assert.That(hasIdentificationData.IdentificationNumber, Is.Empty);
            Assert.That(hasIdentificationData.HasNoIdentificationNumber, Is.True);

            var identificationNumber = "25568736";
            model.Contact.IdentificationNumber = identificationNumber;
            model.Contact.HasNoIdentificationNumber = false;

            // Act
            var data = _accountClient.Agendas.Update(model).AssertResult();

            // Assert
            Assert.NotNull(data);
            Assert.NotNull(data.Contact);
            Assert.That(data.Contact.IdentificationNumber, Is.EqualTo(identificationNumber));
            Assert.That(data.Contact.HasNoIdentificationNumber, Is.False);
        }

        [Test]
        public void AgendaUpdate_InValidIdentification_UpdateFailed()
        {
            // Arrange
            var model = new AgendaPatchModel
            {
                Contact = new AgendaContactPatchModel
                {
                    IdentificationNumber = "invalid",
                    HasNoIdentificationNumber = false
                }
            };

            // Act
            var result = _accountClient.Agendas.Update(model);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.AreEqual(DokladErrorCode.InvalidIdentificationNumber, result.ErrorCode);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Test]
        public void Agenda_GenerateBankStatementMail_ReturnsEmailAddress()
        {
            // Act
            var data = _accountClient.Agendas.GenerateBankStatementMail().AssertResult();

            // Assert
            Assert.That(data, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public void LogoGet_SuccessfullyGet()
        {
            // Act
            var data = _accountClient.Agendas.GetLogo().AssertResult();

            // Assert
            Assert.NotNull(data);
            Assert.NotNull(data.FileBytes);
        }

        [Test]
        public void LogoDelete_SuccessfullyDeleted()
        {
            // Act
            var data = _accountClient.Agendas.DeleteLogo().AssertResult();

            // Assert
            Assert.True(data);
        }

        [Test]
        public void LogoUpdate_SuccessfullyUpdated()
        {
            // Arrange
            var model = new LogoPostModel
            {
                FileBytes = File.ReadAllBytes($"{TestContext.CurrentContext.TestDirectory}/{LogoPath}"),
                FileName = LogoFileName,
                HighResolution = true
            };

            // Act
            var data = _accountClient.Agendas.UploadLogo(model).AssertResult();

            // Assert
            Assert.True(data);
        }

        [Test]
        public void UserList_SuccessfullyGetUserList()
        {
            // Act
            var data = _accountClient.Users.List().Get().AssertResult();

            // Assert
            Assert.Greater(data.TotalItems, 0);
            var user = data.Items.FirstOrDefault();
            Assert.NotNull(user);
            Assert.AreEqual(UserId, user.Id);
        }

        [Test]
        public void UserDetail_SuccessfullyGetUserDetail()
        {
            // Act
            var data = _accountClient.Users.Detail(UserId).Get().AssertResult();

            // Assert
            Assert.NotNull(data);
            Assert.AreEqual(UserId, data.Id);
            Assert.AreEqual("qquc@furusato.tokyo", data.Username);
            Assert.AreEqual(UserRight.Admin, data.Rights);
        }

        [Test]
        public void UserCurrent_SuccessfullyGetUserCurrentDetail()
        {
            // Act
            var data = _accountClient.Users.Current().Get().AssertResult();

            // Assert
            Assert.NotNull(data);
            Assert.AreEqual(UserId, data.Id);
            Assert.AreEqual("qquc@furusato.tokyo", data.Username);
            Assert.AreEqual(UserRight.Admin, data.Rights);
        }

        [Test]
        public void UserUpdate_DoesNotFail()
        {
            // Arrange
            var model = new UserPatchModel();

            // Act
            var data = _accountClient.Users.Update(model).AssertResult();

            // Assert
            Assert.NotNull(data);
        }

        [Test]
        public void GetSubscriptionList_SubscriptionListGotSuccessfully()
        {
            // Act
            var result = _accountClient.Subscriptions.List()
                .Sort(x => x.DateOfIssue.Desc())
                .Get().AssertResult();

            // Assert
            Assert.NotNull(result);
            Assert.Greater(result.TotalItems, 0);
        }
    }
}
