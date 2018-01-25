using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace myLMC
{
    public partial class frmAssemblyEditor : Form 
    {
        frmMain mainForm;
        string[] lines;
        string[] instructions;
        string fileText;
        int numberOfLines;
        int hltIndex;
        int num;
        bool tab;
        bool backspace;
        bool enter;

        public frmAssemblyEditor(frmMain frm)
        {
            mainForm = frm;
            InitializeComponent();
        }

        private void frmAssemblyEditor_Load(object sender, EventArgs e)
        {
            txtAssembler.AppendText("# Enter assembler code here \r\n");
            txtAssembler.AppendText("# \r\n");
            txtAssembler.AppendText("# To add comments begin lines with # \r\n");
            txtAssembler.AppendText("# Code lines have 3 entries separated by tabs \r\n");
            txtAssembler.AppendText("# > First an optional label, \r\n");
            txtAssembler.AppendText("# > second an instruction mnemonic, and \r\n");
            txtAssembler.AppendText("# > third an address label if required. \r\n");
            txtAssembler.AppendText("# \r\n");
            txtAssembler.AppendText("# Valid mnemonics are: \r\n");
            txtAssembler.AppendText("# HLT, ADD, SUB, STA or STO, LDA, \r\n");
            txtAssembler.AppendText("# BRA, BRZ, BRP, IN or INP, OUT, DAT \r\n \r\n");

            lines = txtAssembler.Text.Split('\n');
            numberOfLines = lines.Length;
        }

        private void txtAssembler_TextChanged(object sender, EventArgs e)
        {
            lines = txtAssembler.Text.Split('\n');
            numberOfLines = lines.Length;
            txtLineNo.Clear();



            for (int i = 0; i < numberOfLines; i++)
            {
                txtLineNo.AppendText(string.Format("{0}", i.ToString()).PadLeft(2, '0') + "\r\n");
            }

         
        }

        private void txtAssembler_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!tab && !backspace && !enter)
            {
                if (!char.IsLetterOrDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
        }

        private void txtAssembler_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
            {
                tab = true;
            }
            else
            {
                tab = false;
            }

            if (e.KeyCode == Keys.Back)
            {
                backspace = true;
            }
            else
            {
                backspace = false;
            }

            if (e.KeyCode == Keys.Enter)
            {
                enter = true;
            }
            else
            {
                enter = false;
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            Stream assemblerStream;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Plain Text(*.txt)|*.txt";
            ofd.Title = "Load Text file";

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if ((assemblerStream = ofd.OpenFile()) != null)
                {
                    string strPath = ofd.FileName;
                    instructions = File.ReadAllLines(strPath, Encoding.UTF8);
                    fileText = File.ReadAllText(strPath);
                    txtAssembler.Lines = instructions;
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Plain Text(*.txt)|*.txt";
            sfd.Title = "Save Text File";

            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                File.WriteAllText(sfd.FileName, txtAssembler.Text);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void btnCompile_Click(object sender, EventArgs e)
        {
            string[] mnemonics = { "ADD", "SUB", "STO", "STA", "LDA", "BRA", "BRZ", "BRP", "IN", "INP", "OUT", "HLT", "DAT" };
            string[] mailBox = new string[(txtAssembler.Lines.Length - 11) + 1];
            string[] threeValues = new string[3];
            string[] value1 = new string[(txtAssembler.Lines.Length - 11) + 1];
            string[] value2 = new string[(txtAssembler.Lines.Length - 11) + 1];
            string[] value3 = new string[(txtAssembler.Lines.Length - 11) + 1];
            List<string> dat = new List<string>();
            string assembly = "";
            string opCode ="";
            string address="";
            string temp = "";
            int datCounter = 0;

            try
            {
                int w = 0;

                for (int x = 12; x < txtAssembler.Lines.Length; x++)
                {
                    mailBox[w] = txtAssembler.Lines[x].ToUpper().Trim();
                    hltIndex = Array.FindIndex(txtAssembler.Lines, p => p.Trim().Equals("HLT", StringComparison.CurrentCultureIgnoreCase)) - 11;
                    threeValues = mailBox[w].Split('\t');

                    if (threeValues[0] == " ")
                        threeValues[0] = "-";

                    if (threeValues[1] == " ")
                        threeValues[1] = "-";

                    if (threeValues[2] == " ")
                        threeValues[2] = "-";

                    value1[w] = threeValues[0];
                    value2[w] = threeValues[1];
                    value3[w] = threeValues[2];


                    if (w <= hltIndex)
                    {

                        if (value3[w] != temp)
                        {
                            if (!int.TryParse(value3[3], out num))
                            {
                                dat.Add(value3[w]);
                                temp = value3[w];
                                datCounter++;
                                address = (hltIndex + 1).ToString().PadLeft(2, '0');
                            }
                            else if (int.TryParse(value3[3], out num) && value3[3].Length == 2)
                            {
                                address = value3[w];
                            }

                        }

                        if (value1[w] == "")
                        {
                            value1[w] = "_";
                        }

                        if (mnemonics.Contains(value2[w]))
                        {
                            opCode = value2[w].Substring(0, 3).Trim();
                            address = value2[w].Substring(3).Trim();

                                switch (opCode)
                                {
                                    case "IN":
                                        mainForm.Controls.Find("textBox" + Convert.ToString(w + 1), true)[0].Text = "901";
                                        break;
                                    case "INP":
                                        mainForm.Controls.Find("textBox" + Convert.ToString(w + 1), true)[0].Text = "901";
                                        break;
                                    case "OUT":
                                        mainForm.Controls.Find("textBox" + Convert.ToString(w + 1), true)[0].Text = "902";
                                        break;
                                    case "HLT":
                                        mainForm.Controls.Find("textBox" + Convert.ToString(w + 1), true)[0].Text = "000";
                                        break;
                                    case "ADD":
                                        assembly = "1" + address.PadLeft(2, '0');
                                        mainForm.Controls.Find("textBox" + Convert.ToString(w + 1), true)[0].Text = assembly;
                                        break;
                                    case "SUB":
                                        assembly = "2" + address.PadLeft(2, '0');
                                        mainForm.Controls.Find("textBox" + Convert.ToString(w + 1), true)[0].Text = assembly;
                                        break;
                                    case "STO":
                                        assembly = "3" + address.PadLeft(2, '0');
                                        mainForm.Controls.Find("textBox" + Convert.ToString(w + 1), true)[0].Text = assembly;
                                        break;
                                    case "STA":
                                        assembly = "3" + address.PadLeft(2, '0');
                                        mainForm.Controls.Find("textBox" + Convert.ToString(w + 1), true)[0].Text = assembly;
                                        break;
                                    case "LDA":
                                        assembly = "5" + address.PadLeft(2, '0');
                                        mainForm.Controls.Find("textBox" + Convert.ToString(w + 1), true)[0].Text = assembly;
                                        break;
                                    case "BR":
                                        assembly = "6" + address.PadLeft(2, '0');
                                        mainForm.Controls.Find("textBox" + Convert.ToString(w + 1), true)[0].Text = assembly;
                                        break;
                                    case "BRA":
                                        assembly = "6" + address.PadLeft(2, '0');
                                        mainForm.Controls.Find("textBox" + Convert.ToString(w + 1), true)[0].Text = assembly;
                                        break;
                                    case "BRZ":
                                        assembly = "7" + address.PadLeft(2, '0');
                                        mainForm.Controls.Find("textBox" + Convert.ToString(w + 1), true)[0].Text = assembly;
                                        break;
                                    case "BRP":
                                        assembly = "8" + address.PadLeft(2, '0');
                                        mainForm.Controls.Find("textBox" + Convert.ToString(w + 1), true)[0].Text = assembly;
                                        break;
                                }
                        }
                        if (w > hltIndex)
                        { 
                            
                        }
                    }
                    w++;
                }
                    
            }
            catch (Exception ex)
            {

            }
            
        }

        
    }
    
}
