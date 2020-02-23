using System;
using System.Collections;
using System.Collections.Generic;
namespace Calculator
{
    class Program
    {
        public static Expression mainExpression;
        static void Main(string[] args)
        {
            String input;
            Console.WriteLine("Welcome to the calculator application.\nPlease enter an expression. q to quit. help for format guide.");
            input = Console.In.ReadLine();
            while (input != "q")
            {
                if (input == "help")
                {
                    Console.WriteLine("EXP\n-> (EXP)\n-> SINGLEOP EXP\n-> EXP DOUBLEOP EXP\n-> VALUE\nSINGLEOP\n-> -, sqrt\n"
                        + "DOUBLEOP\n-> +, -, *, /, ^\nVALUE\n-> float");
                }
                else
                {
                    mainExpression = parseExpression(input);
                    Console.WriteLine(mainExpression.ToString() + " = " + mainExpression.execute() +
                        "\nPlease enter an expression. q to quit. help for format guide.");
                }
                input = Console.In.ReadLine();
            }
        }

        static IList<Expression> exp = new List<Expression>();
        public static Expression parseExpression(String arg)
        {
            // get number of open brackets
            Stack brackets = new Stack();
            for (int i = 0; i < arg.Length; i++)
            {
                if (arg[i] == '(')
                    brackets.Push(i);
                else if (arg[i] == ')')
                {
                    int cur = (int)brackets.Pop();
                    if (brackets.Count == 0)
                    {
                        exp.Add(new BracketExp(parseExpression(arg.Substring(cur + 1, i - (cur + 1)))));

                        if (cur == 0 && i == arg.Length - 1)
                            arg = "exp" + (exp.Count - 1);
                        else if (cur == 0)
                            arg = "exp" + (exp.Count - 1) + arg.Substring(i + 1, arg.Length - (i + 1));
                        else if (i == arg.Length - 1)
                            arg = arg.Substring(0, cur) + "exp" + (exp.Count - 1);
                        else
                            arg = arg.Substring(0, cur) + "exp" + (exp.Count - 1) + arg.Substring(i + 1, arg.Length - (i + 1));

                        i = cur + 2 + (exp.Count + "").Length;
                    }
                }
            }
            {
                int i;
                if ((i = arg.IndexOf('+')) > 0)
                    return new ExpOpExp(new Add(), parseExpression(arg.Substring(0, i)), parseExpression(arg.Substring(i + 1, arg.Length - (i + 1))));
                if ((i = arg.IndexOf('-')) > 0)
                    return new ExpOpExp(new Subtract(), parseExpression(arg.Substring(0, i)), parseExpression(arg.Substring(i + 1, arg.Length - (i + 1))));
                if ((i = arg.IndexOf('*')) > 0)
                    return new ExpOpExp(new Multiply(), parseExpression(arg.Substring(0, i)), parseExpression(arg.Substring(i + 1, arg.Length - (i + 1))));
                if ((i = arg.IndexOf('/')) > 0)
                    return new ExpOpExp(new Divide(), parseExpression(arg.Substring(0, i)), parseExpression(arg.Substring(i + 1, arg.Length - (i + 1))));
                if (arg[0] == '-')
                    return new OpExp(new Negate(), parseExpression(arg.Substring(1, arg.Length - 1)));
                if (arg.Length > 4 && arg.Substring(0, 4) == "sqrt")
                    return new OpExp(new SquareRoot(), parseExpression(arg.Substring(4, arg.Length - 4)));
                if ((i = arg.IndexOf('^')) > 0)
                    return new ExpOpExp(new Exponent(), parseExpression(arg.Substring(0, i)), parseExpression(arg.Substring(i + 1, arg.Length - (i + 1))));
            }
            if (arg.Contains("exp"))
            {
                return exp[int.Parse(arg.Substring(3, arg.Length - 3))];
            }
            return new Value(float.Parse(arg));
        }
    }

