using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FormulaEvaluator
{/// <summary>
/// this class gives a evluator which calculates expression based on given expression and values of virables
/// </summary>
    public class FormulaEvaluator
    {
        /// <summary>
        /// a delegate defines the type of look up value function
        /// </summary>
        /// <param name="input">virable type must be string and can only assigned as integer</param>
        /// <returns></returns>
        public delegate double Lookup(string input);
        /// <summary>
        /// test if the token is number
        /// </summary>
        /// <param name="input">the token need to be tested</param>
        /// <returns>converted number or nothing</returns>
       
        public static double ?TryNum(string input)

       {
            double gauntet;
            
            if (Double.TryParse(input, out gauntet))
            {
                return gauntet;
            }
            return null;
        }
        /// <summary>
        /// evaluation function, call it and it will return an double
        /// </summary>
        /// <param name="example"></param>
        /// <param name="VirableEvaluator"></param>
        /// <returns></returns>

        public static double Calculate(string example, Lookup VirableEvaluator)
        {
            var tokens = Regex.Split(example, "(?<=[-+*/(),])(?=.)|(?<=.)(?=[-+*/(),])")
             .Select(element => Regex.Replace(element, "\\s+", ""))
             .Where(element => element != "")
             .ToArray();
            Stack<double> NumStack = new Stack<double>();
            Stack<Operator> OpStack = new Stack<Operator>();
            for (int k = 0; k < tokens.Length; k++)
            {
                var token = tokens[k];
                double? Converted = TryNum(token);
                //decisions on what the token is
                var UnknownToken = new Operator()
                {
                    Operation = token[0]
                };
                if (UnknownToken.IsValid == false && Converted.HasValue == false)
                {
                    //if the first character ins not a operator and the whole token has no value(null), it's an operator
                    Converted = VirableEvaluator(token);
                }
                if (Converted.HasValue)
                {
                    if (OpStack.Count != 0)
                    {
                        if (OpStack.Peek().IsMultiplication || OpStack.Peek().IsDivision)
                        {
                            if (NumStack.Count < 1) throw new ArgumentException("Invalid format");
                            {

                                double number = NumStack.Pop();
                                NumStack.Push(OpStack.Pop().Apply(number, Converted.Value));
                                continue;
                            }
                        }
                    }
                    NumStack.Push(Converted.Value);
                }
                else //it's a operator
                {
                    if (UnknownToken.IsAddition || UnknownToken.IsSubtraction)
                    {
                        if (OpStack.Count > 0)
                        {
                            if (OpStack.Peek().IsAddition || OpStack.Peek().IsSubtraction)
                            {
                                if (NumStack.Count < 2) throw new ArgumentException("invalid format");
                                double num1 = NumStack.Pop();
                                double num2 = NumStack.Pop();
                                NumStack.Push(OpStack.Pop().Apply(num2, num1));

                            }
                        }
                        OpStack.Push(UnknownToken);
                        continue;
                    }
                    if (UnknownToken.IsMultiplication || UnknownToken.IsDivision || UnknownToken.IsOpenBrace)
                    {
                        OpStack.Push(UnknownToken);
                    }
                    if (UnknownToken.IsClosingBrace)
                    {
                        if (OpStack.Count > 0)
                        {
                            if (OpStack.Peek().IsAddition || OpStack.Peek().IsSubtraction)
                            {
                                if (NumStack.Count < 2) throw new ArgumentException("invalid format");
                                double num1 = NumStack.Pop();
                                double num2 = NumStack.Pop();
                                NumStack.Push(OpStack.Pop().Apply(num2, num1));
                            }
                            if (OpStack.Peek().IsOpenBrace == false) throw new ArgumentException("invalid format");
                            OpStack.Pop();
                            if (OpStack.Count > 0)
                            {
                                if (OpStack.Peek().IsMultiplication || OpStack.Peek().IsDivision)
                                {
                                    if (NumStack.Count < 2) throw new ArgumentException("invalid format");
                                    double num1 = NumStack.Pop();
                                    double num2 = NumStack.Pop();
                                    NumStack.Push(OpStack.Pop().Apply(num2, num1));
                                }
                            }
                        }
                    }
                    continue;
                }
            }
                if (NumStack.Count == 1)
                {
                    return NumStack.Pop();
                }
            if (NumStack.Count == 2 || OpStack.Count == 1)
            {
                double num1 = NumStack.Pop();
                double num2 = NumStack.Pop();
                return OpStack.Pop().Apply(num2, num1);
            }
            else throw new ArgumentException("invalid format");
               
            
        }
        /// <summary>
        /// a implement of operators
        /// </summary>
        public class Operator
        {

            public char Operation { get; internal set; }

           
            public bool IsMultiplication
            {
                get
                {
                    return Operation == '*';
                }
            }
            public bool IsDivision
            {
                get
                {
                    return Operation == '/' || Operation == '÷';
                }
            }

            public bool IsAddition
            {
                get
                {
                    return Operation == '+';
                }
            }

            
            public bool IsSubtraction
            {
                get
                {
                    return Operation == '-';
                }
            }

             public bool IsOpenBrace
            {
                get
                {
                    return Operation == '(';
                }
            }
            
            public bool IsClosingBrace
            {
                get
                {
                    return Operation == ')';
                }
            }

          
            public bool IsValid
            {
                get
                {
                    return IsAddition || IsSubtraction || IsMultiplication || IsDivision || IsOpenBrace || IsClosingBrace;
                }
            }

            public bool IsOperation
            {
                get
                {
                    return IsAddition || IsSubtraction || IsMultiplication || IsDivision;
                }
            }

            /// <summary>
            /// use operator on integers
            /// </summary>
            /// <param name="a">fist integer</param>
            /// <param name="b">second integer</param>
            /// <returns>The result of the application</returns>
            public double Apply(double a, double b)
            {
                if (IsMultiplication)
                    return a * b;
                if (IsAddition)
                    return a + b;
                if (IsSubtraction)
                    return a - b;
                if (IsDivision)
                {
                    if (b == 0) throw new DivideByZeroException();
                    return a / b;
                }
                if (IsOpenBrace || IsClosingBrace)
                    throw new ArgumentException("Operator not brace");
                throw new ArgumentException("No valid operation");
            }

            /// <summary>
            /// Returns string representation of this structure.
            /// </summary>
            /// <returns>String representation of this structure.</returns>
            public override string ToString()
            {
                return string.Format("\"{0}\"", Operation);
            }




        }
    }
}
