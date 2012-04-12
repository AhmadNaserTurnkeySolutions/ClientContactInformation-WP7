#define DEBUG_AGENT

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
using Microsoft.Phone.Shell;
using System.IO;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Notification;
using System.Diagnostics;


// Directive for the data model
using WP7LDBStorage.Model;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.Scheduler;

namespace WP7LDBStorage
{
    public partial class Settings : PhoneApplicationPage
    {
        PeriodicTask periodicTask;

        string periodicTaskName = "PeriodicAgent-BW";
        public bool agentsAreEnabled = true;

        public Settings()
        {
            InitializeComponent();
            IsolatedStorageFile directory = IsolatedStorageFile.GetUserStoreForApplication();
            if (directory.FileExists("settings.txt"))
            {
                var textFound = "";
                using (var myFileStream = new IsolatedStorageFileStream("settings.txt", FileMode.Open, IsolatedStorageFile.GetUserStoreForApplication()))
                using (var reader = new StreamReader(myFileStream))
                {
                    var text = reader.ReadToEnd();
                    textFound = text;
                }

                if (textFound == "1")
                    cbLiveTile.IsChecked = true;
                else
                    cbLiveTile.IsChecked = false;
            }
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            // Return to the main page.
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        private void cbLiveTile_Click(object sender, RoutedEventArgs e)
        {
            if (cbLiveTile.IsChecked.Value)
            {
                IsolatedStorageFile directory = IsolatedStorageFile.GetUserStoreForApplication();
                if (directory.FileExists("settings.txt"))
                {
                    using (var myFilestream = new IsolatedStorageFileStream("settings.txt", FileMode.Truncate, IsolatedStorageFile.GetUserStoreForApplication()))
                    using (var writer = new StreamWriter(myFilestream))
                    {
                        writer.Write("1");
                    }
                }

                CreateLiveTile();        
            }
            else
            {
                IsolatedStorageFile directory = IsolatedStorageFile.GetUserStoreForApplication();
                if (directory.FileExists("settings.txt"))
                {
                    using (var myFilestream = new IsolatedStorageFileStream("settings.txt", FileMode.Truncate, IsolatedStorageFile.GetUserStoreForApplication()))
                    using (var writer = new StreamWriter(myFilestream))
                    {
                        writer.Write("0");
                    }
                }

                ResetMainTile(); 
            }
        }

        private void CreateLiveTile()
        {
            var appTile = ShellTile.ActiveTiles.First();

            if (appTile != null)
            {
                var standardTile = new StandardTileData
                {
                    Title = "Contact Info",
                    //BackgroundImage = new Uri("Images/SecondaryTileFrontIcon.jpg", UriKind.Relative),
                    Count = App.ViewModel.AllClientInfoItems.Count, // any number can go here, leaving this null shows NO number  
                    BackTitle = App.ViewModel.AllClientInfoItems[App.ViewModel.AllClientInfoItems.Count-1].FirstName.ToUpper(),
                    //BackBackgroundImage = new Uri("Images/ApplicationTileIcon.jpg", UriKind.Relative),  
                    BackContent = App.ViewModel.AllClientInfoItems[App.ViewModel.AllClientInfoItems.Count-1].LastName.ToUpper()
                };

                appTile.Update(standardTile);
            }
        }

        private void ResetMainTile()
        {
            ShellTile tile = ShellTile.ActiveTiles.First();
            StandardTileData data = new StandardTileData
            {
                BackBackgroundImage = new Uri("LinkThatDoesntGoAnywhere", UriKind.Relative),
                BackContent = string.Empty,
                Count = 0,
                BackTitle = string.Empty
            };
            tile.Update(data);
        }

        private void btnCreateChannel_Click(object sender, RoutedEventArgs e)
        {
            SetupChannel();
        }

        private void SetupChannel()
        {
            HttpNotificationChannel httpChannel = null;
            string channelName = "BW-PushNotificationCenter";

            //if channel exists, retrieve existing channel
            httpChannel = HttpNotificationChannel.Find(channelName);

            if (httpChannel != null)
            {
                //If we cannot get Channel URI, then close the channel and reopen it
                if (httpChannel.ChannelUri == null)
                {
                    httpChannel.UnbindToShellToast();
                    httpChannel.Close();
                    SetupChannel();
                    return;
                }
                else
                {
                    OnChannelUriChanged(httpChannel.ChannelUri);
                }
                BindToShell(httpChannel);
            }
            else
            {
                httpChannel = new HttpNotificationChannel(channelName);
                httpChannel.ChannelUriUpdated += new EventHandler<NotificationChannelUriEventArgs>(httpChannel_ChannelUriUpdated);
                //httpChannel.ShellToastNotificationReceived += new EventHandler<NotificationEventArgs>(httpChannel_ShellToastNotificationReceived);
                httpChannel.Open();
                BindToShell(httpChannel);
            }
        }

        private void OnChannelUriChanged(Uri value)
        {
            Debug.WriteLine("URI: " + value.ToString());

            EmailComposeTask emailComposeTask = new EmailComposeTask();
            emailComposeTask.To = "";
            emailComposeTask.Body = value.ToString();
            emailComposeTask.Subject = "URL For Push Notifications On Client Contacts";
            emailComposeTask.Show();
        }

        void httpChannel_ShellToastNotificationReceived(object sender, NotificationEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                Debug.WriteLine("Toast Notification Message Received: ");
                if (e.Collection != null)
                {
                    Dictionary<string, string> collection = (Dictionary<string, string>)e.Collection;
                    
                    foreach (string elementName in collection.Keys)
                    {
                        Debug.WriteLine(string.Format("Key: {0}, Value:{1}\r\n", elementName, collection[elementName]));
                    }
                }
            });
        }

