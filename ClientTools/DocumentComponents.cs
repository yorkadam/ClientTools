using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace ClientTools
{
    public class DocumentComponents
    {
        public string LineBreaks { get; set; }
        public string FeedLines { get; set; }
        public string UserAgentHeader { get; set; }
        public HashSet<string> InlineTags { get; set; }
        public HashSet<string> ContainerTags { get; set; }
        public HashSet<string> NonDisplayTags { get; set; }
        public HashSet<string> ItemTags { get; set; }


        public DocumentComponents(IConfiguration configuration)
        {
            LineBreaks = configuration.GetSection("ApplicationSettings:Breaks").Value;
            FeedLines = configuration.GetSection("ApplicationSettings:FeedLines").Value;
            UserAgentHeader = configuration.GetSection("ApplicationSettings:UserAgentHeader").Value;
            InlineTags = new HashSet<string>(configuration.GetSection("ApplicationSettings:InlineTags").Value.Split(","));
            ContainerTags = new HashSet<string>(configuration.GetSection("ApplicationSettings:ContainerTags").Value.Split(","));
            NonDisplayTags = new HashSet<string>(configuration.GetSection("ApplicationSettings:NonDisplayTags").Value.Split(","));
            ItemTags = new HashSet<string>(configuration.GetSection("ApplicationSettings:ItemTags").Value.Split(","));
        }
    }
}
