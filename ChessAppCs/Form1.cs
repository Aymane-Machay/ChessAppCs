using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessAppCs
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        ChessBoard cb;
        PictureBox gpb = new PictureBox(), gpb1 = new PictureBox();
        int s_x, s_y,selected=0;
        private void button1_Click(object sender, EventArgs e)
        {
            gpb.Visible = false;
            gpb.SendToBack();
            gpb1.Visible = false;
            gpb1.SendToBack();
            tableLayoutPanel2.Controls.Clear();

            label1.Text = "White Turn";
            cb = new ChessBoard();
            for(int itr1 =1; itr1<9; itr1++)
            {
                for (int itr2 = 1; itr2 < 9; itr2++)
                {
                    PictureBox pb = new PictureBox();
                    pb.MouseDown += on_piece_click;
                    pb.BackColor = Color.Transparent;
                    pb.BackgroundImageLayout = ImageLayout.Stretch;
                    pb.Dock = DockStyle.Fill;
                    //pb.
                    switch (itr1)
                    {
                        case 1:
                            {
                                switch(itr2)
                                {
                                    case 1:
                                        pb.Image = Image.FromFile("..\\..\\assets\\RookB.bmp");
                                        break;
                                    case 2:
                                        pb.Image = Image.FromFile("..\\..\\assets\\KnightB.bmp");
                                        break;
                                    case 3:
                                        pb.Image = Image.FromFile("..\\..\\assets\\BishopB.bmp");
                                        break;
                                    case 4:
                                        pb.Image = Image.FromFile("..\\..\\assets\\QueenB.bmp");
                                        break;
                                    case 5:
                                        pb.Image = Image.FromFile("..\\..\\assets\\KingB.bmp");
                                        break;
                                    case 6:
                                        pb.Image = Image.FromFile("..\\..\\assets\\BishopB.bmp");
                                        break;
                                    case 7:
                                        pb.Image = Image.FromFile("..\\..\\assets\\KnightB.bmp");
                                        break;
                                    case 8:
                                        pb.Image = Image.FromFile("..\\..\\assets\\RookB.bmp");
                                        break;
                                }
                            }
                            break;
                        case 2:
                            pb.Image = Image.FromFile("..\\..\\assets\\PawnB.bmp");
                            break;
                        case 7:
                            pb.Image = Image.FromFile("..\\..\\assets\\PawnW.bmp");
                            break;
                        case 8:
                            {
                                switch (itr2)
                                {
                                    case 1:
                                        pb.Image = Image.FromFile("..\\..\\assets\\RookW.bmp");
                                        break;
                                    case 2:
                                        pb.Image = Image.FromFile("..\\..\\assets\\KnightW.bmp");
                                        break;
                                    case 3:
                                        pb.Image = Image.FromFile("..\\..\\assets\\BishopW.bmp");
                                        break;
                                    case 4:
                                        pb.Image = Image.FromFile("..\\..\\assets\\QueenW.bmp");
                                        break;
                                    case 5:
                                        pb.Image = Image.FromFile("..\\..\\assets\\KingW.bmp");
                                        break;
                                    case 6:
                                        pb.Image = Image.FromFile("..\\..\\assets\\BishopW.bmp");
                                        break;
                                    case 7:
                                        pb.Image = Image.FromFile("..\\..\\assets\\KnightW.bmp");
                                        break;
                                    case 8:
                                        pb.Image = Image.FromFile("..\\..\\assets\\RookW.bmp");
                                        break;
                                }
                            }
                            break;
                    }
                    tableLayoutPanel2.Controls.Add(pb, itr2, itr1);
                }
            }
        }

        private void on_piece_click(object sender, EventArgs e)
        {
            if (selected == 0)
            {
                selected = 1;
                gpb = sender as PictureBox;
                s_x = tableLayoutPanel2.Controls.IndexOf(gpb);
                if (s_x < 8)
                    s_y = 7;
                else
                {
                    s_y = s_x / 8;
                    s_x -= s_y * 8;
                    s_y = 7 - s_y;
                }
                gpb1.Image = gpb.Image;
                //label1.Text = s_x + "  " + s_y;
                if (ChessBoard.board[s_x][s_y] != null)
                    gpb.Image = Image.FromFile("..\\..\\assets\\pieceSelected.png");
            }
            else
            {
                int x, y;
                PictureBox temp;
                gpb.Image = null;
                selected = 0;
                temp = sender as PictureBox;

                x = tableLayoutPanel2.Controls.IndexOf(temp);
                if (x < 8)
                    y = 7;
                else
                {
                    y = x / 8;
                    x -= y * 8;
                    y = 7 - y;
                }
                label1.Text = s_x + "  " + s_y;
                if (ChessBoard.board[s_x][s_y] != null && ChessBoard.board[s_x][s_y].move(x, y))
                {
                    temp.Image = gpb1.Image;
                    if (ChessBoard.turn == var.white)
                        label1.Text = "White Turn";
                    else
                        label1.Text = "Black Turn";
                }
                else
                {
                    label1.Text = "illegal move!";
                    gpb.Image = gpb1.Image;
                }
            }
            if (ChessBoard.check_b || ChessBoard.check_w)
                label1.Text = "Check";
            if (ChessBoard.checkmate_w)
                label1.Text = "Checkmate! Black wins";
            else if (ChessBoard.checkmate_b)
                label1.Text = "Checkmate! White wins";
        }
    }
}
