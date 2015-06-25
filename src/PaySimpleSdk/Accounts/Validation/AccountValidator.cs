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
            RuleFor(m => m.AccountNumber).Matches(@"^([0-9]{4,100})$").WithMessage("AccountNumber must be numeric string and must be between 4 and 100 digits");
            RuleFor(m => m.BankName).NotEmpty().WithMessage("BankName is required").Length(0, 100).WithMessage("BankName cannot exceed 100 characters");
            RuleFor(m => m.RoutingNumber).Matches(@"^[0-9]{9}$").WithMessage("RoutingNumber must be a 9 digit number");
        }
    }

    internal class CreditCardValidator : AccountValidator<CreditCard>
    {
        public CreditCardValidator()
        {
            RuleFor(m => m.CreditCardNumber).NotEmpty().WithMessage("CreditCardNumber is required").CreditCard().WithMessage("CreditCardNumber is invalid");
            RuleFor(m => m.ExpirationDate).NotEmpty().WithMessage("ExpirationDate is required").Matches(@"^(0[1-9]{1}|1[0-2]{1}/\d{4})$").WithMessage("ExpirationDate must be in a \"MM/YYYY\" format");
            RuleFor(m => m.BillingZipCode).PostalCode().WithMessage("BillingZipCode must be a valid US or CA postal code, acceptable formats are 11111, 11111-1111, A1A1A1, or A1A 1A1");
        }
    }
}