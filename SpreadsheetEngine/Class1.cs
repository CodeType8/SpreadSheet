/*
 CptS 321 - Yongmin Joh (011535529)
 Assignment 5 - Spreadsheet
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Xml;

namespace CptS321
{
    abstract public class Cell : INotifyPropertyChanged
    {
        //private readonly int ColumnIndex;
        //private readonly int RowIndex;
        public int ColumnIndex;
        public int RowIndex;
        protected string Text;
        protected string Value;

        public event PropertyChangedEventHandler PropertyChanged;

        public Cell(int x, int y)
        {
            //constructor
            RowIndex = x;
            ColumnIndex = y;
            Text = "";
            Value = "";
        }
        
        public int getRowIndex
        {
            //get index of rows with setter and setter
            get
            {
                return RowIndex;
            }
            internal set
            {
                RowIndex = value;
            }
        }

        public int getColumnIndex
        {
            //get index of columns with setter and setter
            get
            {
                return ColumnIndex;
            }
            internal set
            {
                ColumnIndex = value;
            }
        }

        public string gettext
        {
            //get text with setter and setter
            get
            {
                return Text;
            }
            set
            {
                //if the value is not same as text, then change the text to value
                if (value != Text)
                {
                    Text = value;

                    ToPropertyChanged("Text");
                }
                else
                {
                    Console.Write("There is no change\n");
                }
            }
        }

        public virtual string getvalue
        {
            //get value with setter and setter
            get
            {
                return Value;
            }
            internal set
            {
                //empty
            }
        }

        protected void ToPropertyChanged(string target)
        {
            //creat PropertyChangedEventHandler
            PropertyChangedEventHandler ChangedEventHandler = PropertyChanged;

            if (ChangedEventHandler != null)
            {
                //check the ChangedEventHandler is not empty
                ChangedEventHandler(this, new PropertyChangedEventArgs(target));
            }
        }
    }

    public class SpreadsheetCell : Cell
    {
        internal CptS321.ExpTree tree;

        public SpreadsheetCell(int x, int y) : base(x, y)
        {
            //empty
        }
        public void ToDependantCellUpdate(object sender, EventArgs e)
        {
            this.ToPropertyChanged("Value");
        }

        public override string getvalue
        {
            //override getvalue with setter and setter
            get
            {
                return Value;
            }
            internal set
            {
                //check the target value is not same as value
                if (Value != value)
                {
                    Value = value;

                    ToPropertyChanged("getvalue");
                }
            }
        }
    }

    public class Spreadsheet
    {
        protected int numRows, numColumns;
        protected SpreadsheetCell[,] cells;
        public event PropertyChangedEventHandler CellPropertyChanged;

        public Spreadsheet(int rows, int columns)
        {
            //constructor
            numRows = rows;
            numColumns = columns;

            cells = new SpreadsheetCell[numRows, numColumns];

            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numColumns; j++)
                {
                    cells[i, j] = (SpreadsheetCell) new SpreadsheetCell(i, j);
                    
                    //ToCellPropertyChanged will appear when propertyChangedcell changed
                    cells[i, j].PropertyChanged += new PropertyChangedEventHandler(ToCellPropertyChanged);
                }
            }
        }

        public int RowCount
        {
            //rowcount
            get { return numRows; }
        }

        public int ColCount
        {
            //columns count
            get { return numColumns; }
        }

        protected void ToCellPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = CellPropertyChanged;
            SpreadsheetCell cell = (SpreadsheetCell)sender;

            if (cell.gettext[0] == '=')
            {
                /*
                int len = cell.gettext.Length;
                int col = (char)cell.gettext[1] - 65;
                int row = (int)cell.gettext[2] - 49;
                if (len > 3)
                {
                    int row2 = (int)cell.gettext[3] - 48;
                    int newNumber = int.Parse(row.ToString() + row2.ToString());
                    cell.getvalue = getCell(newNumber + 9, col).getvalue;
                }
                else
                {
                    cell.getvalue = getCell((int)row, col).getvalue;
                }
                */
                try
                {
                    cell.tree = new ExpTree(cell.gettext);

                    string regex = @"([A-Z]+)(\d+)"; // matchees the form A1, B2 etc.
                    MatchCollection matches = Regex.Matches(cell.gettext, regex);

                    int row = 0, column = 0;
                    foreach (Match match in matches)
                    {
                        GroupCollection groups = match.Groups;
                        int len = groups[1].ToString().Length;
                        foreach (char c in groups[1].ToString())
                        {
                            column += (int)(Math.Pow((double)26, (double)(len - 1)) * (double)((int)(c) - 64)); // correction 
                        }
                        column -= 1; // correct for array indexing

                        Int32.TryParse(groups[2].ToString(), out row);
                        row--;

                        if (row >= this.numRows || column >= this.numColumns) // checks for out of bounds error
                        {
                            throw new RefException("Invalid Cell Reference");
                        }
                        else
                        {
                            Cell other = cells[row, column]; //this cell is the cell that is being reference
                            other.PropertyChanged += cell.ToDependantCellUpdate; // have this cell subscribe to when its dependent cell changes
                            double otherVal;

                            try //this updates the cells that are dependent on a cell
                            {
                                Double.TryParse(other.getvalue, out otherVal);
                                cell.tree.Update(match.ToString(), otherVal); //update tree dictionary
                            }
                            catch
                            {
                                throw new RefException("Invalid Cell Reference");
                            }
                        }
                    }
                    cell.getvalue = cell.tree.Eval().ToString(); //evaluate the tree
                }
                catch (RefException)
                {
                    cell.getvalue = "#REF!";
                }

            }
            else
            {
                cell.getvalue = cell.gettext;
            }

            if (handler != null)
            {
                handler(this, e);
            }
        }

        public Cell getCell(int rows, int columns)
        {
            if (rows > numRows || columns > numColumns)
            {
                return null;
            }
            else
            {
                return cells[rows, columns];
            }
        }

        /*
        public string RefException(string x)
        {
            return x;
        }
        */

        public void Load(System.IO.FileStream file)
        {
            XmlReaderSettings xmlSet = new XmlReaderSettings();
            XmlReader xmlRead = XmlReader.Create(file);
            cells = new SpreadsheetCell[numRows, numColumns];

            //create new spreadsheet
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numColumns; j++)
                {
                    cells[i, j] = (SpreadsheetCell)new SpreadsheetCell(i, j);
                    cells[i, j].PropertyChanged += ToCellPropertyChanged;
                }
            }
            
            while (xmlRead.Read())
            {
                if (xmlRead.IsStartElement())
                {
                    //int row, column;

                    switch (xmlRead.Name)
                    {
                        case "Cell":
                            int row, column;
                            string val = "";

                            bool rowCheck = Int32.TryParse(xmlRead["Row"], out row);

                            bool colomnCheck = Int32.TryParse(xmlRead["Column"], out column);

                            if (xmlRead.Read())
                            {
                                val = xmlRead.Value.Trim();
                            }

                            //if (rowCheck)
                            //{                            
                                if (rowCheck && colomnCheck)
                                {
                                    cells[row, column].gettext = val;
                                }
                                break;
                            //}
                    }
                }
                /*
                else
                {
                    MessageBox.Show();
                }
                */
            }
        }

        public void Save(System.IO.FileStream file)
        {
            XmlWriterSettings xmlSet = new XmlWriterSettings();
            XmlWriter xmlWrite = XmlWriter.Create(file);

            xmlWrite.WriteStartDocument();

            xmlWrite.WriteStartElement("Spreadsheet");

            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numColumns; j++)
                {
                    if (cells[i, j].gettext != "")
                    {
                        xmlWrite.WriteStartElement("Cell");

                        xmlWrite.WriteAttributeString("Row", i.ToString());
                        xmlWrite.WriteAttributeString("Column", j.ToString());
                        xmlWrite.WriteString(cells[i, j].gettext);

                        xmlWrite.WriteEndElement();
                    }
                }
            }

            xmlWrite.WriteEndElement();
            xmlWrite.WriteEndDocument();

            xmlWrite.Close();
        }
    }
}

public class RefException : Exception
{
    public RefException(string message) : base(message) { }
}

