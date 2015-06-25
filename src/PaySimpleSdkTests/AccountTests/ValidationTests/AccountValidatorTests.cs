﻿#region License
// The MIT License (MIT)
//
// Copyright (c) 2015 Scott Lance, Ethan Tipton
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

using PaySimpleSdk.Accounts;
using PaySimpleSdk.Accounts.Validation;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Xunit;

namespace PaySimpleSdkTests.AccountTests.ValidationTests
{
    [ExcludeFromCodeCoverage]
    public class AccountValidatorTests
    {
        [Fact]
        public void CustomerId_Is_Zero_Generates_Error()
        {
            // Arrange
            var validator = new AccountValidator<Account>();
            var account = new Account { CustomerId = 0 };

            // Act
            var result = validator.Validate(account);

            // Assert
            Assert.True(result.Errors.Any(e => e.ErrorMessage == "CustomerId is required"));
        }

        [Fact]
        public void CustomerId_Is_One_Is_Valid()
        {
            // Arrange
            var validator = new AccountValidator<Account>();
            var account = new Account { CustomerId = 1 };

            // Act
            var result = validator.Validate(account);

            // Assert
            Assert.False(result.Errors.Any(e => e.ErrorMessage == "CustomerId is required"));
        }

        // *************************************************************************************************

        [Fact]
        public void AccountNumber_Is_Empty_Digit_Generates_Error()
        {
            // Arrange
            var validator = new AchValidator();
            var account = new Ach { AccountNumber = "" };

            // Act
            var result = validator.Validate(account);

            // Assert
            Assert.True(result.Errors.Any(e => e.ErrorMessage == "AccountNumber must be numeric string and must be between 4 and 100 digits"));
        }

        [Fact]
        public void AccountNumber_Is_One_Digit_Generates_Error()
        {
            // Arrange
            var validator = new AchValidator();
            var account = new Ach { AccountNumber = "1" };

            // Act
            var result = validator.Validate(account);

            // Assert
            Assert.True(result.Errors.Any(e => e.ErrorMessage == "AccountNumber must be numeric string and must be between 4 and 100 digits"));
        }

        [Fact]
        public void AccountNumber_Is_Three_Digits_Generates_Error()
        {
            // Arrange
            var validator = new AchValidator();
            var account = new Ach { AccountNumber = "111" };

            // Act
            var result = validator.Validate(account);

            // Assert
            Assert.True(result.Errors.Any(e => e.ErrorMessage == "AccountNumber must be numeric string and must be between 4 and 100 digits"));
        }

        [Fact]
        public void AccountNumber_Is_101_Digits_Generates_Error()
        {
            // Arrange
            var validator = new AchValidator();
            var account = new Ach { AccountNumber = "11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111" };

            // Act
            var result = validator.Validate(account);

            // Assert
            Assert.True(result.Errors.Any(e => e.ErrorMessage == "AccountNumber must be numeric string and must be between 4 and 100 digits"));
        }

        [Fact]
        public void AccountNumber_Is_4_Digits_Is_Valid()
        {
            // Arrange
            var validator = new AchValidator();
            var account = new Ach { AccountNumber = "1111" };

            // Act
            var result = validator.Validate(account);

            // Assert
            Assert.False(result.Errors.Any(e => e.ErrorMessage == "AccountNumber must be numeric string and must be between 4 and 100 digits"));
        }

        [Fact]
        public void AccountNumber_Is_100_Digits_Is_Valid()
        {
            // Arrange
            var validator = new AchValidator();
            var account = new Ach { AccountNumber = "1111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111" };

            // Act
            var result = validator.Validate(account);

            // Assert
            Assert.False(result.Errors.Any(e => e.ErrorMessage == "AccountNumber must be numeric string and must be between 4 and 100 digits"));
        }

        // *************************************************************************************************

        [Fact]
        public void BankName_Is_Empty_Generates_Error()
        {
            // Arrange
            var validator = new AchValidator();
            var account = new Ach { BankName = "" };

            // Act
            var result = validator.Validate(account);

            // Assert
            Assert.True(result.Errors.Any(e => e.ErrorMessage == "BankName is required"));
        }

        [Fact]
        public void BankName_Is_101_Characters_Generates_Error()
        {
            // Arrange
            var validator = new AchValidator();
            var account = new Ach { BankName = "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb" };

            // Act
            var result = validator.Validate(account);

            // Assert
            Assert.True(result.Errors.Any(e => e.ErrorMessage == "BankName cannot exceed 100 characters"));
        }

