﻿using System.ComponentModel.DataAnnotations;
using IdokladSdk.Models.Common;

namespace IdokladSdk.Models.IssuedInvoice
{
    /// <summary>
    /// IssuedInvoiceItemRecountPostModel.
    /// </summary>
    public class IssuedInvoiceItemRecountPostModel : ItemRecountPostModel
    {
        /// <summary>
        /// Gets or sets discount size in percent.
        /// </summary>
        [Range(0.0, 99.99)]
        [Required]
        public decimal DiscountPercentage { get; set; }
    }
}
