using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.UI.Popups;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Foundation;
using Windows.ApplicationModel.Core;
using Windows.System.Display;


namespace audiorec1222012
{
    
    public sealed partial class MainPage : Page
    {
        public enum ErStates
        {
            Initializing,
            Recording,
            Pause,
            Resume,
            Stopped,
            Play,
        }


        private MediaCapture erMediaCapture;
        private ErStates erRecordingState;
        private readonly DisplayRequest displayRequest = new DisplayRequest();
        public string fileName;
        private StorageFile erStorageFile;
        private readonly string erVideoFileName = "RecordAudio.mp3";
        private DispatcherTimer erTimer;
        private TimeSpan erElapsedTime;

        public MainPage()
        {
            this.InitializeComponent();
        }

        

        private void ErAppSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(500, 500));
        }

        
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            await InitMediaCapture();
            UpdateRecordingControls(ErStates.Initializing);
            InitTimer();
        }

        private async Task InitMediaCapture()
        {
            try
            {
                erMediaCapture = new MediaCapture();
                var settings = new Windows.Media.Capture.MediaCaptureInitializationSettings
                {
                    StreamingCaptureMode = Windows.Media.Capture.StreamingCaptureMode.Audio
                };
                await erMediaCapture.InitializeAsync(settings);
                erMediaCapture.Failed += MediaCaptureOnFailed;
                erMediaCapture.RecordLimitationExceeded += MediaCaptureOnRecordLimitationExceeded;
            }

            catch (Exception)
            {
                var dialog = new MessageDialog("Something is wrong.");
                {
                    _ = await dialog.ShowAsync();
                }
            }
        }

        
        public void InitTimer()
        {
            erTimer = new DispatcherTimer();
            erTimer.Interval = new TimeSpan(0, 0, 0, 1);
            erTimer.Tick += ErTimerOnTick;
        }

        private void ErTimerOnTick(object sender, object e)
        {
            erElapsedTime = erElapsedTime.Add(erTimer.Interval);
            Duration.DataContext = erElapsedTime;
        }

       
        private async void UpdateRecordingControls(ErStates recordingState)
        {
            erRecordingState = recordingState;
            string statusMessage = string.Empty;

            switch (recordingState)
            {
                case ErStates.Initializing:
                    RecordButton.IsEnabled = true;
                    StopButton.IsEnabled = false;
                    PauseButton.IsEnabled = false;
                    ResumeButton.IsEnabled = false;
                    PlayAudioPageButton.IsEnabled = true;
                    break;

                case ErStates.Recording:
                    RecordButton.IsEnabled = false;
                    PauseButton.IsEnabled = true;
                    ResumeButton.IsEnabled = false;
                    StopButton.IsEnabled = true;
                    PlayAudioPageButton.IsEnabled = false;
                    statusMessage = "Status: Recording";
                    break;

                case ErStates.Pause:
                    PauseButton.IsEnabled = false;
                    ResumeButton.IsEnabled = true;
                    RecordButton.IsEnabled = false;
                    StopButton.IsEnabled = true;
                    PlayAudioPageButton.IsEnabled = false;
                    statusMessage = "Status: Paused";
                    break;

                case ErStates.Resume:
                    ResumeButton.IsEnabled = false;
                    PauseButton.IsEnabled = true;
                    RecordButton.IsEnabled = false;
                    StopButton.IsEnabled = true;
                    PlayAudioPageButton.IsEnabled = false;
                    statusMessage = "Status: Recording Resumed";
                    break;

                case ErStates.Stopped:
                    StopButton.IsEnabled = false;
                    PauseButton.IsEnabled = false;
                    ResumeButton.IsEnabled = false;
                    RecordButton.IsEnabled = false;
                    PlayAudioPageButton.IsEnabled = true;
                    statusMessage = "Status: Stopped and Saved";
                    break;

                case ErStates.Play:
                    StopButton.IsEnabled = false;
                    PauseButton.IsEnabled = false;
                    ResumeButton.IsEnabled = false;
                    RecordButton.IsEnabled = false;
                    break;

                default:
                    throw new ArgumentOutOfRangeException("recordingMode");
            }
            await UpdateStatus(statusMessage);
        }

        private async Task UpdateStatus(string status)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            Status.Text = status);
        }

        private async void MediaCaptureOnRecordLimitationExceeded(MediaCapture sender)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                await sender.StopRecordAsync();
                var warningMessage = new MessageDialog("Maximum recording time limit has been reached.", "Recording Stoppped.");
                await warningMessage.ShowAsync();
            });
        }

        private async void MediaCaptureOnFailed(MediaCapture sender, MediaCaptureFailedEventArgs errorEventArgs)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                var warningMessage = new MessageDialog(String.Format("The audio capture failed: {0}", errorEventArgs.Message), "Audio Capture Failure.");
                await warningMessage.ShowAsync();
            });
        }

        
        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).Name == "HamburgerButton1")
            {
                AlterHamburger();
            }

            else if ((sender as Button).Name == "FaqButton")
            {
                AlterFaq();
            }

            else if ((sender as Button).Name == "RecordButton")
            {
                AlterRecord();
            }

            else if ((sender as Button).Name == "PauseButton")
            {
                AlterPause();
            }

            else if ((sender as Button).Name == "ResumeButton")
            {
                AlterResume();
            }

            else if ((sender as Button).Name == "StopButton")
            {
                AlterStop();
            }

            else if ((sender as Button).Name == "PlayAudioPageButton")
            {
                AlterPlayAudioPage();
            }

            else if ((sender as Button).Name == "RefreshButton")
            {
                AlterRefresh();
            }

        }

        
        private void AlterHamburger()
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        
        private void AlterFaq()
        {
            this.Frame.Navigate(typeof(FaqPage));
        }

        
        private async void AlterRecord()
        {
            try
            {
                RecordButton.IsEnabled = false;
                String fileName;
                fileName = erVideoFileName;
                erStorageFile = await Windows.Storage.KnownFolders.MusicLibrary.CreateFileAsync(fileName,
                Windows.Storage.CreationCollisionOption.GenerateUniqueName);
                MediaEncodingProfile recordProfile = null;
                recordProfile = MediaEncodingProfile.CreateMp3(Windows.Media.MediaProperties.AudioEncodingQuality.High);
                await erMediaCapture.StartRecordToStorageFileAsync(recordProfile, erStorageFile);
                displayRequest.RequestActive();
                UpdateRecordingControls(ErStates.Recording);
                erTimer.Start();

            }

            catch (Exception)
            {
                var dialog = new MessageDialog("Something is wrong.");
                {
                    _ = await dialog.ShowAsync();
                }

            }

        }

        
        private async void AlterPause()
        {
            try
            {
                await erMediaCapture.PauseRecordAsync(Windows.Media.Devices.MediaCapturePauseBehavior.ReleaseHardwareResources);
                UpdateRecordingControls(ErStates.Pause);
                erTimer.Stop();
            }

            catch (Exception)
            {
                var dialog = new MessageDialog("Something is wrong.");
                {
                    _ = await dialog.ShowAsync();
                }

            }


        }

       
        private async void AlterResume()
        {
            try
            {
                await erMediaCapture.ResumeRecordAsync();
                UpdateRecordingControls(ErStates.Resume);
                erTimer.Start();
            }

            catch (Exception)
            {
                var dialog = new MessageDialog("Something is wrong.");
                {
                    _ = await dialog.ShowAsync();
                }

            }
        }

        
        private async void AlterStop()
        {
            try
            {
                await erMediaCapture.StopRecordAsync();
                UpdateRecordingControls(ErStates.Stopped);
                erTimer.Stop();
            }

            catch (Exception)
            {
                var dialog = new MessageDialog("Something is wrong.");
                {
                    _ = await dialog.ShowAsync();
                }

            }


        }

        
        private void AlterPlayAudioPage()
        {
            this.Frame.Navigate(typeof(PlayPage));
        }
        
        
        private async void AlterRefresh()
        {
            try
            {
                var result = await CoreApplication.RequestRestartAsync("Application Restart Programmatically ");

                if (result == AppRestartFailureReason.NotInForeground ||
                    result == AppRestartFailureReason.RestartPending ||
                    result == AppRestartFailureReason.Other)
                {
                    var msgBox = new MessageDialog("Refresh Failure", result.ToString());
                    await msgBox.ShowAsync();
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
