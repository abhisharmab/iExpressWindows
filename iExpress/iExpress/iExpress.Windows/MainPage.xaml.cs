
using Parse;
using iExpress.Common;
using System;
using Windows.Storage;
using Windows.UI.ApplicationSettings;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;
using System.Diagnostics;
using TETCSharpClient;
using TETCSharpClient.Data;
using Windows.UI.Popups;
using Windows.UI.Core;
using Windows.ApplicationModel.Core;
using Windows.System.Threading;
using Windows.Networking.PushNotifications;


// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237
//Testing push
namespace iExpress
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MainPage : Page, IGazeListener, IConnectionStateListener, ITrackerStateListener
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private Boolean entered;
        private Boolean exited;
        private int counter;
        private int internal_counter = 36;
        private int running_counter;
        private String UserName;
        private List<ButtonHandler> buttons = null;

        // This will be used when the eye tribe is disconnected
        private ThreadPoolTimer PeriodicTimer = null;
        private int timer_duration = 10000;

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        public MainPage()
        {





            GazeManager.Instance.Activate(GazeManager.ApiVersion.VERSION_1_0, GazeManager.ClientMode.Push);
            GazeManager.Instance.AddGazeListener(this);
            // Add listener if EyeTribe Server is closed
            GazeManager.Instance.AddConnectionStateListener(this);
            GazeManager.Instance.AddTrackerStateListener(this);

            if (!GazeManager.Instance.IsActivated)
            {

                Debug.WriteLine("IsActivated not ");
           //     errorMessage("Eye Tribe Is Not Active.");

            }


            this.InitializeComponent();

            buttons = new List<ButtonHandler>();
            buttons.Add(new ButtonHandler(this.b1));
            buttons.Add(new ButtonHandler(this.b2));
            buttons.Add(new ButtonHandler(this.b3));
            buttons.Add(new ButtonHandler(this.b4));
            buttons.Add(new ButtonHandler(this.b5));
            buttons.Add(new ButtonHandler(this.b6));

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;

            ParsePush.ToastNotificationReceived += updateNotification;

            // initial setup
            updateNotification(null, null);                 
        }



        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session. The state will be null the first time a page is visited.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion




        private void mouseEntered(object sender, PointerRoutedEventArgs e)
        {



            Debug.WriteLine(sender.GetHashCode() + "Detected the entering of the button");


            entered = true;
            exited = false;
            //counter = 6;
            //Abhi - Testing if CountDown Testing Works 
            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("CountDown"))
            {
                counter = (int)ApplicationData.Current.RoamingSettings.Values["CountDown"];
                counter++;
            }
            else
            {
                counter = 6;
            }

            running_counter = 0;
        }



        private void mouseExited(object sender, PointerRoutedEventArgs e)
        {
            Debug.WriteLine(sender.GetHashCode() + "Detected the exiting of the button");
            exited = true;
            entered = false;

            (sender as Windows.UI.Xaml.Controls.Button).Background = null;
        }

        private void mousedMoved(object sender, PointerRoutedEventArgs e)
        {
            if (entered == true && exited == false)
            {
                running_counter++;
                if (running_counter == internal_counter)
                {
                    running_counter = 0;
                    counter--;
                    String location = "ms-appx:///Assets/" + counter + ".png";
                    (sender as Windows.UI.Xaml.Controls.Button).Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(location)) };


                }

                if (counter == 1)
                {

                    if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("UserName"))
                        UserName = (String)ApplicationData.Current.RoamingSettings.Values["UserName"];
                    else
                        UserName = "Patient";

                    Debug.WriteLine("Trigger execution!!!!!!!!");
                    (sender as Windows.UI.Xaml.Controls.Button).Background = new ImageBrush { ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Sent.png")) };
                    Windows.UI.Xaml.Controls.Button but = (sender as Windows.UI.Xaml.Controls.Button);
                    String message = but.Content.ToString();

                    ParsePush push = new ParsePush();
                    push.Channels = new List<String> { "global" };
                    IDictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("sound", ".");
                    dic.Add("alert", message);
                   // dic.Add("time", UserName);
                    push.Data = dic;
                    push.SendAsync();


                    ParseObject internal_tweets = new ParseObject("TweetsInternal");
                    internal_tweets["content"] = message;
                    internal_tweets["sender"] = UserName;
                    internal_tweets.SaveAsync();

                    entered = false;
                    exited = true;

                }
            }

        }




        public void OnGazeUpdate(GazeData gazeData)
        {
            // start or stop tracking lost animation
            if ((gazeData.State & GazeData.STATE_TRACKING_GAZE) == 0 &&
                (gazeData.State & GazeData.STATE_TRACKING_PRESENCE) == 0) return;
            var x = (int)Math.Round(gazeData.SmoothedCoordinates.X, 0);
            var y = (int)Math.Round(gazeData.SmoothedCoordinates.Y, 0);
            //var gX = Smooth ? gazeData.SmoothedCoordinates.X : gazeData.RawCoordinates.X;
            //var gY = Smooth ? gazeData.SmoothedCoordinates.Y : gazeData.RawCoordinates.Y;
            //var screenX = (int)Math.Round(x + gX, 0);
            //var screenY = (int)Math.Round(y + gY, 0);
            // Debug.WriteLine("OnGazeUpdate       " + x + "    " + y);

            // return in case of 0,0 
            if (x == 0 && y == 0) return;

            determine_Button(x, y);

        }

        private void determine_Button(int x, int y)
        {
            if (buttons != null)
            {
                for (int i = 0; i < buttons.Count(); i++)
                {
                    buttons.ElementAt(i).entered(x, y);
                }
            }
        }

        private async void errorMessage(String value)
        {
            try
            {
                MessageDialog msgDialog = new MessageDialog(value, "Error");
                await msgDialog.ShowAsync();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }


        // server connection check
        public void OnConnectionStateChanged(bool isConnected)
        {
            if (!GazeManager.Instance.IsActivated)
            {
                //GazeManager.Instance.Deactivate();
                Debug.WriteLine("Deactivate");

                errorMessage("Gaze have been disconnected.");
            }
        }


        public void OnScreenStatesChanged(int screenIndex, int screenResolutionWidth, int screenResolutionHeight, float screenPhysicalWidth, float screenPhysicalHeight)
        {
            // do not need this
        }

        public void OnTrackerStateChanged(GazeManager.TrackerState trackerState)
        {
            switch (trackerState)
            {
                case GazeManager.TrackerState.TRACKER_CONNECTED:
                    Debug.WriteLine("TRACKER_CONNECTED");
                    sysMessage("Eye Tracker has been connected.");
                    if (PeriodicTimer != null)
                    {
                        PeriodicTimer.Cancel();

                        PeriodicTimer = null;
                    }
                    break;
                case GazeManager.TrackerState.TRACKER_CONNECTED_NOUSB3:
                    Debug.WriteLine("TRACKER_CONNECTED_NOUSB3");
                    break;
                case GazeManager.TrackerState.TRACKER_CONNECTED_BADFW:
                    Debug.WriteLine("TRACKER_CONNECTED_BADFW");
                    break;
                case GazeManager.TrackerState.TRACKER_NOT_CONNECTED:
                    Debug.WriteLine("TRACKER_NOT_CONNECTED");
                    sysMessage("Eye Tracker has been disconnected.");
                    PeriodicTimer = ThreadPoolTimer.CreatePeriodicTimer(TimerElapsedHandler,
                                        TimeSpan.FromMilliseconds(timer_duration));
                    break;
                case GazeManager.TrackerState.TRACKER_CONNECTED_NOSTREAM:
                    Debug.WriteLine("TRACKER_CONNECTED_NOSTREAM");
                    break;
            }
        }

        private void TimerElapsedHandler(ThreadPoolTimer timer)
        {
            Debug.WriteLine("TimerElapsedHandler");
            sysMessage("Eye Tracker has been disconnected.");
        }

        private async void sysMessage(String msg)
        {

            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("UserName"))
                UserName = (String)ApplicationData.Current.RoamingSettings.Values["UserName"];
            else
                UserName = "Patient";

            String message = msg;
            ParsePush push = new ParsePush();
            push.Channels = new List<String> { "testing" };
            IDictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("sound", ".");
            dic.Add("alert", message);
            push.Data = dic;
            await push.SendAsync();


            ParseObject internal_tweets = new ParseObject("TweetsInternal");
            internal_tweets["content"] = message;
            internal_tweets["sender"] = UserName;
            await internal_tweets.SaveAsync();
        }

        private async void updateNotification(object sender, PushNotificationReceivedEventArgs e)
        {
            Debug.WriteLine("Debug :: This is from the ToastNotificationReceived    ");

            var query = from comment in ParseObject.GetQuery("TweetsInternal")
                                    .Limit(2)
                        orderby comment.CreatedAt descending
                        select comment;

            IEnumerable<ParseObject> comments = await query.FindAsync();


            bool top = true;
            string forNotification1 ="";
            string forNotification2 ="";

            foreach (ParseObject p in comments)
            {
                if (top)
                {
                    string format = "hh:mm tt";
                    var dt = p.CreatedAt;
                    forNotification1 = dt.Value.ToLocalTime().ToString(format) + " " + p.Get<string>("sender") + " " + p.Get<string>("content");
                    top = !top;
                }
                else
                {
                    string format = "hh:mm tt";
                    var dt = p.CreatedAt;
                    forNotification2 = dt.Value.ToLocalTime().ToString(format) + " " + p.Get<string>("sender") + " " + p.Get<string>("content");
                }
            }
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                this.notification2.Text = forNotification2;
                this.notification1.Text = forNotification1;
            });
                    
               
            
        }


    
    }



}
