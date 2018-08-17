using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BlockieWalkieProto
{
    public partial class Window : Form
    {
        public GameView MyView = new GameView();

        public Window()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            MyView.Render(e.Graphics);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            int X = (int)Math.Floor(e.X / MyView.Scale);
            int Y = (int)Math.Floor(e.Y / MyView.Scale);

            MyView.PlaceShot(new Shot(X, Y));

            Invalidate();
        }
    }

    public class Node
    {
        public int X = 0;
        public int Y = 0;
        public int Width = 0;
        public int Height = 0;
        public Color DebugColor;

        private static Random RandomColorGenerator = new Random();

        public Node(int x, int y, int width, int height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
            this.DebugColor = Color.FromArgb(RandomColorGenerator.Next(256), RandomColorGenerator.Next(256), RandomColorGenerator.Next(256));
        }

        public bool OverlapsShot(Shot s)
        {
            return s.X >= X && s.X <= X + Width && s.Y >= Y && s.Y <= Y + Height;
        }
    }

    public class Shot
    {
        public int X = 0;
        public int Y = 0;

        public Shot(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }

    public class GameView
    {
        public List<Shot> MyShots = new List<Shot>();
        public List<Node> Nodes = new List<Node>();
        public int Width = 10;
        public int Height = 10;

        public float Scale = 100.0f;

        public GameView()
        {
            Nodes.Add(new Node(0, 0, Width, Height));
        }

        public void PlaceShot(Shot s)
        {
            // Check for duplicates
            foreach (Shot shot in MyShots)
            {
                if (shot.X == s.X && shot.Y == s.Y)
                {
                    MessageBox.Show("No duplicate shots allowed.");
                    return;
                }
            }

            MyShots.Add(s);

            // Split affected nodes
            for (int i = Nodes.Count - 1; i >= 0; i--)
            {
                if (Nodes[i].OverlapsShot(s))
                {
                    Node n = Nodes[i];
                    Nodes.RemoveAt(i);

                    // Top
                    if (s.Y > n.Y)
                    {
                        Node top = new Node(n.X, n.Y, n.Width, s.Y - n.Y);
                        Nodes.Add(top);
                    }
                    // Bottom
                    if (s.Y < n.Y + n.Height)
                    {
                        Node bottom = new Node(n.X, s.Y + 1, n.Width, n.Y + n.Height - s.Y - 1);
                        Nodes.Add(bottom);
                    }
                    // Left
                    if (s.X > n.X)
                    {
                        Node left = new Node(n.X, n.Y, s.X - n.X, n.Height);
                        Nodes.Add(left);
                    }
                    // Right
                    if (s.X < n.X + n.Width)
                    {
                        Node right = new Node(s.X + 1, n.Y, n.X + n.Width - s.X - 1, n.Height);
                        Nodes.Add(right);
                    }
                }
            }
        }

        public void Render(Graphics g)
        {
            for (int i = 0; i <= Width; i++)
            {
                g.DrawLine(Pens.Black, i * Scale, 0, i * Scale, Height * Scale);
            }
            for (int i = 0; i <= Height; i++)
            {
                g.DrawLine(Pens.Black, 0, i * Scale, Width * Scale, i * Scale);
            }
            foreach (Shot s in MyShots)
            {
                g.FillRectangle(Brushes.Red, s.X * Scale, s.Y * Scale, Scale, Scale);
            }

            Node largestArea = Nodes[0];

            foreach (Node n in Nodes)
            {
                if (n.Width * n.Height > largestArea.Width * largestArea.Height)
                {
                    largestArea = n;
                }
                using (Pen p = new Pen(n.DebugColor))
                    g.DrawRectangle(p, n.X * Scale + 2, n.Y * Scale + 2, n.Width * Scale - 5, n.Height * Scale - 5);
            }

            using (Brush b = new SolidBrush(Color.FromArgb(50, Color.Red)))
                g.FillRectangle(b, largestArea.X * Scale, largestArea.Y * Scale, largestArea.Width * Scale, largestArea.Height * Scale);
        }
    }


}
