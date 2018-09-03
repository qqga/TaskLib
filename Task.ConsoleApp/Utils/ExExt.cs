using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task.ConsoleApp.Utils
{

    public static class ExExt
    {
        public static string GetMessages(this Exception ex)
        {
            if(ex == null)
                return string.Empty;

            if(ex.InnerException == null)
                return ex.Message;
            else
                return $"{ex.Message} {Environment.NewLine} {GetMessages(ex.InnerException)}";
        }
    }

}
