using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GetValueChirdNode
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            txtUri.Text = "http://m.truyentranh8.com/Shingeki_no_Kyojin/";
            txtTag.Text = "div";
            txtAttri.Text = "class";
            txtValue.Text = "MangaInfo";
        }
        string html;
        List<HtmlNode> _listNode = new List<HtmlNode>();
        private async void btnGetHtml_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtUri.Text.Trim()))
                {
                    string result = await Utils.DownloadString(txtUri.Text.Trim());
                    if (!string.IsNullOrEmpty(result))
                    {
                        html = result;
                        btnXuLy.IsEnabled = true;
                    }
                }
            }
            catch
            {
                MessageBox.Show("Loi");
            }

        }
        List<Models.ResultListNode> listResultListNode;
        private void btnXyLy_Click(object sender, RoutedEventArgs e)
        {
            listResultListNode = new List<Models.ResultListNode>();
            HtmlNode node;
            var list = Utils.GetListTag(html, txtTag.Text.Trim(), txtAttri.Text.Trim(), txtValue.Text.Trim(),(bool)checContain.IsChecked);
            if ((bool)checkMotGiaTri.IsChecked)
            {
                int indexNode = int.Parse(txtIndexLayMotGiaTri.Text.Trim());
                node = list[indexNode];
            }
            else
            {
                node = list[0];
            }
            var listNode = Utils.GetListNode(node,(bool)checkRemoveText1.IsChecked);
            treeViewListNode.ItemsSource = AddListRessultNode(list);
            if (treeViewListNode.ItemsSource != null)
            {
                _listNode = list;
                btnGetValue.IsEnabled = true;
            }
        }
        private List<Models.ResultListNode> AddListRessultNode(List<HtmlNode> listNode)
        {
            List<Models.ResultListNode> list = new List<Models.ResultListNode>();
            try
            {
                foreach (var item in listNode)
                {
                    list.Add(new Models.ResultListNode { NameNode = "[" + listNode.IndexOf(item) + "] " + item.Name, InerText = item.InnerText, InerHtml = item.OuterHtml, Items = AddListRessultNode(item.ChildNodes.ToList()) });
                    AddListRessultNode(item.ChildNodes.ToList());
                }
            }
            catch
            {
            
            }
           
            return list;
        }

        private void treeViewListNode_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var item = treeViewListNode.SelectedItem as Models.ResultListNode;
            if (item == null) return;

            txblNameNode.Text = item.NameNode;
            txtInnterText.Text = item.InerText;
            txtInnterHtml.Text = item.InerHtml;
        }

        private void btnGetValue_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_listNode == null) return;
                int indexNode = int.Parse(txtIndexList.Text.Trim());
                int[] arrIndexChird;
                if (!string.IsNullOrEmpty(txtIndexChirdNode.Text.Trim()))
                {
                    var arr = txtIndexChirdNode.Text.Trim().Split(',');
                    arrIndexChird = new int[arr.Length];
                    for (int i = 0; i < arr.Length; i++)
                    {
                        arrIndexChird[i] = int.Parse(arr[i]);
                    } 
                }
                else
                {
                    arrIndexChird = new int[0];
                }
                string value = txtValueChird.Text.Trim();
                if ((bool)checkListNode.IsChecked)
                {
                    _listNode = Utils.GetListNodeToNode(_listNode[indexNode], arrIndexChird, (bool)checkRmoveText.IsChecked);
                    treeViewListNode.ItemsSource = AddListRessultNode(_listNode);
                }
                else
                {
                    string result = Utils.GetValueHtml(_listNode[indexNode], arrIndexChird, value);
                    txtContent.Text = WebUtility.HtmlDecode(result);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString()); ;
            }
            
            
        }

        private void CheckMotGiaTri_UnChecked(object sender, RoutedEventArgs e)
        {
            txtIndexLayMotGiaTri.IsEnabled = false;
            txtIndexLayMotGiaTri.Text = "";
        }

        private void CheckMotGiaTri_Checked(object sender, RoutedEventArgs e)
        {
            txtIndexLayMotGiaTri.IsEnabled = true;
        }
    }
}
