using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

namespace SuperSoftware.Shared
{
    public interface ILogger
    {
        void Error(string message, params object[] format);
        void Warn(string message, params object[] format);
        void Info(string message, params object[] format);
        void Debug(string message, params object[] format);
        void Error(Exception ex, string message, params object[] format);
        void Warn(Exception ex, string message, params object[] format);
        void Info(Exception ex, string message, params object[] format);
        void Debug(Exception ex, string message, params object[] format);
        void Attatch(TextWriter writer, bool debug);
        void Dettatch(TextWriter writer);
        void Start();
        void Stop();
    }
    public class Logger : ILogger
    {

        private LinkedList<Tuple<TextWriter, bool>> writers = new LinkedList<Tuple<TextWriter, bool>> ();

        public Logger(TextWriter writer, bool debug)
        {
            Attatch(writer, debug);
            workerThread = new Thread(worker);
        }

        public void Attatch(TextWriter writer, bool debug)
        {
            writers.AddLast(new Tuple<TextWriter, bool>(writer, debug));
        }

        public void Debug(Exception ex, string message, params object[] format)
        {
            enqueueMessage(ex, ErrorLevel.DEBUG, String.Format(message, format));
        }

        public void Dettatch(TextWriter writer)
        {
            var tuple = from tpl in writers
                        where tpl.Item1 == writer
                        select tpl;

            if(tuple.Count() > 0)            
                writers.Remove(tuple.First());
        }

        public void Error(string message, params object[] format)
        {
            Error(null, message, format);
        }

        public void Warn(string message, params object[] format)
        {
            Warn(null, message, format);
        }

        public void Info(string message, params object[] format)
        {
            Info(null, message, format);
        }

        public void Debug(string message, params object[] format)
        {
            Debug(null, message, format);
        }

        public void Error(Exception ex, string message, params object[] format)
        {
            enqueueMessage(ex, ErrorLevel.ERROR, String.Format(message, format));
        }

        public void Warn(Exception ex, string message, params object[] format)
        {
            enqueueMessage(ex, ErrorLevel.WARN, String.Format(message, format));
        }

        public void Info(Exception ex, string message, params object[] format)
        {
            enqueueMessage(ex, ErrorLevel.INFO, String.Format(message, format));
        }

        private Queue<Tuple<ErrorLevel, string>> messages = new Queue<Tuple<ErrorLevel, string>>();

        private void enqueueMessage(Exception ex, ErrorLevel errorLevel, string message)
        {
            string finalMessage = $"{DateTime.Now.ToUniversalTime().ToString()} {errorLevel.ToString()} {message}";
            if (ex != null)
                finalMessage += Environment.NewLine + ex.StackTrace;

            lock (messages)
            {
                messages.Enqueue(new Tuple<ErrorLevel, string>(errorLevel, finalMessage));
                Monitor.Pulse(messages);
            }
        }

        private Tuple<ErrorLevel, string> dequeueMessage()
        {
            lock (messages)
            {
                for (;;)
                {
                    if (messages.Count == 0)
                        Monitor.Wait(messages);
                    else
                        return messages.Dequeue();
                }
            }
        }

        private enum ErrorLevel { DEBUG, WARN, INFO, ERROR }


        Thread workerThread;
        private void worker()
        {
            try{
                while (true)
                {
                    var msg = dequeueMessage();
                    foreach (var tuple in writers)
                        if(!(msg.Item1 == ErrorLevel.DEBUG) || tuple.Item2)
                            tuple.Item1.WriteLine(msg.Item2);
                }
            }
            catch (ThreadInterruptedException)
            {
                Console.WriteLine("Ending worker log thread");
            }
            
        }

        public void Start()
        {
            workerThread.Start();
        }

        public void Stop()
        {
            workerThread.Interrupt();
        }

        
    }
}
