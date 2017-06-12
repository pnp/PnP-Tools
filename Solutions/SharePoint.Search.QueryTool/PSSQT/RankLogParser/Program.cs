using System;
using System.IO;

namespace PSSQT.RankLogParser
{
    class Program
    {
        static void Main(string[] args)
        {
            string ranklogXML = File.ReadAllText(args[0]);

            var ranklog = RankLog.CreateRankLogFromXml(ranklogXML);

            foreach (var element in ranklog.Elements)
            {
                Console.WriteLine(element);
            }

            Console.ReadKey();
        }
    }
}
