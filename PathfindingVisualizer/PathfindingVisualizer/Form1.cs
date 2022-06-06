using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Runtime.InteropServices;

namespace PathfindingVisualizer
{
    public partial class Form1 : Form
    {
        //GLOBALNE VARIJABLE:
        int visina = 15, sirina = 15, reset = 0;
        bool[,] graf = new bool[31, 51];
        bool Drawing = false;
        char Item = 'z';
        //BOJE:
        Color Cpoc = Color.FromArgb(170, 204, 0);
        Color Ckraj = Color.FromArgb(186, 24, 27);
        Color Czid = Color.FromArgb(43, 23, 28); 
        Color Cvisited = Color.FromArgb(255, 170, 90);
        Color Cnode = Color.FromArgb(248, 95, 69); 
        Color Cpath = Color.FromArgb(90, 9, 108); 
        //
        Color Cvisited2 = Color.FromArgb(225, 125, 97); 
        Color Cnode2 = Color.FromArgb(226, 49, 49); 

        struct Node
        {
            public int IndexX, IndexY;
            public void SetYX(int Y,int X)
            {
                IndexX = X;
                IndexY = Y;
            }
        };
        Node StartNode = new Node(), EndNode = new Node();
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox1.Width = 51 * sirina + 1;
            pictureBox1.Height = 31 * visina + 1;
            StartNode.SetYX(15, 9);
            EndNode.SetYX(15, 41);
            this.BackColor = Color.FromArgb(225, 125, 97); 
            this.Width = 32 + pictureBox1.Width;
            this.Height = 160 + pictureBox1.Height;
            timer1.Start();
        }
        void OcistiPlocu(bool Reset)
        {
            NoNode_label.Text = "Nodes:";
            NoVisited_label.Text = "Visited:";
            PathLength_label.Text = "Duzina puta:";
            this.Refresh();
            Nacrtaj();
            Graphics g = pictureBox1.CreateGraphics();
            Brush Cetka = new SolidBrush(Czid);
            for (int i = 0; i < 51; i++)
                for (int j = 0; j < 31; j++)
                {
                    if (Reset) graf[j, i] = false;
                    if (graf[j, i]) g.FillRectangle(Cetka, i * sirina + 1, j * visina + 1, sirina - 1, visina - 1);
                }
        }
        private void OcistiPut_button_Click(object sender, EventArgs e)
        {
            OcistiPlocu(false);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            OcistiPlocu(true);
        }
        void Nacrtaj()
        {
            Graphics g = pictureBox1.CreateGraphics();
            Pen Olovka = new Pen(Color.Black);
            SolidBrush Cetka = new SolidBrush(Color.White);

            g.FillRectangle(Cetka, 0, 0, pictureBox1.Width, pictureBox1.Height);
            for (int i = 0; i < pictureBox1.Height; i += visina)
                g.DrawLine(Olovka, 0, i, pictureBox1.Width, i);
            for (int i = 0; i < pictureBox1.Width; i += sirina)
                g.DrawLine(Olovka, i, 0, i, pictureBox1.Height);

            Cetka.Color = Cpoc;
            g.FillRectangle(Cetka, StartNode.IndexX * sirina + 1, StartNode.IndexY * visina + 1, sirina - 1, visina - 1);
            Cetka.Color = Ckraj;
            g.FillRectangle(Cetka, EndNode.IndexX * sirina + 1, EndNode.IndexY * visina + 1, sirina - 1, visina - 1);
        }
        //pocetak koda za vizualizaciju algoritama
        double heuristic(int Y, int X, string s)
        {
            double y = Math.Abs(Y - EndNode.IndexY), x = Math.Abs(X - EndNode.IndexX);
            return Math.Sqrt(y * y + x * x) + s.Length;
        }
        double heuristic2(int Y, int X, string s)//za bidirectional algoritme
        {
            double y = Math.Abs(Y - StartNode.IndexY), x = Math.Abs(X - StartNode.IndexX);
            return Math.Sqrt(y * y + x * x) + s.Length;
        }
        /// 
        private void Vizualiziraj_button_Click(object sender, EventArgs e)
        {
            if (EndNode.IndexX == -1)
            {
                MessageBox.Show("Spustite Node kraj");
                return;
            }
            if (StartNode.IndexX == -1)
            {
                MessageBox.Show("Spustite StartNode");
                return;
            }
            string Algoritam = "";
            try
            {
                Algoritam = comboBox1.SelectedItem.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Odaberite algoritam");
                return;
            }
            this.OcistiPut_button_Click(this, null);

            Graphics g = pictureBox1.CreateGraphics();
            SolidBrush Cetka = new SolidBrush(Ckraj);

            bool[,] bio = new bool[31, 51], bio2 = new bool[31, 51];;
            for (int i = 0; i < 51; i++) 
                for (int j = 0; j < 31; j++) 
                { 
                    bio[j, i] = false;
                    bio2[j, i] = false;
                }
            bio[StartNode.IndexY, StartNode.IndexX] = true;
            bio2[EndNode.IndexY, EndNode.IndexX] = true;

            int[] smjerY = new int[4] { 0, 0, 1, -1 }, smjerX = new int[4] { 1, -1, 0, 0};
            string[] smjer = new string[4] { "r", "l", "d", "u" };
            int Visited = 1;
            LinkedList<Tuple<int, int, string>> lista = new LinkedList<Tuple<int, int, string>>(), lista2 = new LinkedList<Tuple<int, int, string>>();
            lista.AddFirst(new Tuple<int, int, string>(StartNode.IndexY, StartNode.IndexX, ""));
            lista2.AddFirst(new Tuple<int, int, string>(EndNode.IndexY, EndNode.IndexX, ""));//za biderectoinal algoritme
            string put = "", put2 = "";

            while (lista.Count > 0)
            {
                Tuple<int, int, string> Node = new Tuple<int, int, string>(10000, 10000, "");
                Tuple<int, int, string> Node2 = new Tuple<int, int, string>(10000, 10000, "");
                if (Algoritam == "BFS" || Algoritam == "Bidirectional BFS")
                {
                    Node = lista.Last.Value;
                }
                if (Algoritam == "Greedy Best-first Search")
                {
                    foreach (var it in lista)
                        if (heuristic(Node.Item1, Node.Item2, "") >= heuristic(it.Item1, it.Item2, ""))
                            Node = it;
                }
                if (Algoritam == "A*" || Algoritam=="Bidirectional Swarm Algoritham")
                {
                    foreach (var it in lista)
                        if (heuristic(it.Item1, it.Item2, it.Item3 + (Algoritam == "A*" ? "" : it.Item3)) < heuristic(Node.Item1, Node.Item2, Node.Item3 + (Algoritam == "A*" ? "" : Node.Item3)))
                            Node = it;
                }
                if (Algoritam == "Bidirectional BFS")
                {
                    if (lista2.Count == 0) break;
                    Node2 = lista2.Last.Value;
                }
                if (Algoritam == "Bidirectional Swarm Algoritham")
                {
                    if (lista2.Count == 0) break;
                    foreach (var it in lista2)
                        if (heuristic2(it.Item1, it.Item2, it.Item3 + it.Item3) < heuristic2(Node2.Item1,Node2.Item2,Node2.Item3+Node2.Item3 ) )
                             Node2 = it;
                }
                int x = Node.Item2, x2 = Node2.Item2;
                int y = Node.Item1, y2 = Node2.Item1;
                put = Node.Item3; put2 = Node2.Item3;

                if (x == EndNode.IndexX && y == EndNode.IndexY)
                {
                    Cetka.Color = Ckraj; ;
                    g.FillRectangle(Cetka, EndNode.IndexX * sirina + 1, EndNode.IndexY * visina + 1, sirina - 1, visina - 1);
                    break;
                }

                for (int i = 0; i < 4; i++)
                {
                    int noviX = x + smjerX[i];
                    int noviY = y + smjerY[i];
                    if (noviX >= 0 && noviY >= 0 && noviY < 31 && noviX < 51 && !bio[noviY, noviX] && !graf[noviY, noviX])
                    {
                        lista.AddFirst(new Tuple<int, int, string>(noviY, noviX, put + smjer[i]));
                        bio[noviY, noviX] = true;
                        Cetka.Color = Cnode;
                        g.FillRectangle(Cetka, noviX * sirina + 1, noviY * visina + 1, sirina - 1, visina - 1);
                    }
                    if(Algoritam == "Bidirectional BFS" || Algoritam == "Bidirectional Swarm Algoritham")
                    {
                        noviX = x2 + smjerX[i];
                        noviY = y2 + smjerY[i];
                        if (noviX >= 0 && noviY >= 0 && noviY < 31 && noviX < 51 && !bio2[noviY, noviX] && !graf[noviY, noviX])
                        {
                            lista2.AddFirst(new Tuple<int, int, string>(noviY, noviX, put2 + smjer[i]));
                            bio2[noviY, noviX] = true;
                            Cetka.Color = Cnode2;
                            g.FillRectangle(Cetka, noviX * sirina + 1, noviY * visina + 1, sirina - 1, visina - 1);
                        }
                    }
                }
                if(Algoritam == "Bidirectional BFS" || Algoritam == "Bidirectional Swarm Algoritham")
                {
                    bool NasaoPut = false;
                    foreach (var k in lista2)
                        foreach (var p in lista)
                            if (k.Item1 == p.Item1 && k.Item2 == p.Item2)
                            {
                                put = p.Item3;
                                for (int i = k.Item3.Length - 1; i >= 0; i--)
                                {
                                    if (k.Item3[i] == 'r') put += 'l';
                                    else if (k.Item3[i] == 'l') put += 'r';
                                    else if (k.Item3[i] == 'u') put += 'd';
                                    else if (k.Item3[i] == 'd') put += 'u';
                                }
                                NasaoPut = true;
                            }
                    if (NasaoPut) break;
                }
                lista2.Remove(Node2);
                lista.Remove(Node);

                if (StartNode.IndexX != x || StartNode.IndexY != y)
                {
                    Cetka.Color = Cvisited;
                    g.FillRectangle(Cetka, x * sirina + 1, y * visina + 1, sirina - 1, visina - 1);
                    NoVisited_label.Text = "Visited: " + Visited++;
                }
                if ((EndNode.IndexX != x2 || EndNode.IndexY != y2) && Algoritam.Substring(0, 2) == "Bi")
                {
                    Cetka.Color = Cvisited2;
                    g.FillRectangle(Cetka, x2 * sirina + 1, y2 * visina + 1, sirina - 1, visina - 1);
                    NoVisited_label.Text = "Visited: " + Visited++;
                }
                NoVisited_label.Refresh();
                NoNode_label.Text = "Nodes: " + (lista.Count() + lista2.Count() - 1);
                NoNode_label.Refresh();
                put = "";
                System.Threading.Thread.Sleep(trackBar1.Value);
            }
            int xx = StartNode.IndexX * sirina, yy = StartNode.IndexY * visina;
            Cetka.Color = Cpath;
            for (int i = 0; i < put.Length - 1; i++)
            {
                if (put[i] == 'r') xx += sirina;
                if (put[i] == 'l') xx -= sirina;
                if (put[i] == 'd') yy += sirina;
                if (put[i] == 'u') yy -= sirina;
                g.FillRectangle(Cetka, xx + 1, yy + 1, visina - 1, visina - 1);
                System.Threading.Thread.Sleep(1);
            }
            PathLength_label.Text = "Duzina puta: " + put.Length;
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            int X = e.X - e.X % visina;
            int Y = e.Y - e.Y % sirina;
            if (X >= pictureBox1.Width - 1 || Y >= pictureBox1.Height - 1) return;

            Graphics g = pictureBox1.CreateGraphics();
            SolidBrush Cetka = new SolidBrush(Czid);

            if (Item == 'z' && Y == visina * StartNode.IndexY && X == sirina * StartNode.IndexX)//ako korisnik podize StartNode
            {
                Item = 'p';
                StartNode.SetYX(-1, -1);
            }
            else if (Item == 'z' && Y == EndNode.IndexY * visina && X == EndNode.IndexX * sirina)//ako korisnik podize kraj
            {
                Item = 'k';
                EndNode.SetYX(-1, -1);
            }
            else if (Item == 'p' && !(Y == EndNode.IndexY * visina && X == EndNode.IndexX * sirina))//ako korisnik spusta pocetak
            {
                Cetka.Color = Cpoc;
                StartNode.SetYX(Y / visina, X / sirina);
                Item = 'z';
                graf[(Y / visina), (X / sirina)] = false;
            }
            else if (Item == 'k' && !(Y == StartNode.IndexY * visina && X == StartNode.IndexX * sirina))//ako korisnik spusta kraj
            {
                Cetka.Color = Ckraj;
                EndNode.SetYX(Y / visina, X / sirina);
                Item = 'z';     
                graf[(Y / visina), (X / sirina)] = false;
            }
            else
            {
                if (Item == 'p' || Item == 'k') return;
                if (graf[(Y / visina), (X / sirina)] == true)
                {
                    Cetka.Color = Color.White;
                    graf[(Y / visina), (X / sirina)] = false;
                }
                else
                {
                    Cetka.Color = Czid;
                    graf[(Y / visina), (X / sirina)] = true;
                }
            }
            if (Item == 'p' || Item == 'k')
            {
                Cursor = Cursors.Hand;
                Cetka.Color=Color.White;
            }
            else Cursor = Cursors.Default;
            g.FillRectangle(Cetka, X + 1, Y + 1, visina - 1, visina - 1);
        }
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            int X = e.X - e.X % visina;
            int Y = e.Y - e.Y % sirina;
            if (X >= pictureBox1.Width - 1 || Y >= pictureBox1.Height - 1   ) return;
            if (graf[Y / visina, X / sirina] && Item != 'k' && Item != 'p') Item = 'P';
            Drawing = true;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (Drawing == false || Item == 'p' || Item == 'k') return;

