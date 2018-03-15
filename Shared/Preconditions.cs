using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSoftware.Shared
{
    public class Preconditions
    {

        public static void CheckNotNull(object arg, string messge, params object[] format)
        {
            if (arg == null)
                throw new ArgumentNullException(String.Format(messge, format));
        }

        public static void CheckNotNull(object arg)
        {
            CheckNotNull(arg, "Argument cannot be null");
        }

        public static void CheckNotEmpty(string arg, string messge, params object[] format) {
            CheckNotNull(arg, messge, format);
            if (arg.Trim().Length == 0)
                throw new ArgumentException(String.Format(messge, format));
        }

        public static void CheckNotEmpty(string arg)
        {
            CheckNotEmpty(arg, "Argument cannot be empty");
        }

        public delegate bool Condition();
        public static void Check(Condition pred, string message, params object[] format)
        {
            if (!pred())
                throw new ArgumentException(String.Format(message,format));
        }

        public static void Check(Condition pred){
            Check(pred, "Failed to check condition");
        }

        
        

    }
}
