using HtmlAgilityPack;
using System;
using System.Collections.Generic;

namespace ClientTools
{
    public static class UriExtractor
    {
        public static List<Uri> GetLinks(HtmlDocument htmlDocument, Uri baseUri, enums.LinkLocation linkLocation)
        {
            List<Uri> linkList = new List<Uri>();
            var nodes = htmlDocument.DocumentNode.SelectNodes("//a[@href]");

            foreach (HtmlNode linkNode in htmlDocument.DocumentNode.SelectNodes("//a[@href]"))
            {
                string titleValue = string.Empty;
                string hrefValue = string.Empty;
                foreach (HtmlAttribute attribute in linkNode.Attributes)
                {
                    if (attribute.Name == "title")
                    {
                        titleValue = attribute.Value;
                    }
                    if (attribute.Name == "href")
                    {
                        hrefValue = attribute.Value.ToString();
                    }
                }
                if (hrefValue == "#") // ignore both invlaid links and simple in-page navigation anchors
                {
                    continue;
                }
                if (hrefValue.Contains("javascript") || hrefValue.Contains("JAVASCRIPT") || hrefValue.Contains("Javascript"))
                {
                    continue;
                }
                else
                {
                    try
                    {
                        Uri pageLink = null;
                        try
                        {
                            // If href value cannot be cast as URI then it's relative link (should be)
                            pageLink = new Uri(hrefValue);
                        }
                        catch
                        {
                            pageLink = new Uri(baseUri, hrefValue);
                        }

                        // This the location of page link must match the requested location if not ANY
                        if (GetLinkLocation(baseUri, pageLink) != linkLocation) // if doesn't match AND
                        {
                            if (linkLocation != enums.LinkLocation.Any) // it is not ANY then exclude location
                            {
                                continue;
                            }
                        }
                        AddLink(linkList, pageLink);
                    }
                    catch (Exception ex)
                    {
                        var xyz = ex;
                    }
                }
            }
            return linkList;
        }
        private static void AddLink(List<Uri> urlList, Uri linkUri)
        {
            if (!IsDomainExcluded(linkUri))
            {
                // This is a rules method that determines if we should add the link to the list
                if (urlList.Count == 0)
                {
                    urlList.Add(linkUri);
                }
                else
                {
                    if (!urlList.Contains(linkUri))
                    {
                        urlList.Add(linkUri);
                    }
                }
            }
        }

        private static enums.LinkLocation GetLinkLocation(Uri baseUri, Uri linkUri)
        {
            string rootDomain = GetBaseDomain(baseUri);
            string linkBaseDomain = GetBaseDomain(linkUri);
            if (rootDomain == linkBaseDomain)
            {
                return enums.LinkLocation.Internal;
            }
            return enums.LinkLocation.External;
        }

        private static bool IsDomainExcluded(Uri linkUri)
        {
            bool isExcluded = false;
            List<string> excludedDomains = new List<string>();
            List<string> allowedSchemes = new List<string>();

            string baseDomain = String.Empty;
            
            // This is an example only and should not be done this way in a production envrionment.
            // A proper source for this data could be a database, file, or other data store.

            allowedSchemes.Add("http");
            allowedSchemes.Add("https");

            excludedDomains.Add("google.com");
            excludedDomains.Add("facebook.com");
            excludedDomains.Add("amazon.com");
            excludedDomains.Add("twitter.com");
            excludedDomains.Add("wikipedia.com");
            excludedDomains.Add("wikipedia.org");
            excludedDomains.Add("linkedin.com");
            excludedDomains.Add("linkedin.org");
            excludedDomains.Add("youtube.com");

            baseDomain = GetBaseDomain(linkUri);

            string hscheme = linkUri.GetComponents(UriComponents.Scheme, UriFormat.SafeUnescaped);

            // Is scheme allowed?
            if (!allowedSchemes.Contains(hscheme))
            {
                isExcluded = true;
            }

            // Is domain excluded?
            if (excludedDomains.Contains(baseDomain))
            {
                isExcluded = true;
            }

            return isExcluded;
        }

        private static string GetBaseDomain(Uri linkUri)
        {
            string[] hostParts = linkUri.Host.Split('.');
            string baseDomain = string.Empty;
            // Yes it's true there are hosts with five (5) segments. Never saw 6  before, but added 6 here just in case.

            // Get parts of the domain for checking exclusions
            if (hostParts.Length == 6)
            {
                baseDomain = string.Format("{0}.{1}", hostParts[4], hostParts[5]);
            }
            if (hostParts.Length == 5)
            {
                baseDomain = string.Format("{0}.{1}", hostParts[3], hostParts[4]);
            }
            if (hostParts.Length == 4)
            {
                baseDomain = string.Format("{0}.{1}", hostParts[2], hostParts[3]);
            }
            if (hostParts.Length == 3)
            {
                baseDomain = string.Format("{0}.{1}", hostParts[1], hostParts[2]);
            }
            if (hostParts.Length == 2)
            {
                baseDomain = string.Format("{0}.{1}", hostParts[0], hostParts[1]);
            }
            if (hostParts.Length == 1)
            {
                baseDomain = linkUri.Host;
            }

            return baseDomain;
        }
    }
}
