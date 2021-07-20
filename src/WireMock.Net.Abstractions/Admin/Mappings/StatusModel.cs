﻿using System;

namespace WireMock.Admin.Mappings
{
    /// <summary>
    /// Status
    /// </summary>
    [FluentBuilder.AutoGenerateBuilder]
    public class StatusModel
    {
        /// <summary>
        /// The optional guid.
        /// </summary>
        public Guid? Guid { get; set; }

        /// <summary>
        /// The status (can also contain the error message).
        /// </summary>
        public string Status { get; set; }
    }
}