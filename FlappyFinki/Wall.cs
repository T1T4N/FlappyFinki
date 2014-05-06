using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace FlappyFinki
{
    class Wall
    {
        public static List<Color> colors = new List<Color>()
        {
            Color.Blue,
            Color.Purple,
            Color.Salmon,
            Color.RosyBrown,
            Color.PowderBlue,
            Color.Orange,
            Color.DarkOrange,
            Color.Magenta,
            Color.LightSlateGray,
            Color.OrangeRed
        };
        public static Random r = new Random();
        public Rectangle topWall, lowWall;
        private Rectangle topHead, lowHead;
        public Color Color;

        public Wall(Rectangle top, Rectangle low)
        {
            topWall = top;
            lowWall = low;
            CalculateHead();
        }

        public void UpdatePositionTop(int x, int height, int y = -1)
        {
            topWall.X = x;
            topWall.Y = (y == -1)?topWall.Y:y;
            topWall.Height = height;
            CalculateHead();
        }
        public void UpdatePositionBottom(int x, int height, int y = -1)
        {
            lowWall.X = x;
            lowWall.Y = (y == -1) ? lowWall.Y : y;
            lowWall.Height = height;
            CalculateHead();
        }
        public void CalculateHead()
        {
            topHead = new Rectangle(topWall.X - 20, topWall.Bottom - 0, topWall.Width + 35, 30);
            lowHead = new Rectangle(lowWall.X - 20, lowWall.Y-30, lowWall.Width + 35, 30);
        }

        public void DrawWall(Graphics g)
        {
            g.FillRectangle(new SolidBrush(Color), topWall);
            g.FillRectangle(new SolidBrush(Color), topHead);

            g.FillRectangle(new SolidBrush(Color), lowWall);
            g.FillRectangle(new SolidBrush(Color), lowHead);

            g.DrawRectangle(Pens.Black, topWall);
            g.DrawRectangle(Pens.Black, topHead);
            g.DrawRectangle(Pens.Black, lowWall);
            g.DrawRectangle(Pens.Black, lowHead);
        }

        public bool Intersect(Rectangle r)
        {
            return (topHead.IntersectsWith(r) || topWall.IntersectsWith(r) || lowHead.IntersectsWith(r) ||
                    lowWall.IntersectsWith(r));
        }
    }
}
