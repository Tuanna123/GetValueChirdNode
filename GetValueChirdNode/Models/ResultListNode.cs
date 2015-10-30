using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetValueChirdNode.Models
{
    public class ResultListNode
    {
        public ResultListNode()
        {
            this.Items = new List<ResultListNode>();
        }
        public string NameNode { get; set; }
        public string InerText { get; set; }
        public string InerHtml { get; set; }
        public List<ResultListNode> Items { get; set; }
    }
}
