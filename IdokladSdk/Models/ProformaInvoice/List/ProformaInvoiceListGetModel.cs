﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IdokladSdk.Enums;
using IdokladSdk.Models.Attachment;
using IdokladSdk.Models.Common;
using IdokladSdk.Models.DeliveryAddress;
using IdokladSdk.Models.DocumentAddress;

namespace IdokladSdk.Models.ProformaInvoice
{
    /// <summary>
    /// ProformaInvoiceListGetModel.
    /// </summary>
    public class ProformaInvoiceListGetModel : IEntityId
    {
        /// <summary>
        /// Gets or sets accounting invoice Id.
        /// </summary>
        public int? AccountedByInvoiceId { get; set; }

        /// <summary>
        /// Gets or sets list of attachments. Currently, you can attach a maximum of one attachment.
        /// </summary>
        public List<DocumentAttachmentInfo> Attachments { get; set; }

        /// <summary>
        /// Gets or sets constant symbol Id.
        /// </summary>
        public int? ConstantSymbolId { get; set; }

        /// <summary>
        /// Gets or sets currency Id.
        /// </summary>
        public int CurrencyId { get; set; }

        /// <summary>
        /// Gets or sets date of accounting event.
        /// </summary>
        public DateTime DateOfAccountingEvent { get; set; }

        /// <summary>
        /// Gets or sets date of issue.
        /// </summary>
        public DateTime DateOfIssue { get; set; }

        /// <summary>
        /// Gets or sets date of last reminder.
        /// </summary>
        public DateTime DateOfLastReminder { get; set; }

        /// <summary>
        /// Gets or sets date of maturity.
        /// </summary>
        public DateTime DateOfMaturity { get; set; }

        /// <summary>
        /// Gets or sets date of payment.
        /// </summary>
        public DateTime DateOfPayment { get; set; }

        /// <summary>
        /// Gets or sets date of taxing.
        /// </summary>
        public DateTime DateOfTaxing { get; set; }

        /// <summary>
        /// Gets or sets date of VAT application.
        /// </summary>
        public DateTime DateOfVatApplication { get; set; }

        /// <summary>
        /// Gets or sets delivery address.
        /// </summary>
        public DeliveryDocumentAddressGetModel DeliveryAddress { get; set; }

        /// <summary>
        /// Gets or sets description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets documentNumber.
        /// </summary>
        [StringLength(10)]
        public string DocumentNumber { get; set; }

        /// <summary>
        /// Gets or sets document Serial Number.
        /// </summary>
        public int DocumentSerialNumber { get; set; }

        /// <summary>
        /// Gets or sets responsibility for handlig of electronic records of sales.
        /// </summary>
        public EetResponsibility EetResponsibility { get; set; }

        /// <summary>
        /// Gets or sets exchange rate.
        /// </summary>
        public decimal ExchangeRate { get; set; }

        /// <summary>
        /// Gets or sets exchange rate amount.
        /// </summary>
        public decimal ExchangeRateAmount { get; set; }

        /// <summary>
        /// Gets or sets export to another accounting software indication.
        /// </summary>
        public ExportedState Exported { get; set; }

        /// <inheritdoc/>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether flag indicating whether the document is registered in EET.
        /// </summary>
        public bool IsEet { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether zahrnout doklad do daňového přiznání.
        /// </summary>
        /// <summary xml:lang='en'>
        /// Include subject to income tax.
        /// </summary>
        public bool IsIncomeTax { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is proforma a tax movement indication.
        /// </summary>
        public bool IsProformaTaxed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether indicate sending to accountant.
        /// </summary>
        public bool IsSentToAccountant { get; set; }

        /// <summary>
        /// Gets or sets indicate sending to purchaser.
        /// </summary>
        public MailSentType IsSentToPurchaser { get; set; }

        /// <summary>
        /// Gets or sets issued tax document ids.
        /// </summary>
        public List<int> IssuedTaxDocumentIds { get; set; }

        /// <summary>
        /// Gets or sets invoice items.
        /// </summary>
        public List<ProformaInvoiceItemListGetModel> Items { get; set; }

        /// <summary>
        /// Gets or sets items text prefix.
        /// </summary>
        public string ItemsTextPrefix { get; set; }

        /// <summary>
        /// Gets or sets items text suffix.
        /// </summary>
        public string ItemsTextSuffix { get; set; }

        /// <summary>
        /// Gets or sets additional information about the entity.
        /// </summary>
        public Metadata Metadata { get; set; }

        /// <summary>
        /// Gets or sets your contact information.
        /// </summary>
        public DocumentAddressModel MyAddress { get; set; }

        /// <summary>
        /// Gets or sets note.
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Gets or sets order number.
        /// </summary>
        [StringLength(20)]
        public string OrderNumber { get; set; }

        /// <summary>
        /// Gets or sets contact information of the partner.
        /// </summary>
        public DocumentAddressModel PartnerAddress { get; set; }

        /// <summary>
        /// Gets or sets partner contact id.
        /// </summary>
        public int PartnerId { get; set; }

        /// <summary>
        /// Gets or sets payment option id.
        /// </summary>
        public int PaymentOptionId { get; set; }

        /// <summary>
        /// Gets or sets payment status.
        /// </summary>
        public PaymentStatus PaymentStatus { get; set; }

        /// <summary>
        /// Gets or sets prices and calculations.
        /// </summary>
        public InvoicePrices Prices { get; set; }

        /// <summary>
        /// Gets or sets Id of recurring invoice, from which the invoice was created.
        /// </summary>
        public int? RecurringInvoiceId { get; set; }

        /// <summary>
        /// Gets or sets number of sent reminders.
        /// </summary>
        public int ReminderCount { get; set; }

        /// <summary>
        /// Gets or sets report language.
        /// </summary>
        public Language? ReportLanguage { get; set; }

        /// <summary>
        /// Gets or sets pOS equipment id.
        /// </summary>
        public int? SalesPosEquipmentId { get; set; }

        /// <summary>
        /// Gets or sets tags.
        /// </summary>
        public List<TagDocumentListGetModel> Tags { get; set; }

        /// <summary>
        /// Gets or sets variable symbol.
        /// </summary>
        [StringLength(10)]
        public string VariableSymbol { get; set; }

        /// <summary>
        /// Gets or sets attribute for application of VAT based on payments.
        /// </summary>
        public VatOnPayStatus VatOnPayStatus { get; set; }
    }
}
