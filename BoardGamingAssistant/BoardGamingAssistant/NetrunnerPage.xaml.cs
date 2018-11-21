using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace BoardGamingAssistant
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class NetrunnerPage : BoardGamingAssistant.Common.LayoutAwarePage
    {

        private Stack<Dictionary<string, string>> CorpClickHistory = new Stack<Dictionary<string, string>>(3);
        private Stack<Dictionary<string, string>> RunnerClickHistory = new Stack<Dictionary<string, string>>(4);

        public NetrunnerPage()
        {
            this.InitializeComponent();
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

        private void CreditPlusOneButton_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            string user = "";
            if (b.Name == "Runner_CreditPlusOneButton")
                user = "Runner";
            if (b.Name == "Corp_CreditPlusOneButton")
                user = "Corp";
            AddToCredits(user);
        }

        private void CreditMinusOneButton_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            string user = "";
            if (b.Name == "Runner_CreditMinusOneButton")
                user = "Runner";
            if (b.Name == "Corp_CreditMinusOneButton")
                user = "Corp";
            AddToCredits(user, -1);
        }

        private void AddToCredits(string user, int amount = 1)
        {
            if (user == "Runner")
                Runner_CreditSlider.Value += amount;
            if (user == "Corp")
                Corp_CreditSlider.Value += amount;
        }

        private void Runner_ClickHexDrawCard_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Dictionary<string, string> clickState = InsertRunnerClickStateData("draw");
            if (UpdateRunnerHistoryText(Runner_ClickDraw.Text))
            {
                RunnerClickHistory.Push(clickState);
            }
        }

        private void Runner_ClickHexCreditPlusOne_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Dictionary<string, string> clickState = InsertRunnerClickStateData("creditPlusOne");
            if (UpdateRunnerHistoryText(Runner_ClickCreditPlusOne.Text))
            {
                RunnerClickHistory.Push(clickState);
                AddToCredits("Runner");
            }
        }

        private void Runner_ClickHexInstall_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Dictionary<string, string> clickState = InsertRunnerClickStateData("install");
            if (UpdateRunnerHistoryText(Runner_ClickInstall.Text))
            {
                RunnerClickHistory.Push(clickState);
            }
        }

        private void Runner_ClickHexEvent_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Dictionary<string, string> clickState = InsertRunnerClickStateData("event");
            if (UpdateRunnerHistoryText(Runner_ClickEvent.Text))
            {
                RunnerClickHistory.Push(clickState);
            }
        }

        private void Runner_ClickHexRemoveTag_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Dictionary<string, string> clickState = InsertRunnerClickStateData("removeTag");
            if (Runner_CreditSlider.Value >= 2 && UpdateRunnerHistoryText(Runner_ClickRemoveTag.Text))
            {
                RunnerClickHistory.Push(clickState);
                AddToCredits("Runner", -2);
            }
        }

        private void Runner_ClickHexRun_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Dictionary<string, string> clickState = InsertRunnerClickStateData("run");
            if (UpdateRunnerHistoryText(Runner_ClickRun.Text))
            {
                RunnerClickHistory.Push(clickState);
            }
        }

        private void Runner_ClickHexUndo_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (RunnerClickHistory.Count != 0)
            {
                Dictionary<string, string> lastState = RunnerClickHistory.Pop();

                Runner_ClickFourHistoryText.Text = "";
                Runner_ClickThreeHistoryText.Text = lastState["Runner_ClickThreeHistory"];
                Runner_ClickTwoHistoryText.Text = lastState["Runner_ClickTwoHistory"];
                Runner_ClickOneHistoryText.Text = lastState["Runner_ClickOneHistory"];
                Runner_CreditSlider.Value = Convert.ToDouble(lastState["Runner_CreditSliderValue"]);
            }
        }

        private Dictionary<string, string> InsertRunnerClickStateData(string action)
        {
            Dictionary<string, string> clickState = new Dictionary<string, string>();

            clickState["action"] = action;
            clickState["Runner_ClickOneHistory"] = Runner_ClickOneHistoryText.Text;
            clickState["Runner_ClickTwoHistory"] = Runner_ClickTwoHistoryText.Text;
            clickState["Runner_ClickThreeHistory"] = Runner_ClickThreeHistoryText.Text;
            clickState["Runner_CreditSliderValue"] = Runner_CreditSlider.Value.ToString();

            return clickState;
        }

        private bool UpdateRunnerHistoryText(string text)
        {
            bool success = true;
            switch (RunnerClickHistory.Count)
            {
                case 0:
                    ClearCorpClickHistory();
                    Runner_ClickOneHistoryText.Text = text;
                    break;
                case 1:
                    Runner_ClickTwoHistoryText.Text = text;
                    break;
                case 2:
                    Runner_ClickThreeHistoryText.Text = text;
                    break;
                case 3:
                    Runner_ClickFourHistoryText.Text = text;
                    break;
                case 4:
                    success = false;
                    //Stack is full, let player know their finger is tired
                    break;
                default:
                    success = false;
                    break;
            }
            return success;
        }

        private void ClearCorpClickHistory()
        {
            Corp_ClickOneHistoryText.Text = "";
            Corp_ClickTwoHistoryText.Text = "";
            Corp_ClickThreeHistoryText.Text = "";
            CorpClickHistory.Clear();
        }



        private void Corp_ClickHexDrawCard_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Dictionary<string, string> clickState = InsertCorpClickStateData("draw");
            if (UpdateCorpHistoryText(CorpClickHistory.Count, Corp_ClickDraw.Text))
            {
                CorpClickHistory.Push(clickState); 
            }
        }

        private void Corp_ClickHexCreditPlusOne_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Dictionary<string, string> clickState = InsertCorpClickStateData("creditPlusOne");
            if (UpdateCorpHistoryText(CorpClickHistory.Count, Corp_ClickCreditPlusOne.Text))
            {
                CorpClickHistory.Push(clickState);
                AddToCredits("Corp");
            }
        }

        private void Corp_ClickHexInstall_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Dictionary<string, string> clickState = InsertCorpClickStateData("install");
            if (UpdateCorpHistoryText(CorpClickHistory.Count, Corp_ClickInstall.Text))
            {
                CorpClickHistory.Push(clickState); 
            }
        }

        private void Corp_ClickHexOperation_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Dictionary<string, string> clickState = InsertCorpClickStateData("operation");
            if (UpdateCorpHistoryText(CorpClickHistory.Count, Corp_ClickOperation.Text))
            {
                CorpClickHistory.Push(clickState); 
            }
        }

        private void Corp_ClickHexAdvance_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Dictionary<string, string> clickState = InsertCorpClickStateData("advance");
            if (Corp_CreditSlider.Value >= 1 && UpdateCorpHistoryText(CorpClickHistory.Count, Corp_ClickAdvance.Text))
            {
                CorpClickHistory.Push(clickState);
                AddToCredits("Corp", -1);
            }
        }

        private void Corp_ClickHexPurge_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Dictionary<string, string> clickState = InsertCorpClickStateData("purge");
            if (CorpClickHistory.Count == 0)
            {
                UpdateCorpHistoryText(0, Corp_ClickPurge.Text);
                UpdateCorpHistoryText(1, Corp_ClickPurge.Text);
                UpdateCorpHistoryText(2, Corp_ClickPurge.Text);
                CorpClickHistory.Push(clickState);
                CorpClickHistory.Push(clickState);
                CorpClickHistory.Push(clickState);
            }
        }

        private void Corp_ClickHexTrash_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Dictionary<string, string> clickState = InsertCorpClickStateData("trash");
            if (Corp_CreditSlider.Value >= 2 && UpdateCorpHistoryText(CorpClickHistory.Count, Corp_ClickTrash.Text))
            {
                CorpClickHistory.Push(clickState);
                if (Corp_CreditSlider.Value >= 2)
                {
                    AddToCredits("Corp", -2);
                }
            }
        }

        private void Corp_ClickHexUndo_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (CorpClickHistory.Count != 0)
            {
                Dictionary<string, string> lastState = CorpClickHistory.Pop();

                Corp_ClickThreeHistoryText.Text = "";
                Corp_ClickTwoHistoryText.Text = lastState["Corp_ClickTwoHistory"];
                Corp_ClickOneHistoryText.Text = lastState["Corp_ClickOneHistory"];
                Corp_CreditSlider.Value = Convert.ToDouble(lastState["Corp_CreditSliderValue"]);

                if (lastState["action"] == "purge")
                {
                    CorpClickHistory.Pop();
                    CorpClickHistory.Pop();
                }

            }
        }

        private Dictionary<string, string> InsertCorpClickStateData(string action)
        {
            Dictionary<string, string> clickState = new Dictionary<string, string>();

            clickState["action"] = action;
            clickState["Corp_ClickOneHistory"] = Corp_ClickOneHistoryText.Text;
            clickState["Corp_ClickTwoHistory"] = Corp_ClickTwoHistoryText.Text;
            clickState["Corp_CreditSliderValue"] = Corp_CreditSlider.Value.ToString();

            return clickState;
        }

        private bool UpdateCorpHistoryText(int clickCount,string text)
        {
            bool success = true;
            switch (clickCount)
            {
                case 0:
                    ClearRunnerClickHistory();
                    Corp_ClickOneHistoryText.Text = text;
                    break;
                case 1:
                    Corp_ClickTwoHistoryText.Text = text;
                    break;
                case 2:
                    Corp_ClickThreeHistoryText.Text = text;
                    break;
                case 3:
                    success = false;
                    //Stack is full, let player know their finger is tired
                    break;
                default:
                    success = false;
                    break;
            }
            return success;
        }

        private void ClearRunnerClickHistory()
        {
            Runner_ClickOneHistoryText.Text = "";
            Runner_ClickTwoHistoryText.Text = "";
            Runner_ClickThreeHistoryText.Text = "";
            Runner_ClickFourHistoryText.Text = "";
            RunnerClickHistory.Clear();
        }

    }
}
