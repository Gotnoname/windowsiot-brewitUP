using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BrewLib
{
    public enum UIMessageButtons
    {
        OK,
        Cancel,
        Close,
        OKCancel,
        Yes,
        No,
        YesNo,
    }

    public enum UIMessageType
    {
        Error,
        Information,
        Question,
        Warning,
    }

    public enum UIMessageResults
    {
        OK,
        Cancel,
        Yes,
        No,
        Close,
    }

    public class UIMessager
    {
        #region Singleton
        private static UIMessager _instance;
        public static UIMessager Instance { get { return _instance ?? (_instance = new UIMessager()); } }
        #endregion

        #region Private variables
        private UIMessagerUIControl _control;
        #endregion

        #region Public functions
        public void Init(UIMessagerUIControl control)
        {
            _control = control;
        }

        public void ShowMessage(string content)
        {
            _control.ShowMessage(content);
        }

        public async void ShowMessage(string title, string content, UIMessageButtons buttons, UIMessageType messageType, int durationMilliseconds = 5000)
        {
            await _control.ShowMessage(title, content, buttons, messageType, durationMilliseconds);
        }

        public async Task<UIMessageResults> ShowMessageAndWaitForFeedback(
            string title,
            string textContent,
            UIMessageButtons buttons,
            UIMessageType messageType)
        {
            if(_control == null)
            {
                return UIMessageResults.OK;
            }
            
            return await _control.ShowMessageAndWaitForFeedback(title, textContent, buttons, messageType);
        }

        public void CancelMessage()
        {
            _control.CancelMessage();
        }
        #endregion
    }

    public class UIMessagerUIControl : UserControl, INotifyPropertyChanged
    {
        #region Private variables
        private bool _isShowing = false;
        private string _title;
        private string _textContent;
        private CancellationTokenSource _cancelToken = null;
        private readonly StringBuilder _contentBuilder = new StringBuilder();
        private UIMessageType _messageType;

        #endregion
            
        #region Properties
        public string Title
        {
            get { return _title; }
            set { _title = value; OnPropertyChanged("Title"); }
        }

        public string TextContent
        {
            get { return _textContent; }
            set { _textContent = value; OnPropertyChanged("TextContent"); }
        }

        public UIMessageType MessageType
        {
            get { return _messageType; }
            set { _messageType = value; OnPropertyChanged("MessageType");}
        }

        protected virtual UIMessageButtons Buttons { get; set; }
        protected int ShowDurationMilliseconds { get; set; }
        protected UIMessageResults ClickResult { get; set; }
        #endregion

        protected UIMessagerUIControl()
        {
            Visibility = Visibility.Collapsed;
        }

        public async Task ShowMessage(
            string title,
            string textContent,
            UIMessageButtons buttons,
            UIMessageType messageType,
            int durationMilliseconds)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                Title = title;
                _contentBuilder.AppendLine(textContent);
                TextContent = _contentBuilder.ToString();

                if (!_isShowing)
                {
                    ShowDurationMilliseconds = durationMilliseconds;
                    Buttons = buttons;

                    _isShowing = true;
                    Visibility = Visibility.Visible;

                    _cancelToken = new CancellationTokenSource();
                    var ct = _cancelToken.Token;

                    try
                    {
                        await Task.Delay(durationMilliseconds, ct);
                    }
                    catch (TaskCanceledException)
                    {
                    }

                    Visibility = Visibility.Collapsed;
                    _isShowing = false;
                    _contentBuilder.Clear();
                    Reset();
                }
            });
        }

        public async Task<UIMessageResults> ShowMessageAndWaitForFeedback(
            string title,
            string textContent,
            UIMessageButtons buttons,
            UIMessageType messageType)
        {
            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Title = title;
                TextContent = textContent;
                Buttons = buttons;
                Visibility = Visibility.Visible;
            });

            _cancelToken = new CancellationTokenSource();
            var ct = _cancelToken.Token;

            try
            {
                await Task.Delay(-1, ct);
            }
            catch (TaskCanceledException)
            {
            }

            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Visibility = Visibility.Collapsed;
                Reset();
            });

            Debug.WriteLine("ClickResult: " + ClickResult);
            return ClickResult;
        }

        public void ShowMessage(string content)
        {
            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                TextContent = content;
                Buttons = UIMessageButtons.OK;
                Visibility = Visibility.Visible;
            });
        }

        public void CancelMessage()
        {
            if (_cancelToken != null)
            {
                _cancelToken.Cancel();
                _cancelToken.Dispose();
                _cancelToken = null;
            }
            Visibility = Visibility.Collapsed;
        }

        protected virtual void Reset()
        {
            
        }

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
        #endregion
    }
}
