using System.Collections.Generic;
using System.Text;

namespace PSSQT
{
    public static class RankLogHelper
    {
        public static List<double> ParseDoubleList(string input, int numNodes)
        {
            var list = new List<double>(numNodes);
            int n = 1;

            foreach (var item in input.Trim().Split(new char[] { ' ', '\t' }))
            {
                if (n++ <= numNodes)
                {
                    double num = double.Parse(item);
                    list.Add(num);
                }
                else
                {
                    break;
                }
            }

            return list;
        }

        public static List<double> ParseDoubleList(string input)
        {
            var list = new List<double>();

            foreach (var item in input.Trim().Split(new char[] { ' ', '\t' }))
            {
                double num = double.Parse(item);
                list.Add(num);
            }

            return list;
        }

        public static List<int> ParseIntList(string input)
        {
            var list = new List<int>();

            foreach (var item in input.Trim().Split(new char[] { ' ', '\t' }))
            {
                int num = int.Parse(item);
                list.Add(num);
            }

            return list;
        }

        public static void Append(StringBuilder sb, string label, string value, int indent = 0, bool prependNewline = true)
        {
            if (prependNewline)
            {
                sb.Append("\n");
            }

            for (int i = 0; i < indent; i++)
            {
                sb.Append(".");
            }

            sb.Append(label);
            sb.Append(": ");
            sb.Append(value);

        }

    }
}
