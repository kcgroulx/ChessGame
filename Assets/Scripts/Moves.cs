using UnityEngine;

public class Moves : MonoBehaviour
{
    const int empty = 0;
    const int whiteKing = 1;
    const int whiteQueen = 2;
    const int whiteRook = 3;
    const int whiteBishop = 4;
    const int whiteKnight = 5;
    const int whitePawn = 6;
    const int blackKing = 7;
    const int blackQueen = 8;
    const int blackRook = 9;
    const int blackBishop = 10;
    const int blackKnight = 11;
    const int blackPawn = 12;

    bool isWhiteKingCastle = true;
    bool isWhiteQueenCastle = true;
    bool isBlackKingCastle = true;
    bool isBlackQueenCastle = true;


    bool[,] testLegalMoves = new bool[8, 8];
    int[,] testBoard = new int[8, 8];

    public gameManger gamemanager;

    //checks whether the inputed color is mated or not.
    public bool checkForMate(int[,] board, char color)
    {
        bool[,] legalMoves = new bool[8, 8];
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if(getColor(x, y, board) == color)
                {
                    GenerateLegalMovesArray(x, y, board, legalMoves);
                    for (int X = 0; X <8; X++)
                    {
                        for (int Y = 0; Y < 8; Y++)
                        {
                            if (legalMoves[X, Y] == true)
                                return false;
                        }
                    }
                }
            }
        }
        return true;
    }

    //Checks if move is valid and changes the board array, takes care of special moves such as castle and pawn rank ups
    public void ProcessMove(Vector2Int start, Vector2Int final, int[,] board, bool [,] legalMoves)
    {
        if (legalMoves[final.x, final.y] == true)
        {
            char color = getColor(start.x, start.y, board);
            int piece = board[start.x, start.y];

            //Processes move
            board[final.x, final.y] = board[start.x, start.y];
            board[start.x, start.y] = 0;

            //Checks for pawn rank up
            if (final.y == 7 && board[final.x, final.y] == whitePawn)
                board[final.x, final.y] = whiteQueen;
            if (final.y == 0 && board[final.x, final.y] == blackPawn)
                board[final.x, final.y] = blackQueen;

            //Checks if move is White King or Rook
            if (piece == whiteKing)
            {
                isWhiteKingCastle = false;
                isWhiteQueenCastle = false;
            }
            if (start.x == 7 && start.y == 0 && piece == whiteRook)
                isWhiteKingCastle = false;
            if (start.x == 0 && start.y == 0 && piece == whiteRook)
                isWhiteQueenCastle = false;
            if (piece == blackKing)
            {
                isBlackKingCastle = false;
                isBlackQueenCastle = false;
            }
            if (start.x == 7 && start.y == 7 && piece == blackRook)
                isBlackKingCastle = false;
            if (start.x == 0 && start.y == 7 && piece == blackRook)
                isBlackQueenCastle = false;

            //White King Side Castle
            if (start.x == 4 && start.y == 0 && final.x == 6 && final.y == 0)
            {
                board[5, 0] = whiteRook;
                board[7, 0] = empty;
                isWhiteKingCastle = false;
            }

            //White Queen Side Castle
            if (start.x == 4 && start.y == 0 && final.x == 2 && final.y == 0)
            {
                board[3, 0] = whiteRook;
                board[0, 0] = empty;
                isWhiteQueenCastle = false;
            }

            //Black King Side Castle
            if (start.x == 4 && start.y == 7 && final.x == 6 && final.y == 7)
            {
                board[5, 7] = blackRook;
                board[7, 7] = empty;
                isBlackKingCastle = false;
            }
            //Black Queen Side Castle
            if (start.x == 4 && start.y == 7 && final.x == 2 && final.y == 7)
            {
                board[3, 7] = blackRook;
                board[0, 7] = empty;
                isBlackQueenCastle = false;
            }
            //Next turn
            gamemanager.nextTurn();
        }
        else
            return;
    }

    //Generates a legal move array for the piece into the passed 2D bool array (legal moves)
    public void GenerateLegalMovesArray(int x, int y, int[,] board, bool[,] legalMoves)
    {
        int piece = board[x, y];
        char turn = getColor(x, y, board);

        //clears legalMoveArray
        for (int X = 0; X < 8; X++)
        {
            for (int Y = 0; Y < 8; Y++)
            {
                legalMoves[x, y] = false;
            }
        }

        //White King
        if (piece == whiteKing)
        {
            for (int X = x - 1; X < x + 2; X++)
            {
                for (int Y = y - 1; Y < y + 2; Y++)
                {
                    try
                    {
                        if (board[X, Y] == 0 || getColor(X, Y, board) == 'b')
                            legalMoves[X, Y] = true;
                    }
                    catch { }
                }
            }

            //White King Side Castle
            legalMoves[x, y] = false;
            if (isWhiteKingCastle == true && board[5, 0] == empty && board[6, 0] == empty && board[7, 0] == whiteRook && board[4, 0] == whiteKing)
            {
                legalMoves[6, 0] = true;
            }
            //White Queen Side Castle
            legalMoves[x, y] = false;
            if (isWhiteQueenCastle == true && board[3, 0] == empty && board[2, 0] == empty && board[1, 0] == empty && board[0, 0] == whiteRook && board[4, 0] == whiteKing)
            {
                legalMoves[2, 0] = true;
            }
        }
        //White Queen
        if (piece == whiteQueen)
        {
            int X;
            int Y;
            //Up
            X = x;
            Y = y + 1;
            while (Y < 8)
            {
                if (getColor(X, Y, board) == 'w')
                    break;
                legalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'b')
                    break;
                Y++;
            }
            Y = y - 1;
            //Down
            while (Y > -1)
            {
                if (getColor(X, Y, board) == 'w')
                    break;
                legalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'b')
                    break;
                Y--;
            }
            X = x + 1;
            Y = y;
            //Right
            while (X < 8)
            {
                if (getColor(X, Y, board) == 'w')
                    break;
                legalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'b')
                    break;
                X++;
            }
            X = x - 1;
            //Left
            while (X > -1)
            {
                if (getColor(X, Y, board) == 'w')
                    break;
                legalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'b')
                    break;
                X--;
            }
            //Up Right
            X = x + 1;
            Y = y + 1;
            while (X < 8 && Y < 8)
            {
                if (getColor(X, Y, board) == 'w')
                    break;
                legalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'b')
                    break;
                Y++;
                X++;
            }
            X = x + 1;
            Y = y - 1;

            //Down Right
            while (Y > -1 && X < 8)
            {
                if (getColor(X, Y, board) == 'w')
                    break;
                legalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'b')
                    break;
                Y--;
                X++;
            }

            X = x - 1;
            Y = y + 1;
            //Up Left
            while (X > -1 && Y < 8)
            {
                if (getColor(X, Y, board) == 'w')
                    break;
                legalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'b')
                    break;
                X--;
                Y++;
            }

            X = x - 1;
            Y = y - 1;
            //Down Left
            while (X > -1 && Y > -1)
            {
                if (getColor(X, Y, board) == 'w')
                    break;
                legalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'b')
                    break;
                X--;
                Y--;
            }
        }

        //White Rook
        if (piece == whiteRook)
        {
            int X;
            int Y;
            //Up
            X = x;
            Y = y + 1;
            while (Y < 8)
            {
                if (getColor(X, Y, board) == 'w')
                    break;
                legalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'b')
                    break;
                Y++;
            }
            Y = y - 1;
            //Down
            while (Y > -1)
            {
                if (getColor(X, Y, board) == 'w')
                    break;
                legalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'b')
                    break;
                Y--;
            }
            X = x + 1;
            Y = y;
            //Right
            while (X < 8)
            {
                if (getColor(X, Y, board) == 'w')
                    break;
                legalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'b')
                    break;
                X++;
            }
            X = x - 1;
            //Left
            while (X > -1)
            {
                if (getColor(X, Y, board) == 'w')
                    break;
                legalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'b')
                    break;
                X--;
            }
        }

        //White Bishop
        if (piece == whiteBishop)
        {
            int X;
            int Y;
            //Up Right
            X = x + 1;
            Y = y + 1;
            while (X < 8 && Y < 8)
            {
                if (getColor(X, Y, board) == 'w')
                    break;
                legalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'b')
                    break;
                Y++;
                X++;
            }
            X = x + 1;
            Y = y - 1;

            //Down Right
            while (Y > -1 && X < 8)
            {
                if (getColor(X, Y, board) == 'w')
                    break;
                legalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'b')
                    break;
                Y--;
                X++;
            }

            X = x - 1;
            Y = y + 1;
            //Up Left
            while (X > -1 && Y < 8)
            {
                if (getColor(X, Y, board) == 'w')
                    break;
                legalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'b')
                    break;
                X--;
                Y++;
            }

            X = x - 1;
            Y = y - 1;
            //Down Left
            while (X > -1 && Y > -1)
            {
                if (getColor(X, Y, board) == 'w')
                    break;
                legalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'b')
                    break;
                X--;
                Y--;
            }
        }

        //White Knight
        if (piece == whiteKnight)
        {
            if (x + 1 < 8 && y + 2 < 8 && getColor(x + 1, y + 2, board) != 'w')
                legalMoves[x + 1, y + 2] = true;
            if (x + 2 < 8 && y + 1 < 8 && getColor(x + 2, y + 1, board) != 'w')
                legalMoves[x + 2, y + 1] = true;
            if (x - 1 >= 0 && y + 2 < 8 && getColor(x - 1, y + 2, board) != 'w')
                legalMoves[x - 1, y + 2] = true;
            if (x - 2 >= 0 && y + 1 < 8 && getColor(x - 2, y + 1, board) != 'w')
                legalMoves[x - 2, y + 1] = true;
            if (x + 2 < 8 && y - 1 >= 0 && getColor(x + 2, y - 1, board) != 'w')
                legalMoves[x + 2, y - 1] = true;
            if (x + 1 < 8 && y - 2 >= 0 && getColor(x + 1, y - 2, board) != 'w')
                legalMoves[x + 1, y - 2] = true;
            if (x - 2 >= 0 && y - 1 >= 0 && getColor(x - 2, y - 1, board) != 'w')
                legalMoves[x - 2, y - 1] = true;
            if (x - 1 >= 0 && y - 2 >= 0 && getColor(x - 1, y - 2, board) != 'w')
                legalMoves[x - 1, y - 2] = true;
        }

        //White Pawn
        if (piece == whitePawn)
        {
            //Up one
            if (board[x, y + 1] == 0)
                legalMoves[x, y + 1] = true;
            //Up two
            if (y == 1 && board[x, y + 1] == 0 && board[x, y + 2] == 0)
                legalMoves[x, y + 2] = true;
            try
            {
                if (getColor(x - 1, y + 1, board) == 'b')
                    legalMoves[x - 1, y + 1] = true;
                if (getColor(x + 1, y + 1, board) == 'b')
                    legalMoves[x + 1, y + 1] = true;
            }
            catch { }
        }


        //Black King
        if (piece == blackKing)
        {
            for (int X = x - 1; X < x + 2; X++)
            {
                for (int Y = y - 1; Y < y + 2; Y++)
                {
                    try
                    {
                        if (board[X, Y] == 0 || getColor(X, Y, board) == 'w')
                            legalMoves[X, Y] = true;
                    }
                    catch { }
                }
            }
            legalMoves[x, y] = false;

            //Black King Side Castle
            if (isBlackKingCastle == true && board[5, 7] == empty && board[6, 7] == empty && board[7, 7] == blackRook && board[4, 7] == blackKing)
                legalMoves[6, 7] = true;
            //Black Queen Side Castle
            if (isBlackQueenCastle == true && board[3, 7] == empty && board[2, 7] == empty && board[1, 7] == empty && board[0, 7] == blackRook && board[4, 7] == blackKing)
                legalMoves[2, 7] = true;
        }

        //Black Queen
        if (piece == blackQueen)
        {
            int X;
            int Y;
            //Up
            X = x;
            Y = y + 1;
            while (Y < 8)
            {
                if (getColor(X, Y, board) == 'b')
                    break;
                legalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'w')
                    break;
                Y++;
            }
            Y = y - 1;
            //Down
            while (Y > -1)
            {
                if (getColor(X, Y, board) == 'b')
                    break;
                legalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'w')
                    break;
                Y--;
            }
            X = x + 1;
            Y = y;
            //Right
            while (X < 8)
            {
                if (getColor(X, Y, board) == 'b')
                    break;
                legalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'w')
                    break;
                X++;
            }
            X = x - 1;
            //Left
            while (X > -1)
            {
                if (getColor(X, Y, board) == 'b')
                    break;
                legalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'w')
                    break;
                X--;
            }
            //Up Right
            X = x + 1;
            Y = y + 1;
            while (X < 8 && Y < 8)
            {
                if (getColor(X, Y, board) == 'b')
                    break;
                legalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'w')
                    break;
                Y++;
                X++;
            }
            X = x + 1;
            Y = y - 1;

            //Down Right
            while (Y > -1 && X < 8)
            {
                if (getColor(X, Y, board) == 'b')
                    break;
                legalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'w')
                    break;
                Y--;
                X++;
            }

            X = x - 1;
            Y = y + 1;
            //Up Left
            while (X > -1 && Y < 8)
            {
                if (getColor(X, Y, board) == 'b')
                    break;
                legalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'w')
                    break;
                X--;
                Y++;
            }

            X = x - 1;
            Y = y - 1;
            //Down Left
            while (X > -1 && Y > -1)
            {
                if (getColor(X, Y, board) == 'b')
                    break;
                legalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'w')
                    break;
                X--;
                Y--;
            }
        }

        //Black Rook
        if (piece == blackRook)
        {
            int X;
            int Y;
            //Up
            X = x;
            Y = y + 1;
            while (Y < 8)
            {
                if (getColor(X, Y, board) == 'b')
                    break;
                legalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'w')
                    break;
                Y++;
            }
            Y = y - 1;
            //Down
            while (Y > -1)
            {
                if (getColor(X, Y, board) == 'b')
                    break;
                legalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'w')
                    break;
                Y--;
            }
            X = x + 1;
            Y = y;
            //Right
            while (X < 8)
            {
                if (getColor(X, Y, board) == 'b')
                    break;
                legalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'w')
                    break;
                X++;
            }
            X = x - 1;
            //Left
            while (X > -1)
            {
                if (getColor(X, Y, board) == 'b')
                    break;
                legalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'w')
                    break;
                X--;
            }
        }
        //White Bishop
        if (piece == blackBishop)
        {
            int X;
            int Y;
            //Up Right
            X = x + 1;
            Y = y + 1;
            while (X < 8 && Y < 8)
            {
                if (getColor(X, Y, board) == 'b')
                    break;
                legalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'w')
                    break;
                Y++;
                X++;
            }
            X = x + 1;
            Y = y - 1;
            //Down Right
            while (Y > -1 && X < 8)
            {
                if (getColor(X, Y, board) == 'b')
                    break;
                legalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'w')
                    break;
                Y--;
                X++;
            }
            X = x - 1;
            Y = y + 1;
            //Up Left
            while (X > -1 && Y < 8)
            {
                if (getColor(X, Y, board) == 'b')
                    break;
                legalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'w')
                    break;
                X--;
                Y++;
            }
            X = x - 1;
            Y = y - 1;
            //Down Left
            while (X > -1 && Y > -1)
            {
                if (getColor(X, Y, board) == 'b')
                    break;
                legalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'w')
                    break;
                X--;
                Y--;
            }
        }
        //Black Knight
        if (piece == blackKnight)
        {
            if (x + 1 < 8 && y + 2 < 8 && getColor(x + 1, y + 2, board) != 'b')
                legalMoves[x + 1, y + 2] = true;
            if (x + 2 < 8 && y + 1 < 8 && getColor(x + 2, y + 1, board) != 'b')
                legalMoves[x + 2, y + 1] = true;
            if (x - 1 >= 0 && y + 2 < 8 && getColor(x - 1, y + 2, board) != 'b')
                legalMoves[x - 1, y + 2] = true;
            if (x - 2 >= 0 && y + 1 < 8 && getColor(x - 2, y + 1, board) != 'b')
                legalMoves[x - 2, y + 1] = true;
            if (x + 2 < 8 && y - 1 >= 0 && getColor(x + 2, y - 1, board) != 'b')
                legalMoves[x + 2, y - 1] = true;
            if (x + 1 < 8 && y - 2 >= 0 && getColor(x + 1, y - 2, board) != 'b')
                legalMoves[x + 1, y - 2] = true;
            if (x - 2 >= 0 && y - 1 >= 0 && getColor(x - 2, y - 1, board) != 'b')
                legalMoves[x - 2, y - 1] = true;
            if (x - 1 >= 0 && y - 2 >= 0 && getColor(x - 1, y - 2, board) != 'b')
                legalMoves[x - 1, y - 2] = true;
        }
        //Black Pawn
        if (piece == blackPawn)
        {
            //Down one
            if (board[x, y - 1] == 0)
                legalMoves[x, y - 1] = true;
            //Down two
            if (y == 6 && board[x, y - 1] == 0 && board[x, y - 2] == 0)
                legalMoves[x, y - 2] = true;
            try
            {
                if (getColor(x - 1, y - 1, board) == 'w')
                    legalMoves[x - 1, y - 1] = true;
                if (getColor(x + 1, y - 1, board) == 'w')
                    legalMoves[x + 1, y - 1] = true;
            }
            catch { }
        }


        //Checks king safety

        //Creates a copy of board in testBoard
        for (int file = 0; file < 8; file++)
        {
            for (int rank = 0; rank < 8; rank++)
            {
                testBoard[file, rank] = board[file, rank];
            }
        }

        for (int X = 0; X < 8; X++)
        {
            for (int Y = 0; Y < 8; Y++)
            {
                //Finds a Legal Move
                if (legalMoves[X, Y] == true)
                {
                    clearTestLegalMovesArray();
                    //does move
                    board[X, Y] = board[x, y];
                    board[x, y] = empty;

                    //creates total possible move array from opposite color
                    for (int file = 0; file < 8; file++)
                    {
                        for (int rank = 0; rank < 8; rank++)
                        {
                            if (getColor(file, rank, board) != 'e')
                            {
                                if (getColor(file, rank, board) != turn)
                                    GenerateTestLegalMovesArray(file, rank, board, testLegalMoves);
                            }
                        }
                    }

                    //Checks if the king is in check
                    for (int file = 0; file < 8; file++)
                    {
                        for (int rank = 0; rank < 8; rank++)
                        {
                            if (testLegalMoves[file, rank] == true && turn == 'w' && board[file, rank] == whiteKing)
                                legalMoves[X, Y] = false;
                            if (testLegalMoves[file, rank] == true && turn == 'b' && board[file, rank] == blackKing)
                                legalMoves[X, Y] = false;
                        }
                    }

                    //resets the board to its original state
                    for (int file = 0; file < 8; file++)
                    {
                        for (int rank = 0; rank < 8; rank++)
                        {
                            board[file, rank] = testBoard[file, rank];
                        }
                    }
                }
            }
        }
    }

    void GenerateTestLegalMovesArray(int x, int y, int[,] board, bool[,] legalMoves)
    {
        int piece = board[x, y];
        //White King
        if (piece == whiteKing)
        {
            for (int X = x - 1; X < x + 2; X++)
            {
                for (int Y = y - 1; Y < y + 2; Y++)
                {
                    try
                    {
                        if (board[X, Y] == 0 || getColor(X, Y, board) == 'w')
                        {
                            testLegalMoves[X, Y] = true;
                        }
                    }
                    catch { }
                }
            }

            //White King Side Castle
            testLegalMoves[x, y] = false;
            if (isWhiteKingCastle == true && board[5, 0] == empty && board[6, 0] == empty && board[7, 0] == whiteRook && board[4, 0] == whiteKing)
            {
                testLegalMoves[6, 0] = true;
            }
            //White Queen Side Castle
            testLegalMoves[x, y] = false;
            if (isWhiteQueenCastle == true && board[3, 0] == empty && board[2, 0] == empty && board[1, 0] == empty && board[0, 0] == whiteRook && board[4, 0] == whiteKing)
            {
                testLegalMoves[2, 0] = true;
            }
        }
        //White Queen
        if (piece == whiteQueen)
        {
            int X;
            int Y;
            //Up
            X = x;
            Y = y + 1;
            while (Y < 8)
            {
                if (getColor(X, Y, board) == 'w')
                    break;
                testLegalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'b')
                    break;
                Y++;
            }
            Y = y - 1;
            //Down
            while (Y > -1)
            {
                if (getColor(X, Y, board) == 'w')
                    break;
                testLegalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'b')
                    break;
                Y--;
            }
            X = x + 1;
            Y = y;
            //Right
            while (X < 8)
            {
                if (getColor(X, Y, board) == 'w')
                    break;
                testLegalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'b')
                    break;
                X++;
            }
            X = x - 1;
            //Left
            while (X > -1)
            {
                if (getColor(X, Y, board) == 'w')
                    break;
                testLegalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'b')
                    break;
                X--;
            }
            //Up Right
            X = x + 1;
            Y = y + 1;
            while (X < 8 && Y < 8)
            {
                if (getColor(X, Y, board) == 'w')
                    break;
                testLegalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'b')
                    break;
                Y++;
                X++;
            }
            X = x + 1;
            Y = y - 1;

            //Down Right
            while (Y > -1 && X < 8)
            {
                if (getColor(X, Y, board) == 'w')
                    break;
                testLegalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'b')
                    break;
                Y--;
                X++;
            }

            X = x - 1;
            Y = y + 1;
            //Up Left
            while (X > -1 && Y < 8)
            {
                if (getColor(X, Y, board) == 'w')
                    break;
                testLegalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'b')
                    break;
                X--;
                Y++;
            }

            X = x - 1;
            Y = y - 1;
            //Down Left
            while (X > -1 && Y > -1)
            {
                if (getColor(X, Y, board) == 'w')
                    break;
                testLegalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'b')
                    break;
                X--;
                Y--;
            }
        }

        //White Rook
        if (piece == whiteRook)
        {
            int X;
            int Y;
            //Up
            X = x;
            Y = y + 1;
            while (Y < 8)
            {
                if (getColor(X, Y, board) == 'w')
                    break;
                testLegalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'b')
                    break;
                Y++;
            }
            Y = y - 1;
            //Down
            while (Y > -1)
            {
                if (getColor(X, Y, board) == 'w')
                    break;
                testLegalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'b')
                    break;
                Y--;
            }
            X = x + 1;
            Y = y;
            //Right
            while (X < 8)
            {
                if (getColor(X, Y, board) == 'w')
                    break;
                testLegalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'b')
                    break;
                X++;
            }
            X = x - 1;
            //Left
            while (X > -1)
            {
                if (getColor(X, Y, board) == 'w')
                    break;
                testLegalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'b')
                    break;
                X--;
            }
        }

        //White Bishop
        if (piece == whiteBishop)
        {
            int X;
            int Y;
            //Up Right
            X = x + 1;
            Y = y + 1;
            while (X < 8 && Y < 8)
            {
                if (getColor(X, Y, board) == 'w')
                    break;
                testLegalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'b')
                    break;
                Y++;
                X++;
            }
            X = x + 1;
            Y = y - 1;

            //Down Right
            while (Y > -1 && X < 8)
            {
                if (getColor(X, Y, board) == 'w')
                    break;
                testLegalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'b')
                    break;
                Y--;
                X++;
            }

            X = x - 1;
            Y = y + 1;
            //Up Left
            while (X > -1 && Y < 8)
            {
                if (getColor(X, Y, board) == 'w')
                    break;
                testLegalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'b')
                    break;
                X--;
                Y++;
            }

            X = x - 1;
            Y = y - 1;
            //Down Left
            while (X > -1 && Y > -1)
            {
                if (getColor(X, Y, board) == 'w')
                    break;
                testLegalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'b')
                    break;
                X--;
                Y--;
            }
        }

        //White Knight
        if (piece == whiteKnight)
        {
            if (x + 1 < 8 && y + 2 < 8 && getColor(x + 1, y + 2, board) != 'w')
                testLegalMoves[x + 1, y + 2] = true;

            if (x + 2 < 8 && y + 1 < 8 && getColor(x + 2, y + 1, board) != 'w')
                testLegalMoves[x + 2, y + 1] = true;

            if (x - 1 >= 0 && y + 2 < 8 && getColor(x - 1, y + 2, board) != 'w')
                testLegalMoves[x - 1, y + 2] = true;

            if (x - 2 >= 0 && y + 1 < 8 && getColor(x - 2, y + 1, board) != 'w')
                testLegalMoves[x - 2, y + 1] = true;

            if (x + 2 < 8 && y - 1 >= 0 && getColor(x + 2, y - 1, board) != 'w')
                testLegalMoves[x + 2, y - 1] = true;

            if (x + 1 < 8 && y - 2 >= 0 && getColor(x + 1, y - 2, board) != 'w')
                testLegalMoves[x + 1, y - 2] = true;

            if (x - 2 >= 0 && y - 1 >= 0 && getColor(x - 2, y - 1, board) != 'w')
                testLegalMoves[x - 2, y - 1] = true;
            if (x - 1 >= 0 && y - 2 >= 0 && getColor(x - 1, y - 2, board) != 'w')
                testLegalMoves[x - 1, y - 2] = true;
        }

        //White Pawn
        if (piece == whitePawn)
        {
            //Up one
            if (board[x, y + 1] == 0)
                testLegalMoves[x, y + 1] = true;
            //Up two
            if (y == 1 && board[x, y + 1] == 0 && board[x, y + 2] == 0)
                testLegalMoves[x, y + 2] = true;
            try
            {
                if (getColor(x - 1, y + 1, board) == 'b')
                {
                    testLegalMoves[x - 1, y + 1] = true;
                }
                if (getColor(x + 1, y + 1, board) == 'b')
                {
                    testLegalMoves[x + 1, y + 1] = true;
                }
            }
            catch { }
        }

        //Black King
        if (piece == blackKing)
        {
            for (int X = x - 1; X < x + 2; X++)
            {
                for (int Y = y - 1; Y < y + 2; Y++)
                {
                    try
                    {
                        if (board[X, Y] == 0 || getColor(X, Y, board) == 'w')
                            testLegalMoves[X, Y] = true;
                    }
                    catch { }
                }
            }
            testLegalMoves[x, y] = false;
            //Black King Side Castle
            if (isBlackKingCastle == true && board[5, 7] == empty && board[6, 7] == empty && board[7, 7] == blackRook && board[4, 7] == blackKing)
                testLegalMoves[6, 7] = true;
            //Black Queen Side Castle
            if (isBlackQueenCastle == true && board[3, 7] == empty && board[2, 7] == empty && board[1, 7] == empty && board[0, 7] == blackRook && board[4, 7] == blackKing)
                testLegalMoves[2, 7] = true;
        }

        //Black Queen
        if (piece == blackQueen)
        {
            int X;
            int Y;
            //Up
            X = x;
            Y = y + 1;
            while (Y < 8)
            {
                if (getColor(X, Y, board) == 'b')
                    break;
                testLegalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'w')
                    break;
                Y++;
            }
            Y = y - 1;
            //Down
            while (Y > -1)
            {
                if (getColor(X, Y, board) == 'b')
                    break;
                testLegalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'w')
                    break;
                Y--;
            }
            X = x + 1;
            Y = y;
            //Right
            while (X < 8)
            {
                if (getColor(X, Y, board) == 'b')
                    break;
                testLegalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'w')
                    break;
                X++;
            }
            X = x - 1;
            //Left
            while (X > -1)
            {
                if (getColor(X, Y, board) == 'b')
                    break;
                testLegalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'w')
                    break;
                X--;
            }
            //Up Right
            X = x + 1;
            Y = y + 1;
            while (X < 8 && Y < 8)
            {
                if (getColor(X, Y, board) == 'b')
                    break;
                testLegalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'w')
                    break;
                Y++;
                X++;
            }
            X = x + 1;
            Y = y - 1;

            //Down Right
            while (Y > -1 && X < 8)
            {
                if (getColor(X, Y, board) == 'b')
                    break;
                testLegalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'w')
                    break;
                Y--;
                X++;
            }

            X = x - 1;
            Y = y + 1;
            //Up Left
            while (X > -1 && Y < 8)
            {
                if (getColor(X, Y, board) == 'b')
                    break;
                testLegalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'w')
                    break;
                X--;
                Y++;
            }

            X = x - 1;
            Y = y - 1;
            //Down Left
            while (X > -1 && Y > -1)
            {
                if (getColor(X, Y, board) == 'b')
                    break;
                testLegalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'w')
                    break;
                X--;
                Y--;
            }
        }

        //Black Rook
        if (piece == blackRook)
        {
            int X;
            int Y;
            //Up
            X = x;
            Y = y + 1;
            while (Y < 8)
            {
                if (getColor(X, Y, board) == 'b')
                    break;
                testLegalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'w')
                    break;
                Y++;
            }
            Y = y - 1;
            //Down
            while (Y > -1)
            {
                if (getColor(X, Y, board) == 'b')
                    break;
                testLegalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'w')
                    break;
                Y--;
            }
            X = x + 1;
            Y = y;
            //Right
            while (X < 8)
            {
                if (getColor(X, Y, board) == 'b')
                    break;
                testLegalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'w')
                    break;
                X++;
            }
            X = x - 1;
            //Left
            while (X > -1)
            {
                if (getColor(X, Y, board) == 'b')
                    break;
                testLegalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'w')
                    break;
                X--;
            }
        }

        //White Bishop
        if (piece == blackBishop)
        {
            int X;
            int Y;
            //Up Right
            X = x + 1;
            Y = y + 1;
            while (X < 8 && Y < 8)
            {
                if (getColor(X, Y, board) == 'b')
                    break;
                testLegalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'w')
                    break;
                Y++;
                X++;
            }
            X = x + 1;
            Y = y - 1;

            //Down Right
            while (Y > -1 && X < 8)
            {
                if (getColor(X, Y, board) == 'b')
                    break;
                testLegalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'w')
                    break;
                Y--;
                X++;
            }

            X = x - 1;
            Y = y + 1;
            //Up Left
            while (X > -1 && Y < 8)
            {
                if (getColor(X, Y, board) == 'b')
                    break;
                testLegalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'w')
                    break;
                X--;
                Y++;
            }

            X = x - 1;
            Y = y - 1;
            //Down Left
            while (X > -1 && Y > -1)
            {
                if (getColor(X, Y, board) == 'b')
                    break;
                testLegalMoves[X, Y] = true;
                if (getColor(X, Y, board) == 'w')
                    break;
                X--;
                Y--;
            }
        }

        //Black Knight
        if (piece == blackKnight)
        {
            if (x + 1 < 8 && y + 2 < 8 && getColor(x + 1, y + 2, board) != 'b')
                testLegalMoves[x + 1, y + 2] = true;
            if (x + 2 < 8 && y + 1 < 8 && getColor(x + 2, y + 1, board) != 'b')
                testLegalMoves[x + 2, y + 1] = true;
            if (x - 1 >= 0 && y + 2 < 8 && getColor(x - 1, y + 2, board) != 'b')
                testLegalMoves[x - 1, y + 2] = true;
            if (x - 2 >= 0 && y + 1 < 8 && getColor(x - 2, y + 1, board) != 'b')
                testLegalMoves[x - 2, y + 1] = true;
            if (x + 2 < 8 && y - 1 >= 0 && getColor(x + 2, y - 1, board) != 'b')
                testLegalMoves[x + 2, y - 1] = true;
            if (x + 1 < 8 && y - 2 >= 0 && getColor(x + 1, y - 2, board) != 'b')
                testLegalMoves[x + 1, y - 2] = true;
            if (x - 2 >= 0 && y - 1 >= 0 && getColor(x - 2, y - 1, board) != 'b')
                testLegalMoves[x - 2, y - 1] = true;
            if (x - 1 >= 0 && y - 2 >= 0 && getColor(x - 1, y - 2, board) != 'b')
                testLegalMoves[x - 1, y - 2] = true;
        }

        //Black Pawn
        if (piece == blackPawn)
        {
            //Down one
            if (board[x, y - 1] == 0)
                testLegalMoves[x, y - 1] = true;
            //Down two
            if (y == 6 && board[x, y - 1] == 0 && board[x, y - 2] == 0)
                testLegalMoves[x, y - 2] = true;
            try
            {
                if (getColor(x - 1, y - 1, board) == 'w')
                    testLegalMoves[x - 1, y - 1] = true;
                if (getColor(x + 1, y - 1, board) == 'w')
                    testLegalMoves[x + 1, y - 1] = true;
            }
            catch { }
        }
    }

    char getColor(int x, int y, int[,] board)
    {
        if (board[x, y] > 0 && board[x, y] < 7)
            return 'w';
        if (board[x, y] > 6 && board[x, y] < 13)
            return 'b';
        return 'e';
    }

    void clearTestLegalMovesArray()
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                testLegalMoves[x, y] = false;
            }
        }
    }
}
