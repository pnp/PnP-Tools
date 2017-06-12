using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SearchQueryTool.Helpers;
using SearchQueryTool.Model;
using ResultItem = SearchQueryTool.Model.ResultItem;

namespace SearchQueryTool
{
    /// <summary>
    /// Interaction logic for PropertiesDetail.xaml
    /// </summary>
    public partial class PropertiesDetail : Window
    {
        public ResultItem Item { get; set; }

        public PropertiesDetail(ResultItem item, string queryText)
        {
            InitializeComponent();
            Item = item;
            txtQuery.Text = string.Format("Query: {0}", queryText);
            gridProperties.ItemsSource = Item;
        }

        private void Grid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            //// Have to do this in the unusual case where the border of the cell gets selected
            //// and causes a crash 'EditItem is not allowed'
            e.Cancel = true;
        }
    }
}
