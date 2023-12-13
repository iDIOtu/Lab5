using Lab5;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab5
{
    public enum TokenType
    {
        Parenthesis,
        Number,
        Operation
    }

    public class Token
    {
        public TokenType Type;
    }

    public class Parenthesis : Token
    {
        public char Value;
        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public class Number : Token
    {
        public float Value;

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public class Operation : Token
    {
        public char Value;
        public int Priority;

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    class Program
    {
        public static List<Token> ParseMathExpression(string mathExpression, Dictionary<char, int> priority)
        {
            mathExpression = mathExpression.Replace(" ", "").Replace(",", ".");
            List<Token> tokens = new List<Token>();

            for (int i = 0; i < mathExpression.Length; i++)
            {
                char c = mathExpression[i];
                if (Char.IsDigit(c) || c == '.')
                {
                    string numberString = c.ToString();

                    while (i + 1 < mathExpression.Length && (Char.IsDigit(mathExpression[i + 1]) || mathExpression[i + 1] == '.'))
                    {
                        numberString += mathExpression[i + 1];
                        i++;
                    }

                    tokens.Add(new Number { Value = float.Parse(numberString, CultureInfo.InvariantCulture.NumberFormat)} );
                }

                else if (c != '(' && c != ')')
                {
                    tokens.Add(new Operation { Value = c, Priority = priority[c] });
                }
                else
                {
                    tokens.Add(new Parenthesis { Value = c });
                }
            }
            return tokens;
        }

        public static List<Token> RevPolNotConverter(List<Token> tokens)
        {
            Stack<Token> operators = new Stack<Token>();
            List<Token> output = new List<Token>();

            foreach (Token token in tokens)
            {
                if (token is Number)
                {
                    output.Add(token);
                }

                else if (token is Operation)
                {
                    while (operators.Count > 0 && operators.Peek() is Operation && ((Operation)operators.Peek()).Priority >= ((Operation)token).Priority)
                    {
                        output.Add(operators.Pop());
                    }
                    operators.Push(token);
                }

                else
                {
                    if (((Parenthesis)token).Value == '(')
                    {
                        operators.Push(token);
                    }

                    else
                    {
                        while (!(operators.Peek() is Parenthesis))
                        {
                            output.Add(operators.Pop());
                        }
                        operators.Pop();
                    }
                }
            }

            while (operators.Count > 0)
            {
                output.Add(operators.Pop());
            }

            return output;
        }
        public static float CalculateRevPolNot(List<Token> tokens)
        {
            Stack<float> numbers = new Stack<float>();

            foreach (Token token in tokens)
            {
                if (token is Number)
                {
                    numbers.Push(((Number)token).Value);
                }

                else switch (((Operation)token).Value)
                    {
                        case '*':
                            numbers.Push(numbers.Pop() * numbers.Pop());
                            break;
                        case '/':
                            numbers.Push(1 / numbers.Pop()  / (1 / numbers.Pop()));
                            break;
                        case '+':
                            numbers.Push(numbers.Pop() + numbers.Pop());
                            break;
                        case '-':
                            numbers.Push(-numbers.Pop() + numbers.Pop());
                            break;
                    }
            }
            return numbers.Pop();
        }
        public static void Main()
        {
            Dictionary<char, int> priority = new Dictionary<char, int>()
            {
                { '*', 2 },
                { '/', 2 },
                { '+', 1 },
                { '-', 1 }
            };

            Console.Write("Введите математическое выражение: ");
            List<Token> tokens = RevPolNotConverter(ParseMathExpression(Console.ReadLine(), priority));
            float number = CalculateRevPolNot(tokens);

            Console.WriteLine($"Обратная Польская Запись: {string.Join(" ", tokens)}");
            Console.Write($"Ваш результат: {number}\n");
        }
    }
}