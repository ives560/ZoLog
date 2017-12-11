using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZoLog
{
    class Program
    {
        static void Main(string[] args)
        {
            ZoLog log = new ZoLog(new Configuration());

            ZoLog.LogDebug("this is LogDebug");

            ZoLog.LogError("this is LogError");

            ZoLog.LogInfo("this is LogInfo");

            bool IsRun = true;
            while (IsRun == true)
            {
                string str = Console.ReadLine();

                if (str == "exit")
                {
                    IsRun = false;
                }
                else if(str=="runlog")
                {
                    log = new ZoLog(new Configuration());
                }
                else if(str=="exitlog")
                {
                    log.Dispose();
                }
            }

        }
    }
}
