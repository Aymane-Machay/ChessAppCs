using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace ChessAppCs
{
    /*
     * How it works:
     *      a game is started by creating a chessboard object which would via it's constructor create a 2 dimensional list of pieces that servers as a table and initialize each piece
     *      to it's correct placement.
     *      the chessboard is our interactive class, it's throught it that we move pieces and store informations like check and checkmate status, turn and the zone that cause the check status
     *      Piece is the mother class for each individual piece, allowing for backward communication of things like pieces that cause a check and methods to peek unto (check status, checkmate, turn)
     *      as well as switching turn and an absolute method for integers
     *      each piece has a constructor whose job is to make sure the piece got loaded in the correct placement as well as a move function that work to check legal moves as well as apply them
     * Disabled option:
     *      Singleton chessboard -> can be enabled by setting the constructor to private and uncommenting the code in the class chessboard
     *      Smart CheckMate -> although through the check zone and check list it's possible to do that i opted for the much simpler method to detected a checkmate:
     *              if there is two succesif checks on the same player then it's a checkmate
     *              if there is two pieces causing the check, then removing a piece would still keep the king in check thus a checkmate
     *              there is also a code to test if the king can move out of the way, but i optted to comment both as the first condition suffices to accurately detect a checkmate
     *      Castle Docking -> being an optional move i opted not to add it although i did make the code for it commented in the king.move() method / ps if you chose to add it you need to
     *                          add some code in the frontend to accurately display the docking
     *      Pawn to Queen conversion -> another optional move i chose not to add, can be implemented by destroying the pawn and creating a queen piece in the end rows y==0||y==7
     *                                  If chosen, the queen constructor needs to be updated to accept such a move (you only need to add a potential placement on the opposite side of the player's color
     */
    public enum var { a, b, c, d, e, f, g, h, white = 40, black = 50, king, queen, rook, bishop, knight, pawn };
    struct point
    {
        public int x;
        public int y;
    }

    class ChessBoard
    {
        public static var turn;
        public ChessBoard()
        {
            checkmate_w = false;
            checkmate_b = false;
            check_w = false;
            check_b = false;
            turn = var.white;
            board = new List<List<Piece>>();
            check_zone = new List<point>();
            for (int i = 0; i < 8; i++)
            {
                board.Add(new List<Piece>());
                for (int j = 0; j < 8; j++)
                {
                    board[i].Add(new Piece());
                    board[i][j] = null;
                }
            }
            board[3][0] = new Queen(3, 0, var.white);
            board[3][7] = new Queen(3, 7, var.black);

            board[4][0] = new King(4, 0, var.white);
            board[4][7] = new King(4, 7, var.black);

            board[5][0] = new Bishop(5, 0, var.white);
            board[2][0] = new Bishop(2, 0, var.white);
            board[5][7] = new Bishop(5, 7, var.black);
            board[2][7] = new Bishop(2, 7, var.black);

            board[0][0] = new Rook(0, 0, var.white);
            board[7][0] = new Rook(7, 0, var.white);
            board[0][7] = new Rook(0, 7, var.black);
            board[7][7] = new Rook(7, 7, var.black);

            board[1][0] = new Knight(1, 0, var.white);
            board[6][0] = new Knight(6, 0, var.white);
            board[1][7] = new Knight(1, 7, var.black);
            board[6][7] = new Knight(6, 7, var.black);

            for (int itr = 0; itr <= 7; itr++)
            {
                board[itr][1] = new Pawn(itr, 1, var.white);
            }
            for (int itr = 0; itr <= 7; itr++)
            {
                board[itr][6] = new Pawn(itr, 6, var.black);
            }
        }
        public static  bool checkmate_w,checkmate_b,check_w,check_b;

        public static List<List<Piece>> board;
        public static List<point> check_zone;
        /*public ChessBoard instance;
          
          public static ChessBoard Load()
            {
                if (instance == null)
                {
                    instance = new ChessBoard();
                }
                return instance;
            }*/
    };

    class Piece
    {
        public var type;
        public int x_pos, y_pos;
        public var color; // 40 for white 50 for black
        static protected int wkx_pos, wky_pos, bkx_pos, bky_pos;
        static protected List<Piece> check_list;

        //return true if the piece blocks a check
        protected bool simulate_piece_block(int old_x, int old_y, int new_x, int new_y)
        {
            bool res;
            Piece foo = ChessBoard.board[new_x][new_y];
            if (ChessBoard.board[new_x][new_y] == null || ChessBoard.board[new_x][new_y].color != color)
            {
                ChessBoard.board[new_x][new_y] = ChessBoard.board[old_x][old_y];
                ChessBoard.board[old_x][old_y] = null;
                if (color == var.white)
                    res = check(wkx_pos, wky_pos);
                else
                    res = check(bkx_pos, bky_pos);
                ChessBoard.board[old_x][old_y] = ChessBoard.board[new_x][new_y];
                ChessBoard.board[new_x][new_y] = foo;
                return res;
            }
            return false;
        }

        public virtual bool move(int x, int y)
        {
            return false;
        }

        protected int abs(int x)
        {
            if (x < 0)
                return -x;
            else
                return x;
        }

        //returns true if the piece isn't checked
        protected bool check(int x, int y)
        {
            if (x < 8 && y < 8)
            {
                var c;
                int itr;
                if (ChessBoard.board[x][y].color == var.white)
                {
                    c = var.black;
                    if (y != 7 && (x != 7 && ChessBoard.board[x + 1][y +1] != null && (ChessBoard.board[x + 1][y + 1].type == var.pawn || ChessBoard.board[x + 1][y + 1].type == var.king) && ChessBoard.board[x + 1][y + 1].color == c))
                    {
                        check_list.Add(ChessBoard.board[x + 1][y + 1]);
                        ChessBoard.check_w = true;
                        return false;
                    }
                    else if (y!=7 && x != 0 && ChessBoard.board[x - 1][y + 1] != null && (ChessBoard.board[x - 1][y + 1].type == var.pawn || ChessBoard.board[x - 1][y + 1].type == var.king) && ChessBoard.board[x - 1][y + 1].color == c)
                    {
                        check_list.Add(ChessBoard.board[x - 1][y + 1]);
                        ChessBoard.check_w = true;
                        return false;
                    }
                }
                else
                {
                    c = var.white;
                    if (y != 0 && (x != 7 && ChessBoard.board[x + 1][y - 1] != null && (ChessBoard.board[x + 1][y - 1].type == var.pawn || ChessBoard.board[x + 1][y - 1].type == var.king) && ChessBoard.board[x + 1][y - 1].color == c))
                    {
                        check_list.Add(ChessBoard.board[x + 1][y - 1]);
                        ChessBoard.check_b = true;
                        return false;
                    }
                    else if (y != 0 &&  ChessBoard.board[x - 1][y - 1] != null && x != 0 && (ChessBoard.board[x - 1][y - 1].type == var.pawn || ChessBoard.board[x - 1][y - 1].type == var.king) && ChessBoard.board[x - 1][y - 1].color == c)
                    {
                        check_list.Add(ChessBoard.board[x - 1][y - 1]);
                        ChessBoard.check_b = true;
                        return false;
                    }
                }
                if (x < 7 && y < 6 && ChessBoard.board[x + 1][y + 2] != null && ChessBoard.board[x + 1][y + 2].type == var.knight && ChessBoard.board[x + 1][y + 2].color == c)
                {
                    check_list.Add(ChessBoard.board[x+1][y+2]);
                    if (color == var.white)
                        ChessBoard.check_w = true;
                    else
                        ChessBoard.check_b = true;
                    return false;
                }
                if (x > 0 && y < 6 && ChessBoard.board[x - 1][y + 2] != null && ChessBoard.board[x - 1][y + 2].type == var.knight && ChessBoard.board[x - 1][y + 2].color == c)
                {
                    check_list.Add(ChessBoard.board[x - 1][y + 2]);
                    if (color == var.white)
                        ChessBoard.check_w = true;
                    else
                        ChessBoard.check_b = true;
                    return false;
                }
                if (x < 7 && y > 1 && ChessBoard.board[x + 1][y - 2] != null && ChessBoard.board[x + 1][y - 2].type == var.knight && ChessBoard.board[x + 1][y - 2].color == c)
                {
                    check_list.Add(ChessBoard.board[x + 1][y - 2]);
                    if (color == var.white)
                        ChessBoard.check_w = true;
                    else
                        ChessBoard.check_b = true;
                    return false;
                }
                if (x > 0 && y > 1 && ChessBoard.board[x - 1][y - 2] != null && ChessBoard.board[x - 1][y - 2].type == var.knight && ChessBoard.board[x - 1][y - 2].color == c)
                {
                    check_list.Add(ChessBoard.board[x - 1][y - 2]);
                    if (color == var.white)
                        ChessBoard.check_w = true;
                    else
                        ChessBoard.check_b = true;
                    return false;
                }
                if (x < 6 && y < 7 && ChessBoard.board[x + 2][y + 1] != null && ChessBoard.board[x + 2][y + 1].type == var.knight && ChessBoard.board[x + 2][y + 1].color == c)
                {
                    check_list.Add(ChessBoard.board[x +2][y + 1]);
                    if (color == var.white)
                        ChessBoard.check_w = true;
                    else
                        ChessBoard.check_b = true;
                    return false;
                }
                if (x > 1 && y < 7 && ChessBoard.board[x - 2][y + 1] != null && ChessBoard.board[x - 2][y + 1].type == var.knight && ChessBoard.board[x - 2][y + 1].color == c)
                {
                    check_list.Add(ChessBoard.board[x - 2][y + 1]);
                    if (color == var.white)
                        ChessBoard.check_w = true;
                    else
                        ChessBoard.check_b = true;
                    return false;
                }
                if (x < 6 && y > 0 && ChessBoard.board[x + 2][y - 1] != null && ChessBoard.board[x + 2][y - 1].type == var.knight && ChessBoard.board[x + 2][y - 1].color == c)
                {
                    check_list.Add(ChessBoard.board[x + 2][y - 1]);
                    if (color == var.white)
                        ChessBoard.check_w = true;
                    else
                        ChessBoard.check_b = true;
                    return false;
                }
                if (x > 1 && y > 0 && ChessBoard.board[x - 2][y - 1] != null && ChessBoard.board[x - 2][y - 1].type == var.knight && ChessBoard.board[x - 2][y - 1].color == c)
                {
                    check_list.Add(ChessBoard.board[x - 2][y - 1]);
                    if (color == var.white)
                        ChessBoard.check_w = true;
                    else
                        ChessBoard.check_b = true;
                    return false;
                }
                if (x < 7)
                {
                    for (itr = x + 1; itr < 8; itr++)
                    {
                        if (ChessBoard.board[itr][y] != null)
                        {
                            if (ChessBoard.board[itr][y].color == c && (ChessBoard.board[itr][y].type == var.queen || ChessBoard.board[itr][y].type == var.rook || (ChessBoard.board[itr][y].type == var.king && itr == x + 1)))
                            {
                                check_list.Add(ChessBoard.board[itr][y]);
                                if (color == var.white)
                                    ChessBoard.check_w = true;
                                else
                                    ChessBoard.check_b = true;
                                return false;
                            }
                            break;
                        }
                    }
                    if (y > 0)
                    {
                        for (int itr1 = x + 1, itr2 = y - 1; itr1 <= 7 && itr2 >= 0; itr1++, itr2--)
                        {
                            if (ChessBoard.board[itr1][itr2] != null)
                            {
                                if (ChessBoard.board[itr1][itr2].color == c && (ChessBoard.board[itr1][itr2].type == var.queen || ChessBoard.board[itr1][itr2].type == var.bishop))
                                {
                                    check_list.Add(ChessBoard.board[itr1][itr2]);
                                    if (color == var.white)
                                        ChessBoard.check_w = true;
                                    else
                                        ChessBoard.check_b = true;
                                    return false;
                                }
                                break;
                            }
                        }
                    }
                    if (y < 7)
                    {
                        for (int itr1 = x + 1, itr2 = y + 1; itr1 <= 7 && itr2 <= 7; itr1++, itr2++)
                        {
                            if (ChessBoard.board[itr1][itr2] != null)
                            {
                                if (ChessBoard.board[itr1][itr2].color == c && (ChessBoard.board[itr1][itr2].type == var.queen || ChessBoard.board[itr1][itr2].type == var.bishop))
                                {
                                    check_list.Add(ChessBoard.board[itr1][itr2]);
                                    if (color == var.white)
                                        ChessBoard.check_w = true;
                                    else
                                        ChessBoard.check_b = true;
                                    return false;
                                }
                                break;
                            }
                        }
                    }
                }
                if (x > 0)
                {
                    for (itr = x - 1; itr >= 0; itr--)
                    {
                        if (ChessBoard.board[itr][y] != null)
                        {
                            if (ChessBoard.board[itr][y].color == c && (ChessBoard.board[itr][y].type == var.queen || ChessBoard.board[itr][y].type == var.rook || (ChessBoard.board[itr][y].type == var.king && itr == x - 1)))
                            {
                                check_list.Add(ChessBoard.board[itr][y]);
                                if (color == var.white)
                                    ChessBoard.check_w = true;
                                else
                                    ChessBoard.check_b = true;
                                return false;
                            }
                            break;
                        }
                    }
                    if (y > 0)
                    {
                        for (int itr1 = x - 1, itr2 = y - 1; itr1 >= 0 && itr2 >= 0; itr1--, itr2--)
                        {
                            if (ChessBoard.board[itr1][itr2] != null)
                            {
                                if (ChessBoard.board[itr1][itr2].color == c && (ChessBoard.board[itr1][itr2].type == var.queen || ChessBoard.board[itr1][itr2].type == var.bishop))
                                {
                                    check_list.Add(ChessBoard.board[itr1][itr2]);
                                    if (color == var.white)
                                        ChessBoard.check_w = true;
                                    else
                                        ChessBoard.check_b = true;
                                    return false;
                                }
                                break;
                            }
                        }
                    }
                    if (y < 7)
                    {
                        for (int itr1 = x - 1, itr2 = y + 1; itr1 >= 0 && itr2 <= 7; itr1--, itr2++)
                        {
                            if (ChessBoard.board[itr1][itr2] != null)
                            {
                                if (ChessBoard.board[itr1][itr2].color == c && (ChessBoard.board[itr1][itr2].type == var.queen || ChessBoard.board[itr1][itr2].type == var.bishop))
                                {
                                    check_list.Add(ChessBoard.board[itr1][itr2]);
                                    if (color == var.white)
                                        ChessBoard.check_w = true;
                                    else
                                        ChessBoard.check_b = true;
                                    return false;
                                }
                                break;
                            }
                        }
                    }
                }
                if (y < 7)
                {
                    for (itr = y + 1; itr < 8; itr++)
                    {
                        if (ChessBoard.board[x][itr] != null)
                        {
                            if (ChessBoard.board[x][itr].color == c && (ChessBoard.board[x][itr].type == var.queen || ChessBoard.board[x][itr].type == var.rook) || (ChessBoard.board[x][itr].type == var.king && itr == y + 1))
                            {
                                check_list.Add(ChessBoard.board[x][itr]);
                                if (color == var.white)
                                    ChessBoard.check_w = true;
                                else
                                    ChessBoard.check_b = true;
                                return false;
                            }
                            break;
                        }
                    }
                }
                if (y > 0)
                {
                    for (itr = y - 1; itr >= 0; itr--)
                    {
                        if (ChessBoard.board[x][itr] != null)
                        {
                            if (ChessBoard.board[x][itr].color == c && (ChessBoard.board[x][itr].type == var.queen || ChessBoard.board[x][itr].type == var.rook) || (ChessBoard.board[itr][y].type == var.king && itr == y - 1))
                            {
                                check_list.Add(ChessBoard.board[x][itr]);
                                if (color == var.white)
                                    ChessBoard.check_w = true;
                                else
                                    ChessBoard.check_b = true;
                                return false;
                            }
                            break;
                        }
                    }
                }
                if (color == var.white)
                    ChessBoard.check_w = false;
                else
                    ChessBoard.check_b = false;
                return true;
            }
            else
            {
                Exception e = new Exception();
                throw e;
            }
        }

        protected bool test_checkmate()
        {
            int x, y;
            if (ChessBoard.turn == var.white)
            {
                x = bkx_pos;
                y = bky_pos;
                if (ChessBoard.check_b && !check(x, y))
                {
                    ChessBoard.checkmate_w = true;
                    return true;
                }
            }
            else
            {
                x = wkx_pos;
                y = wky_pos;
                if (ChessBoard.check_w && !check(x, y))
                {
                    ChessBoard.checkmate_b = true;
                    return true;
                }
            }
            /*if (!check(x, y))
            {
                if (check_list.Count > 1)
                {
                    if (ChessBoard.turn == var.white)
                        ChessBoard.checkmate_b = true;
                    else
                        ChessBoard.checkmate_w = true;
                    return true;
                }
                else
                {
                    int i, j;
                    for (i = -1; i < 2; i++)
                    {
                        for (j = -1; j < 2; j++)
                        {
                            if (move(i, j))
                            {
                                change_turn();
                                if (check(i, j))
                                {
                                    move(x, y);
                                    change_turn();
                                    return false;
                                }
                                move(x, y);
                                change_turn();
                            }
                        }
                    }
                    check(x, y);
                    int x1, y1;
                    x1 = check_list[0].x_pos;
                    y1 = check_list[0].y_pos;
                    if (!check(x1, y1))
                        return false;
                    return true;
                }
            }*/
            return false;
        }

        protected bool check_turn()
        {
            if (color == ChessBoard.turn)
                return true;
            else
                return false;
        }

        protected void change_turn()
        {
            if (ChessBoard.turn == var.white)
                ChessBoard.turn = var.black;
            else
                ChessBoard.turn = var.white;
        }
    };

    class Pawn : Piece
    {
        public Pawn(int x, int y, var c)
        {
            Exception e = new Exception();
            type = var.pawn;
            color = c;
            if (color == var.white && y != 1 || color == var.black && y != 6 || x > 7 || y > 7 || ChessBoard.board[x][y] != null)
                throw e;
            x_pos = x;
            y_pos = y;
        }
        public override bool move(int x, int y)
        {            
            bool illegal_move = false;
            if (check_turn())
            {
                if (x < 8 && y < 8)
                {
                    int mv_x = abs(x - x_pos), mv_y;
                    if (color == var.white)
                        mv_y = y - y_pos;
                    else
                        mv_y = y_pos - y;
                    if (mv_x == 0)
                    {
                        if (mv_y == 1)
                        {
                            Piece ptr = ChessBoard.board[x][y];
                            if (ptr != null)
                                illegal_move = true;
                        }
                        else if (mv_y == 2)
                        {
                            Piece ptr1 = ChessBoard.board[x][y];
                            Piece ptr2;
                            if (color == var.white)
                                ptr2 = ChessBoard.board[x][y - 1];
                            else
                                ptr2 = ChessBoard.board[x][y + 1];
                            if (ptr1 != null || ptr2 != null || (y_pos != 1 && color == var.white) || (y_pos != 6 && color == var.black))
                                illegal_move = true;
                        }
                        else
                            illegal_move = true;
                    }
                    else if (mv_x == 1 && mv_y == 1)
                    {
                        Piece ptr = ChessBoard.board[x][y];
                        if (ptr == null || ptr.color == color)
                            illegal_move = true;
                        else
                        {
                            ChessBoard.board[x][y] = null;
                        }
                    }
                    else
                        illegal_move = true;
                    if (!illegal_move)
                    {
                        change_turn();
                        ChessBoard.board[x_pos][y_pos] = null;
                        ChessBoard.board[x][y] = this;
                        x_pos = x;
                        y_pos = y;
                    }
                }
                else
                    illegal_move = true;
            }
            else
                illegal_move = true;
            test_checkmate();
            if (color == var.white)
                check(bkx_pos, bky_pos);
            else
                check(wkx_pos, wky_pos);
            return !illegal_move;
        }
    };

    class Rook : Piece
    {
        public Rook(int x, int y, var c)
        {
            Exception e = new Exception();
            type = var.rook;
            color = c;
            if (color == var.white && (y != 0 || x != 0 && x != 7) || color == var.black && (y != 7 || x != 0 && x != 7) || x > 7 || y > 7 || ChessBoard.board[x][y] != null)
                throw e;
            x_pos = x;
            y_pos = y;
        }
        public override bool move(int x, int y)
        {
            bool illegal_move = false;
            if (check_turn())
            {
                if (x < 8 && y < 8)
                {
                    int mv_x = x - x_pos, mv_y = y - y_pos;
                    if (mv_x != 0 && mv_y != 0)
                        illegal_move = true;
                    else
                    {
                        if (mv_x == 0)
                        {
                            if (mv_y < 0)
                            {
                                for (int itr = y_pos - 1; itr > y; itr--)
                                {
                                    if (ChessBoard.board[x][itr] != null)
                                        illegal_move = true;
                                }
                            }
                            else if (mv_y > 0)
                            {
                                for (int itr = y_pos + 1; itr < y; itr++)
                                {
                                    if (ChessBoard.board[x][itr] != null)
                                        illegal_move = true;
                                }
                            }
                        }
                        else
                        {
                            if (mv_x < 0)
                            {
                                for (int itr = x_pos - 1; itr > x; itr--)
                                {
                                    if (ChessBoard.board[itr][y] != null)
                                        illegal_move = true;
                                }
                            }
                            else if (mv_x > 0)
                            {
                                for (int itr = x_pos + 1; itr < x; itr++)
                                {
                                    if (ChessBoard.board[itr][y] != null)
                                        illegal_move = true;
                                }
                            }
                        }
                        if (ChessBoard.board[x][y] != null && ChessBoard.board[x][y].color == color)
                            illegal_move = true;
                    }
                    if (!illegal_move)
                    {
                        change_turn();
                        if (ChessBoard.board[x][y] != null)
                        {
                            ChessBoard.board[x][y] = null;
                        }
                        ChessBoard.board[x_pos][y_pos] = null;
                        ChessBoard.board[x][y] = this;
                        x_pos = x;
                        y_pos = y;
                    }
                }
                else
                    illegal_move = true;
            }
            else
                illegal_move = true;
            test_checkmate();
            if (color == var.white)
                check(bkx_pos, bky_pos);
            else
                check(wkx_pos, wky_pos);
            return !illegal_move;
        }
    };

    class Bishop : Piece
    {
        public Bishop(int x, int y, var c)
        {
            Exception e = new Exception();
            type = var.bishop;
            color = c;
            if (color == var.white && (y != 0 || x != 2 && x != 5) || color == var.black && (y != 7 || x != 2 && x != 5) || x > 7 || y > 7 || ChessBoard.board[x][y] != null)
                throw e;
            x_pos = x;
            y_pos = y;
        }
        public override bool move(int x, int y)
        {
            bool illegal_move = false;
            if (check_turn())
            {
                if (x < 8 && y < 8)
                {
                    int mv_x = x - x_pos, mv_y = y - y_pos, diff = (abs(mv_x) - abs(mv_y));
                    if (diff != 0)
                        illegal_move = true;
                    else
                    {
                        if (mv_x > 0)
                        {
                            if (mv_y < 0)
                            {
                                for (int itr_x = x_pos + 1, itr_y = y_pos - 1; itr_x < x && itr_y > y; itr_x++, itr_y--)
                                {
                                    if (ChessBoard.board[itr_x][itr_y] != null)
                                        illegal_move = true;
                                }
                            }
                            else
                            {
                                for (int itr_x = x_pos + 1, itr_y = y_pos + 1; itr_x < x && itr_y < y; itr_x++, itr_y++)
                                {
                                    if (ChessBoard.board[itr_x][itr_y] != null)
                                        illegal_move = true;
                                }
                            }
                        }
                        else
                        {
                            if (mv_y < 0)
                            {
                                for (int itr_x = x_pos - 1, itr_y = y_pos - 1; itr_x > x && itr_y > y; itr_x--, itr_y--)
                                {
                                    if (ChessBoard.board[itr_x][itr_y] != null)
                                        illegal_move = true;
                                }
                            }
                            else
                            {
                                for (int itr_x = x_pos - 1, itr_y = y_pos + 1; itr_x > x && itr_y < y; itr_x--, itr_y++)
                                {
                                    if (ChessBoard.board[itr_x][itr_y] != null)
                                        illegal_move = true;
                                }
                            }
                        }
                        if (ChessBoard.board[x][y] != null && ChessBoard.board[x][y].color == color)
                            illegal_move = true;
                    }
                    if (!illegal_move)
                    {
                        change_turn();
                        if (ChessBoard.board[x][y] != null)
                        {
                            ChessBoard.board[x][y] = null;
                        }
                        ChessBoard.board[x_pos][y_pos] = null;
                        ChessBoard.board[x][y] = this;
                        x_pos = x;
                        y_pos = y;
                    }
                }
                else
                    illegal_move = true;
            }
            else
                illegal_move = true;
            test_checkmate();
            if (color == var.white)
                check(bkx_pos, bky_pos);
            else
                check(wkx_pos, wky_pos);
            return !illegal_move;
        }
    };

    class Knight : Piece
    {
        public Knight(int x, int y, var c)
        {
            Exception e = new Exception();
            type = var.knight;
            color = c;
            if (color == var.white && (y != 0 || x != 1 && x != 6) || color == var.black && (y != 7 || x != 1 && x != 6) || x > 7 || y > 7 || ChessBoard.board[x][y] != null)
                throw e;
            x_pos = x;
            y_pos = y;
        }
        public override bool move(int x, int y)
        {
            bool illegal_move = false;
            if (check_turn())
            {
                if (x < 8 && y < 8)
                {
                    int mv_x = abs((x - x_pos)), mv_y = abs((y - y_pos));
                    if (mv_x == 1 && mv_y == 2 || mv_x == 2 && mv_y == 1)
                    {
                        if (ChessBoard.board[x][y] != null && ChessBoard.board[x][y].color == color)
                            illegal_move = true;
                        else
                        {
                            change_turn();
                            if (ChessBoard.board[x][y] != null)
                            {
                                ChessBoard.board[x][y] = null;
                            }
                            ChessBoard.board[x_pos][y_pos] = null;
                            ChessBoard.board[x][y] = this;
                            x_pos = x;
                            y_pos = y;
                        }
                    }
                    else
                        illegal_move = true;
                }
                else
                    illegal_move = true;
            }
            else
                illegal_move = true;
            test_checkmate();
            if (color == var.white)
                check(bkx_pos, bky_pos);
            else
                check(wkx_pos, wky_pos);
            return !illegal_move;
        }
    };

    class Queen : Piece
    {
        public Queen(int x, int y, var c)
        {
            Exception e = new Exception();
            type = var.queen;
            color = c;
            if (color == var.white && (y != 0 || x != 3) || color == var.black && (y != 7 || x != 3) || x > 7 || y > 7 || ChessBoard.board[x][y] != null)
                throw e;
            x_pos = x;
            y_pos = y;
        }
        public override bool move(int x, int y)
        {
            bool illegal_move = false;
            if (check_turn())
            {
                if (x < 8 && y < 8)
                {
                    int mv_x = x - x_pos, mv_y = y - y_pos, diff = (abs(mv_x) - abs(mv_y));
                    if (diff != 0 && mv_x != 0 && mv_y != 0)
                        illegal_move = true;
                    else
                    {
                        if (diff != 0)
                        {
                            if (mv_x == 0)
                            {
                                if (mv_y < 0)
                                {
                                    for (int itr = y_pos - 1; itr < y; itr--)
                                    {
                                        if (ChessBoard.board[x][itr] != null)
                                            illegal_move = true;
                                    }
                                }
                                else if (mv_y > 0)
                                {
                                    for (int itr = y_pos + 1; itr < y; itr++)
                                    {
                                        if (ChessBoard.board[x][itr] != null)
                                            illegal_move = true;
                                    }
                                }
                            }
                            else if (mv_y == 0)
                            {
                                if (mv_x < 0)
                                {
                                    for (int itr = x_pos - 1; itr < y; itr--)
                                    {
                                        if (ChessBoard.board[itr][y] != null)
                                            illegal_move = true;
                                    }
                                }
                                else if (mv_x > 0)
                                {
                                    for (int itr = x_pos + 1; itr < y; itr++)
                                    {
                                        if (ChessBoard.board[itr][y] != null)
                                            illegal_move = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (mv_x > 0)
                            {
                                if (mv_y < 0)
                                {
                                    for (int itr_x = x_pos + 1, itr_y = y_pos - 1; itr_x < x; itr_x++, itr_y--)
                                    {
                                        if (ChessBoard.board[itr_x][itr_y] != null)
                                            illegal_move = true;
                                    }
                                }
                                else
                                {
                                    for (int itr_x = x_pos + 1, itr_y = y_pos + 1; itr_x < x; itr_x++, itr_y++)
                                    {
                                        if (ChessBoard.board[itr_x][itr_y] != null)
                                            illegal_move = true;
                                    }
                                }
                            }
                            else
                            {
                                if (mv_y < 0)
                                {
                                    for (int itr_x = x_pos - 1, itr_y = y_pos - 1; itr_x < x; itr_x--, itr_y--)
                                    {
                                        if (ChessBoard.board[itr_x][itr_y] != null)
                                            illegal_move = true;
                                    }
                                }
                                else
                                {
                                    for (int itr_x = x_pos - 1, itr_y = y_pos + 1; itr_x < x; itr_x--, itr_y++)
                                    {
                                        if (ChessBoard.board[itr_x][itr_y] != null)
                                            illegal_move = true;
                                    }
                                }
                            }
                        }
                        if (ChessBoard.board[x][y] != null && ChessBoard.board[x][y].color == color)
                            illegal_move = true;
                        if (!illegal_move)
                        {
                            change_turn();
                            if (ChessBoard.board[x][y] != null)
                            {
                                ChessBoard.board[x][y] = null;
                            }
                            ChessBoard.board[x_pos][y_pos] = null;
                            ChessBoard.board[x][y] = this;
                            x_pos = x;
                            y_pos = y;
                        }
                    }
                }
                else
                    illegal_move = true;
            }
            else
                illegal_move = true;
            test_checkmate();
            if (color == var.white)
                check(bkx_pos, bky_pos);
            else
                check(wkx_pos, wky_pos);            
            return !illegal_move;
        }
    };

    class King : Piece
    {         

        public King(int x, int y, var c)
        {
            check_list = new List<Piece>();
            Exception e = new Exception();
            type = var.king;
            color = c;
            if (color == var.white && (y != 0 || x != 4) || color == var.black && (y != 7 || x != 4) || x > 7 || y > 7 || ChessBoard.board[x][y] != null)
                throw e;
            x_pos = x;
            y_pos = y;
            if(c==var.white)
            {
                wkx_pos = x;
                wky_pos = y;
            }
            else
            {
                bkx_pos = x;
                bky_pos = y;
            }
        }

        public override bool move(int x, int y)
        {
            bool illegal_move = false;
            if (check_turn())
            {
                if (x < 8 && y < 8)
                {
                    int mv_x = abs(x - x_pos), mv_y = abs(y - y_pos);
                    if (mv_x <= 1 && mv_y <= 1)
                    {
                        if (ChessBoard.board[x][y] != null && ChessBoard.board[x][y].color == color)
                            illegal_move = true;
                        else
                        {
                            if (ChessBoard.board[x][y] != null)
                            {
                                ChessBoard.board[x][y] = null;
                            }
                            ChessBoard.board[x_pos][y_pos] = null;
                            ChessBoard.board[x][y] = this;
                            x_pos = x;
                            y_pos = y;
                            if(color == var.white)
                            {
                                wkx_pos = x;
                                wky_pos = y;
                            }
                            else
                            {
                                bkx_pos = x;
                                bky_pos = y;
                            }
                        }
                    }
                    /*else if (x_pos == 3 && mv_y == 0 && mv_x == 2)
                    {
                        if (color == var.white)
                        {
                            if (ChessBoard.board[4][0] == null && ChessBoard.board[5][0] == null && ChessBoard.board[6][0] != null && ChessBoard.board[7][0].type == var.rook)
                            {
                                ChessBoard.board[x_pos][y_pos] = null;
                                ChessBoard.board[x][y] = this;
                                x_pos = x;
                                y_pos = y;
                                wkx_pos = x;
                                wky_pos = y;
                                ChessBoard.board[4][0] = ChessBoard.board[7][0];
                                ChessBoard.board[7][0] = null;
                            }
                            else if (ChessBoard.board[2][0] == null && ChessBoard.board[1][0] == null && ChessBoard.board[0][0].type == var.rook)
                            {
                                ChessBoard.board[x_pos][y_pos] = null;
                                ChessBoard.board[x][y] = this;
                                x_pos = x;
                                y_pos = y;
                                wkx_pos = x;
                                wky_pos = y;
                                ChessBoard.board[2][0] = ChessBoard.board[0][0];
                                ChessBoard.board[0][0] = null;
                            }
                            else
                                illegal_move = true;
                        }
                        else
                        {
                            if (ChessBoard.board[4][7] == null && ChessBoard.board[5][7] == null && ChessBoard.board[6][7] != null && ChessBoard.board[7][7].type == var.rook)
                            {
                                ChessBoard.board[x_pos][y_pos] = null;
                                ChessBoard.board[x][y] = this;
                                x_pos = x;
                                y_pos = y;
                                bkx_pos = x;
                                bky_pos = y;
                                ChessBoard.board[4][7] = ChessBoard.board[7][7];
                                ChessBoard.board[7][7] = null;
                            }
                            else if (ChessBoard.board[2][7] == null && ChessBoard.board[1][7] == null && ChessBoard.board[0][7].type == var.rook)
                            {
                                ChessBoard.board[x_pos][y_pos] = null;
                                ChessBoard.board[x][y] = this;
                                x_pos = x;
                                y_pos = y;
                                bkx_pos = x;
                                bky_pos = y;
                                ChessBoard.board[2][7] = ChessBoard.board[0][0];
                                ChessBoard.board[0][7] = null;
                            }
                            else
                                illegal_move = true;
                        }
                    }*/
                    else
                        illegal_move = true;

                }
                else
                    illegal_move = true;
            }
            else
                illegal_move = true;
            test_checkmate();
            if (color == var.white)
                check(bkx_pos, bky_pos);
            else
                check(wkx_pos, wky_pos);
            return !illegal_move;
        }
    };
}
