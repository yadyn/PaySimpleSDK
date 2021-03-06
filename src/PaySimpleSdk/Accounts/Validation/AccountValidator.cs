﻿#region License
// The MIT License (MIT)
//
// Copyright (c) 2015 Scott Lance
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// 
// The most recent version of this license can be found at: http://opensource.org/licenses/MIT
#endregion

using FluentValidation;

namespace PaySimpleSdk.Accounts.Validation
{
    internal class AccountValidator<T> : AbstractValidator<T>
        where T : Account
    {
        public AccountValidator()
        {
            RuleFor(m => m.CustomerId).Must(c => c > 0).WithMessage("CustomerId is required");
        }
    }

    internal class AchValidator : AccountValidator<Ach>
    {
        public AchValidator()
        {
            RuleFor(m => m.AccountNumber).Matches(@"^([0-9]{4,100})$|(^(\*{1,96})[0-9]{4})$").WithMessage("AccountNumber must be numeric string and must be between 4 and 100 digits");
            RuleFor(m => m.BankName).NotEmpty().WithMessage("BankName is required").Length(0, 100).WithMessage("BankName cannot exceed 100 characters");
            RuleFor(m => m.RoutingNumber).Matches(@"^[0-9]{9}$").WithMessage("RoutingNumber must be a 9 digit number");
        }
    }

    internal class CreditCardValidator : AccountValidator<CreditCard>
    {
        public CreditCardValidator()
        {
            RuleFor(m => m.CreditCardNumber).NotEmpty().WithMessage("CreditCardNumber is required").Matches(@"^(?:4[0-9]{12}(?:[0-9]{3})?|5[1-5][0-9]{14}|3[47][0-9]{13}|6(?:011|5[0-9]{2})[0-9]{12})$|^((\*{11,12})[0-9]{4})$").WithMessage("CreditCardNumber is invalid");
            RuleFor(m => m.ExpirationDate).NotEmpty().WithMessage("ExpirationDate is required").Matches(@"^(([0][1-9])|([1][0-2]))/20[0-9][0-9]$").WithMessage("ExpirationDate must be in a \"MM/YYYY\" format");
            RuleFor(m => m.BillingZipCode).PostalCode().WithMessage("BillingZipCode must be no more than 10 characters");
        }
    }
}