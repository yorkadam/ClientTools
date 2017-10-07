using System;
using System.Collections.Generic;
using System.Text;

namespace ClientTools
{
    public class SqlQuery
    {
        public string Statement { get; set; }
        public Dictionary<string, string> Parameters { get; set; }

        public SqlQuery()
        {
            this.Parameters = new Dictionary<string, string>();
            this.Statement = String.Empty;
        }
    }
}
