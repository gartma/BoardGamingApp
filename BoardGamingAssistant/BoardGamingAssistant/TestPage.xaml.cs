﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace BoardGamingAssistant
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class TestPage : BoardGamingAssistant.Common.LayoutAwarePage
    {
        private Windows.Foundation.Collections.IPropertySet appSettings;
        private const String photoKey = "capturedPhoto";

        public TestPage()
        {
            this.InitializeComponent();
            LoadNumbers();
        }


        public void LoadNumbers()
        {
            List<int> list = new List<int>();
            for (int i = 0; i < 30; i++)
            {
                list.Add(i);
            }

            ListView.ItemsSource = list;
            
            
        }


        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }

        private async void ListView_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            MessageDialog msg = new MessageDialog("HI", "Title");
            UICommand okBtn = new UICommand("OK");
            okBtn.Invoked = OkBtnClick;
            msg.Commands.Add(okBtn);
            await msg.ShowAsync();

        }

        private void OkBtnClick(IUICommand command)
        {
        }

        private async void CameraClick(object sender, RoutedEventArgs e)
        {
            try
            {
                //rootPage.NotifyUser("", NotifyType.StatusMessage);

                // Using Windows.Media.Capture.CameraCaptureUI API to capture a photo
                CameraCaptureUI dialog = new CameraCaptureUI();
                Size aspectRatio = new Size(16, 9);
                dialog.PhotoSettings.CroppedAspectRatio = aspectRatio;
                var selector = "System.Devices.InterfaceClassGuid:=\"" + "{E5323777-F976-4F5B-9B55-B94699C46E44}" + "\"";
                var interfaces = await DeviceInformation.FindAllAsync(selector, null);
                //var interfaces = await DeviceInformation.FindAllAsync("{E5323777-F976-4F5B-9B55-B94699C46E44}", null);
                if (interfaces.Count > 0)
                {

                    StorageFile file = await dialog.CaptureFileAsync(CameraCaptureUIMode.Photo);
                    if (file != null)
                    {
                        BitmapImage bitmapImage = new BitmapImage();
                        using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read))
                        {
                            bitmapImage.SetSource(fileStream);
                        }
                        //CapturedPhoto.Source = bitmapImage;


                        // Store the file path in Application Data
                        appSettings[photoKey] = file.Path;
                    }
                    else
                    {
                        //rootPage.NotifyUser("No photo captured.", NotifyType.StatusMessage);
                    }
                }
                else
                {
                    MessageDialog diag = new MessageDialog("No Camera", "No Camera");
                    await diag.ShowAsync();
                }
            }
            catch (Exception ex)
            {
                //rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage);
            }
        }
    }
}
