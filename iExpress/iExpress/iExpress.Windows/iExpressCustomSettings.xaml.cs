using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Storage;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using iExpress;

// The Settings Flyout item template is documented at http://go.microsoft.com/fwlink/?LinkId=273769

namespace iExpress
{
    public sealed partial class iExpressCustomSettings : SettingsFlyout
    {
        public iExpressCustomSettings()
        {
            this.InitializeComponent();

            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("CountDown"))
            {
                int counter = (int)ApplicationData.Current.RoamingSettings.Values["CountDown"];
                switch (counter)
                {
                    case 5:
                        Five.IsChecked = true;
                        Four.IsChecked = false;
                        Three.IsChecked = false;
                        Two.IsChecked = false;
                        break;

                    case 4:
                        Four.IsChecked = true;
                        Five.IsChecked = false;
                        Three.IsChecked = false;
                        Two.IsChecked = false;
                        break;

                    case 3:
                        Three.IsChecked = true;
                        Four.IsChecked = false;
                        Five.IsChecked = false;
                        Two.IsChecked = false;
                        break;

                    case 2:
                        Two.IsChecked = true;
                        Five.IsChecked = false;
                        Four.IsChecked = false;
                        Three.IsChecked = false;
                        break;
                }
            }
            else
            {
                Five.IsChecked = true;
                Four.IsChecked = false;
                Three.IsChecked = false;
                Two.IsChecked = false;
            }


            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("UserName"))
            {
                Username.Text = (String)ApplicationData.Current.RoamingSettings.Values["UserName"];
            }
        }

        private void Set_Click(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.RoamingSettings.Values["UserName"] = Username.Text;
        }

        private void Five_Checked(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.RoamingSettings.Values["CountDown"] = 5;
        }

        private void Four_Checked(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.RoamingSettings.Values["CountDown"] = 4;
        }

        private void Three_Checked(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.RoamingSettings.Values["CountDown"] = 3;
        }

        private void Two_Checked(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.RoamingSettings.Values["CountDown"] = 2;
        }


    }
}
