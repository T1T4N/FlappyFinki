using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FlappyFinki
{
    internal class Game
    {
        public Rectangle Instance;
        public Point Location;
        public Point OldLocation;

        public float Rotation = 0;
        public bool Active = false;
        public bool Over = false;
        public Stats Score { get; private set; }
        public int heightPoller = 0;

        private float fallingRotation = 0;
        private float fallingSpeed = 0;

        private Matrix matrix = new Matrix();
        private Point maxSize;
        private Wall wall1, wall2;
        private Graphics p;
        private Bitmap graphics;

        public Game(string PlayerName, Point MaxSize, Rectangle instance)
        {
            Score = new Stats(PlayerName, 0);

            maxSize = new Point(MaxSize.Y - 38, MaxSize.X - 5);
            Instance = instance;
            Active = true;
            graphics = new Bitmap(Instance.Width + 1, Instance.Height + 1);
            p = Graphics.FromImage(graphics);

            Rectangle topBox = new Rectangle(new Point(maxSize.X - 64, -1), new Size(64, r.Next(90, 200)));
            Rectangle lowerBox = new Rectangle(new Point(maxSize.X - 64, topBox.Height + 235),
                new Size(64, maxSize.Y - topBox.Height + 300));
            wall1 = new Wall(topBox, lowerBox);

            Rectangle topBox2 = new Rectangle(new Point(maxSize.X + (maxSize.X/2) - 64, -1),
                new Size(64, r.Next(90, 200)));
            Rectangle lowerBox2 = new Rectangle(new Point(maxSize.X + (maxSize.X/2) - 64, topBox2.Height + 235),
                new Size(64, maxSize.Y - topBox2.Height + 300));
            wall2 = new Wall(topBox2, lowerBox2);
        }

        public void PaintPlayer(Graphics e, Color background)
        {
            p.Clear(background);
            if (Active)
            {
                fallingSpeed += (float) 0.08;
                Location.Y += (int) (2*fallingSpeed);
                //ADD MOMENTUM
            }

            matrix = new Matrix();
            if (heightPoller > 0 && Active)
            {
                //smooth jumping
                Location.Y -= 8;
                heightPoller -= 8;
                fallingSpeed = 0;
                if (Rotation <= 0)
                    Rotation = 1;
            }
            if (Rotation > 0)
            {
                fallingRotation = 0;
                matrix.RotateAt(
                    Rotation - (Rotation*2),
                    new PointF(Instance.Left + ((float) Instance.Width/2), Instance.Top + ((float) Instance.Height/2)),
                    MatrixOrder.Append
                    );
                if (Active && heightPoller <= 0)
                    Rotation--;
                else if (Rotation < 30)
                    Rotation += 2;
            }
            else
            {
                if (fallingRotation == 0)
                    fallingRotation += (float) 0.5;

                matrix.RotateAt(fallingRotation,
                    new PointF(Instance.Left + ((float) Instance.Width/2), Instance.Top + ((float) Instance.Height/2)),
                    MatrixOrder.Append);
                if (fallingRotation < 35 && fallingRotation != 0 && Active)
                    fallingRotation += (float) 0.7;
            }
            p.Transform = matrix;

            int offset = 10;

            Rectangle stub = new Rectangle(Instance.X, Instance.Y, offset, Instance.Height);
            Rectangle circle = new Rectangle(Instance.X + offset + 3, Instance.Y, Instance.Width - (2*offset),
                Instance.Height);
            Rectangle innerCircle = new Rectangle(circle.X + (offset + 1)/2, circle.Y + (offset + 1)/2,
                circle.Width - offset, circle.Height - offset);
            Rectangle eye = new Rectangle(circle.Right - 7, circle.Y + 3, 15, 15);
            Rectangle zen = new Rectangle(eye.X + 7, eye.Y + 5, 6, 6);


            p.FillRectangle(new SolidBrush(Color.Blue), stub);

            p.FillEllipse(new SolidBrush(Color.SkyBlue), circle);
            p.FillEllipse(new SolidBrush(Color.White), innerCircle);
            p.DrawEllipse(Pens.Black, circle);
            p.DrawEllipse(Pens.Black, innerCircle);


            p.FillEllipse(new SolidBrush(Color.White), eye);
            p.FillEllipse(new SolidBrush(Color.Blue), zen);
            p.DrawEllipse(Pens.Black, eye);
            p.DrawEllipse(Pens.Black, zen);


            //p.FillRectangle(new SolidBrush(Color.DarkCyan), Instance);
            //p.DrawRectangle(Pens.Black, Instance);

            e.DrawImage(graphics, Location);
        }

        private static Random r = new Random(Environment.TickCount);

        private bool scoreGiven = false;

        private Color currentColor = Wall.colors[0];
        private Color currentColor2 = Wall.colors[r.Next(0, Wall.colors.Count - 1)];

        public void PaintObstacles(Graphics e)
        {
            //move Rectangles.
            if (Active && !Over)
            {
                wall1.topWall.X -= 2;
                wall1.lowWall.X -= 2;

                wall2.topWall.X -= 2;
                wall2.lowWall.X -= 2;
            }

            wall1.Color = currentColor;
            wall2.Color = currentColor2;

            wall1.DrawWall(e);
            wall2.DrawWall(e);

            //calculate where they should be at, start at the max length of the form.
            if (wall1.topWall.X <= -64)
            {
                //update.
                //+1 point.
                scoreGiven = false;

                wall1.UpdatePositionTop(maxSize.X, r.Next(90, 200));
                currentColor = Wall.colors[r.Next(0, Wall.colors.Count - 1)];
            }

            if (wall1.lowWall.X <= -64)
            {
                wall1.UpdatePositionBottom(maxSize.X, maxSize.Y - wall1.topWall.Height + 250, wall1.topWall.Height + 235);
            }

            if (wall2.topWall.X <= -64)
            {
                //update.
                //+1 point.
                scoreGiven = false;
                wall2.UpdatePositionTop(maxSize.X, r.Next(150, 250));
                currentColor2 = Wall.colors[r.Next(0, Wall.colors.Count - 1)];
            }

            if (wall2.lowWall.X <= -64)
            {
                wall2.UpdatePositionBottom(maxSize.X, maxSize.Y - wall2.topWall.Height + 250, wall2.topWall.Height + 235);
            }
            wall1.CalculateHead();
            wall2.CalculateHead();

            //check score.
            if (Location.X > wall1.topWall.X + wall1.topWall.Width && !scoreGiven)
            {
                Score.IncreaseScore();
                scoreGiven = true;
            }
            if (Location.X > wall2.topWall.X + wall2.topWall.Width && !scoreGiven)
            {
                Score.IncreaseScore();
                scoreGiven = true;
            }
        }

        public bool CheckCollision()
        {
            if ((OldLocation.Y + Instance.Height) >= maxSize.Y || OldLocation.Y <= 0)
            {
                Active = false;
                fallingRotation = 0;

                //activate main form's shit and show score.
                return true;
            }

            //Check for wall collision.
            Rectangle collide = new Rectangle(OldLocation.X, OldLocation.Y, Instance.Width, Instance.Height);
            if (wall1.Intersect(collide) || wall2.Intersect(collide))
            {
                Active = false;
                fallingRotation = 0;
                return true;
            }
            return false;
        }
    }
}