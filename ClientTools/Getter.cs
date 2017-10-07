using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace ClientTools
{
    public static class Getter
    {
        /// <summary>
        /// Standard HTTP GET which checks for valid certificates by default. Invalid certificate raises chertificate security error. 
        /// </summary>
        /// <param name="uri">The URI for the request</param>
        /// <param name="httpHeaders">Standard http headers</param>
        /// <param name="mediaHeaders">Content Type accepted</param>
        /// <param name="queryParameters">Query string parameters</param>
        /// <returns>HttpResponseMessage</returns>
        public static HttpResponseMessage Get(Uri uri, Dictionary<string, string> httpHeaders, List<string> mediaHeaders, List<KeyValuePair<string, string>> queryParameters = null)
        {
            HttpResponseMessage response = new HttpResponseMessage();
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

            if (queryParameters != null)
            {
                if (queryParameters.Count > 0)
                {
                    StringBuilder builder = new StringBuilder();
                    char last = uri.OriginalString[uri.OriginalString.Length - 1];
                    string uriString = string.Empty;

                    if (last == '/')
                    {
                        uriString = uri.OriginalString.TrimEnd('/');
                    }
                    else
                    {
                        uriString = uri.OriginalString;
                    }
                    builder.Append(uriString); // .net uri constructor appends "/". We don't want that for query parameters.


                    int keyPosition = 0;
                    string parameterTempate = "{0}={1}";
                    foreach (KeyValuePair<string, string> parameter in queryParameters)
                    {
                        if (keyPosition == 0)
                        {
                            parameterTempate = "?{0}={1}";
                        }
                        else
                        {
                            parameterTempate = "&{0}={1}&";
                        }
                        builder.Append(string.Format(parameterTempate, parameter.Key, parameter.Value));
                        keyPosition++;
                    }

                    uriString = builder.ToString().TrimEnd('&');
                    response = client.GetAsync(uriString).Result;
                }
            }
            else
            {
                response = client.GetAsync(uri).Result;
            }
            return response;
        }


        /// <summary>
        /// Standard HTTP GET which IGNORES certificates by default. Invalid certificate raises chertificate security error if set to check for valid.
        /// </summary>
        /// <param name="uri">The URI for the request</param>
        /// <param name="httpHeaders">Standard http headers</param>
        /// <param name="mediaHeaders">Content Type accepted</param>
        /// <param name="ignoreCertificate">Default: True = Yes ignore certificates, False = NO don't ignore invalid certificates</param>
        /// <param name="queryParameters">Query string parameters</param>
        /// <returns></returns>
        public static HttpResponseMessage Get(Uri uri, Dictionary<string, string> httpHeaders, List<string> mediaHeaders, bool ignoreCertificate, List<KeyValuePair<string, string>> queryParameters = null)
        {           

            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return ignoreCertificate; };
            HttpResponseMessage response = new HttpResponseMessage();
            var client = new HttpClient(httpClientHandler);

            foreach (string mediaHeader in mediaHeaders)
            {
                MediaTypeWithQualityHeaderValue mediaType = new MediaTypeWithQualityHeaderValue(mediaHeader);
                client.DefaultRequestHeaders.Accept.Add(mediaType);
            }

            foreach (KeyValuePair<string, string> header in httpHeaders)
            {
                client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }

            if (queryParameters != null)
            {
                if (queryParameters.Count > 0)
                {
                    StringBuilder builder = new StringBuilder();
                    char last = uri.OriginalString[uri.OriginalString.Length - 1];
                    string uriString = string.Empty;

                    if (last == '/')
                    {
                        uriString = uri.OriginalString.TrimEnd('/');
                    }
                    else
                    {
                        uriString = uri.OriginalString;
                    }
                    builder.Append(uriString); // .net uri constructor appends "/". We don't want that for query parameters.

                    int keyPosition = 0;
                    string parameterTempate = "{0}={1}";
                    foreach (KeyValuePair<string, string> parameter in queryParameters)
                    {
                        if (keyPosition == 0)
                        {
                            parameterTempate = "?{0}={1}";
                        }
                        else
                        {
                            parameterTempate = "&{0}={1}&";
                        }
                        builder.Append(string.Format(parameterTempate, parameter.Key, parameter.Value));
                        keyPosition++;
                    }

                    uriString = builder.ToString().TrimEnd('&');
                    response = client.GetAsync(uriString).Result;
                    Program.FileLogger.LogInformation(string.Format("Satus: {0} URI: {1} ", response.StatusCode, uri.ToString()));

                }
            }
            else
            {
                response = client.GetAsync(uri).Result;
                Program.FileLogger.LogInformation(string.Format("Satus: {0} URI: {1} ", response.StatusCode, uri.ToString()));
            }
            return response;
        }
    }
}

// NOTES:  Don't forget that other members are available for use 
/*
            var headers = response.Headers;
            var messages = response.RequestMessage;
            var statusBool = response.IsSuccessStatusCode;
            var statusCode = response.StatusCode;
            var version = response.Version;
     Also to read the result you have to append ReadAsync and .Result
     var content = response.Content.ReadAsStringAsync().Result;
 */
