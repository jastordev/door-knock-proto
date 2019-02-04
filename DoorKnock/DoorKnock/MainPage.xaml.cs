using Microsoft.QueryStringDotNET;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace DoorKnock
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.ViewModel = new KnockListModel();
            
        }

        public KnockListModel ViewModel { get; set; }

        // Method meant to simulate a knock received
        private void SimulateKnock(object sender, RoutedEventArgs e)
        {
            // Generate knock.
            KnockInfo receivedKnock = GenerateKnock();

            // Add to incList in ViewModel
            this.ViewModel.IncKnockList.AddKnock(receivedKnock);

            // Trigger notification
            SendNotification(receivedKnock);
        }


        private void EditName_Click(object sender, RoutedEventArgs e)
        {
            if (this.DoorName.IsReadOnly)
            {
                this.DoorName.IsReadOnly = false;
                this.EditNameSymbol.Symbol = Symbol.Save;
            }
            else
            {
                this.DoorName.IsReadOnly = true;
                this.EditNameSymbol.Symbol = Symbol.Edit;
            }
        }

        private void Reply_Knock(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            KnockInfo clickedKnock = ((clickedButton.Parent as StackPanel).Parent as Grid).DataContext as KnockInfo;

            this.SendKnock(clickedKnock.KnockGuid, clickedButton.Name);
            
        }

        public void SendKnock(Guid knockGUID, String reply)
        {
            // Get Knock in question.
            KnockInfo knock = this.ViewModel.IncKnockList.GetKnock(knockGUID);

            // Order
            // Simulate reply success (Here there would be network code sending reply back to appropriate device)
            // On failure, prompt user for a retry (if yes, retry knock reply, if no return)     

            ToastNotificationManager.History.Remove(knockGUID.ToString("N"));            

            // On success continue to change the knock info
            knock.Reply = GetSymbol(reply);

            // Remove from incKnockList and add to knockHistory
            this.ViewModel.IncKnockList.RemoveKnock(knockGUID);
            this.ViewModel.KnockHistory.AddKnock(knock);

            // DisplayTestDialog(clickedKnockInfo.FromUser + ((clickedButton.Content as Viewbox).Child as SymbolIcon).Symbol);
        }

        private async void DisplayTestDialog(String content)
        {
            ContentDialog testDialog = new ContentDialog
            {
                Title = "Test Results",
                Content = content,
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await testDialog.ShowAsync();
        }

        // Randomly generates a Knock
        private KnockInfo GenerateKnock()
        {
            Random rng = new Random();

            string[] users = { "Jimmy", "Fred", "Bob", "Harry", "James", "Frank",
                                "Louise", "Berta", "Mary", "Liz", "Sally", "Jess" };

            string[] msg = { "Hey I'm here!", "Open the door!", "Dinner is ready!",
                            "Time to leave.", "Knock knock!", "Random message" };

            KnockInfo result = new KnockInfo("DEF", users[rng.Next(11)], msg[rng.Next(6)]);

            return result;
        }

        // Send Notification
        private void SendNotification(KnockInfo knock)
        {

            String knockGUID = knock.KnockGuid.ToString("N");

            // First, the code for the visual aspects of the toast
            ToastVisual visual = new ToastVisual()
            {
                BindingGeneric = new ToastBindingGeneric()
                {
                    Children =
                    {
                        new AdaptiveText()
                        {
                            Text = knock.FromUser
                        },
                        new AdaptiveText()
                        {
                            Text = knock.Message
                        }
                    }
                }

            };

            // Second, code for the actions for the toast.
            ToastActionsCustom actions = new ToastActionsCustom()
            {
                Buttons =
                {
                    new ToastButton("No", new QueryString()
                    {
                        {"action", "Clear" },
                        {"knockGUID", knockGUID }
                    }.ToString())
                    {
                        ActivationType = ToastActivationType.Background
                    },

                    new ToastButton("Wait", new QueryString()
                    {
                        {"action", "Clock" },
                        {"knockGUID", knockGUID }
                    }.ToString())
                    {
                        ActivationType = ToastActivationType.Background
                    },

                    new ToastButton("OK", new QueryString()
                    {
                        {"action", "Accept" },
                        {"knockGUID",  knockGUID}
                    }.ToString())
                    {                        
                        ActivationType = ToastActivationType.Background
                    }
                }
            };


            // Third, construction of the toast
            ToastContent toastContent = new ToastContent()
            {
                Visual = visual,
                Actions = actions,
                Audio = new ToastAudio()
                {
                    Src = new Uri("ms-appx:///Assets/Menu1A.wav")
                }
            };

            // Create toast notification
            var toast = new ToastNotification(toastContent.GetXml());
            toast.Tag = knockGUID;            

            // Show the notification
            ToastNotificationManager.CreateToastNotifier().Show(toast);
            
        }

        // Helper method. Gets appropriate symbol for string
        private Symbol GetSymbol(String symbolName)
        {
            if (symbolName.Equals("Accept")) return Symbol.Accept;
            else if (symbolName.Equals("Clock")) return Symbol.Clock;
            else if (symbolName.Equals("Clear")) return Symbol.Clear;

            // If we reach this, something went wrong.
            return Symbol.Clear;
        }

    }

}
