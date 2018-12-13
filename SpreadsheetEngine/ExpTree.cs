/*
 CptS 321 - Yongmin Joh (011535529)
 Assignment 7 - Spreadsheet v3.0
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace CptS321
{
    public class ExpTree
    {
        Node node;
        public Dictionary<string, double> dict = new Dictionary<string, double>();
        public string expression;
        string[] var;
        char operate;
        char[] symbol = {'+', '-', '*', '/'};

        public ExpTree(string express)
        {
            expression = express;
            
            //define the var
            defineVar();

            //build the expression tree
            buildTree(expression);
        }
        public void setVar(string id, double value)
        {
            try
            {
                this.dict[id] = value;
            }
            catch (NullReferenceException) // varName not already defined
            {
                this.dict.Add(id, value);
            }
        }

        public double Eval()
        {
            return node.Eval();
        }

        public void defineVar()
        {
            var = expression.Split(symbol);

            //define what expression is
            foreach (char x in expression)
            {
                if (x == '+')
                {
                    operate = x;
                    break;
                }
                else if (x == '-')
                {
                    operate = x;
                    break;
                }
                else if (x == '*')
                {
                    operate = x;
                    break;
                }
                else if (x == '/')
                {
                    operate = x;
                    break;
                }
            }
        }

        private void buildTree(string expression)
        {
            //create tree
            //when it works
            try
            {
                Queue<string> queue = converter(expression);
                Stack<Node> curExpression = new Stack<Node>();

                //this loop builds the tree using the queue of the equation
                foreach (string x in queue)
                {
                    //if x is operator
                    if (x == "+" || x == "-" || x == "*" || x == "/")
                    {
                        operatorNode operatornode = (operatorNode)createNode(x);

                        //pop function from right to left
                        operatornode.Right = curExpression.Pop();
                        operatornode.Left = curExpression.Pop();

                        curExpression.Push(operatornode);
                    }
                    else
                    {
                        curExpression.Push(createNode(x));
                    }
                }

                //pop the top node
                node = curExpression.Pop();
            }
            //if it is not works
            catch
            {
                expression = "NULL";

                node = new numNode(0.0);
            }

        }

        public Node createNode(string symbol)
        {
            if (symbol == "+")
            {
                return new Plus();
            }
            else if(symbol == "-")
            {
                return new Minus();
            }
            else if(symbol == "*")
            {
                return new Multi();
            }
            else if(symbol == "/")
            {
                return new Divide();
            }
            else if(symbol.All(char.IsDigit))
            {
                double x = 0.0;

                Double.TryParse(symbol, out x);

                return new numNode(x);
            }
            else
            {
                if (symbol != "")
                {
                    return new varNode(ref this.dict, symbol);
                }
            }
            return null;
        }

        public Queue<string> converter(string expression)
        {
            // seperates fields in expression
            //string regex = @"[a-zA-Z][a-zA-Z0-9]";
            string regex = @"([a-zA-Z][a-zA-Z1-9]*|[\*\+-/\(\)]|\d+\.?\d*)";
            double temp;

            MatchCollection result = Regex.Matches(expression, regex);

            Queue<string> outQueue = new Queue<string>();
            Stack<string> operatorStack = new Stack<string>();
            
            //for each of collections will check the operator or other
            foreach (Match collection in result)
            {
                

                if (Double.TryParse(collection.Value, out temp))
                {
                    //if the collection is double the enqueue
                    outQueue.Enqueue(collection.Value);
                }
                else if (collection.Value == "+" || collection.Value == "-" || collection.Value == "*" || collection.Value == "/")
                {
                    while ((collection.Value == "+" || collection.Value == "-") && ((operatorStack.Peek() == "*" || operatorStack.Peek() == "/") && operatorStack.Count > 0))
                    {
                        outQueue.Enqueue(operatorStack.Pop());
                    }

                    operatorStack.Push(collection.Value);
                }
                else if (collection.Value == "(")
                {
                    //if the collection is ( then push
                    operatorStack.Push(collection.Value);
                }
                else if (collection.Value == ")")
                {
                    //if the collection is ) then 
                    while (!(operatorStack.Count == 0) && operatorStack.Peek() != "(")
                    {
                        //there was not empty
                        if (operatorStack.Count > 0)
                        {
                            //add to out queue
                            outQueue.Enqueue(operatorStack.Pop());
                        }
                        else
                        {
                            Console.WriteLine("Parenthesis do not match");
                        }
                    }

                    operatorStack.Pop();
                }
                else if (collection.Value == "+" || collection.Value == "/" || collection.Value == "-" || collection.Value == "*")
                {
                    //if the collection is operator
                    while ((collection.Value == "-" || collection.Value == "+") && (operatorStack.Count > 0 && (operatorStack.Peek() == "*" || operatorStack.Peek() == "/")))
                    {
                        //if top stack operator is higher Priority
                        outQueue.Enqueue(operatorStack.Pop());
                    }
                    operatorStack.Push(collection.Value);
                }                
                else
                {
                    outQueue.Enqueue(collection.Value);
                }
            }
            
            foreach (string symbol in operatorStack)
            {
                if (symbol != "(")
                {
                    outQueue.Enqueue(symbol);
                }
                else
                {
                    Console.WriteLine("Parenthesis do not match");
                }

            }

            return outQueue;
        }

        /*
        public void PrintStack(Stack<> stack)
        {

        }
        */
        public string setExpression()
        {
            //if expression is not empty
            if (expression != null)
                return expression;

            return null;
        }

        public void Update(string name, double value)
        {
            if (this.dict.ContainsKey(name))
            {
                dict[name] = value;
            }
            else
            {
                dict.Add(name, value);
            }
        }
    }

    //need to be a abstract Node
    abstract public class Node
    {
        public abstract string Type();
        public abstract double Eval();
    }

    //number nudes
    public class numNode : Node
    {
        double num;

        public numNode(double newNum)
        {
            this.num = newNum;
        }

        //override double and string
        public override double Eval()
        {
            return num;
        }

        public override string Type()
        {
            return "numNode";
        }
    }
    
    //var nodes
    public class varNode : Node
    {
        //use dictionary for define var node
        private Dictionary<string, double> dictionary;
        string id = "";

        public varNode(ref Dictionary<string, double> tempDict, string tempId)
        {
            //put the copy reference
            this.id = tempId;
            this.dictionary = tempDict;
        }

        //override double and string
        public override double Eval()
        {
            //check the id
            if (dictionary.ContainsKey(id))
            {
                return dictionary[id];
            }
            else
            {
                return 0.0;
            }
        }

        public override string Type()
        {
            return "varNode";
        }
    }

    //operator nodes
    public abstract class operatorNode : Node
    {
        //private Node left;
        //private Node right;
        protected Node left;
        protected Node right;

        public Node Left
        {
            set
            {
                left = value;
            }
        }

        public Node Right
        {
            set
            {
                right = value;
            }
        }

        //override string
        public override string Type()
        {
            return "operatorNode";
        }
    }

    /*
    public double setNode(Node node)
    {
        return node.
    }
    */

    //when the node is plus
    internal class Plus : operatorNode
    {
        //override double
        public override double Eval()
        {
            return this.left.Eval() + this.right.Eval();
        }
    }

    //when the node is minus  
    internal class Minus : operatorNode
    {
        //override double
        public override double Eval()
        {
            return this.left.Eval() - this.right.Eval();
        }
    }

    //when the node is multiply
    internal class Multi : operatorNode
    {
        public override double Eval()
        {
            //override double
            return this.left.Eval() * this.right.Eval();
        }
    }

    //when the node is divide
    internal class Divide : operatorNode
    {
        //override double
        public override double Eval()
        {
            return this.left.Eval() / this.right.Eval();
        }
    }
}
