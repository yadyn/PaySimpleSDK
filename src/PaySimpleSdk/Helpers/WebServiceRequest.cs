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

using PaySimpleSdk.Exceptions;
using PaySimpleSdk.Models;
using System;
using System.Net;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PaySimpleSdk.Helpers
{
    [ExcludeFromCodeCoverage]
    internal class WebServiceRequest : IWebServiceRequest
    {
        private readonly ISerialization serialization;
        private readonly ISignatureGenerator signatureGenerator;
        private readonly int retryCount;

        public WebServiceRequest(ISerialization serialization, ISignatureGenerator signatureGenerator, int retryCount)
        {
            ServicePointManager.SecurityProtocol = ServicePointManager.SecurityProtocol | SecurityProtocolType.Tls12;

            this.serialization = serialization;
            this.signatureGenerator = signatureGenerator;
            this.retryCount = retryCount;
        }

        public async Task<HttpResponseMessage> GetAsync(Uri requestUri)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);

            return await MakeRequestAsync(requestMessage);
        }

        public async Task<T> GetDeserializedAsync<T>(Uri requestUri) where T : class
        {
            var result = await GetAsync(requestUri);
            var content = await result.Content.ReadAsStringAsync();

            return serialization.Deserialize<T>(content);
        }

        public async Task<HttpResponseMessage> PutAsync(Uri requestUri)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Put, requestUri);
            return await MakeRequestAsync(requestMessage);
        }

        public async Task<T> PutAsync<T>(Uri requestUri) where T : class
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Put, requestUri);
            var result = await MakeRequestAsync(requestMessage);
            var content = await result.Content.ReadAsStringAsync();
            return serialization.Deserialize<T>(content);
        }

        public async Task<HttpResponseMessage> PutAsync<T>(Uri requestUri, T payload) where T : class
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Put, requestUri);
            var content = new StringContent(serialization.Serialize(payload));

            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            requestMessage.Content = content;

            return await MakeRequestAsync(requestMessage);
        }

        public async Task<T2> PutDeserializedAsync<T1, T2>(Uri requestUri, T1 payload)
            where T1 : class
            where T2 : class
        {
            var result = await PutAsync<T1>(requestUri, payload);
            var content = await result.Content.ReadAsStringAsync();

            return serialization.Deserialize<T2>(content);
        }

        public async Task<HttpResponseMessage> DeleteAsync(Uri requestUri)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, requestUri);

            return await MakeRequestAsync(requestMessage);
        }

        public async Task<HttpResponseMessage> PostAsync<T>(Uri requestUri, T payload) where T : class
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri);
            var content = new StringContent(serialization.Serialize(payload));

            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            requestMessage.Content = content;

            return await MakeRequestAsync(requestMessage);
        }

        public async Task<T2> PostDeserializedAsync<T1, T2>(Uri requestUri, T1 payload)
            where T1 : class
            where T2 : class
        {
            var result = await PostAsync<T1>(requestUri, payload);
            var content = await result.Content.ReadAsStringAsync();

            return serialization.Deserialize<T2>(content);
        }

        private async Task<HttpResponseMessage> MakeRequestAsync(HttpRequestMessage request)
        {
            var exceptions = new List<Exception>();

            // Minor optimization: skip the loop entirely if we don't need it
            if (retryCount <= 1)
                return await DoRequestAsync(request);

            for (int retry = 0; retry < retryCount; retry++)
            {
                try
                {
                    // Wait 1 second between additional attempts
                    if (retry > 0)
                        System.Threading.Thread.Sleep(1000);

                    return await DoRequestAsync(request);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            throw new AggregateException(exceptions);
        }
        private async Task<HttpResponseMessage> DoRequestAsync(HttpRequestMessage request)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var authToken = signatureGenerator.GenerateSignature();
                httpClient.DefaultRequestHeaders.Add("Authorization", authToken);
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

                var result = await httpClient.SendAsync(request).ConfigureAwait(false);

                if (result.IsSuccessStatusCode)
                    return result;

                var content = await result.Content.ReadAsStringAsync();
                try
                {
                    var errors = serialization.Deserialize<ErrorResult>(content);
                    throw new PaySimpleEndpointException(errors, result.StatusCode);
                }
                catch (Exception e) when (!(e is PaySimpleEndpointException))
                {
                    throw new PaySimpleEndpointException($"Error deserializing response: {content}", e);
                }
            }
        }
    }
}