using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Microsoft.VisualBasic;
using System.IO;
using System.Text.RegularExpressions;
using Transitions;

namespace myLMC
{
    public partial class frmMain : Form
    {
        public List<TextBox> mailboxes = new List<TextBox>();
        private static readonly object padlock = new object(); //SINGLETON PATTERN
        Thread mainThread, fetchExecuteThread;
        int ctr = 0;
        int opCode;
        int address;
        int haltIndex = 0;
        int negNum;
        bool negative;
        bool backspace;
        bool fetchExecute;
        bool suspended;
        bool running;
        bool loadingFile;

        public frmMain()
        {
            InitializeComponent();
        }

        //INITIALIZE
        private void frmMain_Load(object sender, EventArgs e)
        {
            for (int i = 1; i <= 100; i++)
            {
                mailboxes.Add((TextBox)this.Controls.Find("textBox" + Convert.ToString(i), true)[0]); // LAHAT NG TEXTBOX SA LIST
                mailboxes[i - 1].KeyDown += mailBoxes_KeyDown;
                mailboxes[i - 1].KeyPress += mailBoxes_KeyPress;
                mailboxes[i - 1].LostFocus += mailBoxes_LostFocus;
                mailboxes[i - 1].BackColor = Color.White;
                mailboxes[i - 1].MaxLength = 3;
                mailboxes[i - 1].TabIndex = i - 1;
            }

            location("@startup");
            btnRun.Enabled = true;
            btnFetchExecute.Enabled = true;
            btnStop.Enabled = false;
            btnResetAll.Enabled = false;
            fetchExecute = false;
            suspended = false;
            loadingFile = false;
            lblCalculator.TextChanged += new EventHandler(lblCalculator_TextChanged);
            lblMAR.TextChanged += new EventHandler(lblMAR_TextChanged);
            lblMDR.TextChanged += new EventHandler(lblMDR_TextChanged);
            lstConsole.Items.Add("Welcome to my own simple version of Little Man Computer.");
            lstConsole.Items.Add(" ");
        }

        //MAIN LOOP
        private void processThread()
        {
            for (int i = 0; i < 100; i++)
            {
                if (mailboxes[i].Text == "000")
                {
                    haltIndex = i + 2;
                    break;
                }
            }

            while (ctr < haltIndex)
            {
                Control.CheckForIllegalCrossThreadCalls = false;      // Para shortcut, hindi na mag-invoke sa lahat ng controls

                //STARTS AT COUNTER
                location("@counter");
                lblOperation.Text = "Fetch";
                lblOperation.Refresh();

                if (suspended)
                {
                    lstConsole.Items.Add("--------------------------------------------------------");
                    lstConsole.Items.Add("...Fetch and execute instruction in mailbox " + format2(ctr.ToString()) + "... ");
                    lstConsole.Items.Add("--------------------------------------------------------");
                    lstConsole.Items.Add("");
                    suspended = false;
                }
                else
                    Thread.Sleep(500);


                lstConsole.Items.Add("Program Counter is at " + format2(ctr.ToString()));
                lstConsole.Items.Add("Incrementing program counter ");
                txtCounter.Text = format2((ctr + 1).ToString());
                txtCounter.Refresh();
                lblMAR.Text = format2(ctr.ToString());
                lblMAR.Refresh();

                //FETCH TO MAILBOXES
                location("@mailboxes");

                lblOperation.Text = "Fetch";
                lblOperation.Refresh();
                textHighlight(mailboxes[int.Parse(lblMAR.Text)]);
                lblMDR.Text = format3(mailboxes[int.Parse(lblMAR.Text)].Text);  // <--- DITO UNG INSTRUCTION CODE ex. 5xx
                lblMDR.Refresh();
                lstConsole.Items.Add("Reading mailbox " + lblMAR.Text);
                opCode = int.Parse(lblMDR.Text.Substring(0, 1));    // <--- DITO UNG OPCODE ex. 5
                address = int.Parse(lblMDR.Text.Substring(1, 2));   // <--- DITO UNG ADDRESS ex. xx
                lstConsole.Items.Add("Instruction: " + lblMDR.Text + " opCode: " + opCode + " Address: " + format2(address.ToString()));

                operationCodes(opCode);

                if (!suspended)
                    Thread.Sleep(500);

                lstConsole.Items.Add(" ");
                ctr++;                      // <--- INCREMENT NG PROGRAM COUNTER (+1)
                lstConsole.SelectedIndex = lstConsole.Items.Count - 1;
                lstConsole.SelectedIndex = -1;
                lblOperation.Text = "done";
                lblOperation.Refresh();
            }

        }

