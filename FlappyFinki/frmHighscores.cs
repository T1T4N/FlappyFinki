using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FlappyFinki
{
    public partial class frmHighscores : Form
    {
        private frmMain MainForm;
        public frmHighscores(frmMain mainForm) : this()
        {
            MainForm = mainForm;
            listBox1.Items.AddRange(mainForm.players.ToArray());
        }
        private frmHighscores()
        {
            InitializeComponent();
        }
    }
}
