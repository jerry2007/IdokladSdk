﻿using System;
using IdokladSdk.Enums;
using IdokladSdk.Models.ReceivedInvoice;
using IdokladSdk.Requests.Core.Modifiers.Filters.Common;

namespace IdokladSdk.Requests.ReceivedInvoice.Filter
{
    /// <inheritdoc cref="ReceivedInvoiceGetModel"/>
    public class ReceivedInvoiceFilter
    {
        /// <inheritdoc cref="ReceivedInvoiceListGetModel.Id"/>
        public CompareFilterItem<int> Id { get; set; } = new CompareFilterItem<int>(nameof(ReceivedInvoiceGetModel.Id));

        /// <inheritdoc cref="ReceivedInvoiceListGetModel.CurrencyId"/>
        public FilterItem<int> CurrencyId { get; set; } = new FilterItem<int>(nameof(ReceivedInvoiceGetModel.CurrencyId));

        /// <summary>
        /// Gets or sets date last changed.
        /// </summary>
        public CompareFilterItem<DateTime> DateLastChanged { get; set; } = new CompareFilterItem<DateTime>("DateLastChanged");

        /// <inheritdoc cref="ReceivedInvoiceListGetModel.DateOfIssue"/>
        public CompareFilterItem<DateTime> DateOfIssue { get; set; } = new CompareFilterItem<DateTime>(nameof(ReceivedInvoiceGetModel.DateOfIssue));

        /// <inheritdoc cref="ReceivedInvoiceListGetModel.DateOfPayment"/>
        public CompareFilterItem<DateTime> DateOfPayment { get; set; } = new CompareFilterItem<DateTime>(nameof(ReceivedInvoiceGetModel.DateOfPayment));

        /// <inheritdoc cref="ReceivedInvoiceListGetModel.DateOfMaturity"/>
        public CompareFilterItem<DateTime> DateOfMaturity { get; set; } = new CompareFilterItem<DateTime>(nameof(ReceivedInvoiceGetModel.DateOfMaturity));

        /// <inheritdoc cref="ReceivedInvoiceListGetModel.Exported"/>
        public FilterItem<ExportedState> Exported { get; set; } = new FilterItem<ExportedState>(nameof(ReceivedInvoiceGetModel.Exported));

        /// <inheritdoc cref="ReceivedInvoiceListGetModel.Description"/>
        public ContainFilterItem<string> Description { get; set; } = new ContainFilterItem<string>(nameof(ReceivedInvoiceGetModel.Description));

        /// <summary>
        /// Gets or sets numeric sequence id.
        /// </summary>
        public FilterItem<int> NumericSequenceId { get; set; } = new FilterItem<int>("NumericSequenceId");

        /// <inheritdoc cref="ReceivedInvoiceListGetModel.DocumentNumber"/>
        public ContainFilterItem<string> DocumentNumber { get; set; } = new ContainFilterItem<string>(nameof(ReceivedInvoiceGetModel.DocumentNumber));

        /// <summary>
        /// Gets or sets is paid.
        /// </summary>
        public FilterItem<bool> IsPaid { get; set; } = new FilterItem<bool>("IsPaid");

        /// <inheritdoc cref="ReceivedInvoiceListGetModel.PaymentStatus"/>
        public FilterItem<PaymentStatus> PaymentStatus { get; set; } = new FilterItem<PaymentStatus>(nameof(ReceivedInvoiceGetModel.PaymentStatus));

        /// <inheritdoc cref="ReceivedInvoiceListGetModel.DateOfReceiving"/>
        public CompareFilterItem<DateTime> DateOfReceiving { get; set; } = new CompareFilterItem<DateTime>(nameof(ReceivedInvoiceGetModel.DateOfReceiving));

        /// <inheritdoc cref="ReceivedInvoiceListGetModel.PartnerId"/>
        public FilterItem<int> PartnerId { get; set; } = new FilterItem<int>(nameof(ReceivedInvoiceGetModel.PartnerId));
    }
}
