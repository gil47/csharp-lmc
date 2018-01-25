using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace myLMC
{
    public partial class frmInstructionSet : Form
    {
        public frmInstructionSet()
        {
            InitializeComponent();
        }

        private void frmInstructionSet_Load(object sender, EventArgs e)
        {
            dgvInstructionSet.DataSource = createInstructionSet();
            dgvInstructionSet.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvInstructionSet.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            for(int i= 0; i < dgvInstructionSet.Columns.Count; i++)
            {
                dgvInstructionSet.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            dgvInstructionSet.Show();
        }

        private DataTable createInstructionSet()
        {
            DataTable tbl = new DataTable();
            tbl.Columns.Add("#", typeof(string));
            tbl.Columns.Add("Instruction Set",typeof(string));
            tbl.Columns.Add("Mnemonic",typeof(string));
            tbl.Columns.Add("Machine Code",typeof(string));
            tbl.Columns.Add("Explanation",typeof(string));

            tbl.Rows.Add(" 1", "  Add", "  ADD","  1xx", "  Add the contents of the given mailbox to onto the accumulator.");
            tbl.Rows.Add(" 2", "  Subtract", "  SUB", "  2xx", "  Subtract the contents of the given mailbox from the accumulator (calculator).");
            tbl.Rows.Add(" 3", "  Store", "  STA", "  3xx", "  Store the contents of the accumulator (calculator) to the mailbox of the given address.");
            tbl.Rows.Add(" 4", "  Load", "  LDA", "  5xx", "  Load the contents of the given mailbox onto the accumulator.");
            tbl.Rows.Add(" 5", "  Branch Always", "  BRA","  6xx", "  Set the contents of the accumulator (calculator) to the given address.");
            tbl.Rows.Add(" 6", "  Branch if Zero", "  BRZ", "  7xx", "  If the contents of the accumulator (calculator) are 000, the PC (program counter) will be set to the given address.");
            tbl.Rows.Add(" 7", "  Branch if Positive", "  BRP", "  8xx", "  If the contents of the accumulator (calculator) are 000 or positive (i.e. the negative flag is not set), ");
            tbl.Rows.Add(" ", "", "", "", "  the PC (program counter) will be set to hte given address.");
            tbl.Rows.Add(" 8", "  Input", "  IN", "  901", "  Copy the value from the \" in box \" onto the accumulator (calculator) ");
            tbl.Rows.Add(" 9", "  Output", "  OUT", "  902", "  Copy the value from the accumulator(calculator) to the \" out box \".");
            tbl.Rows.Add(" 10", "  End", "  HLT", "  000", "  Causes the Little Man Computer to stop executing your program.");
            tbl.Rows.Add(" 11", "  Data Storage", "  DAT", "", "  When completed, a program converts each instruction into a three-digit code. These codes are placed in the sequential mailboxes.");
            tbl.Rows.Add("", "", "", "", "  Instead of a program component, this instruction will reserve the next mailbox for data storage.");
            tbl.Rows.Add("", "", "", "", "  *xx refers to a mailbox number");

            return tbl;
        }

 

    }
}
