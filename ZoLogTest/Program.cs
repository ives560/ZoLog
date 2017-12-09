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

            log.LogDebug("this is LogDebug");

            log.LogError("this is LogError");

            log.LogInfo("this is LogInfo");

            bool IsRun = true;
            while (IsRun == true)
            {
                string str = Console.ReadLine();

                if (str == "exit")
                {
                    IsRun = false;
                }
            }

        }
    }
}
