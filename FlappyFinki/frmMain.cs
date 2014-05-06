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
    public partial class frmMain : Form
    {
        public SortedSet<Stats> players { get; private set; }
        public frmMain()
        {
            InitializeComponent();
            players = new SortedSet<Stats>();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (txtName.Text.Trim().Equals(""))
            {
                MessageBox.Show("Please enter your name");
            }
            else
            {
                new GameForm(txtName.Text, this).ShowDialog();
            }
            
        }

        public void AddPlayer(Stats player)
        {
            players.Add(player);
        }

        private void btnHighScores_Click(object sender, EventArgs e)
        {
            new frmHighscores(this).ShowDialog();
        }
    }
}
