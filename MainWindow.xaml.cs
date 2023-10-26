using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JsonWPFExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Random rand = new Random();
            try
            {
                dynamic obj = new ExpandoObject();
                obj.x = new[] { 1, 2, 3, 4, 5 };
                obj.y = new[] { 2, rand.Next(), 1, 6, 3 };
                obj.type = "scatter";
                obj.mode = "lines+points";
                obj.marker = new ExpandoObject();
                obj.marker.color = "blue";

                string jsonData = JsonConvert.SerializeObject(obj);

                // You can pass directly a json 
                webView.ExecuteScriptAsync($"createPlotlyChart({jsonData})");
            }
            catch (Exception ex)
            {
                string msg = "Could not call script: " +
                                ex.Message +
                            "\n\nPlease click the 'Load HTML Document with Script' button to load.";
                MessageBox.Show(msg);
            }

        }
    }
}
