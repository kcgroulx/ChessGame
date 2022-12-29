using System.Diagnostics;
using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using Debug = UnityEngine.Debug;

public class startBoard : MonoBehaviour
{
    public GameObject blackSquare;
    public GameObject whiteSquare;
    public GameObject yellowCircle;
    public GameObject Text;
    public GameObject BoardArrayText;


    public GameObject wKing;
    public GameObject wQueen;
    public GameObject wRook;
    public GameObject wBishop;
    public GameObject wKnight;
    public GameObject wPawn;
    public GameObject bKing;
    public GameObject bQueen;
    public GameObject bRook;
    public GameObject bBishop;
    public GameObject bKnight;
    public GameObject bPawn;

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

    public int[,] board = new int[8, 8];
    public int[,] testBoard = new int[8, 8];

    public bool[,] legalMoves = new bool[8, 8];
    public bool[,] testLegalMoves = new bool[8, 8];

    public bool isWhiteMated = false;
    public bool isBlackMated = false;

    public Vector2Int clickPos;
    public Vector2Int start;
    public Vector2Int final;

    public bool isPieceSelected = false;

    public bool isWhiteKingCastle = true;
    public bool isWhiteQueenCastle = true;
    public bool isBlackKingCastle = true;
    public bool isBlackQueenCastle = true;

    public char turn = 'b';

    public controller MyController;

    float time;
    public float timeDelay;

    public class pieces
    {
        public int piece;
        public int X;
        public int Y;
    }
        

    // Start is called before the first frame update
    void Start()
    {
        createBoard();
        SetPiecesToArray("k7/8/8/8/8/8/8/RNBQKBRR");
        SetPieces();
        MyController = new controller();
        List<controller> ControllerList = new List<controller>();
        foreach(var item in ControllerList)
        {
            //item.doSomething();
        }
    }

    // Update is called once per frame
    void Update()
    {   
        if (Input.GetMouseButtonUp(0))
        {
            clickPos = new Vector2Int((int)Mathf.Round(Camera.main.ScreenToWorldPoint(Input.mousePosition).x), (int)Mathf.Round(Camera.main.ScreenToWorldPoint(Input.mousePosition).y));
            if (isPieceSelected == false && board[clickPos.x, clickPos.y] > 0)
            {
                start = clickPos;
                GenerateLegalMovesArray(start.x, start.y);
                ShowLegalMovesDots();
                isPieceSelected = true;
            }
            if (isPieceSelected == true)
            {
                if (isWhite(start.x, start.y) == true && isWhite(clickPos.x, clickPos.y) == true)
                {
                    start = clickPos;
                    GenerateLegalMovesArray(start.x, start.y);
                    ShowLegalMovesDots();
                }
                else if (isBlack(start.x, start.y) == true && isBlack(clickPos.x, clickPos.y) == true)
                {
                    start = clickPos;
                    GenerateLegalMovesArray(start.x, start.y);
                    ShowLegalMovesDots();
                }
                else
                {
                    final = clickPos;
                    ProcessMove(start, final);
                    isPieceSelected = false;
                    clearLegalMovesArray();
                    ClearLegalMovesDots();
                }
            }
        }
    }