            int X = e.X - e.X % sirina;
            int Y = e.Y - e.Y % visina;

            if ((X == StartNode.IndexX * sirina && Y == StartNode.IndexY * visina) || (X == EndNode.IndexX * sirina && Y == EndNode.IndexY * visina)
                || X >= pictureBox1.Width - 1 || Y >= pictureBox1.Height - 1 || X < 0 || Y < 0) return;

            Graphics g = pictureBox1.CreateGraphics();
            SolidBrush Cetka = new SolidBrush(Czid);

            if (Item == 'P')
            {
                Cetka.Color = Color.White;
                graf[(Y / visina), (X / sirina)] = false;
            }
            else
            {
                Cetka.Color = Czid;
                graf[(Y / visina), (X / sirina)] = true;
            }
            g.FillRectangle(Cetka, X + 1, Y + 1, visina - 1, visina - 1);
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            Drawing = false;
            if (Item == 'P') Item = 'z';
        }

        void Povezi(int y, int x)
        {
            graf[y, x] = false;
            Graphics g = pictureBox1.CreateGraphics();
            Brush Cetka = new SolidBrush(Color.White);
            g.FillRectangle(Cetka, x * sirina + 1, y * visina + 1, sirina - 1, visina - 1);
            int[] smjer = new int[4] { 0, 1, 2, 3 };
            Random rnd = new Random();
            for (int i = 0; i < 4; i++)
            {
                int j = rnd.Next(i, 4);
                int temp = smjer[i];
                smjer[i] = smjer[j];
                smjer[j] = temp;
            }
            for (int i = 0; i < 4; i++)
            {
                int dx = 0, dy = 0;
                switch (smjer[i])
                {
                    case 0: dy = -1; break;
                    case 1: dy = 1; break;
                    case 2: dx = -1; break;
                    case 3: dx = 1; break;
                }
                int x2 = x + 2 * dx;
                int y2 = y + 2 * dy;
                if (x2 >= 0 && y2 >= 0 && x2 < 51 && y2 < 31 && graf[y2, x2] == true)
                {
                    graf[y2 - dy, x2 - dx] = false;
                    g.FillRectangle(Cetka, (x2 - dx) * sirina + 1, (y2 - dy) * visina + 1, sirina - 1, visina - 1);
                    System.Threading.Thread.Sleep(1);
                    Povezi(y2, x2);
                }
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.OcistiPut_button_Click(this, null);
            Graphics g = pictureBox1.CreateGraphics();
            SolidBrush Cetka = new SolidBrush(Czid);
            Random rnd = new Random();
            if (comboBox2.SelectedIndex == 0)//nasumicne prepreke
                for (int i = 0; i < 51; i++)
                    for (int j = 0; j < 31; j++)
                        if (rnd.Next(0, 100) < 25 && (StartNode.IndexX != i || StartNode.IndexY != j) && (EndNode.IndexX != i || EndNode.IndexY != j))//25% sanse
                        {
                            graf[j, i] = true;
                            g.FillRectangle(Cetka, i * sirina + 1, j * visina + 1, sirina - 1, visina - 1);
                        }

            if (comboBox2.SelectedIndex == 1)//maze generator
            {
                Cetka.Color = Czid;
                for (int i = 0; i < 51; i++)
                    for (int j = 0; j < 31; j++)
                    {
                        graf[j, i] = true;
                        g.FillRectangle(Cetka, i * sirina + 1, j * visina + 1, sirina - 1, visina - 1);
                    }
                StartNode.SetYX(0, 1);
                EndNode.SetYX(30, 49);
                Cetka.Color = Cpoc;
                g.FillRectangle(Cetka, StartNode.IndexX * sirina + 1, StartNode.IndexY * visina + 1, sirina - 1, visina - 1);
                Cetka.Color = Ckraj;
                g.FillRectangle(Cetka, EndNode.IndexX * sirina + 1, EndNode.IndexY * visina + 1, sirina - 1, visina - 1);
                Povezi(1, 1);
                graf[0, 1] = false;
                graf[30, 49] = false;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Nacrtaj();
            timer1.Stop();
        }
    }
}