        [Fact]
        public void BankName_Is_1_Characters_Is_Valid()
        {
            // Arrange
            var validator = new AchValidator();
            var account = new Ach { BankName = "b" };

            // Act
            var result = validator.Validate(account);

            // Assert
            Assert.False(result.Errors.Any(e => e.ErrorMessage == "BankName cannot exceed 100 characters"));
        }

        [Fact]
        public void BankName_Is_100_Characters_Is_Valid()
        {
            // Arrange
            var validator = new AchValidator();
            var account = new Ach { BankName = "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb" };

            // Act
            var result = validator.Validate(account);

            // Assert
            Assert.False(result.Errors.Any(e => e.ErrorMessage == "BankName cannot exceed 100 characters"));
        }

        // *************************************************************************************************

        [Fact]
        public void RoutingNumber_Empty_Generates_Error()
        {
            // Arrange
            var validator = new AchValidator();
            var account = new Ach { RoutingNumber = "" };

            // Act
            var result = validator.Validate(account);

            // Assert
            Assert.True(result.Errors.Any(e => e.ErrorMessage == "RoutingNumber must be a 9 digit number"));
        }

        [Fact]
        public void RoutingNumber_Has_Alpha_Generates_Error()
        {
            // Arrange
            var validator = new AchValidator();
            var account = new Ach { RoutingNumber = "12345678A" };

            // Act
            var result = validator.Validate(account);

            // Assert
            Assert.True(result.Errors.Any(e => e.ErrorMessage == "RoutingNumber must be a 9 digit number"));
        }

        [Fact]
        public void RoutingNumber_Has_8_Digits_Generates_Error()
        {
            // Arrange
            var validator = new AchValidator();
            var account = new Ach { RoutingNumber = "12345678" };

            // Act
            var result = validator.Validate(account);

            // Assert
            Assert.True(result.Errors.Any(e => e.ErrorMessage == "RoutingNumber must be a 9 digit number"));
        }

        [Fact]
        public void RoutingNumber_Has_10_Digits_Generates_Error()
        {
            // Arrange
            var validator = new AchValidator();
            var account = new Ach { RoutingNumber = "1234567890" };

            // Act
            var result = validator.Validate(account);

            // Assert
            Assert.True(result.Errors.Any(e => e.ErrorMessage == "RoutingNumber must be a 9 digit number"));
        }

        [Fact]
        public void RoutingNumber_Has_9_Digits_Is_Valid()
        {
            // Arrange
            var validator = new AchValidator();
            var account = new Ach { RoutingNumber = "123456789" };

            // Act
            var result = validator.Validate(account);

            // Assert
            Assert.False(result.Errors.Any(e => e.ErrorMessage == "RoutingNumber must be a 9 digit number"));
        }

        // *************************************************************************************************

        /// <summary>
        /// 400373665617786, 365389111509143 are fake credit card numbers generated at http://bradconte.com/cc_generator
        /// this number passes the Luhn Checksum (https://en.wikipedia.org/wiki/Luhn_algorithm)
        /// </summary>

        [Fact]
        public void CreditCardNumber_Is_Empty_Generates_Error()
        {
            // Arrange
            var validator = new CreditCardValidator();
            var account = new CreditCard { CreditCardNumber = "" };

            // Act
            var result = validator.Validate(account);

            // Assert
            Assert.True(result.Errors.Any(e => e.ErrorMessage == "CreditCardNumber is required"));
        }

        [Fact]
        public void CreditCardNumber_Is_14_Digits_Generates_Error()
        {
            // Arrange
            var validator = new CreditCardValidator();
            var account = new CreditCard { CreditCardNumber = "40037366561778" };

            // Act
            var result = validator.Validate(account);

            // Assert
            Assert.True(result.Errors.Any(e => e.ErrorMessage == "CreditCardNumber is invalid"));
        }

        [Fact]
        public void CreditCardNumber_Is_17_Digits_Generates_Error()
        {
            // Arrange
            var validator = new CreditCardValidator();
            var account = new CreditCard { CreditCardNumber = "40037366561778601" };

            // Act
            var result = validator.Validate(account);

            // Assert
            Assert.True(result.Errors.Any(e => e.ErrorMessage == "CreditCardNumber is invalid"));
        }

        [Fact]
        public void CreditCardNumber_Is_15_Digits_Is_Valid()
        {
            // Arrange
            var validator = new CreditCardValidator();
            var account = new CreditCard { CreditCardNumber = "365389111509143" };

            // Act
            var result = validator.Validate(account);

            // Assert
            Assert.False(result.Errors.Any(e => e.ErrorMessage == "CreditCardNumber is invalid"));
        }

