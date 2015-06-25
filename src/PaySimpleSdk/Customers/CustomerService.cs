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

using PaySimpleSdk.Accounts;
using PaySimpleSdk.Helpers;
using PaySimpleSdk.Models;
using PaySimpleSdk.Payments;
using PaySimpleSdk.PaymentSchedules;
using PaySimpleSdk.Validation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace PaySimpleSdk.Customers
{
    public class CustomerService : ServiceBase, ICustomerService
    {
        public CustomerService(IPaySimpleSettings settings)
            : base(settings)
        { }

        internal CustomerService(IPaySimpleSettings settings, IValidationService validationService, IWebServiceRequest webServiceRequest, IServiceFactory serviceFactory)
            : base(settings, validationService, webServiceRequest, serviceFactory)
        { }

        public async Task<Result<Customer>> CreateCustomerAsync(Customer customer)
        {
            validationService.Validate(customer);
            var endpoint = string.Format("{0}{1}", settings.BaseUrl, Endpoints.Customer);
            return await webServiceRequest.PostDeserializedAsync<Customer, Result<Customer>>(new Uri(endpoint), customer);
        }

        public async Task DeleteCustomerAsync(int customerId)
        {
            var endpoint = string.Format("{0}{1}/{2}", settings.BaseUrl, Endpoints.Customer, customerId);
            await webServiceRequest.DeleteAsync(new Uri(endpoint));
        }

        public async Task<Result<SearchResults>> FindCustomer(string query)
        {
            var endpoint = string.Format("{0}{1}?Query={2}", settings.BaseUrl, Endpoints.GlobalSearch, HttpUtility.UrlEncode(query));
            return await webServiceRequest.GetDeserializedAsync<Result<SearchResults>>(new Uri(endpoint));
        }

        public async Task<Result<IEnumerable<Ach>>> GetAchAccountsAsync(int customerId)
        {
            var endpoint = string.Format("{0}{1}/{2}/achaccounts", settings.BaseUrl, Endpoints.Customer, customerId);
            return await webServiceRequest.GetDeserializedAsync<Result<IEnumerable<Ach>>>(new Uri(endpoint));
        }

        public async Task<Result<AccountList>> GetAllAccountsAsync(int customerId)
        {
            var endpoint = string.Format("{0}{1}/{2}/accounts", settings.BaseUrl, Endpoints.Customer, customerId);
            return await webServiceRequest.GetDeserializedAsync<Result<AccountList>>(new Uri(endpoint));
        }

        public async Task<Result<IEnumerable<CreditCard>>> GetCreditCardAccountsAsync(int customerId)
        {
            var endpoint = string.Format("{0}{1}/{2}/creditcardaccounts", settings.BaseUrl, Endpoints.Customer, customerId);
            return await webServiceRequest.GetDeserializedAsync<Result<IEnumerable<CreditCard>>>(new Uri(endpoint));
        }

        public async Task<Result<Customer>> GetCustomerAsync(int customerId)
        {
            var endpoint = string.Format("{0}{1}/{2}", settings.BaseUrl, Endpoints.Customer, customerId);
            return await webServiceRequest.GetDeserializedAsync<Result<Customer>>(new Uri(endpoint));
        }

        public async Task<Result<IEnumerable<Customer>>> GetCustomersAsync(CustomerSort sortBy = CustomerSort.LastName, SortDirection direction = SortDirection.ASC, int page = 1, int pageSize = 200, bool lite = false)
        {
            StringBuilder endpoint = new StringBuilder(string.Format("{0}{1}?lite={2}", settings.BaseUrl, Endpoints.Customer, lite));

            if (sortBy != CustomerSort.LastName)
                endpoint.AppendFormat("&sortby={0}", EnumStrings.CustomerSortStrings[sortBy]);

            if (direction != SortDirection.ASC)
                endpoint.AppendFormat("&direction={0}", EnumStrings.SortDirectionStrings[direction]);

            if (page != 1)
                endpoint.AppendFormat("&page={0}", page);

            if (pageSize != 200)
                endpoint.AppendFormat("&pagesize={0}", pageSize);

            return await webServiceRequest.GetDeserializedAsync<Result<IEnumerable<Customer>>>(new Uri(endpoint.ToString()));
        }

        public async Task<Result<Ach>> GetDefaultAchAccountAsync(int customerId)
        {
            var endpoint = string.Format("{0}{1}/{2}/defaultach", settings.BaseUrl, Endpoints.Customer, customerId);
            return await webServiceRequest.GetDeserializedAsync<Result<Ach>>(new Uri(endpoint));
        }

        public async Task<Result<CreditCard>> GetDefaultCreditCardAccountAsync(int customerId)
        {
            var endpoint = string.Format("{0}{1}/{2}/defaultcreditcard", settings.BaseUrl, Endpoints.Customer, customerId);
            return await webServiceRequest.GetDeserializedAsync<Result<CreditCard>>(new Uri(endpoint));
        }

        public async Task<Result<IEnumerable<PaymentPlan>>> GetPaymentPlansAsync(int customerId, DateTime? startDate = null, DateTime? endDate = null, ScheduleStatus status = ScheduleStatus.None, ScheduleSort sortBy = ScheduleSort.Id, SortDirection direction = SortDirection.ASC, int page = 1, int pageSize = 200, bool lite = false)
        {
            StringBuilder endpoint = new StringBuilder(string.Format("{0}{1}/{2}/paymentplans?lite={2}", settings.BaseUrl, Endpoints.Customer, customerId, lite));

            if (startDate != null)
                endpoint.AppendFormat("&startdate={0}", startDate.Value.ToString("yyyy-MM-dd"));

            if (endDate != null)
                endpoint.AppendFormat("&enddate={0}", endDate.Value.ToString("yyyy-MM-dd"));

            if (status != ScheduleStatus.None)
                endpoint.Append(string.Format("&status={0}", status));

            if (sortBy != ScheduleSort.Id && sortBy != ScheduleSort.PaymentScheduleType)
                endpoint.AppendFormat("&sortby={0}", EnumStrings.ScheduleSortStrings[sortBy]);

            if (direction != SortDirection.ASC)
                endpoint.AppendFormat("&direction={0}", EnumStrings.SortDirectionStrings[direction]);

            if (page != 1)
                endpoint.AppendFormat("&page={0}", page);

            if (pageSize != 200)
                endpoint.AppendFormat("&pagesize={0}", pageSize);

            return await webServiceRequest.GetDeserializedAsync<Result<IEnumerable<PaymentPlan>>>(new Uri(endpoint.ToString()));
        }

        public async Task<Result<IEnumerable<Payment>>> GetPaymentsAsync(int customerId, DateTime? startDate = null, DateTime? endDate = null, IEnumerable<PaymentStatus> status = null, PaymentSort sortBy = PaymentSort.PaymentId, SortDirection direction = SortDirection.ASC, int page = 1, int pageSize = 200, bool lite = false)
        {
            StringBuilder endpoint = new StringBuilder(string.Format("{0}{1}/{2}/payments?lite={3}", settings.BaseUrl, Endpoints.Customer, customerId, lite));

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

        public async Task<Result<PaymentScheduleList>> GetPaymentSchedulesAsync(int customerId, DateTime? startDate = null, DateTime? endDate = null, ScheduleStatus status = ScheduleStatus.None, ScheduleSort sortBy = ScheduleSort.Id, SortDirection direction = SortDirection.ASC, int page = 1, int pageSize = 200, bool lite = false)
        {
            StringBuilder endpoint = new StringBuilder(string.Format("{0}{1}/{2}/paymentschedules?lite={3}", settings.BaseUrl, Endpoints.Customer, customerId, lite));

            if (startDate != null)
                endpoint.AppendFormat("&startdate={0}", startDate.Value.ToString("yyyy-MM-dd"));

            if (endDate != null)
                endpoint.AppendFormat("&enddate={0}", endDate.Value.ToString("yyyy-MM-dd"));

            if (status != ScheduleStatus.None)
                endpoint.Append(string.Format("&status={0}", status));

            if (sortBy != ScheduleSort.Id)
                endpoint.AppendFormat("&sortby={0}", EnumStrings.ScheduleSortStrings[sortBy]);

            if (direction != SortDirection.ASC)
                endpoint.AppendFormat("&direction={0}", EnumStrings.SortDirectionStrings[direction]);

            if (page != 1)
                endpoint.AppendFormat("&page={0}", page);

            if (pageSize != 200)
                endpoint.AppendFormat("&pagesize={0}", pageSize);

            return await webServiceRequest.GetDeserializedAsync<Result<PaymentScheduleList>>(new Uri(endpoint.ToString()));
        }

        public async Task<Result<IEnumerable<RecurringPayment>>> GetRecurringPaymentSchedulesAsync(int customerId, DateTime? startDate = null, DateTime? endDate = null, ScheduleStatus status = ScheduleStatus.None, ScheduleSort sortBy = ScheduleSort.Id, SortDirection direction = SortDirection.ASC, int page = 1, int pageSize = 200, bool lite = false)
        {
            StringBuilder endpoint = new StringBuilder(string.Format("{0}{1}/{2}/recurringpayments?lite={3}", settings.BaseUrl, Endpoints.Customer, customerId, lite));

            if (startDate != null)
                endpoint.AppendFormat("&startdate={0}", startDate.Value.ToString("yyyy-MM-dd"));

            if (endDate != null)
                endpoint.AppendFormat("&enddate={0}", endDate.Value.ToString("yyyy-MM-dd"));

            if (status != ScheduleStatus.None)
                endpoint.Append(string.Format("&status={0}", status));

            if (sortBy != ScheduleSort.Id)
                endpoint.AppendFormat("&sortby={0}", EnumStrings.ScheduleSortStrings[sortBy]);

            if (direction != SortDirection.ASC)
                endpoint.AppendFormat("&direction={0}", EnumStrings.SortDirectionStrings[direction]);

            if (page != 1)
                endpoint.AppendFormat("&page={0}", page);

            if (pageSize != 200)
                endpoint.AppendFormat("&pagesize={0}", pageSize);

            return await webServiceRequest.GetDeserializedAsync<Result<IEnumerable<RecurringPayment>>>(new Uri(endpoint.ToString()));
        }

        public async Task SetDefaultAccountAsync(int customerId, int accountId)
        {
            var endpoint = string.Format("{0}{1}/{2}/{3}", settings.BaseUrl, Endpoints.Customer, customerId, accountId);
            await webServiceRequest.PutAsync(new Uri(endpoint));
        }

        public async Task<Result<Customer>> UpdateCustomerAsync(Customer customer)
        {
            validationService.Validate(customer);
            var endpoint = string.Format("{0}{1}", settings.BaseUrl, Endpoints.Customer);
            return await webServiceRequest.PutDeserializedAsync<Customer, Result<Customer>>(new Uri(endpoint), customer);
        }
    }
}