/*
 CptS 321 - Yongmin Joh (011535529)
 Assignment 5 - Spreadsheet
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CptS321;

namespace Spreadsheet
{
    public partial class Form1 : Form
    {
        private CptS321.Spreadsheet target;
        //int[][] curCell;
        int curCellRow, curCellCol;
        DataGridViewCellEventArgs data;


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //clear columns
            DataSheet.Columns.Clear();

            //load CptS321 namespace
            target = new CptS321.Spreadsheet(50, 26);
            target.CellPropertyChanged += new PropertyChangedEventHandler(ToCellChange);

            //creat calumns
            char x = 'A';
            for (int i = 0; i <= 25; i++)
            {
                string y = char.ToString(x);
                DataSheet.Columns.Add(y, y);
                x++;
            }
            
            //creat rows
            for (int j = 0; j < 50; j++)
            {
                int temp = j + 1;
                DataSheet.Rows.Add();
                DataSheet.Rows[j].HeaderCell.Value = temp.ToString();
            }
        }
        public void ToCellChange(object sender, PropertyChangedEventArgs e)
        {
            //updates
            for (int i = 0; i < DataSheet.RowCount; i++)
            {
                for (int j = 0; j < DataSheet.ColumnCount; j++)
                {
                    DataSheet.Rows[i].Cells[j].Value = target.getCell(i, j).getvalue;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Random ran = new Random();

            for (int i = 0; i < 50; i++)
            {   
                //get random number in 50 rows and26 columns
                int rows = ran.Next(0, 50);
                int columns = ran.Next(0, 26);

                //display "Hello world!" to the cell
                target.getCell(rows, columns).gettext = "Hello world!";
            }

            for (int i = 0; i < 50; i++)
            {
                //display "This is cell B#"
                target.getCell(i, 1).gettext = "This is cell B" + (i + 1).ToString();
            }

            for (int j = 0; j < 50; j++)
            {
                //display "=B#"
                target.getCell(j, 0).gettext = "=B" + (j + 1).ToString();
            }
        }

        private void DataSheet_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void txtCell_TextChanged(object sender, EventArgs e)
        {
            //curCellCol = data.ColumnIndex;
            //curCellRow = data.RowIndex;

            target.getCell(curCellRow, curCellCol).gettext = txtCell.Text;
        }

        private void DataSheet_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //declare the target cell address
            curCellCol = e.ColumnIndex;
            curCellRow = e.RowIndex;

            txtCell.Text = target.getCell(curCellRow, curCellCol).gettext;
        }

        private void txtCell_KeyDown2(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                target.getCell(curCellRow, curCellCol).gettext = txtCell.Text;
            }
        }

        private void DataSheet_Keydown(object sender, KeyEventArgs e)
        {
            curCellCol = data.ColumnIndex;
            curCellRow = data.RowIndex;

            if (e.KeyCode == Keys.Enter)
            {
                txtCell.Text = target.getCell(curCellRow, curCellCol).gettext;
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 about = new AboutBox1();
            about.Show();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Filter = "XML |*.xml";

            if (op.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.IO.FileStream file = (System.IO.FileStream)op.OpenFile();

                this.target.Load(file);

                file.Close();
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "XML |*.xml";
            save.ShowDialog();

            if (save.FileName != "")
            {
                System.IO.FileStream file = (System.IO.FileStream)save.OpenFile();

                this.target.Save(file);

                file.Close();
            }
        }

        private void txtCell_KeyDown(object sender, MouseEventArgs e)
        {
            
        }
    }
}
