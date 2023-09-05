using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Sentient_Editor.Utilities
{
    public enum MessageType
    {
        Info = 0x01,
        Warning = 0x02,
        Error = 0x04,
    }

    public class LogMessage 
    {
        public DateTime Time { get; }
        public MessageType MessageType { get; }
        public string Message { get; }
        public string File { get; }
        public string Caller { get; }
        public int Line { get; }
        public string MetaData => $"{File}: {Caller} ({Line})";

        public LogMessage(MessageType type, string message, string file, string caller, int line)
        {
            Time = DateTime.Now;
            MessageType = type;
            Message = message;
            File = Path.GetFileName(file);
            Caller = caller;
            Line = line;
        }
    }

    public static class Logger
    {
        private static int messageFilterMask = (int)(MessageType.Info | MessageType.Warning | MessageType.Error);
        private static readonly ObservableCollection<LogMessage> logMessages = new ObservableCollection<LogMessage>();
        public static ReadOnlyObservableCollection<LogMessage> LogMessages { get; private set; } = new ReadOnlyObservableCollection<LogMessage>(logMessages);
        public static CollectionViewSource FilteredMessages { get; } = new CollectionViewSource() { Source = LogMessages };

        public static async void Log(
            MessageType type,
            string message,
            [CallerFilePath]string file = "",
            [CallerMemberName]string caller = "",
            [CallerLineNumber]int line = 0) 
        {
            await Application.Current.Dispatcher.BeginInvoke(new Action(() => 
            {
                logMessages.Add(new LogMessage(type, message, file, caller, line));            
            }));
        }

        public static async void Clear()
        {
            await Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                logMessages.Clear();
            }));
        }

        public static void SetMessageFilter(int mask) 
        {
            messageFilterMask = mask;
            FilteredMessages.View.Refresh();
        }

        public static void SetMessageFilter(bool info, bool warning, bool error) 
        {
            var filter = 0x0;

            if (info) filter |= (int)MessageType.Info;
            if (warning) filter |= (int)MessageType.Warning;
            if (error) filter |= (int)MessageType.Error;

            SetMessageFilter(filter);
        }

        static Logger()
        {
            FilteredMessages.Filter += (sender, args) =>
            {
                var type = (int)(args.Item as LogMessage).MessageType;
                args.Accepted = (type & messageFilterMask) != 0;
            };
        }
    }
}