    public abstract class Expression
    {
        public abstract float execute();
    }
    public class Value : Expression
    {
        float val;
        public Value(float val)
        {
            this.val = val;
        }
        public override String ToString()
        {
            return val.ToString();
        }
        public override float execute()
        {
            return val;
        }

    }
    public class OpExp : Expression
    {
        private Operation op;
        private Expression exp;
        private float value = 0;
        private bool executed = false;
        public OpExp(Operation op, Expression exp)
        {
            this.exp = exp;
            this.op = op;
        }

        public override String ToString()
        {
            if (executed)
                return "" + value;
            return op.ToString() + exp.ToString();
        }

        public override float execute()
        {
            value = op.execute(exp.execute());
            executed = true;
            return value;
        }
    }

    public class ExpOpExp : Expression
    {
        private Operation op;
        private Expression[] exp;
        private float value = 0;
        private bool executed = false;
        public ExpOpExp(Operation op, Expression exp1, Expression exp2)
        {
            this.exp = new Expression[2];
            this.exp[0] = exp1;
            this.exp[1] = exp2;
            this.op = op;
        }

        public override String ToString()
        {
            if (executed)
                return "" + value;
            return exp[0] + op.ToString() + exp[1].ToString();
        }
        public override float execute()
        {
            value = op.execute(exp[0].execute(), exp[1].execute());
            executed = true;
            return value;
        }
    }

    public class BracketExp : Expression
    {
        private Expression exp;
        private float value = 0;
        private bool executed = false;
        public BracketExp(Expression exp)
        {
            this.exp = exp;
        }

        public override String ToString()
        {
            if (executed)
                return "" + value;
            return '(' + exp.ToString() + ')';
        }

        public override float execute()
        {
            value = exp.execute();
            executed = true;
            return value;
        }
    }

    public abstract class Operation
    {
        public abstract float execute(float exp);
        public abstract float execute(float exp1, float exp2);
    }

    public abstract class SingleOperation : Operation
    {
        public override float execute(float exp1, float exp2)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class DoubleOperation : Operation
    {
        public override float execute(float exp)
        {
            throw new NotImplementedException();
        }
    }

    public class Add : DoubleOperation
    {
        public override float execute(float exp1, float exp2)
        {
            Console.WriteLine(Program.mainExpression.ToString());
            return exp1 + exp2;
        }

        public override String ToString()
        {
            return "+";
        }
    }

    public class Subtract : DoubleOperation
    {
        public override float execute(float exp1, float exp2)
        {
            Console.WriteLine(Program.mainExpression.ToString());
            return exp1 - exp2;
        }

        public override String ToString()
        {
            return "-";
        }
    }

    public class Multiply : DoubleOperation
    {
        public override float execute(float exp1, float exp2)
        {
            Console.WriteLine(Program.mainExpression.ToString());
            return exp1 * exp2;
        }

        public override String ToString()
        {
            return "*";
        }
    }
    public class Divide : DoubleOperation
    {
        public override float execute(float exp1, float exp2)
        {
            Console.WriteLine(Program.mainExpression.ToString());
            return exp1 / exp2;
        }

        public override String ToString()
        {
            return "/";
        }
    }

    public class Exponent : DoubleOperation
    {
        public override float execute(float exp1, float exp2)
        {
            Console.WriteLine(Program.mainExpression.ToString());
            return (float)Math.Pow(exp1, exp2);
        }

        public override String ToString()
        {
            return "^";
        }
    }

    public class Negate : SingleOperation
    {
        public override float execute(float exp)
        {
            Console.WriteLine(Program.mainExpression.ToString());
            return -exp;
        }

        public override String ToString()
        {
            return "-";
        }
    }

    public class SquareRoot : SingleOperation
    {
        public override float execute(float exp)
        {
            Console.WriteLine(Program.mainExpression.ToString());
            return (float)Math.Sqrt(exp);
        }

        public override String ToString()
        {
            return "sqrt";
        }
    }
}
