using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace myLMC
{
    public partial class frmAbout : Form
    {
        public frmAbout()
        {
            InitializeComponent();
        }

        private void frmAbout_Load(object sender, EventArgs e)
        {
           
            rtbAbout.SelectionAlignment = HorizontalAlignment.Left;
            rtbAbout.SelectionIndent = 3;
            rtbAbout.SelectionRightIndent = 3;
            rtbAbout.AppendText(Environment.NewLine);
            rtbAbout.AppendText(" SIMPLE LMC SIMULATOR version 1.0 (w/o Assembly Compiler)");
            rtbAbout.AppendText(Environment.NewLine);
            rtbAbout.AppendText(Environment.NewLine);
            rtbAbout.AppendText("\t A simple Little Man Computer Simulator program based developed in C# by Guiller Apan as a project for La Consolacion College-Binan School of Computer Studies.");
            rtbAbout.AppendText(Environment.NewLine);
            rtbAbout.AppendText(Environment.NewLine);
            rtbAbout.AppendText("\t It was inspired from the program developed by Dr. Magnus Bordewich [http://community.dur.ac.uk/m.j.r.bordewich] of the School of Engineering and Computing Sciences, Durham University, U.K.");
            rtbAbout.AppendText(Environment.NewLine);
            rtbAbout.AppendText(Environment.NewLine);
            rtbAbout.AppendText("\t The Little Man Computer (LMC) is an instructional model of a computer, created by Dr. Stuart Madnick in 1965. The LMC is generally used to teach students, because it models a simple von Neumann architecture computer—which has all of the basic features of a modern computer. It can be programmed in machine code (albeit in decimal rather than binary) or assembly code. ");
            rtbAbout.AppendText(Environment.NewLine);
            rtbAbout.AppendText("WIKIPEDIA: [https://en.wikipedia.org/wiki/Little_man_computer]\n");
        }

        private void rtbAbout_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Process.Start(e.LinkText);
        }

        
    }
}
