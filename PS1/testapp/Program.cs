using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace testapp
{   class Program
    {
        public static double LookUp(string k)
        {
            if (k == "b3") { return 5; }

            else
            {
                return 7;
            }

        }
        static void Main(string[] args)
        {
            string exp = "2*(c-1)";
            
            Console.WriteLine(FormulaEvaluator.FormulaEvaluator.Calculate(exp, LookUp));
            Console.Read();
        }
    }
}
