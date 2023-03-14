﻿// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by CashOverflow Team
// --------------------------------------------------------

using Xeptions;

namespace CashOverflow.Models.Locations.Exceptions
{
    public class LocationValidationException : Xeption
    {
        public LocationValidationException(Xeption innerException)
            : base(message: "Location validation error occured, contact support.", 
                  innerException)
        { }
    }
}