        //OPERATIONS
        private void operationCodes(int code)
        {
            Thread.Sleep(500);

            switch (opCode)
            {
                case 0:
                    halt();
                    break;
                case 1:
                    add();
                    break;
                case 2:
                    subtract();
                    break;
                case 3:
                    store();
                    break;
                case 5:
                    load();
                    break;
                case 6:
                    branch();
                    break;
                case 7:
                    branchzero();
                    break;
                case 8:
                    branchpositive();
                    break;
                case 9:
                    if (address == 01) { input(); }
                    else if (address == 02) { output(); }
                    break;
            }
        }

        //HALT :: 000
        private void halt()
        {
            location("@mailboxes");
            lstConsole.Items.Add("Halting");
            lblOperation.Text = "HLT";
            MessageBox.Show("LMC Program has ended.");
            lstConsole.SelectedIndex = lstConsole.Items.Count - 1;

            if (fetchExecute == false)
            {
                mainThread.Abort();
            }

            //btnStop_Click(this,new EventArgs());
        }

        //ADDITION :: 1XX
        private void add()
        {
            lstConsole.Items.Add("Adding from address " + format2(address.ToString()));
            lblOperation.Text = "ADD";
            lblOperation.Refresh();
            lblMAR.Text = format2(address.ToString());
            lblMAR.Refresh();
            location("@mailboxes");
            textHighlight(mailboxes[address]);
            lblMDR.Text = mailboxes[address].Text;
            lblMDR.Refresh();
            lstConsole.Items.Add("Adding " + lblMDR.Text + " to calculator");
            location("@calculator");
            lblCalculator.Text = format3((int.Parse(lblCalculator.Text) + int.Parse(lblMDR.Text)).ToString());
            lblCalculator.Refresh();
            lstConsole.Items.Add("Calculator: " + lblCalculator.Text);
        }

        //SUBTRACTION :: 2XX
        private void subtract()
        {

            lstConsole.Items.Add("Subtracting from address " + format2(address.ToString()));
            lblOperation.Text = "SUB";
            lblMAR.Text = format2(address.ToString());
            lblMAR.Refresh();
            location("@mailboxes");
            textHighlight(mailboxes[address]);
            lblMDR.Text = mailboxes[address].Text;
            lblMDR.Refresh();
            lstConsole.Items.Add("Subtracting " + lblMDR.Text + " from calculator");

            if (int.Parse(lblMDR.Text) > int.Parse(lblCalculator.Text))
            {
                negative = true;
            }
            else
            {
                negative = false;
            }

            location("@calculator");

            if (negative)
            {
                negNum = (((int.Parse(lblCalculator.Text) - int.Parse(lblMDR.Text)) + 1000) * -1);
                lblCalculator.Text = negNum.ToString();
                lblCalculator.Refresh();
                lstConsole.Items.Add("Calculator: " + int.Parse(lblCalculator.Text) * -1);
            }
            else
            {
                lblCalculator.Text = format3((int.Parse(lblCalculator.Text) - int.Parse(lblMDR.Text)).ToString());
                lblCalculator.Refresh();
                lstConsole.Items.Add("Calculator: " + lblCalculator.Text);
            }
        }

