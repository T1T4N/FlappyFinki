using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace FlappyFinki
{
    public partial class GameForm : Form
    {
        private delegate void UpdateFormCallback();

        //Game Object Declarations.
        private Game p1;

        private Thread updateThread;
        private Thread CountDownThread;

        private bool paused = false;
        private bool once = false;
        private bool gameover = false;
        private int countdown = 4;

        //fonts
        private Font fnt = new Font("Arial", (float) 23, FontStyle.Italic | FontStyle.Bold);
        private Font smfnt = new Font("Arial", (float) 9, FontStyle.Italic | FontStyle.Bold);

        private frmMain MainForm;
        private string PlayerName;

        public GameForm(string playerName, frmMain mainForm) : this()
        {
            PlayerName = playerName;
            MainForm = mainForm;
        }

        private GameForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            p1 = new Game(PlayerName, new Point(this.Size.Width, this.Size.Height), new Rectangle(0, 0, 59, 38))
            {
                Location = new Point(100, 100),
            };
            p1.OldLocation = p1.Location;

            updateThread = new Thread(new ThreadStart(UpdateThread)) {IsBackground = true};
            updateThread.Start();

            CountDownThread = new Thread(new ThreadStart(CountDown)) {IsBackground = true};
            CountDownThread.Start();
        }

        private void UpdateThread()
        {
            while (!gameover)
            {
                UpdateForm();
                Thread.Sleep(10);
            }
        }

        private void CountDown()
        {
            while (countdown > 0)
            {
                p1.Active = false;
                countdown--;
                Thread.Sleep(1000);
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            /*
            Rectangle rc = new Rectangle(0, 0, this.ClientSize.Width, this.ClientSize.Height);
            using (LinearGradientBrush brush = new LinearGradientBrush(rc, Color.GreenYellow, Color.LimeGreen, 90F))
            {
                e.Graphics.FillRectangle(brush, rc);
            }
            */

            base.OnPaintBackground(e);
        }

        private void UpdateForm()
        {
            if (this.InvokeRequired)
            {
                UpdateFormCallback d = new UpdateFormCallback(UpdateForm);
                try
                {
                    this.Invoke(d);
                }
                catch
                {
                }
            }
            else
            {
                this.Refresh();
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (p1.Instance != null)
            {
                p1.PaintPlayer(e.Graphics, this.BackColor);
                p1.PaintObstacles(e.Graphics);

                //Score
                e.Graphics.FillRectangle(new SolidBrush(Color.DimGray), new Rectangle(-1, -1, 75, 25));
                e.Graphics.DrawRectangle(Pens.Black, new Rectangle(-1, -1, 75, 25));
                e.Graphics.DrawString(string.Format("Score: {0}", p1.Score.Score), smfnt, new SolidBrush(Color.White),
                    new PointF(8, 5));

                //fixed collision check
                if (p1.CheckCollision())
                {
                    //game over.
                    while (p1.Location.Y < Size.Height - 50)
                    {
                        p1.Active = true;
                        p1.Over = true;
                        p1.Location.Y += 30;
                        p1.OldLocation = p1.Location;
                        return;
                    }
                    EndGame();
                }
                p1.OldLocation = p1.Location;

                if (countdown > 0)
                {
                    //draw new countdown.
                    Font f = new Font("Arial", (float) 9, FontStyle.Italic | FontStyle.Bold);
                    e.Graphics.DrawString("Факултет за Информатички Науки\n и Компјутерско Инженерство", f,
                        new SolidBrush(Color.Black), 170, 100);
                    e.Graphics.DrawString(countdown.ToString(), fnt, new SolidBrush(Color.Black),
                        new PointF((this.Width/2) - 30, this.Height/2 - 50));
                }
                else if (!once)
                {
                    //active. //never activate again.
                    p1.Active = true;
                    once = true;
                }
            }
            if (paused)
            {
                //draw paused on the screen.
                e.Graphics.DrawString("Paused.", fnt, new SolidBrush(Color.Black),
                    new PointF((Width/2) - 80, Height/2 - 30));
            }
            if (gameover)
            {
                e.Graphics.DrawString(string.Format("Final Score: {0}", p1.Score.Score), fnt,
                    new SolidBrush(Color.Black), new PointF(Width/2 - 120, Height/2 - 60));
                e.Graphics.DrawString("Game Over. Press Enter.", fnt, new SolidBrush(Color.Black),
                    new PointF(50, Height/2 - 30));
            }
        }

        private void EndGame()
        {
            gameover = true;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //update player location
            if (gameover && e.KeyCode == Keys.Enter)
            {
                gameover = false;
                countdown = 4;
                once = false;
                MainForm.AddPlayer(p1.Score);
                if (
                    MessageBox.Show("Do you want to play another game ?", "Flappy FINKI", MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Form1_Load(this, EventArgs.Empty);
                }
                else
                {
                    new frmHighscores(MainForm).Show();
                    Close();
                }

            }
            if (e.KeyCode == Keys.Space && p1.Instance != null && p1.Active && !p1.Over)
            {
                p1.heightPoller += 100;
            }
            else if (e.KeyCode == Keys.Q)
            {
                p1.Active = false;
                paused = true;
                if (
                    MessageBox.Show("Are you sure you want to quit?", "Are you sure?", MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Close();
                }
                p1.Active = true;
                paused = false;
            }
            else if (e.KeyCode == Keys.P && (p1.Active || paused))
            {
                //pause game.
                p1.Active = !p1.Active;
                paused = !p1.Active;
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (p1.Instance != null && p1.Active && !p1.Over)
            {
                //instead store height poller
                p1.heightPoller += 100;
                //p1.Location.Y -= 75;
            }
        }
    }
}