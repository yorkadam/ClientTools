using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace ClientTools
{
    public static class HtmlContentExtractor
    {

        private static bool PreviousSiblingExists(HtmlNode node, string tag)
        {
            bool siblingExists = false;
            HtmlNode siblingNode = node.PreviousSibling;

            while (siblingNode != null)
            {
                if (siblingNode.Name == tag)
                {
                    siblingExists = true;
                    break;
                }

                siblingNode = siblingNode.PreviousSibling;
            }

            return siblingExists;
        }

        public static string GetFragmentText(string html, DocumentComponents components)
        {
            // Decode any html encoded characters
            html = WebUtility.HtmlDecode(html);

            // Remove any existing new lines
            html = Regex.Replace(html, components.LineBreaks, String.Empty);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            string nodeText = GetNodeText(htmlDocument.DocumentNode, components);
            nodeText = Regex.Replace(nodeText, components.FeedLines, string.Format("{0}{1}", Environment.NewLine, Environment.NewLine));

            return nodeText;
        }

        public static string GetDocumentText(HtmlDocument htmlDocument, DocumentComponents components)
        {
            StringBuilder builder = new StringBuilder();
            HtmlNodeCollection collection = htmlDocument.DocumentNode.SelectNodes("//a[@href]");

            string nodeText = GetNodeText(htmlDocument.DocumentNode, components);
            nodeText = Regex.Replace(nodeText, components.LineBreaks, String.Empty);
            nodeText = Regex.Replace(nodeText, components.FeedLines, string.Format("{0}{1}", Environment.NewLine, Environment.NewLine));

            // remove extra white space between words
            string[] words = nodeText.Split(' ');
            foreach (string word in words)
            {
                if (!string.IsNullOrEmpty(word))
                {
                    builder.Append(word + " ");
                }
            }
            string documentText = WebUtility.HtmlDecode(builder.ToString());
            return documentText;
        }

        private static string GetNodeText(HtmlNode node, DocumentComponents components)
        {
            return GetNodeText(new HtmlNode[] { node }, components);
        }

        private static string GetNodeText(IEnumerable<HtmlNode> nodes, DocumentComponents components)
        {
            StringBuilder nodeText = new StringBuilder();

            foreach (HtmlNode node in nodes)
            {
                if (node.NodeType == HtmlNodeType.Document)
                {
                    nodeText.Append(GetNodeText(node.ChildNodes, components));
                }
                else if (node.NodeType == HtmlNodeType.Element)
                {
                    var tag = node.Name;

                    if (tag == "a")
                    {
                        var anchorHref = node.GetAttributeValue("href", string.Empty);
                        var anchorText = node.InnerText;
                        // nodeText.Append(string.Format("{0} [URL: {1}]", anchorText, anchorHref));

                        nodeText.Append(string.Format(" {0} ", anchorText));
                    }

                    else if (tag == "br")
                    {
                        nodeText.AppendLine();
                    }
                    else if (components.NonDisplayTags.Contains(tag))
                    {
                    }
                    else if (components.InlineTags.Contains(tag))
                    {
                        nodeText.Append(GetNodeText(node.ChildNodes, components));
                    }
                    else
                    {
                        if (components.ContainerTags.Contains(tag) && nodeText.Length > 0)
                        {
                            nodeText.AppendLine();
                        }

                        if (tag == "td" || tag == "th")
                        {
                            if (PreviousSiblingExists(node, tag) && !ChildTagExists(node, "table"))
                            {
                                nodeText.Append("  |  ");
                            }
                        }

                        nodeText.Append(GetNodeText(node.ChildNodes, components));

                        if ((components.ContainerTags.Contains(tag) || components.ItemTags.Contains(tag)) && nodeText.Length > 0)
                        {
                            nodeText.AppendLine();
                        }
                    }
                }
                else if (node.NodeType == HtmlNodeType.Text)
                {
                    string text = node.InnerText;
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        nodeText.Append(node.InnerText);
                    }
                }
            }
            return nodeText.ToString();
        }

        private static bool ChildTagExists(HtmlNode node, string tag)
        {
            bool childExists = false;

            foreach (HtmlNode childNode in node.ChildNodes)
            {
                if (childNode.Name == tag)
                {
                    childExists = true;
                    break;
                }
            }
            return childExists;
        }
    }
}