        //STORE :: 3XX
        private void store()
        {
            lblMAR.Text = format2(address.ToString());
            lblMAR.Refresh();
            location("@calculator");
            lblOperation.Text = "Fetch";
            lblMDR.Text = lblCalculator.Text;
            lblMDR.Refresh();
            location("@mailboxes");
            lblOperation.Text = "STO";
            mailboxes[address].Text = lblMDR.Text;
            textHighlight(mailboxes[address]);
            lstConsole.Items.Add("Storing calculator value " + lblMDR.Text + " in address " + lblMAR.Text);
            lblOperation.Refresh();
        }

        //LOAD :: 5XX
        private void load()
        {
            lstConsole.Items.Add("Loading calculator with data from address from " + format2(address.ToString()));
            lblMAR.Text = format2(address.ToString());
            lblMAR.Refresh();
            location("@mailboxes");
            lblOperation.Text = "LDA";
            lblOperation.Refresh();
            textHighlight(mailboxes[address]);
            lblMDR.Text = format3(mailboxes[address].Text);
            lblMDR.Refresh();
            location("@calculator");
            lblCalculator.Text = format3(lblMDR.Text);
            lblCalculator.Refresh();
            lstConsole.Items.Add("Calculator: " + lblCalculator.Text);
        }

        //BRANCH ALWAYS :: 6XX
        private void branch()
        {
            lblMAR.Text = format2(address.ToString());
            lblMAR.Refresh();
            textHighlight(mailboxes[address]);
            lblMDR.Text = format2(mailboxes[address].Text);
            lblMDR.Refresh();
            lstConsole.Items.Add("Branching to " + lblMAR.Text);
            ctr = int.Parse(lblMAR.Text) - 1;
        }

        //BRANCH IF ZERO :: 7XX
        private void branchzero()
        {
            lstConsole.Items.Add("Branch on zero.");

            if (lblCalculator.Text == "000")
            {

                lblMAR.Text = format2(address.ToString());
                lblMAR.Refresh();
                textHighlight(mailboxes[address]);
                lblMDR.Text = format2(mailboxes[address].Text);
                lblMDR.Refresh();
                lstConsole.Items.Add("Calculator: " + lblCalculator.Text);
                lstConsole.Items.Add("Branching to " + lblMAR.Text);
                ctr = int.Parse(lblMAR.Text) - 1;
            }
            else if (lblCalculator.Text != "000")
            {
                lstConsole.Items.Add("Calculator: " + lblCalculator.Text);
                lstConsole.Items.Add("Not zero - no branching");
            }
        }

        //BRANCH POSITIVE :: 8XX
        private void branchpositive()
        {
            lstConsole.Items.Add("Branch on positive.");

            if (int.Parse(lblCalculator.Text) >= 0)
            {
                lblMAR.Text = format2(address.ToString());
                lblMAR.Refresh();
                textHighlight(mailboxes[address]);
                lblMDR.Text = format2(mailboxes[address].Text);
                lblMDR.Refresh();
                lstConsole.Items.Add("Calculator: " + lblCalculator.Text);
                lstConsole.Items.Add("Branching to " + lblMAR.Text);
                ctr = int.Parse(lblMAR.Text) - 1;
            }
            else if (int.Parse(lblCalculator.Text) < 0)
            {
                lstConsole.Items.Add("Negative flag is set - no branching");
            }
        }

        //INPUT :: 901
        private void input()
        {
            try
            {
                location("@input");
                lstConsole.Items.Add("Input/Output - Address " + format2(address.ToString()) + ": input");
                lblOperation.Text = "IN";
                lblOperation.Refresh();
                String str = int.Parse(format3(Microsoft.VisualBasic.Interaction.InputBox("Enter value: "))).ToString();
                if (str.Length > 3) { str = str.Substring(0, 3); }
                lblCalculator.Text = format3(str);
                lblCalculator.Refresh();
                lstConsole.Items.Add("Entering input " + lblCalculator.Text + " in calculator");
            }
            catch
            {
                MessageBox.Show("Enter numbers only within three-digit format.");
                input();
            }

        }

