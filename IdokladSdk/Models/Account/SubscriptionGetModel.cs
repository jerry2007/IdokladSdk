﻿using System;
using IdokladSdk.Enums;

namespace IdokladSdk.Models.Account
{
    /// <summary>
    /// SubscriptionGetModel.
    /// </summary>
    public class SubscriptionGetModel
    {
        /// <summary xml:lang="en">
        /// Gets or sets date from.
        /// </summary>
        public DateTime? DateFrom { get; set; }

        /// <summary>
        /// Gets or sets date to.
        /// </summary>
        public DateTime? DateTo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the account is trial account.
        /// </summary>
        public bool IsTrial { get; set; }

        /// <summary>
        /// Gets or sets mobile store type.
        /// </summary>
        public MobileStoreType MobileStoreType { get; set; }

        /// <summary>
        /// Gets or sets subscription type.
        /// </summary>
        public SubscriptionType? Type { get; set; }
    }
}
