using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace audiorec1222012
{
    
    public sealed partial class FaqPage : Page
    {
        public FaqPage()
        {
            this.InitializeComponent();
        }

        private void ErAppSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(500, 500));
        }

        
        private void ButtonClick2(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).Name == "HamburgerButton2")
            {
                AlterHamburger2();
            }

            else if ((sender as Button).Name == "HomeButton")
            {
                AlterHome();
            }
            
           
            void AlterHamburger2()
            {
                MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
            }

            
            void AlterHome()
            {
                this.Frame.Navigate(typeof(MainPage));
            }

        }
    }
}
