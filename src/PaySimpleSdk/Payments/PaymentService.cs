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

using PaySimpleSdk.Helpers;
using PaySimpleSdk.Models;
using PaySimpleSdk.Validation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PaySimpleSdk.Payments
{
    public class PaymentService : ServiceBase, IPaymentService
    {
        public PaymentService(IPaySimpleSettings settings)
            : base(settings)
        { }

        internal PaymentService(IPaySimpleSettings settings, IValidationService validationService, IWebServiceRequest webServiceRequest, IServiceFactory serviceFactory)
            : base(settings, validationService, webServiceRequest, serviceFactory)
        { }
        /*
        public async Task<Result<NewAccountPayment<T>>> CreateNewAccountPaymentAsync<T>(NewAccountPayment<T> accountPayment)
            where T : Account, new()
        {
            // Validate objects
            validationService.Validate(accountPayment);

            var newAccountPayment = new NewAccountPayment<T>();

            // Create the account
            var accountService = serviceFactory.GetAccountService();
            var newAccountResult = await accountService.CreateAccountAsync<T>(accountPayment.Account);
            newAccountPayment.Account = newAccountResult.Response;

            // Make the Payment            
            accountPayment.Payment.AccountId = newAccountPayment.Account.Id;
            var newPayment = await CreatePaymentAsync(accountPayment.Payment);
            newAccountPayment.Payment = newPayment.Response;

            var result = new Result<NewAccountPayment<T>>()
            {
                ResultData = newPayment.ResultData,
                Response = newAccountPayment
            };

            return result;
        }

        public async Task<Result<NewCustomerPayment<T>>> CreateNewCustomerPaymentAsync<T>(NewCustomerPayment<T> customerPayment)
            where T : Account, new()
        {
            // Validate objects
            validationService.Validate(customerPayment);

            var newCustomerPayment = new NewCustomerPayment<T>();

            // Create the new Customer
            var customerService = serviceFactory.GetCustomerService();
            var createdCustomer = await customerService.CreateCustomerAsync(customerPayment.Customer);
            newCustomerPayment.Customer = createdCustomer.Response;

            // Create the new Account, and make the payment
            customerPayment.Account.CustomerId = createdCustomer.Response.Id;
            var paymentResult = await CreateNewAccountPaymentAsync<T>(customerPayment);

            newCustomerPayment.Account = paymentResult.Response.Account;
            newCustomerPayment.Payment = paymentResult.Response.Payment;

            var result = new Result<NewCustomerPayment<T>>()
            {
                ResultData = paymentResult.ResultData,
                Response = newCustomerPayment
            };

            return result;
        }
        */

        public async Task<Result<Payment>> CreatePaymentAsync(Payment payment)
        {
            validationService.Validate(payment);
            var endpoint = string.Format("{0}{1}", settings.BaseUrl, Endpoints.Payment);
            return await webServiceRequest.PostDeserializedAsync<Payment, Result<Payment>>(new Uri(endpoint), payment);
        }

        public async Task<Result<Payment>> GetPaymentAsync(int paymentId)
        {
            var endpoint = string.Format("{0}{1}/{2}", settings.BaseUrl, Endpoints.Payment, paymentId);
            return await webServiceRequest.GetDeserializedAsync<Result<Payment>>(new Uri(endpoint));
        }

        public async Task<Result<IEnumerable<Payment>>> GetPaymentsAsync(DateTime? startDate = null, DateTime? endDate = null, IEnumerable<PaymentStatus> status = null, PaymentSort sortBy = PaymentSort.PaymentId, SortDirection direction = SortDirection.DESC, int page = 1, int pageSize = 200, bool lite = false)
        {
            StringBuilder endpoint = new StringBuilder(string.Format("{0}{1}?lite={2}", settings.BaseUrl, Endpoints.Payment, lite));

            if (startDate != null)
                endpoint.AppendFormat("&startdate={0}", startDate.Value.ToString("yyyy-MM-dd"));

            if (endDate != null)
                endpoint.AppendFormat("&enddate={0}", endDate.Value.ToString("yyyy-MM-dd"));

            if (status != null)
            {
                var stati = new List<string>();
                foreach (var s in status)
                    stati.Add(EnumStrings.PaymentStatusStrings[s]);

                endpoint.Append(string.Format("&status={0}", string.Join(",", stati)));
            }

            if (sortBy != PaymentSort.PaymentId)
                endpoint.AppendFormat("&sortby={0}", EnumStrings.PaymentSortStrings[sortBy]);

            if (direction != SortDirection.ASC)
                endpoint.AppendFormat("&direction={0}", EnumStrings.SortDirectionStrings[direction]);

            if (page != 1)
                endpoint.AppendFormat("&page={0}", page);

            if (pageSize != 200)
                endpoint.AppendFormat("&pagesize={0}", pageSize);

            return await webServiceRequest.GetDeserializedAsync<Result<IEnumerable<Payment>>>(new Uri(endpoint.ToString()));
        }

        public async Task<Result<Payment>> ReversePaymentAsync(int paymentId)
        {
            var endpoint = string.Format("{0}{1}/{2}/reverse", settings.BaseUrl, Endpoints.Payment, paymentId);
            return await webServiceRequest.PutAsync<Result<Payment>>(new Uri(endpoint));
        }

        public async Task<Result<Payment>> VoidPaymentAsync(int paymentId)
        {
            var endpoint = string.Format("{0}{1}/{2}/void", settings.BaseUrl, Endpoints.Payment, paymentId);
            return await webServiceRequest.PutAsync<Result<Payment>>(new Uri(endpoint));
        }
    }
}