        //OUTPUT :: 902
        private void output()
        {
            location("@calculator");
            lstConsole.Items.Add("Input/Output - Address " + format2(address.ToString()) + ": output");
            lblMDR.Text = format3(lblCalculator.Text);
            lblMDR.Refresh();
            lblMAR.Text = format2(address.ToString());
            lblMAR.Refresh();
            location("@output");
            lblOperation.Text = "OUT";
            lblOperation.Refresh();

            if (int.Parse(lblCalculator.Text) < 0)
            {
                lstOutput.Items.Add(format3((negNum * -1).ToString()));
            }
            else
            {
                lstOutput.Items.Add(lblCalculator.Text);
            }

            lstConsole.Items.Add("Send calculator value to output: " + lblCalculator.Text);
            lstOutput.Refresh();
            lstOutput.SelectedIndex = lstOutput.Items.Count - 1;
            lstOutput.SelectedIndex = -1;
        }

        //HIGLIGHT THE TEXTBOX - FOR UI PURPOSES
        private void textHighlight(TextBox tb)
        {
            int msec;

            if (loadingFile)
            {
                msec = 50;
            }
            else
            {
                msec = 250;
            }
            tb.BackColor = Color.Red;
            tb.ForeColor = Color.White;
            tb.Refresh();
            Thread.Sleep(msec);
            tb.BackColor = Color.White;
            tb.ForeColor = Color.Black;
            tb.Refresh();
        }

        // LITTLE MAN LOCATION (GROUPBOX)
        private void location(string loc)
        {
            switch (loc)
            {
                case "@startup":
                    move(453, 228);
                    break;
                case "@counter":
                    move(455, 288);
                    break;
                case "@mailboxes":
                    if (int.Parse(lblMAR.Text) < 20) { move(633, 54); }
                    else if (int.Parse(lblMAR.Text) >= 20 && int.Parse(lblMAR.Text) < 40) { move(633, 146); }
                    else if (int.Parse(lblMAR.Text) >= 40 && int.Parse(lblMAR.Text) < 60) { move(633, 238); }
                    else if (int.Parse(lblMAR.Text) >= 60 && int.Parse(lblMAR.Text) < 80) { move(633, 330); }
                    else if (int.Parse(lblMAR.Text) >= 80 && int.Parse(lblMAR.Text) < 100) { move(633, 422); }
                    Thread.Sleep(1000);
                    break;
                case "@output":
                    move(102, 248);
                    break;
                case "@input":
                    move(102, 127);
                    break;
                case "@calculator":
                    move(316, 103);
                    break;
            }
        }

        // FORMATTING STRINGS

        public static String format2(String str)
        {
            str = String.Format("{0}", str.ToString().PadLeft(2, '0'));
            return str;
        }

        public static String format3(String str)
        {
            str = String.Format("{0}", str.ToString().PadLeft(3, '0'));
            return str;
        }


        // BUTTON & EVENTHANDLERS


        private void btnRun_Click(object sender, EventArgs e)
        {
            try
            {
                running = true;
                fetchExecute = false;
                btnFetchExecute.Enabled = false;
                btnStop.Enabled = true;
                btnRun.Enabled = false;
                btnClearMBoxes.Enabled = false;
                btnAssemblyEditor.Enabled = false;
                mainThread = new Thread(processThread);
                mainThread.IsBackground = true;
                mainThread.Start();
            }
            catch
            {
                MessageBox.Show("Error Threads");
                mainThread.Abort();
            }
        }

