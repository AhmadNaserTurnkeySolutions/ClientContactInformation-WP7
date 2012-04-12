#define DEBUG_AGENT
using System.Windows;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Info;
using System.Linq;
using System;

namespace PeriodicTaskAgentBW
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        private static volatile bool _classInitialized;
        int count;
        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        public ScheduledAgent()
        {
            count = 0;
            if (!_classInitialized)
            {
                _classInitialized = true;
                // Subscribe to the managed exception handler
                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    Application.Current.UnhandledException += ScheduledAgent_UnhandledException;
                });
            }
        }

        /// Code to execute on Unhandled Exceptions
        private void ScheduledAgent_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        
        /// <summary>
        /// Agent that runs a scheduled task
        /// </summary>
        /// <param name="task">
        /// The invoked task
        /// </param>
        /// <remarks>
        /// This method is called when a periodic or resource intensive task is invoked
        /// </remarks>
        protected override void OnInvoke(ScheduledTask task)
        {
            //TODO: Add code to perform your task in background
            string toastTitle = "";
            if (task is PeriodicTask)
            {
                // Execute periodic task actions here.
                toastTitle = "Update";

                ShellTile appTile = ShellTile.ActiveTiles.First();

                if (appTile != null)
                {
                    var standardTile = new StandardTileData
                    {
                        //Title = "Contact Info",
                        //BackgroundImage = new Uri("Images/SecondaryTileFrontIcon.jpg", UriKind.Relative),
                        Count = count // any number can go here, leaving this null shows NO number  
                        //BackTitle = App.ViewModel.AllClientInfoItems[App.ViewModel.AllClientInfoItems.Count - 1].FirstName.ToUpper(),
                        //BackBackgroundImage = new Uri("Images/ApplicationTileIcon.jpg", UriKind.Relative),  
                        //BackContent = App.ViewModel.AllClientInfoItems[App.ViewModel.AllClientInfoItems.Count - 1].LastName.ToUpper()
                    };

                    appTile.Update(standardTile);
                }

            }
            else
            {
                // Perform resource intensive code here
            }

            string toastMessage = string.Format("Last Update: {0}", DateTime.Now); 

            // Launch a toast to show that the agent is running.
            // The toast will not be shown if the foreground application is running.
            ShellToast toast = new ShellToast();
            toast.Title = toastTitle;
            toast.Content = toastMessage;
            toast.Show();


            // If debugging is enabled, launch the agent again in one minute.
            #if DEBUG_AGENT
                ScheduledActionService.LaunchForTest(task.Name, System.TimeSpan.FromSeconds(60));
            #endif

            NotifyComplete();
        }
    }
}