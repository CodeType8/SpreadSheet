/*
 CptS 321 - Yongmin Joh (011535529)
 Assignment 7 - Spreadsheet v3.0
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            string answer = "";
            string expression = "";
            CptS321.ExpTree tree = null;

            while (answer != "4")
            {
                //clear the console
                //Console.Clear();

                //display
                Menu(expression);
                //get input
                answer = Console.ReadLine();

                if (answer == "1")
                {
                    tree = enterExpression();
                    expression = tree.setExpression();
                }
                else if (answer == "2")
                {
                    if (tree != null)
                    {
                        setVarfromtree(tree);
                    }
                    else
                    {
                        Console.WriteLine("Tree is empty\n");
                    }
                }
                else if (answer == "3")
                {
                    Evaluate(tree);
                }
                else if (answer == "4")
                {
                    Console.WriteLine("Done");
                }
                else
                {
                    Console.WriteLine("Wrong input, ignored. \n");
                }
            }
        }

        public static void Menu(string express)
        {
            string exp = express;

            //display menu
            Console.WriteLine("Menu (current expression=\"{0}\")", exp);
            Console.WriteLine("\t 1. Enter a new expression\n\t 2. Set a variable value\n\t 3. Evaluate tree\n\t 4. Quit");
        }

        public static CptS321.ExpTree enterExpression()
        {
            string express = "";

            //display and get input
            Console.WriteLine("Enter new expression: ");
            express = Console.ReadLine();
            
            CptS321.ExpTree tree = new CptS321.ExpTree(express);

            return tree;
        }

        public static void setVarfromtree(CptS321.ExpTree tree)
        {
            string name;
            double value;

            //display and get input
            Console.WriteLine("Enter variable name: ");
            name = Console.ReadLine();

            Console.WriteLine("Enter variable value: ");
            double.TryParse(Console.ReadLine(), out value);

            Console.WriteLine("successed saved variable Name = {0}, variable Value = {1}", name, value);

            tree.setVar(name, value);
        }

        public static double Evaluate(CptS321.ExpTree tree)
        {
            double evaluate = tree.Eval();
            
            //display the evalate
            Console.WriteLine("Evaluate = {0}", evaluate);

            return evaluate;
        }
    }
}