        private void btnFetchExecute_Click(object sender, EventArgs e)
        {
            try
            {
                running = true;
                fetchExecute = true;
                fetchExecuteThread = new Thread(processThread);
                fetchExecuteThread.IsBackground = true;
                fetchExecuteThread.Start();
            }
            catch
            {
                MessageBox.Show("Error Threads");
                fetchExecuteThread.Abort();
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            try
            {
                running = false;
                fetchExecute = false;
                btnFetchExecute.Enabled = true;
                btnRun.Enabled = true;
                btnResetAll.Enabled = true;
                btnClearMBoxes.Enabled = true;
                btnAssemblyEditor.Enabled = true;
                suspended = true;
                mainThread.Suspend();
            }
            catch
            {
                mainThread.Abort();
            }
        }

        private void btnResetAll_Click(object sender, EventArgs e)
        {
            if (fetchExecute)
            {
                if (fetchExecuteThread.ThreadState == ThreadState.Suspended)
                {
                    fetchExecuteThread.Start();
                    fetchExecuteThread.Abort();
                }
            }
            else
            {
                if (!suspended)
                {
                    mainThread.Abort();
                }
                else if (suspended && mainThread.ThreadState == ThreadState.Suspended)
                {
                    mainThread.Start();
                    mainThread.Abort();
                }
            }

            lblCalculator.Text = "000";
            lblMAR.Text = "00";
            lblMDR.Text = "000";
            lblOperation.Text = "- - - -";
            txtCounter.Text = "00";
            lstOutput.Items.Clear();
            lstConsole.Items.Clear();
            tsProgramName.Clear();
            ctr = 0;
            opCode = 0;
            address = 0;
            haltIndex = 0;
            location("startup");
            frmMain_Load(this, new EventArgs());
        }

        private void btnClearMBoxes_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 100; i++)
            {
                mailboxes[i].Text = "000";
                mailboxes[i].ForeColor = Color.Black;
                mailboxes[i].BackColor = Color.White;
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtCounter.Text = "00";
        }

