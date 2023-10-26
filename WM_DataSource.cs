using Microsoft.Web.WebView2.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace JsonWPFExample
{
    public class WM_DataSource : WPF_Notifier
    {
        private string _source;

        public WM_DataSource() 
        {
            // Your folder url
            Source = "C:\\Users\\lmelis\\source\\repos\\JsonWPFExample\\ProgettoJson\\index1.html";      
        }

        public string Source
        {
            get { return _source; }
            set
            {
                if (_source != value)
                {
                    _source = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
