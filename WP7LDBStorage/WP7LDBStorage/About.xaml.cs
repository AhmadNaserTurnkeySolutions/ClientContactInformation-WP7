using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Reflection;

namespace WP7LDBStorage
{
    public partial class About : PhoneApplicationPage
    {
        public About()
        {
            InitializeComponent();

            //Version v = Assembly.GetExecutingAssembly().GetName().Version;
            //txtVersion.Text = "Version " + v.Major + "." + v.Minor + " (Build " + v.Build + "." + v.Revision + ")";
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            // Return to the main page.
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }
    }
}