        private static void BindToShell(HttpNotificationChannel httpChannel)
        {
            try
            {
                httpChannel.BindToShellToast();
            }
            catch (Exception)
            {
            }
        }

        void httpChannel_ChannelUriUpdated(object sender, NotificationChannelUriEventArgs e)
        {
            //You get the new Uri (or maybe it's updated)
            OnChannelUriChanged(e.ChannelUri);
        }

        private void StartPeriodicAgent()
        {
            // Variable for tracking enabled status of background agents for this app.
            agentsAreEnabled = true;

            // Obtain a reference to the period task, if one exists
            periodicTask = ScheduledActionService.Find(periodicTaskName) as PeriodicTask;

            // If the task already exists and background agents are enabled for the
            // application, you must remove the task and then add it again to update 
            // the schedule
            if (periodicTask != null)
            {
                RemoveAgent(periodicTaskName);
            }

            periodicTask = new PeriodicTask(periodicTaskName);

            // The description is required for periodic agents. This is the string that the user
            // will see in the background services Settings page on the device.
            periodicTask.Description = "This is a periodic background agent for Client Contact Information";

            // Place the call to Add in a try block in case the user has disabled agents
            try
            {
                ScheduledActionService.Add(periodicTask);
                
                // If debugging is enabled, use LaunchForTest to launch the agent in one minute.
                #if(DEBUG_AGENT)
                    ScheduledActionService.LaunchForTest(periodicTaskName, TimeSpan.FromSeconds(60));
                #endif
            }
            catch (InvalidOperationException exception)
            {
                if (exception.Message.Contains("BNS Error: The action is disabled"))
                {
                    MessageBox.Show("Background agents for this application have been disabled by the user.");
                    agentsAreEnabled = false;
                }
                if (exception.Message.Contains("BNS Error: The maximum number of ScheduledActions of this type have already been added."))
                {
                    // No user action required. The system prompts the user when the hard limit of periodic tasks has been reached.
                }
                cbBackgroundAgent.IsChecked = false;
            }
            catch (SchedulerServiceException)
            {
                // No user action required.
                cbBackgroundAgent.IsChecked = false;
            }
        }

        private void RemoveAgent(string name)
        {
            try
            {
                ScheduledActionService.Remove(name);
            }
            catch (Exception)
            {
            }
        }

        bool ignoreCheckBoxEvents = false;

        private void cbBackgroundAgent_Checked(object sender, RoutedEventArgs e)
        {
            if (ignoreCheckBoxEvents)
                return;
            StartPeriodicAgent();
        }

        private void cbBackgroundAgent_Unchecked(object sender, RoutedEventArgs e)
        {
            if (ignoreCheckBoxEvents)
                return;
            RemoveAgent(periodicTaskName);
        }

        // Use this method to bind to the checkbox and keep it checked or unchecked
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            ignoreCheckBoxEvents = true;

            periodicTask = ScheduledActionService.Find(periodicTaskName) as PeriodicTask;

            if (periodicTask != null)
            {
                ContentPanel.DataContext = periodicTask;
            }

            ignoreCheckBoxEvents = false;

        }
    }
}