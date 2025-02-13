﻿// Copyright (c) Andrew Butson.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using ApiStudioIO.Common.Models.Http;
using ApiStudioIO.Vs.Options;

namespace ApiStudioIO.HttpApiDefaults
{
    internal static class HttpApiResponseStatusCodeExtension
    {
        internal static HttpApi WithDefaultResponses(this HttpApi httpApi)
        {
            ApiStudioUserSettingsStore.Instance.VsOptionStoreLoad(); //Load vs-options
            ClearAutoGeneratedDefaults(httpApi);
            CreateDefaults(httpApi);

            return httpApi;
        }

        private static void ClearAutoGeneratedDefaults(HttpApi httpApi)
        {
            using (var t = httpApi.Store.TransactionManager.BeginTransaction(
                       "HttpApiResponseStatusCodeExtension.ClearAutoGeneratedDefaults"))
            {
                httpApi.HttpApiResponseStatusCodes
                    .Where(x => x.IsAutoGenerated)
                    .ToList()
                    .ForEach(x => httpApi.HttpApiResponseStatusCodes.Remove(x));
                t.Commit();
            }
        }

        private static void CreateDefaults(HttpApi httpApi)
        {
            var standardResponseCodes =
                ApiStudioUserSettingsStore.Instance.Data.DefaultResponseCodes.StandardResponseCodes;
            var apiResponses = new List<HttpResourceResponseStatusCode>();
            var responseCodes = ApiStudioUserSettingsStore.Instance.Data.DefaultResponseCodes;

            switch (httpApi)
            {
                case HttpApiGet _: // https://developer.mozilla.org/en-US/docs/Web/HTTP/Methods/GET
                    apiResponses.Add(ConvertResponseStatusCode(responseCodes.SuccessGet));
                    break;

                case HttpApiPut _: // https://developer.mozilla.org/en-US/docs/Web/HTTP/Methods/PUT
                    apiResponses.Add(ConvertResponseStatusCode(responseCodes.SuccessPut)); // 200, 201, 204
                    if (standardResponseCodes.Contains(422)) //[Unprocessable]
                        apiResponses.Add(ConvertResponseStatusCode(422));
                    break;

                case HttpApiPost _: // https://developer.mozilla.org/en-US/docs/Web/HTTP/Methods/POST
                    apiResponses.Add(ConvertResponseStatusCode(responseCodes.SuccessPost));
                    if (standardResponseCodes.Contains(422)) //[Unprocessable]
                        apiResponses.Add(ConvertResponseStatusCode(422));
                    break;

                case HttpApiDelete _: // https://developer.mozilla.org/en-US/docs/Web/HTTP/Methods/DELETE
                    apiResponses.Add(ConvertResponseStatusCode(responseCodes.SuccessDelete)); // 200, 202, 204
                    break;

                case HttpApiPatch _: // https://developer.mozilla.org/en-US/docs/Web/HTTP/Methods/PATCH
                    apiResponses.Add(ConvertResponseStatusCode(responseCodes.SuccessPatch)); // 200, 204
                    break;

                case HttpApiTrace _: // https://developer.mozilla.org/en-US/docs/Web/HTTP/Methods/TRACE
                    apiResponses.Add(ConvertResponseStatusCode(responseCodes.SuccessTrace));
                    break;

                case HttpApiHead _: // https://developer.mozilla.org/en-US/docs/Web/HTTP/Methods/HEAD
                    apiResponses.Add(ConvertResponseStatusCode(responseCodes.SuccessHead));
                    break;

                case HttpApiOptions _: // https://developer.mozilla.org/en-US/docs/Web/HTTP/Methods/OPTIONS
                    apiResponses.Add(ConvertResponseStatusCode(responseCodes.SuccessOptions));
                    break;
            }

            if (HasDocumentResourceInPath(httpApi) && standardResponseCodes.Contains(404))
                apiResponses.Add(ConvertResponseStatusCode(404)); //[Not found]

            foreach (var responseCode in standardResponseCodes)
                if (responseCode != 404 &&
                    responseCode != 422) //Remove technical response codes (handled by above logic)
                    apiResponses.Add(ConvertResponseStatusCode(responseCode));

            ApiStudioComponentTransactionManager.Save(httpApi, apiResponses);
        }

        //Check if the resource has a document in path e.g. the resource could not be found (404)
        private static bool HasDocumentResourceInPath(HttpApi httpApi)
        {
            var source = httpApi.Resourced.FirstOrDefault();
            while (source != null)
            {
                //break on the first document found
                if (source is ResourceInstance) return true;

                source = source.SourceResource.FirstOrDefault();
            }

            return false;
        }


        private static HttpResourceResponseStatusCode ConvertResponseStatusCode(int httpStatus)
        {
            var apiStudioComponent = new HttpResourceResponseStatusCode
            {
                HttpStatus = httpStatus,
                IsAutoGenerated = true
            };
            return apiStudioComponent;
        }
    }
}