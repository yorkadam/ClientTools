using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ClientTools
{

    public static class Poster
    {
        /// <summary>
        /// Posts a HTML form data with certificate checks defaulted to always check. Exception is raised on invalid certificate.
        /// </summary>
        /// <param name="formFields">The dictionary of fields and values from a html form object</param>
        /// <param name="uri">The URI to post to (form action=) </param>
        /// <param name="httpHeaders">Headers such as User Agent and any other header that belongs in the http header section of a request.</param>
        /// <param name="mediaHeaders">The accepted media type in response to request. Note: QualiyValue is defaulted here.</param>
        /// <returns>HttpResponseMessage</returns>
        public static HttpResponseMessage Post(List<KeyValuePair<string, string>> formFields, Uri uri, Dictionary<string,string> httpHeaders, List<string> mediaHeaders)
        {
            var formContent = new FormUrlEncodedContent(formFields);
            HttpClient client = new HttpClient();
            foreach (string mediaHeader in mediaHeaders)
            {
                MediaTypeWithQualityHeaderValue mediaType = new MediaTypeWithQualityHeaderValue(mediaHeader);
                client.DefaultRequestHeaders.Accept.Add(mediaType);
            }

            foreach (KeyValuePair<string, string> header in httpHeaders)
            {
                client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }

            HttpResponseMessage response = client.PostAsync(uri, formContent).Result;
            return response;

        }
        /// <summary>
        /// Posts a HTML form data with option to disable (ignore) chertificate validation. Exception is raised if checking is enabled and certificate is invalid.
        /// </summary>
        /// <param name="formFields">The dictionary of fields and values from a html form object</param>
        /// <param name="uri">The URI to post to (form action=)</param>
        /// <param name="ignoreCertificate">Ignore Certificate True = Yes, False = No</param>
        /// <param name="httpHeaders">Headers such as User Agent and any other header that belongs in the http header section of a request.</param>
        /// <param name="mediaHeaders">The accepted media type in response to request. Note: QualiyValue is defaulted here.</param>
        /// <returns></returns>
        public static HttpResponseMessage Post(List<KeyValuePair<string, string>> formFields, Uri uri, bool ignoreCertificate, Dictionary<string, string> httpHeaders, List<string> mediaHeaders)
        {
            var formContent = new FormUrlEncodedContent(formFields);
            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return ignoreCertificate; };

            HttpClient client = new HttpClient(httpClientHandler);

            foreach(string mediaHeader in mediaHeaders)
            {
                MediaTypeWithQualityHeaderValue mediaType = new MediaTypeWithQualityHeaderValue(mediaHeader);
                client.DefaultRequestHeaders.Accept.Add(mediaType);
            }

            foreach (KeyValuePair<string, string> header in httpHeaders)
            {
                client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }

            HttpResponseMessage response = client.PostAsync(uri, formContent).Result;
            return response;
        }

    }
}
