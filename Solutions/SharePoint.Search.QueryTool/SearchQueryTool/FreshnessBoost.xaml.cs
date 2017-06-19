using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows;
using SearchQueryTool.Helpers;

namespace SearchQueryTool
{
    /// <summary>
    ///     Interaction logic for RankDetail.xaml
    /// </summary>
    public partial class FreshnessBoost : Window
    {
        public FreshnessBoost()
        {
            InitializeComponent();
            Loaded += FreshnessBoost_Loaded;
        }

        private void FreshnessBoost_Loaded(object sender, RoutedEventArgs e)
        {
            var assembly = Assembly.GetExecutingAssembly();
            const string resourceName = "SearchQueryTool.FreshnessBoost_template.html";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                string html = reader.ReadToEnd();
                htmlControl.NavigateToString(html);
            }
        }
    }
}