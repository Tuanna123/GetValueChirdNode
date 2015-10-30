using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GetValueChirdNode
{
    public class Utils
    {
        #region HttpClient
        public static HttpClient GetHttpClient()
        {
            HttpClient httpClient = new HttpClient(new HttpClientHandler
            {
                AllowAutoRedirect = false,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            });
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/45.0.2454.99 Safari/537.36");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate, sdch");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Language", "vi-VN,vi;q=0.8,fr-FR;q=0.6,fr;q=0.4,en-US;q=0.2,en;q=0.2");
            //  httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Language", "vi-VN,vi;q=0.8,en-US;q=0.5,en;q=0.3");
            return httpClient;
        }

        public static async System.Threading.Tasks.Task<string> DownloadString(string url, Dictionary<string, string> headers = null)
        {
            HttpClient httpClient = Utils.GetHttpClient();
            if (headers != null)
            {
                foreach (var item in headers)
                {
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation(item.Key, item.Value);
                }
            }

            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(url);
            return await httpResponseMessage.Content.ReadAsStringAsync();
        }
        #endregion
        #region GetValue enum with value string
        public static string GetEnumDescription(Enum value)
        {
            System.Reflection.FieldInfo fi = value.GetType().GetField(value.ToString());

            System.ComponentModel.DescriptionAttribute[] attributes =
                (System.ComponentModel.DescriptionAttribute[])fi.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }
        #endregion
        #region Xử lý HtmlNode
        public static HtmlNode GetOneTag(string html, string tag, string attt, string value, bool Contain = false, int index = 0)
        {
            HtmlNode node;
            node = GetListTag(html, tag, attt, value, Contain)[index];
            return node;
        }
        public static List<HtmlNode> GetListTag(string html, string tag, string attt, string value, bool Contain = false)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            List<HtmlNode> list = new List<HtmlNode>();
            if (Contain)
            {
                list = htmlDoc.DocumentNode.Descendants(tag).Where(t => t.GetAttributeValue(attt, "").Contains(value)).ToList();
            }
            else
            {
                list = htmlDoc.DocumentNode.Descendants(tag).Where(t => t.GetAttributeValue(attt, "").ToString() == value).ToList();
            }
            return list;

        }
        public static List<HtmlNode> GetListNode(HtmlNode itemNode,bool remove_text = false)
        {
            List<HtmlNode> list = new List<HtmlNode>();
            list.Add(itemNode);
            var listNode = Remove_comment(itemNode,remove_text);
            foreach (var item in listNode)
            {
                list[0].ChildNodes.Add(item);
            }
            return list;
        }
        public static List<HtmlNode> Remove_comment(HtmlNode itemNode,bool remove_text = false)
        {
            var list = itemNode.ChildNodes.ToList();
            if (remove_text)
            {
                list = list.Where(t=>t.Name != "#text").ToList();
            }
            list = list.Where(t => t.Name != "#comment").ToList();
            foreach (var item in list)
            {
                var listNode = Remove_comment(item,remove_text);
                list[list.IndexOf(item)].ChildNodes.Clear();
                foreach (var item1 in listNode)
                {
                    list[list.IndexOf(item)].ChildNodes.Add(item1);
                }
            }
            itemNode.ChildNodes.Clear();
            return list;
        }
        public static string GetValueHtml(HtmlNode node, int[] countChidNode, string attibutes, Dictionary<string, string> replace = null, string valueCatch = null)
        {
            string value = "";
            try
            {
                // for vao chirdNode
                foreach (var item in countChidNode)
                {
                    node = node.ChildNodes[item];
                }
                //End

                value = !string.IsNullOrEmpty(attibutes) ? node.Attributes[attibutes].Value.ToString() : node.InnerText.ToString();
                if (string.IsNullOrEmpty(value))
                {
                    HtmlDocument htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(node.OuterHtml);
                    value = htmlDoc.DocumentNode.InnerText;
                }
                // ReplaceValue nếu có
                if (replace != null)
                {
                    foreach (var item in replace)
                    {
                        value = value.Replace(item.Key, item.Value);
                    }
                }
            }
            catch
            {
                value = valueCatch;
            }


            //end 
            return WebUtility.HtmlDecode(value.Replace("\t", "").Replace("\n", "").Trim());
        }
        public static List<HtmlNode> GetListNodeToNode(HtmlNode node, int[] countChidNode,bool remove_text = false)
        {
            // for vao chirdNode
            foreach (var item in countChidNode)
            {
                node = node.ChildNodes[item];
            }
            //end 
                return GetListNode(node,remove_text);
        }
        #endregion
    }
}
