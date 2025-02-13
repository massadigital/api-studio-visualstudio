﻿// Copyright (c) Andrew Butson.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using ApiStudioIO.Common.Models.Http;
using ApiStudioIO.Vs.Options;

namespace ApiStudioIO.HttpApiDefaults
{
    internal static class HttpApiHeaderExtension
    {
        internal static HttpApi WithDefaultHeaders(this HttpApi httpApi)
        {
            ApiStudioUserSettingsStore.Instance.VsOptionStoreLoad(); //Load vs-options
            ClearAutoGeneratedDefaults(httpApi);
            CreateDefaultRequestHeader(httpApi);
            CreateDefaultResponseHeader(httpApi);

            return httpApi;
        }

        private static void ClearAutoGeneratedDefaults(HttpApi httpApi)
        {
            using (var t = httpApi.Store.TransactionManager.BeginTransaction(
                       "HttpApiHeaderExtension.ClearAutoGeneratedDefaults"))
            {
                httpApi?.HttpApiHeaderRequests
                    .Where(x => x.IsAutoGenerated)
                    .ToList()
                    .ForEach(x => httpApi.HttpApiHeaderRequests.Remove(x));

                httpApi?.HttpApiHeaderResponses
                    .Where(x => x.IsAutoGenerated)
                    .ToList()
                    .ForEach(x => httpApi.HttpApiHeaderResponses.Remove(x));

                t.Commit();
            }
        }

        private static void CreateDefaultRequestHeader(HttpApi httpApi)
        {
            var managedList = new List<HttpResourceHeaderRequest>();

            foreach (var headersRequest in ApiStudioUserSettingsStore.Instance.Data.DefaultHeaders.Request.Values)
                managedList.Add(new HttpResourceHeaderRequest
                {
                    Name = headersRequest.Name,
                    Description = headersRequest.Description,
                    IsRequired = headersRequest.IsRequired,
                    AllowEmptyValue = headersRequest.AllowEmptyValue,
                    IsAutoGenerated = true
                });

            ApiStudioComponentTransactionManager.Save(httpApi, managedList);
        }

        private static void CreateDefaultResponseHeader(HttpApi httpApi)
        {
            var managedList = new List<HttpResourceHeaderResponse>();

            managedList.AddRange(AddResponseHeaderLocation(httpApi));

            foreach (var headersResponse in ApiStudioUserSettingsStore.Instance.Data.DefaultHeaders.Response.Values)
                managedList.Add(new HttpResourceHeaderResponse
                {
                    Name = headersResponse.Name,
                    Description = headersResponse.Description,
                    IsRequired = headersResponse.IsRequired,
                    AllowEmptyValue = headersResponse.AllowEmptyValue,
                    IsAutoGenerated = true,
                    IncludeOn = headersResponse.IncludeOn
                });

            ApiStudioComponentTransactionManager.Save(httpApi, managedList);
        }

        private static List<HttpResourceHeaderResponse> AddResponseHeaderLocation(HttpApi httpApi)
        {
            var managedList = new List<HttpResourceHeaderResponse>();

            //  201 (Created) New resource created @ location
            //  301 (Moved Permanently) and 302(Found) don't change the method most of the time, though older user-agents may (so you basically don't know).
            //  303 (See Also) responses always lead to the use of a GET method.
            //  307 (Temporary Redirect) and 308(Permanent Redirect) don't change the method used in the original request.
            int[] validLocation = { 201, 301, 302, 303, 307 };
            if (httpApi.ResponseStatusCodes.Select(x => x.HttpStatus).Intersect(validLocation).Any())
                managedList.Add(new HttpResourceHeaderResponse
                {
                    Name = "Location",
                    Description =
                        "The Location response header indicates the URL to redirect a page to. It only provides a meaning when served with a 3xx (redirection) or 201 (created) status response. In cases of resource creation (201), it indicates the URL to the newly created resource.",
                    IsRequired = true,
                    AllowEmptyValue = false,
                    IsAutoGenerated = true,
                    IncludeOn = HttpTypeHeaderOnResponse.OnSuccess
                });
            return managedList;
        }
    }
}