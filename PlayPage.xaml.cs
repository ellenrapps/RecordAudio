using System;
using Windows.Foundation;
using Windows.Media.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using Windows.ApplicationModel.Core;
using Windows.System.Display;


namespace audiorec1222012
{
    
    public sealed partial class PlayPage : Page
    {
        private readonly DisplayRequest displayRequest = new DisplayRequest();
        public PlayPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            ErMediaPlayer.Source = null;
        }


        private void ErAppSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(500, 500));
        }

        

        private void ButtonClick3(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).Name == "HamburgerButton3")
            {
                AlterHamburger3();
            }

            else if ((sender as Button).Name == "HomeButton2")
            {
                AlterHome2();
            }

            else if ((sender as Button).Name == "OpenAudioFileButton")
            {
                AlterAudioFileButton();                
            }

            
            void AlterHamburger3()
            {
                MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
            }

            
            void AlterHome2()
            {
                this.Frame.Navigate(typeof(MainPage));
            }

            
            async void AlterAudioFileButton()
            {
                try
                {
                    {
                        await SetLocalMedia();
                    }

                    async System.Threading.Tasks.Task SetLocalMedia()
                    {
                        var openPicker = new Windows.Storage.Pickers.FileOpenPicker();
                        openPicker.FileTypeFilter.Add(".wma");
                        openPicker.FileTypeFilter.Add(".mp3");

                        var file = await openPicker.PickSingleFileAsync();

                        if (file != null)
                        {
                            ErMediaPlayer.Source = MediaSource.CreateFromStorageFile(file);
                            ErMediaPlayer.MediaPlayer.Play();
                            displayRequest.RequestActive();
                        }


                    }
                }
                catch (Exception)
                {
                    var dialog = new MessageDialog("Something is wrong.");
                    {
                        _ = await dialog.ShowAsync();
                    }

                }
            }
            

        }
    }
}
