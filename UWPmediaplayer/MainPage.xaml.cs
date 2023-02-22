using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UWPmediaplayer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private MediaPlayer mediaplayer;
        private DispatcherTimer dispatcherTimer;
        private TimeSpan durationMF;
        private bool manipulationSlider;

        public MainPage()
        {
            this.InitializeComponent();

            playButton.IsEnabled = false;
            resumeButton.IsEnabled = false;
            pauseButton.IsEnabled = false;


            mediaplayer = new MediaPlayer();
            mediaplayerElement.SetMediaPlayer(mediaplayer);

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);

            slider.TickFrequency = 1.00;

            dispatcherTimer.Tick += DispatcherTimerTick_EventHandler;

            manipulationSlider = false;


        }

        private async void mediapicker()
        {
            var mediaPicker = new FileOpenPicker();

            mediaPicker.FileTypeFilter.Add(".mp4");

            mediaPicker.SuggestedStartLocation = PickerLocationId.VideosLibrary;

            StorageFile mediaFile = await mediaPicker.PickSingleFileAsync();

            try
            {
                mediaplayer.Source = MediaSource.CreateFromStorageFile(mediaFile);
                playButton.IsEnabled = true;
                mediaplayerElement.MediaPlayer.PlaybackSession.NaturalDurationChanged += NaturalDurationChanged_EventHandler;
            }
            catch
            {

            }
        }


        private void DispatcherTimerTick_EventHandler(object sender, object e)
        {
            slider.Value += 1;


        }

        private void NaturalDurationChanged_EventHandler(MediaPlaybackSession sender, object args)
        {
            durationMF = sender.NaturalDuration;
        }

        private void SliderValueChanged_EventHandler(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (mediaplayer.PlaybackSession.Position.Seconds > 9)
            {
                ProgressionTextbox.Text = mediaplayer.PlaybackSession.Position.Minutes.ToString() + ":" + mediaplayer.PlaybackSession.Position.Seconds.ToString();
            }
            else
            {
                ProgressionTextbox.Text = mediaplayer.PlaybackSession.Position.Minutes.ToString() + ":0" + mediaplayer.PlaybackSession.Position.Seconds.ToString();
            }


            if (manipulationSlider == true)
            {
                int sliderValue = Convert.ToInt32(e.NewValue.ToString());
                mediaplayer.PlaybackSession.Position = new TimeSpan(0, 0, sliderValue);
                 
            }
        }

        private void SliderManipulationStarting_EventHandler(object sender, ManipulationStartingRoutedEventArgs e)
        {
            dispatcherTimer.Stop();
            manipulationSlider = true;
        }

        private void SliderManipulationCompleted_EventHandler(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            dispatcherTimer.Start();
            manipulationSlider = false;
        }


        private void loadButton_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Stop();
            mediapicker();
            mediaplayer.Pause();
            maxDurationTextbox.Text = "0:00";
            ProgressionTextbox.Text = "0:00";
            slider.Value = 0;



        }

        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            pauseButton.IsEnabled = true;
            resumeButton.IsEnabled = false;
            mediaplayer.PlaybackSession.Position = new TimeSpan(0,0,0);
            mediaplayer.Play();
            dispatcherTimer.Start();
            slider.Maximum = durationMF.TotalSeconds;
            slider.Value = 0;


            if (durationMF.Seconds > 9)
            {
                maxDurationTextbox.Text = durationMF.Minutes.ToString() + ":" + durationMF.Seconds.ToString();
            }
            else
            {
                maxDurationTextbox.Text = durationMF.Minutes.ToString() + ":0" + durationMF.Seconds.ToString();
            }


        }

        private void resumeButton_Click(object sender, RoutedEventArgs e)
        {
            pauseButton.IsEnabled = true;
            resumeButton.IsEnabled = false;
            mediaplayer.Play();
            dispatcherTimer.Start();

        }

        private void pauseButton_Click(object sender, RoutedEventArgs e)
        {
            pauseButton.IsEnabled = false;
            resumeButton.IsEnabled = true;
            mediaplayer.Pause();
            dispatcherTimer.Stop();
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            CoreApplication.Exit();
        }

        
    }
}