        [Fact]
        public void CreditCardNumber_Is_16_Digits_Is_Valid()
        {
            // Arrange
            var validator = new CreditCardValidator();
            var account = new CreditCard { CreditCardNumber = "4003736656177860" };

            // Act
            var result = validator.Validate(account);

            // Assert
            Assert.False(result.Errors.Any(e => e.ErrorMessage == "CreditCardNumber is invalid"));
        }

        // *************************************************************************************************

        [Fact]
        public void ExprirationDate_Is_Empty_Generates_Errors()
        {
            // Arrange
            var validator = new CreditCardValidator();
            var account = new CreditCard { ExpirationDate = "" };

            // Act
            var result = validator.Validate(account);

            // Assert
            Assert.True(result.Errors.Any(e => e.ErrorMessage == "ExpirationDate must be in a \"MM/YYYY\" format"));
        }

        [Fact]
        public void ExprirationDate_Is_Not_In_MM_YYYY_Format_Generates_Errors()
        {
            // Arrange
            var validator = new CreditCardValidator();
            var account = new CreditCard { ExpirationDate = "102015" };

            // Act
            var result = validator.Validate(account);

            // Assert
            Assert.True(result.Errors.Any(e => e.ErrorMessage == "ExpirationDate must be in a \"MM/YYYY\" format"));
        }

        [Fact]
        public void ExprirationDate_Is_In_MM_YYYY_Format_Is_Valid()
        {
            // Arrange
            var validator = new CreditCardValidator();
            var account = new CreditCard { ExpirationDate = "10/2015" };

            // Act
            var result = validator.Validate(account);

            // Assert
            Assert.False(result.Errors.Any(e => e.ErrorMessage == "ExpirationDate must be in a \"MM/YYYY\" format"));
        }

        // *************************************************************************************************

        [Fact]
        public void BillingZipcode_Is_Not_Valid_Generates_Error()
        {
            // Arrange
            var validatior = new CreditCardValidator();
            var account = new CreditCard { BillingZipCode = "789456" };

            // Act
            var result = validatior.Validate(account);

            // Assert
            Assert.True(result.Errors.Any(e => e.ErrorMessage == "BillingZipCode must be a valid US or CA postal code, acceptable formats are 11111, 11111-1111, A1A1A1, or A1A 1A1"));
        }

        [Fact]
        public void BillingZipcode_Is_5_Digits_Is_Valid()
        {
            // Arrange
            var validatior = new CreditCardValidator();
            var account = new CreditCard { BillingZipCode = "84101" };

            // Act
            var result = validatior.Validate(account);

            // Assert
            Assert.False(result.Errors.Any(e => e.ErrorMessage == "BillingZipCode must be a valid US or CA postal code, acceptable formats are 11111, 11111-1111, A1A1A1, or A1A 1A1"));
        }

        [Fact]
        public void BillingZipcode_Is_5_Plus_4_Digits_Is_Valid()
        {
            // Arrange
            var validatior = new CreditCardValidator();
            var account = new CreditCard { BillingZipCode = "84101-7331" };

            // Act
            var result = validatior.Validate(account);

            // Assert
            Assert.False(result.Errors.Any(e => e.ErrorMessage == "BillingZipCode must be a valid US or CA postal code, acceptable formats are 11111, 11111-1111, A1A1A1, or A1A 1A1"));
        }

        [Fact]
        public void BillingZipcode_Is_Canadian_Postal_Code_With_Space_Is_Valid()
        {
            // Arrange
            var validatior = new CreditCardValidator();
            var account = new CreditCard { BillingZipCode = "L4L 9C8" };

            // Act
            var result = validatior.Validate(account);

            // Assert
            Assert.False(result.Errors.Any(e => e.ErrorMessage == "BillingZipCode must be a valid US or CA postal code, acceptable formats are 11111, 11111-1111, A1A1A1, or A1A 1A1"));
        }

        [Fact]
        public void BillingZipcode_Is_Canadian_Postal_Code_Without_Space_Is_Valid()
        {
            // Arrange
            var validatior = new CreditCardValidator();
            var account = new CreditCard { BillingZipCode = "L4L9C8" };

            // Act
            var result = validatior.Validate(account);

            // Assert
            Assert.False(result.Errors.Any(e => e.ErrorMessage == "BillingZipCode must be a valid US or CA postal code, acceptable formats are 11111, 11111-1111, A1A1A1, or A1A 1A1"));
        }

        // *************************************************************************************************
    }
}