    private void AIMove(char color)
    {
        Vector2Int start = new Vector2Int();
        Vector2Int final = new Vector2Int();
        bool IsAIPieceSelected = new bool();
        IsAIPieceSelected = false;

        //selectes random piece (start)
        while(IsAIPieceSelected == false)
        {
            start = new Vector2Int(Random.Range(0, 8), Random.Range(0, 8));

            GenerateLegalMovesArray(start.x, start.y);

            if (getColor(start.x, start.y) == color)
            {
                for (int x = 0; x < 8; x++)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        if (legalMoves[x, y] == true)
                        {
                            IsAIPieceSelected = true;
                            break;
                        }
                    }
                    if (IsAIPieceSelected == true)
                        break;
                }
                
            }
        }

        //Selects random move (final)
        List<Vector2Int> moves = new List<Vector2Int>();

        GenerateLegalMovesArray(start.x, start.y);

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if(legalMoves[x, y] == true)
                {
                    moves.Add(new Vector2Int(x, y));
                }
            }
        }
        final = moves[Random.Range(0, moves.Count)];

        //Processes move
        ProcessMove(start, final);
    }

    private void checkForMate()
    {
        clearTestLegalMovesArray();
        //Checks for white checkmate
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if(getColor(x,y) == 'w')
                {
                    GenerateTestLegalMovesArray(x, y);
                }
            }
        }
        isWhiteMated = true; 
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (testLegalMoves[x,y] == true)
                {
                    isWhiteMated = false;
                }
            }
        }

        //Checks for black checkmate
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (getColor(x, y) == 'b')
                {
                    GenerateTestLegalMovesArray(x, y);
                }
            }
        }
        isBlackMated = true;
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (testLegalMoves[x, y] == true)
                {
                    isBlackMated = false;
                }
            }
        }
        clearTestLegalMovesArray(); 
    }

    private void ProcessMove(Vector2Int start, Vector2Int final)
    {
        if (legalMoves[final.x, final.y] == true)
        {
            int piece = board[start.x, start.y];

            //processes move
            board[final.x, final.y] = board[start.x, start.y];
            board[start.x, start.y] = 0;

            //checks for pawn rank up
            if (final.y == 7 && board[final.x, final.y] == whitePawn)
            {
                board[final.x, final.y] = whiteQueen;
            }
            if (final.y == 0 && board[final.x, final.y] == blackPawn)
            {
                board[final.x, final.y] = blackQueen;
            }

            //Checks if move is White King or Rook
            if (piece == whiteKing)
            {
                isWhiteKingCastle = false;
                isWhiteQueenCastle = false;
            }
            if (start.x == 7 && start.y == 0 && piece == whiteRook)
            {
                isWhiteKingCastle = false;
            }
            if (start.x == 0 && start.y == 0 && piece == whiteRook)
            {
                isWhiteQueenCastle = false;
            }
            if (piece == blackKing)
            {
                isBlackKingCastle = false;
                isBlackQueenCastle = false;
            }
            if (start.x == 7 && start.y == 7 && piece == blackRook)
            {
                isBlackKingCastle = false;
            }
            if (start.x == 0 && start.y == 7 && piece == blackRook)
            {
                isBlackQueenCastle = false;
            }

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
        }
        else
        {
            return;
        }
        nextTurn();
        ClearBoard();
        SetPieces();
        checkForMate();
        if (isWhiteMated == true)
            Debug.Log("White is Mated");
        if (isBlackMated == true)
            Debug.Log("Black is Mated");

    }

    void GenerateTestLegalMovesArray(int x, int y)
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
                        if (board[X, Y] == 0 || isWhite(X, Y) == false)
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
                if (isWhite(X, Y) == true)
                    break;
                testLegalMoves[X, Y] = true;
                if (isBlack(X, Y) == true)
                    break;
                Y++;
            }
            Y = y - 1;
            //Down
            while (Y > -1)
            {
                if (isWhite(X, Y) == true)
                    break;
                testLegalMoves[X, Y] = true;
                if (isBlack(X, Y) == true)
                    break;
                Y--;
            }
            X = x + 1;
            Y = y;
            //Right
            while (X < 8)
            {
                if (isWhite(X, Y) == true)
                    break;
                testLegalMoves[X, Y] = true;
                if (isBlack(X, Y) == true)
                    break;
                X++;
            }
            X = x - 1;
            //Left
            while (X > -1)
            {
                if (isWhite(X, Y) == true)
                    break;
                testLegalMoves[X, Y] = true;
                if (isBlack(X, Y) == true)
                    break;
                X--;
            }
            //Up Right
            X = x + 1;
            Y = y + 1;
            while (X < 8 && Y < 8)
            {
                if (isWhite(X, Y) == true)
                    break;
                testLegalMoves[X, Y] = true;
                if (isBlack(X, Y) == true)
                    break;
                Y++;
                X++;
            }
            X = x + 1;
            Y = y - 1;

            //Down Right
            while (Y > -1 && X < 8)
            {
                if (isWhite(X, Y) == true)
                    break;
                testLegalMoves[X, Y] = true;
                if (isBlack(X, Y) == true)
                    break;
                Y--;
                X++;
            }

            X = x - 1;
            Y = y + 1;
            //Up Left
            while (X > -1 && Y < 8)
            {
                if (isWhite(X, Y) == true)
                    break;
                testLegalMoves[X, Y] = true;
                if (isBlack(X, Y) == true)
                    break;
                X--;
                Y++;
            }

            X = x - 1;
            Y = y - 1;
            //Down Left
            while (X > -1 && Y > -1)
            {
                if (isWhite(X, Y) == true)
                    break;
                testLegalMoves[X, Y] = true;
                if (isBlack(X, Y) == true)
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
                if (isWhite(X, Y) == true)
                    break;
                testLegalMoves[X, Y] = true;
                if (isBlack(X, Y) == true)
                    break;
                Y++;
            }
            Y = y - 1;
            //Down
            while (Y > -1)
            {
                if (isWhite(X, Y) == true)
                    break;
                testLegalMoves[X, Y] = true;
                if (isBlack(X, Y) == true)
                    break;
                Y--;
            }
            X = x + 1;
            Y = y;
            //Right
            while (X < 8)
            {
                if (isWhite(X, Y) == true)
                    break;
                testLegalMoves[X, Y] = true;
                if (isBlack(X, Y) == true)
                    break;
                X++;
            }
            X = x - 1;
            //Left
            while (X > -1)
            {
                if (isWhite(X, Y) == true)
                    break;
                testLegalMoves[X, Y] = true;
                if (isBlack(X, Y) == true)
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
                if (isWhite(X, Y) == true)
                    break;
                testLegalMoves[X, Y] = true;
                if (isBlack(X, Y) == true)
                    break;
                Y++;
                X++;
            }
            X = x + 1;
            Y = y - 1;

            //Down Right
            while (Y > -1 && X < 8)
            {
                if (isWhite(X, Y) == true)
                    break;
                testLegalMoves[X, Y] = true;
                if (isBlack(X, Y) == true)
                    break;
                Y--;
                X++;
            }

            X = x - 1;
            Y = y + 1;
            //Up Left
            while (X > -1 && Y < 8)
            {
                if (isWhite(X, Y) == true)
                    break;
                testLegalMoves[X, Y] = true;
                if (isBlack(X, Y) == true)
                    break;
                X--;
                Y++;
            }

            X = x - 1;
            Y = y - 1;
            //Down Left
            while (X > -1 && Y > -1)
            {
                if (isWhite(X, Y) == true)
                    break;
                testLegalMoves[X, Y] = true;
                if (isBlack(X, Y) == true)
                    break;
                X--;
                Y--;
            }
        }

        //White Knight
        if (piece == whiteKnight)
        {
            if (x + 1 < 8 && y + 2 < 8 && isWhite(x + 1, y + 2) == false)
            {
                testLegalMoves[x + 1, y + 2] = true;
            }

            if (x + 2 < 8 && y + 1 < 8 && isWhite(x + 2, y + 1) == false)
            {
                testLegalMoves[x + 2, y + 1] = true;
            }

            if (x - 1 >= 0 && y + 2 < 8 && isWhite(x - 1, y + 2) == false)
            {
                testLegalMoves[x - 1, y + 2] = true;
            }

            if (x - 2 >= 0 && y + 1 < 8 && isWhite(x - 2, y + 1) == false)
            {
                testLegalMoves[x - 2, y + 1] = true;
            }

            if (x + 2 < 8 && y - 1 >= 0 && isWhite(x + 2, y - 1) == false)
            {
                testLegalMoves[x + 2, y - 1] = true;
            }

            if (x + 1 < 8 && y - 2 >= 0 && isWhite(x + 1, y - 2) == false)
            {
                testLegalMoves[x + 1, y - 2] = true;
            }

            if (x - 2 >= 0 && y - 1 >= 0 && isWhite(x - 2, y - 1) == false)
            {
                testLegalMoves[x - 2, y - 1] = true;
            }
            if (x - 1 >= 0 && y - 2 >= 0 && isWhite(x - 1, y - 2) == false)
            {
                testLegalMoves[x - 1, y - 2] = true;
            }
        }

        //White Pawn
        if (piece == whitePawn)
        {
            //Up one
            if (board[x, y + 1] == 0)
            {
                testLegalMoves[x, y + 1] = true;
            }
            //Up two
            if (y == 1 && board[x, y + 1] == 0 && board[x, y + 2] == 0)
            {
                testLegalMoves[x, y + 2] = true;
            }
            try
            {
                if (isBlack(x - 1, y + 1) == true)
                {
                    testLegalMoves[x - 1, y + 1] = true;
                }
                if (isBlack(x + 1, y + 1) == true)
                {
                    testLegalMoves[x + 1, y + 1] = true;
                }
            }
            catch
            {

            }
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
                        if (board[X, Y] == 0 || isWhite(X, Y) == true)
                        {
                            testLegalMoves[X, Y] = true;
                        }
                    }
                    catch { }
                }
            }
            testLegalMoves[x, y] = false;

            //Black King Side Castle
            if (isBlackKingCastle == true && board[5, 7] == empty && board[6, 7] == empty && board[7, 7] == blackRook && board[4, 7] == blackKing)
            {
                testLegalMoves[6, 7] = true;
            }
            //Black Queen Side Castle
            if (isBlackQueenCastle == true && board[3, 7] == empty && board[2, 7] == empty && board[1, 7] == empty && board[0, 7] == blackRook && board[4, 7] == blackKing)
            {
                testLegalMoves[2, 7] = true;
            }
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
                if (isBlack(X, Y) == true)
                    break;
                testLegalMoves[X, Y] = true;
                if (isWhite(X, Y) == true)
                    break;
                Y++;
            }
            Y = y - 1;
            //Down
            while (Y > -1)
            {
                if (isBlack(X, Y) == true)
                    break;
                testLegalMoves[X, Y] = true;
                if (isWhite(X, Y) == true)
                    break;
                Y--;
            }
            X = x + 1;
            Y = y;
            //Right
            while (X < 8)
            {
                if (isBlack(X, Y) == true)
                    break;
                testLegalMoves[X, Y] = true;
                if (isWhite(X, Y) == true)
                    break;
                X++;
            }
            X = x - 1;
            //Left
            while (X > -1)
            {
                if (isBlack(X, Y) == true)
                    break;
                testLegalMoves[X, Y] = true;
                if (isWhite(X, Y) == true)
                    break;
                X--;
            }
            //Up Right
            X = x + 1;
            Y = y + 1;
            while (X < 8 && Y < 8)
            {
                if (isBlack(X, Y) == true)
                    break;
                testLegalMoves[X, Y] = true;
                if (isWhite(X, Y) == true)
                    break;
                Y++;
                X++;
            }
            X = x + 1;
            Y = y - 1;

            //Down Right
            while (Y > -1 && X < 8)
            {
                if (isBlack(X, Y) == true)
                    break;
                testLegalMoves[X, Y] = true;
                if (isWhite(X, Y) == true)
                    break;
                Y--;
                X++;
            }

            X = x - 1;
            Y = y + 1;
            //Up Left
            while (X > -1 && Y < 8)
            {
                if (isBlack(X, Y) == true)
                    break;
                testLegalMoves[X, Y] = true;
                if (isWhite(X, Y) == true)
                    break;
                X--;
                Y++;
            }

            X = x - 1;
            Y = y - 1;
            //Down Left
            while (X > -1 && Y > -1)
            {
                if (isBlack(X, Y) == true)
                    break;
                testLegalMoves[X, Y] = true;
                if (isWhite(X, Y) == true)
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
                if (isBlack(X, Y) == true)
                    break;
                testLegalMoves[X, Y] = true;
                if (isWhite(X, Y) == true)
                    break;
                Y++;
            }
            Y = y - 1;
            //Down
            while (Y > -1)
            {
                if (isBlack(X, Y) == true)
                    break;
                testLegalMoves[X, Y] = true;
                if (isWhite(X, Y) == true)
                    break;
                Y--;
            }
            X = x + 1;
            Y = y;
            //Right
            while (X < 8)
            {
                if (isBlack(X, Y) == true)
                    break;
                testLegalMoves[X, Y] = true;
                if (isWhite(X, Y) == true)
                    break;
                X++;
            }
            X = x - 1;
            //Left
            while (X > -1)
            {
                if (isBlack(X, Y) == true)
                    break;
                testLegalMoves[X, Y] = true;
                if (isWhite(X, Y) == true)
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
                if (isBlack(X, Y) == true)
                    break;
                testLegalMoves[X, Y] = true;
                if (isWhite(X, Y) == true)
                    break;
                Y++;
                X++;
            }
            X = x + 1;
            Y = y - 1;

            //Down Right
            while (Y > -1 && X < 8)
            {
                if (isBlack(X, Y) == true)
                    break;
                testLegalMoves[X, Y] = true;
                if (isWhite(X, Y) == true)
                    break;
                Y--;
                X++;
            }

            X = x - 1;
            Y = y + 1;
            //Up Left
            while (X > -1 && Y < 8)
            {
                if (isBlack(X, Y) == true)
                    break;
                testLegalMoves[X, Y] = true;
                if (isWhite(X, Y) == true)
                    break;
                X--;
                Y++;
            }

            X = x - 1;
            Y = y - 1;
            //Down Left
            while (X > -1 && Y > -1)
            {
                if (isBlack(X, Y) == true)
                    break;
                testLegalMoves[X, Y] = true;
                if (isWhite(X, Y) == true)
                    break;
                X--;
                Y--;
            }
        }

        //Black Knight
        if (piece == blackKnight)
        {
            if (x + 1 < 8 && y + 2 < 8 && isBlack(x + 1, y + 2) == true)
            {
                testLegalMoves[x + 1, y + 2] = true;
            }

            if (x + 2 < 8 && y + 1 < 8 && isBlack(x + 2, y + 1) == false)
            {
                testLegalMoves[x + 2, y + 1] = true;
            }

            if (x - 1 >= 0 && y + 2 < 8 && isBlack(x - 1, y + 2) == false)
            {
                testLegalMoves[x - 1, y + 2] = true;
            }

            if (x - 2 >= 0 && y + 1 < 8 && isBlack(x - 2, y + 1) == false)
            {
                testLegalMoves[x - 2, y + 1] = true;
            }

            if (x + 2 < 8 && y - 1 >= 0 && isBlack(x + 2, y - 1) == false)
            {
                testLegalMoves[x + 2, y - 1] = true;
            }

            if (x + 1 < 8 && y - 2 >= 0 && isBlack(x + 1, y - 2) == false)
            {
                testLegalMoves[x + 1, y - 2] = true;
            }

            if (x - 2 >= 0 && y - 1 >= 0 && isBlack(x - 2, y - 1) == false)
            {
                testLegalMoves[x - 2, y - 1] = true;
            }
            if (x - 1 >= 0 && y - 2 >= 0 && isBlack(x - 1, y - 2) == false)
            {
                testLegalMoves[x - 1, y - 2] = true;
            }
        }

        //Black Pawn
        if (piece == blackPawn)
        {
            //Down one
            if (board[x, y - 1] == 0)
            {
                testLegalMoves[x, y - 1] = true;
            }
            //Down two
            if (y == 6 && board[x, y - 1] == 0 && board[x, y - 2] == 0)
            {
                testLegalMoves[x, y - 2] = true;
            }
            try
            {
                if (isWhite(x - 1, y - 1) == true)
                {
                    testLegalMoves[x - 1, y - 1] = true;
                }
                if (isWhite(x + 1, y - 1) == true)
                {
                    testLegalMoves[x + 1, y - 1] = true;
                }
            }
            catch
            {
            }
        }
    }

    void GenerateLegalMovesArray(int x, int y)
    {
        clearLegalMovesArray();
        int piece = board[x, y];

        //White Pieces
        if (turn == 'w')
        {
            //White King
            if (piece == whiteKing)
            {
                for (int X = x - 1; X < x + 2; X++)
                {
                    for (int Y = y - 1; Y < y + 2; Y++)
                    {
                        try
                        {
                            if (board[X, Y] == 0 || isWhite(X, Y) == false)
                            {
                                legalMoves[X, Y] = true;
                            }
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
                    if (isWhite(X, Y) == true)
                        break;
                    legalMoves[X, Y] = true;
                    if (isBlack(X, Y) == true)
                        break;
                    Y++;
                }
                Y = y - 1;
                //Down
                while (Y > -1)
                {
                    if (isWhite(X, Y) == true)
                        break;
                    legalMoves[X, Y] = true;
                    if (isBlack(X, Y) == true)
                        break;
                    Y--;
                }
                X = x + 1;
                Y = y;
                //Right
                while (X < 8)
                {
                    if (isWhite(X, Y) == true)
                        break;
                    legalMoves[X, Y] = true;
                    if (isBlack(X, Y) == true)
                        break;
                    X++;
                }
                X = x - 1;
                //Left
                while (X > -1)
                {
                    if (isWhite(X, Y) == true)
                        break;
                    legalMoves[X, Y] = true;
                    if (isBlack(X, Y) == true)
                        break;
                    X--;
                }
                //Up Right
                X = x + 1;
                Y = y + 1;
                while (X < 8 && Y < 8)
                {
                    if (isWhite(X, Y) == true)
                        break;
                    legalMoves[X, Y] = true;
                    if (isBlack(X, Y) == true)
                        break;
                    Y++;
                    X++;
                }
                X = x + 1;
                Y = y - 1;

                //Down Right
                while (Y > -1 && X < 8)
                {
                    if (isWhite(X, Y) == true)
                        break;
                    legalMoves[X, Y] = true;
                    if (isBlack(X, Y) == true)
                        break;
                    Y--;
                    X++;
                }

                X = x - 1;
                Y = y + 1;
                //Up Left
                while (X > -1 && Y < 8)
                {
                    if (isWhite(X, Y) == true)
                        break;
                    legalMoves[X, Y] = true;
                    if (isBlack(X, Y) == true)
                        break;
                    X--;
                    Y++;
                }

                X = x - 1;
                Y = y - 1;
                //Down Left
                while (X > -1 && Y > -1)
                {
                    if (isWhite(X, Y) == true)
                        break;
                    legalMoves[X, Y] = true;
                    if (isBlack(X, Y) == true)
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
                    if (isWhite(X, Y) == true)
                        break;
                    legalMoves[X, Y] = true;
                    if (isBlack(X, Y) == true)
                        break;
                    Y++;
                }
                Y = y - 1;
                //Down
                while (Y > -1)
                {
                    if (isWhite(X, Y) == true)
                        break;
                    legalMoves[X, Y] = true;
                    if (isBlack(X, Y) == true)
                        break;
                    Y--;
                }
                X = x + 1;
                Y = y;
                //Right
                while (X < 8)
                {
                    if (isWhite(X, Y) == true)
                        break;
                    legalMoves[X, Y] = true;
                    if (isBlack(X, Y) == true)
                        break;
                    X++;
                }
                X = x - 1;
                //Left
                while (X > -1)
                {
                    if (isWhite(X, Y) == true)
                        break;
                    legalMoves[X, Y] = true;
                    if (isBlack(X, Y) == true)
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
                    if (isWhite(X, Y) == true)
                        break;
                    legalMoves[X, Y] = true;
                    if (isBlack(X, Y) == true)
                        break;
                    Y++;
                    X++;
                }
                X = x + 1;
                Y = y - 1;

                //Down Right
                while (Y > -1 && X < 8)
                {
                    if (isWhite(X, Y) == true)
                        break;
                    legalMoves[X, Y] = true;
                    if (isBlack(X, Y) == true)
                        break;
                    Y--;
                    X++;
                }

                X = x - 1;
                Y = y + 1;
                //Up Left
                while (X > -1 && Y < 8)
                {
                    if (isWhite(X, Y) == true)
                        break;
                    legalMoves[X, Y] = true;
                    if (isBlack(X, Y) == true)
                        break;
                    X--;
                    Y++;
                }

                X = x - 1;
                Y = y - 1;
                //Down Left
                while (X > -1 && Y > -1)
                {
                    if (isWhite(X, Y) == true)
                        break;
                    legalMoves[X, Y] = true;
                    if (isBlack(X, Y) == true)
                        break;
                    X--;
                    Y--;
                }
            }

            //White Knight
            if (piece == whiteKnight)
            {
                if (x + 1 < 8 && y + 2 < 8 && isWhite(x + 1, y + 2) == false)
                {
                    legalMoves[x + 1, y + 2] = true;
                }

                if (x + 2 < 8 && y + 1 < 8 && isWhite(x + 2, y + 1) == false)
                {
                    legalMoves[x + 2, y + 1] = true;
                }

                if (x - 1 >= 0 && y + 2 < 8 && isWhite(x - 1, y + 2) == false)
                {
                    legalMoves[x - 1, y + 2] = true;
                }

                if (x - 2 >= 0 && y + 1 < 8 && isWhite(x - 2, y + 1) == false)
                {
                    legalMoves[x - 2, y + 1] = true;
                }

                if (x + 2 < 8 && y - 1 >= 0 && isWhite(x + 2, y - 1) == false)
                {
                    legalMoves[x + 2, y - 1] = true;
                }

                if (x + 1 < 8 && y - 2 >= 0 && isWhite(x + 1, y - 2) == false)
                {
                    legalMoves[x + 1, y - 2] = true;
                }

                if (x - 2 >= 0 && y - 1 >= 0 && isWhite(x - 2, y - 1) == false)
                {
                    legalMoves[x - 2, y - 1] = true;
                }
                if (x - 1 >= 0 && y - 2 >= 0 && isWhite(x - 1, y - 2) == false)
                {
                    legalMoves[x - 1, y - 2] = true;
                }
            }

            //White Pawn
            if (piece == whitePawn)
            {
                //Up one
                if (board[x, y + 1] == 0)
                {
                    legalMoves[x, y + 1] = true;
                }
                //Up two
                if (y == 1 && board[x, y + 1] == 0 && board[x, y + 2] == 0)
                {
                    legalMoves[x, y + 2] = true;
                }
                try
                {
                    if (isBlack(x - 1, y + 1) == true)
                    {
                        legalMoves[x - 1, y + 1] = true;
                    }
                    if (isBlack(x + 1, y + 1) == true)
                    {
                        legalMoves[x + 1, y + 1] = true;
                    }
                }
                catch
                {
                }
            }
        }
        //Black Pieces
        if (turn == 'b')
        {
            //Black King
            if (piece == blackKing)
            {
                for (int X = x - 1; X < x + 2; X++)
                {
                    for (int Y = y - 1; Y < y + 2; Y++)
                    {
                        try
                        {
                            if (board[X, Y] == 0 || isWhite(X, Y) == true)
                            {
                                legalMoves[X, Y] = true;
                            }
                        }
                        catch { }
                    }
                }
                legalMoves[x, y] = false;

                //Black King Side Castle
                if (isBlackKingCastle == true && board[5, 7] == empty && board[6, 7] == empty && board[7, 7] == blackRook && board[4, 7] == blackKing)
                {
                    legalMoves[6, 7] = true;
                }
                //Black Queen Side Castle
                if (isBlackQueenCastle == true && board[3, 7] == empty && board[2, 7] == empty && board[1, 7] == empty && board[0, 7] == blackRook && board[4, 7] == blackKing)
                {
                    legalMoves[2, 7] = true;
                }
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
                    if (isBlack(X, Y) == true)
                        break;
                    legalMoves[X, Y] = true;
                    if (isWhite(X, Y) == true)
                        break;
                    Y++;
                }
                Y = y - 1;
                //Down
                while (Y > -1)
                {
                    if (isBlack(X, Y) == true)
                        break;
                    legalMoves[X, Y] = true;
                    if (isWhite(X, Y) == true)
                        break;
                    Y--;
                }
                X = x + 1;
                Y = y;
                //Right
                while (X < 8)
                {
                    if (isBlack(X, Y) == true)
                        break;
                    legalMoves[X, Y] = true;
                    if (isWhite(X, Y) == true)
                        break;
                    X++;
                }
                X = x - 1;
                //Left
                while (X > -1)
                {
                    if (isBlack(X, Y) == true)
                        break;
                    legalMoves[X, Y] = true;
                    if (isWhite(X, Y) == true)
                        break;
                    X--;
                }
                //Up Right
                X = x + 1;
                Y = y + 1;
                while (X < 8 && Y < 8)
                {
                    if (isBlack(X, Y) == true)
                        break;
                    legalMoves[X, Y] = true;
                    if (isWhite(X, Y) == true)
                        break;
                    Y++;
                    X++;
                }
                X = x + 1;
                Y = y - 1;

                //Down Right
                while (Y > -1 && X < 8)
                {
                    if (isBlack(X, Y) == true)
                        break;
                    legalMoves[X, Y] = true;
                    if (isWhite(X, Y) == true)
                        break;
                    Y--;
                    X++;
                }

                X = x - 1;
                Y = y + 1;
                //Up Left
                while (X > -1 && Y < 8)
                {
                    if (isBlack(X, Y) == true)
                        break;
                    legalMoves[X, Y] = true;
                    if (isWhite(X, Y) == true)
                        break;
                    X--;
                    Y++;
                }

                X = x - 1;
                Y = y - 1;
                //Down Left
                while (X > -1 && Y > -1)
                {
                    if (isBlack(X, Y) == true)
                        break;
                    legalMoves[X, Y] = true;
                    if (isWhite(X, Y) == true)
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
                    if (isBlack(X, Y) == true)
                        break;
                    legalMoves[X, Y] = true;
                    if (isWhite(X, Y) == true)
                        break;
                    Y++;
                }
                Y = y - 1;
                //Down
                while (Y > -1)
                {
                    if (isBlack(X, Y) == true)
                        break;
                    legalMoves[X, Y] = true;
                    if (isWhite(X, Y) == true)
                        break;
                    Y--;
                }
                X = x + 1;
                Y = y;
                //Right
                while (X < 8)
                {
                    if (isBlack(X, Y) == true)
                        break;
                    legalMoves[X, Y] = true;
                    if (isWhite(X, Y) == true)
                        break;
                    X++;
                }
                X = x - 1;
                //Left
                while (X > -1)
                {
                    if (isBlack(X, Y) == true)
                        break;
                    legalMoves[X, Y] = true;
                    if (isWhite(X, Y) == true)
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
                    if (isBlack(X, Y) == true)
                        break;
                    legalMoves[X, Y] = true;
                    if (isWhite(X, Y) == true)
                        break;
                    Y++;
                    X++;
                }
                X = x + 1;
                Y = y - 1;
                //Down Right
                while (Y > -1 && X < 8)
                {
                    if (isBlack(X, Y) == true)
                        break;
                    legalMoves[X, Y] = true;
                    if (isWhite(X, Y) == true)
                        break;
                    Y--;
                    X++;
                }
                X = x - 1;
                Y = y + 1;
                //Up Left
                while (X > -1 && Y < 8)
                {
                    if (isBlack(X, Y) == true)
                        break;
                    legalMoves[X, Y] = true;
                    if (isWhite(X, Y) == true)
                        break;
                    X--;
                    Y++;
                }
                X = x - 1;
                Y = y - 1;
                //Down Left
                while (X > -1 && Y > -1)
                {
                    if (isBlack(X, Y) == true)
                        break;
                    legalMoves[X, Y] = true;
                    if (isWhite(X, Y) == true)
                        break;
                    X--;
                    Y--;
                }
            }
            //Black Knight
            if (piece == blackKnight)
            {
                if (x + 1 < 8 && y + 2 < 8 && isBlack(x + 1, y + 2) == true)
                {
                    legalMoves[x + 1, y + 2] = true;
                }

                if (x + 2 < 8 && y + 1 < 8 && isBlack(x + 2, y + 1) == false)
                {
                    legalMoves[x + 2, y + 1] = true;
                }

                if (x - 1 >= 0 && y + 2 < 8 && isBlack(x - 1, y + 2) == false)
                {
                    legalMoves[x - 1, y + 2] = true;
                }

                if (x - 2 >= 0 && y + 1 < 8 && isBlack(x - 2, y + 1) == false)
                {
                    legalMoves[x - 2, y + 1] = true;
                }

                if (x + 2 < 8 && y - 1 >= 0 && isBlack(x + 2, y - 1) == false)
                {
                    legalMoves[x + 2, y - 1] = true;
                }

                if (x + 1 < 8 && y - 2 >= 0 && isBlack(x + 1, y - 2) == false)
                {
                    legalMoves[x + 1, y - 2] = true;
                }

                if (x - 2 >= 0 && y - 1 >= 0 && isBlack(x - 2, y - 1) == false)
                {
                    legalMoves[x - 2, y - 1] = true;
                }
                if (x - 1 >= 0 && y - 2 >= 0 && isBlack(x - 1, y - 2) == false)
                {
                    legalMoves[x - 1, y - 2] = true;
                }
            }
            //Black Pawn
            if (piece == blackPawn)
            {
                //Down one
                if (board[x, y - 1] == 0)
                {
                    legalMoves[x, y - 1] = true;
                }
                //Down two
                if (y == 6 && board[x, y - 1] == 0 && board[x, y - 2] == 0)
                {
                    legalMoves[x, y - 2] = true;
                }
                try
                {
                    if (isWhite(x - 1, y - 1) == true)
                    {
                        legalMoves[x - 1, y - 1] = true;
                    }
                    if (isWhite(x + 1, y - 1) == true)
                    {
                        legalMoves[x + 1, y - 1] = true;
                    }
                }
                catch
                {
                }
            }
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

                    //debugBoardArray();

                    //creates total possible move array from opposite color
                    for(int file = 0; file < 8; file++)
                    {
                        for (int rank = 0; rank < 8; rank++)
                        {
                            if(getColor(file, rank) != 'e')
                            {
                                if (getColor(file, rank) != turn)
                                {
                                    GenerateTestLegalMovesArray(file, rank);
                                }
                            }
                        }
                    }
                    //debugTestLegalMoveArray();

                    for (int file = 0; file < 8; file++)
                    {
                        for (int rank = 0; rank < 8; rank++)
                        {
                            if (testLegalMoves[file, rank] == true && turn == 'w' && board[file, rank] == whiteKing )
                            {
                                legalMoves[X, Y] = false;
                            }
                            if (testLegalMoves[file, rank] == true && turn == 'b' && board[file, rank] == blackKing)
                            {
                                legalMoves[X, Y] = false;
                            }
                        }
                    }

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

        for (int X = 0; X < 8; X++)
        {
            for (int Y = 0; Y < 8; Y++)
            {
               if(board[X,Y] == whiteKing)
                {

                }
            }
        }
    }

    char getColor(int x, int y)
    {
        if (board[x, y] > 0 && board[x, y] < 7)
        {
            return 'w';
        }
        if (board[x, y] > 6 && board[x, y] < 13)
        {
            return 'b';
        }
        return 'e';
    }

    void ClearLegalMovesDots()
    {
        GameObject[] circles = GameObject.FindGameObjectsWithTag("circle");
        foreach (GameObject circle in circles)
            GameObject.Destroy(circle);
    }

    void ShowLegalMovesDots()
    {
        ClearLegalMovesDots();
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                if (legalMoves[x, y] == true)
                {
                    Instantiate(yellowCircle, new Vector3(x, y, 1), Quaternion.identity);
                }
            }
        }
    }
    void clearLegalMovesArray()
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                legalMoves[x, y] = false;
            }
        }
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
    void createBoard()
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if ((x + y) % 2 == 0)
                {
                    Instantiate(blackSquare, new Vector3(x, y, 1), Quaternion.identity);
                }
                else
                {
                    Instantiate(whiteSquare, new Vector3(x, y, 1), Quaternion.identity);
                }
            }
        }
    }

    void SetPieces()
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (board[x, y] == whiteKing)
                {
                    Instantiate(wKing, new Vector3(x, y, 0), Quaternion.identity);
                }
                else if (board[x, y] == whiteQueen)
                {
                    Instantiate(wQueen, new Vector3(x, y, 0), Quaternion.identity);
                }
                else if (board[x, y] == whiteRook)
                {
                    Instantiate(wRook, new Vector3(x, y, 0), Quaternion.identity);
                }
                else if (board[x, y] == whiteBishop)
                {
                    Instantiate(wBishop, new Vector3(x, y, 0), Quaternion.identity);
                }
                else if (board[x, y] == whiteKnight)
                {
                    Instantiate(wKnight, new Vector3(x, y, 0), Quaternion.identity);
                }
                else if (board[x, y] == whitePawn)
                {
                    Instantiate(wPawn, new Vector3(x, y, 0), Quaternion.identity);
                }
                else if (board[x, y] == blackKing)
                {
                    Instantiate(bKing, new Vector3(x, y, 0), Quaternion.identity);
                }
                else if (board[x, y] == blackQueen)
                {
                    Instantiate(bQueen, new Vector3(x, y, 0), Quaternion.identity);
                }
                else if (board[x, y] == blackRook)
                {
                    Instantiate(bRook, new Vector3(x, y, 0), Quaternion.identity);
                }
                else if (board[x, y] == blackBishop)
                {
                    Instantiate(bBishop, new Vector3(x, y, 0), Quaternion.identity);
                }
                else if (board[x, y] == blackKnight)
                {
                    Instantiate(bKnight, new Vector3(x, y, 0), Quaternion.identity);
                }
                else if (board[x, y] == blackPawn)
                {
                    Instantiate(bPawn, new Vector3(x, y, 0), Quaternion.identity);
                }
            }
        }
    }

    void ClearBoard()
    {
        GameObject[] pieces = GameObject.FindGameObjectsWithTag("chessPiece");
        foreach (GameObject piece in pieces)
        GameObject.Destroy(piece);
    }

    void SetPiecesToArray(string fen)
    {
        int x = 0;
        int y = 7;
        int length = fen.Length;
        char a = fen[0];

        for (int i = 0; i < length; i++)
        {
            if (fen[i] <= '9' && fen[i] >= '0')
            {
                x = x + (fen[i] - '0');
            }
            else if (fen[i] == '/')
            {
                y--;
                x = 0;
            }
            else if (fen[i] == 'K')
            {
                board[x, y] = whiteKing;
                x++;
            }
            else if (fen[i] == 'Q')
            {
                board[x, y] = whiteQueen;
                x++;
            }
            else if (fen[i] == 'R')
            {
                board[x, y] = whiteRook;
                x++;
            }
            else if (fen[i] == 'N')
            {
                board[x, y] = whiteKnight;
                x++;
            }
            else if (fen[i] == 'B')
            {
                board[x, y] = whiteBishop;
                x++;
            }
            else if (fen[i] == 'P')
            {
                board[x, y] = whitePawn;
                x++;
            }
            else if (fen[i] == 'k')
            {
                board[x, y] = blackKing;
                x++;
            }
            else if (fen[i] == 'q')
            {
                board[x, y] = blackQueen;
                x++;
            }
            else if (fen[i] == 'r')
            {
                board[x, y] = blackRook;
                x++;
            }
            else if (fen[i] == 'n')
            {
                board[x, y] = blackKnight;
                x++;
            }
            else if (fen[i] == 'b')
            {
                board[x, y] = blackBishop;
                x++;
            }
            else if (fen[i] == 'p')
            {
                board[x, y] = blackPawn;
                x++;
            }
        }
    }

    void debugTestLegalMoveArray()
    {
        string boardString = "";

        for (int y = 7; y >= 0; y--)
        {
            for (int x = 0; x < 8; x++)
            {
                if (testLegalMoves[x, y] == true)
                {
                    boardString += "1";
                }
                else
                {
                    boardString += "0";
                }
            }
            boardString += "\n";
        }
        Debug.Log(boardString);
    }

    void debugBoardArray()
    {
        string boardString = "";

        for (int y = 7; y >= 0; y--)
        {
            for (int x = 0; x < 8; x++)
            {
                boardString += GetPieceName(board[x, y]);
            }
            boardString += "\n";
        }
        Debug.Log(boardString);
    }

    void debugLegalMoveArray()
    {
        string boardString = "";

        for (int y = 7; y >= 0; y--)
        {
            for (int x = 0; x < 8; x++)
            {
                if (legalMoves[x, y] == true)
                {
                    boardString += 1;
                }
                else
                {
                    boardString += 0;
                }
            }
            boardString += "\n";
        }
        Debug.Log(boardString);
    }

    public void nextTurn()
    {
        if (turn == 'w')
        {
            turn = 'b';
        }
        else if (turn == 'b')
        {
            turn = 'w';
        }
    }
    private string GetPieceName(int num)
    {
        string name = "";
        if (num == empty)
            return "[]";
        if (num == whiteKing)
            return "K";
        if (num == whiteQueen)
            return "Q";
        if (num == whiteRook)
            return "R";
        if (num == whiteKnight)
            return "N";
        if (num == whiteBishop)
            return "B";
        if (num == whitePawn)
            return "P";
        if (num == blackKing)
            return "k";
        if (num == blackQueen)
            return "q";
        if (num == blackRook)
            return "r";
        if (num == blackKnight)
            return "n";
        if (num == blackBishop)
            return "b";
        if (num == blackPawn)
            return "p";
        return "null";
    }

    bool isWhite(int x, int y)
    {
        if (board[x, y] > 0 && board[x, y] < 7)
            return true;
        return false;
    }

    bool isBlack(int x, int y)
    {
        if (board[x, y] > 6 && board[x, y] < 13)
            return true;
        return false;
    }
}