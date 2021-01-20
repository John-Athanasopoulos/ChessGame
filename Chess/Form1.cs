using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chess
{
    public partial class Form1 : Form
    {
        SQLiteConnection dbcon = new SQLiteConnection("Data Source=database.sqlite3");

        bool alreadycreatedlog = false;
        Point point;
        string startgametime = DateTime.Now.ToString("dddd, MMM dd yyyy, hh:mm:ss");
        bool move;
        bool timerEnable=true; //Enables the timer, used to prevent multiple timer alerts when the "No" error is selected at the exit button
        Point startOfChessboard;
        List<ChessboardBox> Boxes = new List<ChessboardBox>();
        ChessboardBox killPoint = new ChessboardBox();

        List<PictureBox> Towers = new List<PictureBox>();
        List<PictureBox> Knights = new List<PictureBox>();
        List<PictureBox> Queens = new List<PictureBox>();
        List<PictureBox> Pawns = new List<PictureBox>();
        List<PictureBox> Bishops = new List<PictureBox>();
        List<PictureBox> Kings = new List<PictureBox>();

        Player p1 = new Player(10,"John");
        Player p2 = new Player(10,"Stratis");
        int playerPlaying = 1; //1 if player1 is playing or else 2

        public Form1()
        {
            InitializeComponent();
        }

        PictureBox createOnepicBox()
        {
            PictureBox p = new PictureBox();
            p.BackColor = Color.Transparent;
            p.Width = pictureBox1.Width / 8;
            p.Height = pictureBox1.Height / 8;
            p.SizeMode = PictureBoxSizeMode.StretchImage;
            return p;
        }

        Point calculateDistanceOfPoints(Point p1, int x, int y, int currMaxDistSq,Point currPoint)
        {
            return (p1.X - x) * (p1.X - x) + (p1.Y - y) * (p1.Y - y) < currMaxDistSq ? new Point(x, y) : currPoint;
        }

        Point calcMinDist(List<ChessboardBox> cbb, Point p1)
        {
            Point min = pictureBox1.Location;
            int currmaxd = 640000;
            foreach(ChessboardBox c in cbb)
            {
                min = calculateDistanceOfPoints(p1, c.X, c.Y, currmaxd, min);
                if(currmaxd > (p1.X - c.X) * (p1.X - c.X) + (p1.Y - c.Y) * (p1.Y - c.Y))
                    currmaxd = (p1.X - c.X) * (p1.X - c.X) + (p1.Y - c.Y) * (p1.Y - c.Y);
            }
            return min;
        }

        void createAllBoxes()
        {
            for(int j = 1; j <= 64; j++)
            {
                ChessboardBox cb = new ChessboardBox(j,pictureBox1.Height/8);
                Boxes.Add(cb);
            }
            Boxes.Add(killPoint);
        }

        void troopCreation(string troop, int firstBoxIndex, int secondBoxIndex, List<PictureBox> picList)
        {
            for (int i = 0; i < 2; i++) //Black Tower creation and placement
            {
                PictureBox p = createOnepicBox();
                p.Image = Image.FromFile("images/" + troop + ".png");
                pictureBox1.Controls.Add(p);
                p.Location = i == 0 ? new Point(Boxes[firstBoxIndex].X, Boxes[firstBoxIndex].Y) : new Point(Boxes[secondBoxIndex].X, Boxes[secondBoxIndex].Y);
                picList.Add(p);
            }
        }

        void aristocratCreation(string troop, int boxIndex1, int boxIndex2, List<PictureBox> picList)
        {
            for (int i = 0; i < 2; i++) //Black Tower creation and placement
            {
                PictureBox p = createOnepicBox();
                string img = i == 0 ? troop + "Black" : troop + "White";
                p.Tag = i == 0 ? "Black" : "White";
                int ind = i == 0 ? boxIndex1 : boxIndex2;
                p.Image = Image.FromFile("images/" + img + ".png");
                pictureBox1.Controls.Add(p);
                p.Location = new Point(Boxes[ind].X, Boxes[ind].Y);
                picList.Add(p);
            }
        }

        void pawnCreation(string troop, List<PictureBox> picList)
        {
            for (int i = 0; i < 2; i++)
            {
                string img = i == 0 ? troop + "White" : troop + "Black";
                int startingIndex = i == 0 ? 48 : 8;
                for(int j = 0; j < 8; j++)
                {
                    PictureBox p = createOnepicBox();
                    p.Image = Image.FromFile("images/" + img + ".png");
                    pictureBox1.Controls.Add(p);
                    p.Location = new Point(Boxes[j + startingIndex].X, Boxes[j + startingIndex].Y);
                    picList.Add(p);
                }
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            label8.Text = DateTime.Now.ToString("dddd, MMM dd yyyy, hh:mm:ss");
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            pictureBox1.Width = Height - 100;
            pictureBox1.Height = Height - 100;
            pictureBox1.Location = new Point(Width/2 - pictureBox1.Width / 2, (Height - pictureBox1.Height)/2);

            button2.Location = new Point(Width / 2 - pictureBox1.Width / 2 - button2.Width - 20, Height / 2 - button2.Height / 2);
            button3.Location = new Point(Width / 2 + pictureBox1.Width / 2 + 20, Height / 2 - button2.Height / 2);
            button1.Location = new Point(Width / 2 + pictureBox1.Width / 2 + pictureBox1.Location.X/2 - button1.Width/2, (Height - pictureBox1.Height) / 2);
            label1.Location = new Point((Width-pictureBox1.Width)/4-label1.Width/2, (Height + pictureBox1.Height) / 2 - label1.Height);
            label2.Location = new Point((Width + pictureBox1.Location.X+pictureBox1.Width)/2 - label2.Width/2, (Height + pictureBox1.Height) / 2 - label1.Height);
            label3.Location = new Point((Width - pictureBox1.Width) / 4 - label3.Width / 2, label1.Location.Y - 20);
            label4.Location = new Point((Width + pictureBox1.Location.X + pictureBox1.Width) / 2 - label4.Width / 2, label2.Location.Y - 20);
            label5.Location = new Point((Width - pictureBox1.Width) / 4 - label5.Width / 2, (label3.Location.Y + button2.Location.Y + button2.Height) / 2 - label5.Height / 2);
            label6.Location = new Point((Width + pictureBox1.Location.X + pictureBox1.Width) / 2 - label6.Width / 2, label5.Location.Y);

            //This is the kill point
            pictureBox2.Location = new Point(pictureBox1.Location.X / 2 - pictureBox2.Width / 2, pictureBox1.Location.Y + pictureBox1.Height / 4);
            label7.Location = new Point(pictureBox1.Location.X / 2 - label7.Width / 2, pictureBox2.Location.Y - label7.Height / 2 - 20);


            label8.Location = new Point(pictureBox1.Location.X + pictureBox1.Width/2 - label8.Width / 2, pictureBox1.Location.Y/2 - label8.Height/2);


            killPoint.X = pictureBox2.Location.X;
            killPoint.Y = pictureBox2.Location.Y;

            startOfChessboard = pictureBox1.Location;
            createAllBoxes();
            troopCreation("towerWhite", 56, 63, Towers);
            troopCreation("towerBlack", 0, 7, Towers);
            troopCreation("knightBlack", 1, 6, Knights);
            troopCreation("knightWhite", 57, 62, Knights);
            troopCreation("bishopBlack", 2, 5, Bishops);
            troopCreation("bishopWhite", 58, 61, Bishops);
            aristocratCreation("King", 3, 59, Kings);
            aristocratCreation("queen", 4, 60, Queens);
            pawnCreation("pawn", Pawns);

            //Placement of the pieces on the chessboard
            towers(Towers);
            bishops(Bishops);
            knights(Knights);
            kings(Kings);
            queens(Queens);
            pawns(Pawns);

            timer1.Start();
            timer2.Start();
        }

        void knights(List<PictureBox> picboxes)
        {
            foreach (PictureBox p in picboxes)
            {
                p.MouseDown += (sender, e) => { point = e.Location; move = true; p.BringToFront(); };
                p.MouseUp += (sender, e) => { move = false;
                    if(MousePosition.X >= killPoint.X && MousePosition.X <= killPoint.X + pictureBox2.Width && MousePosition.Y >= killPoint.Y && MousePosition.Y <= killPoint.Y + pictureBox2.Height)
                    {
                        pictureBox1.Controls.Remove(p);
                    }
                    Point tempPoint = calcMinDist(Boxes, p.Location);
                    p.Location = tempPoint;
                };
                p.MouseMove += (sender, e) => { if (move) { p.Location = new Point(p.Left + e.X - point.X, p.Top + e.Y - point.Y); } };
            }
        }

        void towers(List<PictureBox> pictureBoxes)
        {
            foreach(PictureBox p in pictureBoxes)
            {
                p.MouseDown += (sender, e) => { point = e.Location; move = true; p.BringToFront(); };
                p.MouseUp += (sender, e) => {
                    move = false;
                    if (MousePosition.X >= killPoint.X && MousePosition.X <= killPoint.X + pictureBox2.Width && MousePosition.Y >= killPoint.Y && MousePosition.Y <= killPoint.Y + pictureBox2.Height)
                    {
                        pictureBox1.Controls.Remove(p);
                    }
                    Point tempPoint = calcMinDist(Boxes, p.Location);
                    p.Location = tempPoint;
                };
                p.MouseMove += (sender, e) => { if (move) { p.Location = new Point(p.Left + e.X - point.X, p.Top + e.Y - point.Y); } };
            }
        }

        void pawns(List<PictureBox> pictureBoxes)
        {
            foreach (PictureBox p in pictureBoxes)
            {
                p.MouseDown += (sender, e) => { point = e.Location; move = true; p.BringToFront(); };
                p.MouseUp += (sender, e) => {
                    move = false;
                    if (MousePosition.X >= killPoint.X && MousePosition.X <= killPoint.X + pictureBox2.Width && MousePosition.Y >= killPoint.Y && MousePosition.Y <= killPoint.Y + pictureBox2.Height)
                    {
                        pictureBox1.Controls.Remove(p);
                    }
                    Point tempPoint = calcMinDist(Boxes, p.Location);
                    p.Location = tempPoint;
                };
                p.MouseMove += (sender, e) => { if (move) { p.Location = new Point(p.Left + e.X - point.X, p.Top + e.Y - point.Y); } };
            }
        }

        void queens(List<PictureBox> pictureBoxes)
        {
            foreach (PictureBox p in pictureBoxes)
            {
                p.MouseDown += (sender, e) => { point = e.Location; move = true; p.BringToFront(); };
                p.MouseUp += (sender, e) => {
                    move = false;
                    if (MousePosition.X >= killPoint.X && MousePosition.X <= killPoint.X + pictureBox2.Width && MousePosition.Y >= killPoint.Y && MousePosition.Y <= killPoint.Y + pictureBox2.Height)
                    {
                        pictureBox1.Controls.Remove(p);
                    }
                    Point tempPoint = calcMinDist(Boxes, p.Location);
                    p.Location = tempPoint;
                };
                p.MouseMove += (sender, e) => { if (move) { p.Location = new Point(p.Left + e.X - point.X, p.Top + e.Y - point.Y); } };
            }
        }

        void bishops(List<PictureBox> pictureBoxes)
        {
            foreach (PictureBox p in pictureBoxes)
            {
                p.MouseDown += (sender, e) => { point = e.Location; move = true; p.BringToFront(); };
                p.MouseUp += (sender, e) => {
                    move = false;
                    if (MousePosition.X >= killPoint.X && MousePosition.X <= killPoint.X + pictureBox2.Width && MousePosition.Y >= killPoint.Y && MousePosition.Y <= killPoint.Y + pictureBox2.Height)
                    {
                        pictureBox1.Controls.Remove(p);
                    }
                    Point tempPoint = calcMinDist(Boxes, p.Location);
                    p.Location = tempPoint;
                };
                p.MouseMove += (sender, e) => { if (move) { p.Location = new Point(p.Left + e.X - point.X, p.Top + e.Y - point.Y); } };
            }
        }

        void kings(List<PictureBox> pictureBoxes)
        {
            foreach (PictureBox p in pictureBoxes)
            {
                p.MouseDown += (sender, e) => { point = e.Location; move = true; p.BringToFront(); };
                p.MouseUp += (sender, e) => {
                    move = false;
                    if (MousePosition.X >= killPoint.X && MousePosition.X <= killPoint.X + pictureBox2.Width && MousePosition.Y >= killPoint.Y && MousePosition.Y <= killPoint.Y + pictureBox2.Height)
                    {
                        if (p.Tag.Equals("White")){
                            timer1.Stop();
                            timerEnable = false;
                            foreach (Control c in pictureBox1.Controls)
                            {
                                c.Enabled = false;
                            }
                            dbcon.Open();
                            string query = "insert into logs(player1,player2,startofgame, endofgame,winner) values('" + p1.name + "','" + p2.name + "','" + startgametime + "','" + DateTime.Now.ToString("dddd, MMM dd yyyy, hh:mm:ss") + "','" + p2.name + "')";
                            SQLiteCommand cmd = new SQLiteCommand(query, dbcon);
                            cmd.ExecuteNonQuery();
                            dbcon.Close();
                            alreadycreatedlog = true;
                            MessageBox.Show(p1.name + " lost their king!\n" + p2.name + " Wins!");
                        }
                        else
                        {
                            timer1.Stop();
                            timerEnable = false;
                            foreach (Control c in pictureBox1.Controls)
                            {
                                c.Enabled = false;
                            }
                            dbcon.Open();
                            string query = "insert into logs(player1,player2,startofgame, endofgame,winner) values('" + p1.name + "','" + p2.name + "','" + startgametime + "','" + DateTime.Now.ToString("dddd, MMM dd yyyy, hh:mm:ss") + "','" + p1.name + "')";
                            SQLiteCommand cmd = new SQLiteCommand(query, dbcon);
                            cmd.ExecuteNonQuery();
                            dbcon.Close();
                            alreadycreatedlog = true;
                            MessageBox.Show(p2.name + " lost their king!\n" + p1.name + " Wins!");
                        }
                        pictureBox1.Controls.Remove(p);
                    }
                    Point tempPoint = calcMinDist(Boxes, p.Location);
                    p.Location = tempPoint;
                };
                p.MouseMove += (sender, e) => { if (move) { p.Location = new Point(p.Left + e.X - point.X, p.Top + e.Y - point.Y); } };
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            DialogResult dialogResult = MessageBox.Show("Do you really wish to exit?", "Exit Game", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes && !alreadycreatedlog)
            {
                dbcon.Open();
                string endgametime = DateTime.Now.ToString("dddd, MMM dd yyyy, hh:mm:ss");
                string query = "insert into logs(player1,player2,startofgame, endofgame, winner) values('" + p1.name + "','" + p2.name + "','" + startgametime + "','" +endgametime + "','" + "No winner" +"')";
                SQLiteCommand cmd = new SQLiteCommand(query, dbcon);
                cmd.ExecuteNonQuery();
                dbcon.Close();

                Application.Exit();
            }
            else if(dialogResult == DialogResult.Yes && alreadycreatedlog)
            {
                Application.Exit();
            }
            else
            {
                if (timerEnable)
                {
                    timer1.Start();
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (p1.currentSeconds == 0)
            {
                timer1.Stop();
                timerEnable = false;
                foreach(Control c in pictureBox1.Controls)
                {
                    c.Enabled = false;
                }
                dbcon.Open();
                string endgametime = DateTime.Now.ToString("dddd, MMM dd yyyy, hh:mm:ss");
                string query = "insert into logs(player1,player2,startofgame, endofgame,winner) values('" + p1.name + "','" + p2.name + "','" + startgametime + "','" + endgametime + "','" + p2.name +"')";
                SQLiteCommand cmd = new SQLiteCommand(query, dbcon);
                cmd.ExecuteNonQuery();
                dbcon.Close();
                alreadycreatedlog = true;
                MessageBox.Show(p1.name + " ran out of time!\n" + p2.name + " Wins!");
            }
            else if (p2.currentSeconds == 0)
            {
                timer1.Stop();
                timerEnable = false;
                foreach (Control c in pictureBox1.Controls)
                {
                    c.Enabled = false;
                }
                dbcon.Open();
                string endgametime = DateTime.Now.ToString("dddd, MMM dd yyyy, hh:mm:ss");
                string query = "insert into logs(player1,player2,startofgame, endofgame,winner) values('" + p1.name + "','" + p2.name + "','" + startgametime + "','" + endgametime + "','" + p1.name+"')";
                SQLiteCommand cmd = new SQLiteCommand(query, dbcon);
                cmd.ExecuteNonQuery();
                dbcon.Close();
                alreadycreatedlog = true;
                MessageBox.Show(p2.name + " ran out of time!\n" + p1.name + " Wins!");
            }
            else
            {
                if (playerPlaying == 1)
                {
                    p2.enable = false;
                    p1.enable = true;
                    p1.calculateRemainingSeconds();
                    label1.Text = p1.currentSeconds.ToString();
                }
                if (playerPlaying == 2)
                {
                    p1.enable = false;
                    p2.enable = true;
                    p2.calculateRemainingSeconds();
                    label2.Text = p2.currentSeconds.ToString();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            playerPlaying = 2;
            button2.Enabled = false;
            button3.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            playerPlaying = 1;
            button3.Enabled = false;
            button2.Enabled = true;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            label8.Text = DateTime.Now.ToString("dddd, MMM dd yyyy, hh:mm:ss");
        }
    }
}