        private void mailBoxes_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!backspace)
            {
                if (!char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
        }

        private void mailBoxes_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back)
            {
                backspace = true;
            }
            else
            {
                backspace = false;
            }
        }

        private void mailBoxes_LostFocus(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (String.IsNullOrEmpty(tb.Text))
            {
                tb.Text = "000";
            }
        }

        private void lblCalculator_TextChanged(object sender, EventArgs e)
        {
            if (!negative)
            {
                if (int.Parse(lblCalculator.Text) > 0)
                {
                    lblCalculator.ForeColor = Color.LawnGreen;
                    lblCalculator.Refresh();
                    Thread.Sleep(250);
                    lblCalculator.ForeColor = Color.White;
                    lblCalculator.Refresh();
                }
                else if (int.Parse(lblCalculator.Text) == 0)
                {
                    lblCalculator.ForeColor = Color.White;
                }
            }
            else if (negative)
            {
                lblCalculator.ForeColor = Color.Red;
                lblCalculator.Refresh();
                Thread.Sleep(250);
                lblCalculator.ForeColor = Color.White;
                lblCalculator.Refresh();
            }
        }

        private void lblMAR_TextChanged(object sender, EventArgs e)
        {
            lblMAR.ForeColor = Color.Red;
            Thread.Sleep(250);
            lblMAR.ForeColor = Color.Black;
        }

        private void lblMDR_TextChanged(object sender, EventArgs e)
        {
            lblMDR.ForeColor = Color.Red;
            Thread.Sleep(250);
            lblMDR.ForeColor = Color.Black;
        }

        private void lblOperation_TextChanged(object sender, EventArgs e)
        {
            if (lblOperation.Text == "done" && fetchExecute == true)
            {
                suspended = true;
                running = false;
                if (suspended)
                {
                    btnResetAll.Enabled = true;
                    fetchExecuteThread.Suspend();
                }
            }
        }

        private void instructionSetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form frm = new frmInstructionSet();
            frm.Show();
        }


        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dr = MessageBox.Show("Are you sure to exit program?", "QUIT PROGRAM", MessageBoxButtons.YesNo);

            if (dr == DialogResult.Yes)
            {
                if (running)
                {
                    mainThread.Abort();
                }
                this.Dispose();
            }
            else if (dr == DialogResult.No)
            {
                e.Cancel = (dr == DialogResult.No);
            }
        }

        private void smSave_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.FileName = tsProgramName.Text;
                sfd.Filter = "LMC File|*.lmc";
                sfd.Title = "Save LMC File";

                if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    lstConsole.Items.Add("");
                    lstConsole.Items.Add("> Saving file...");

                    using (FileStream fstream = new FileStream(sfd.FileName, FileMode.Create, FileAccess.Write))
                    {
                        using (StreamWriter writer = new StreamWriter(fstream))
                        {
                            writer.WriteLine("# Simple Little Man Computer Simulator - Zilvercodes © 2017 #");
                            writer.Write(format2(haltIndex.ToString()) + "%");
                            writer.Write(format3(lblCalculator.Text) + "%");
                            for (int i = 0; i < 100; i++)
                            {
                                writer.Write(mailboxes[i].Text + ",");
                            }
                            writer.Close();
                        }
                        fstream.Close();
                    }


                    MessageBox.Show("LMC file was saved successfully.");
                    lstConsole.Items.Add("> LMC file was saved successfully.");
                    lstConsole.Items.Add("");
                }
                else if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                {
                    lstConsole.Items.Add("> File saving was canceled.");
                }
            }
            catch
            {
                MessageBox.Show("Error on saving file. File cannot be overwritten.");
                lstConsole.Items.Add("Error on saving file. File cannot be overwritten.");
            }

        }

        private void smOpen_Click(object sender, EventArgs e)
        {
            try
            {
                Stream lmcStream;
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "LMC files(*.lmc)|*.lmc|All files(*.*)|*.*";
                ofd.Title = "Load LMC file";

                lstConsole.Items.Add("> Loading file...");
                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if ((lmcStream = ofd.OpenFile()) != null)
                    {
                        string strPath = ofd.FileName;
                        string strFilename = Path.GetFileName(strPath);
                        string strDirectory = Path.GetDirectoryName(strPath);
                        string strLine = File.ReadAllText(strPath);
                        string[] parts = strLine.Split(',');
                        int index = strLine.IndexOf("%", strLine.IndexOf('%') + 1);
                        parts[0] = strLine.Substring(index + 1, 3);                 //change the first line of text array
                        lstConsole.Items.Add("> " + strFilename.ToUpper() + "...");
                        lstConsole.Items.Add("> from " + strDirectory + "...");

                        for (int i = 0; i < 100; i++)
                        {
                            mailboxes[i].Text = parts[i].ToString();
                            loadingFile = true;
                            textHighlight(mailboxes[i]);
                        }
                        loadingFile = false;
                        lstConsole.Items.Add("> LMC file was successfully loaded");
                        lstConsole.Items.Add("");
                        tsProgramName.Text = " " + Path.GetFileNameWithoutExtension(strPath).ToUpper();
                        MessageBox.Show("LMC file was successfully loaded.");
                    }
                }
                else if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                {
                    lstConsole.Items.Add("Loading was canceled.");
                }
            }
            catch
            {
                MessageBox.Show("Error on loading file.");
            }
        }

        private void mnuAbout_Click(object sender, EventArgs e)
        {
            Form frm = new frmAbout();
            frm.Show();
        }

        private void btnAssemblyEditor_Click(object sender, EventArgs e)  //SINGLETON PATTERN
        {
            Form fc = Application.OpenForms["frmAssemblyEditor"];

            lock (padlock)
            {
                if (fc == null)
                {
                    new frmAssemblyEditor(this).Show();
                }
            }
        }

        private void move(int x, int y)
        {
            Transition t = new Transition(new TransitionType_EaseInEaseOut(500));
            t.add(gbLMC, "Left", x);
            t.add(gbLMC, "Top", y);
            t.run();
        }
    }
}