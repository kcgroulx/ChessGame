using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class gameManger : MonoBehaviour
{
    public int[,] board = new int[8, 8];
    public bool[,] legalMoves = new bool[8, 8];

    public setBoard setboard;
    public Moves moves;
    public AI ai;

    public bool isWhiteMated = false;
    public bool isBlackMated = false;
    public bool isWhiteAI;
    public bool isBlackAI;

    Vector2Int clickPos;
    Vector2Int start;
    Vector2Int final;
    bool isPieceSelected;
    public char turn = 'w';

    string fenStandard = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";

    void Start()
    {
        setboard.createBoard();
        setboard.SetPiecesToArray(fenStandard, board);
        setboard.SetPieces(board);
    }

    void Update()
    {
        checkForMouseMove();


        //Thread.Sleep(100);
        if (isBlackAI == true && turn == 'b' && moves.checkForMate(board, 'b') == false)
        {
            ai.RandomAIMove('b');
            setboard.SetPieces(board);
        }
        //Thread.Sleep(100);
        if (isWhiteAI == true && turn == 'w' && moves.checkForMate(board, 'w') == false)
        {
            ai.RandomAIMove('w');
            setboard.SetPieces(board);
        }
    }

    void checkForMouseMove()
    {
        if (Input.GetMouseButtonUp(0))
        {
            //Sets clicksPos to where the user clicks on the board
            clickPos = new Vector2Int((int)Mathf.Round(Camera.main.ScreenToWorldPoint(Input.mousePosition).x), (int)Mathf.Round(Camera.main.ScreenToWorldPoint(Input.mousePosition).y));
            
            //Selects the first piece
            if (isPieceSelected == false && board[clickPos.x, clickPos.y] > 0 && getColor(clickPos.x, clickPos.y, board) == turn)
            {
                start = clickPos;
                moves.GenerateLegalMovesArray(start.x, start.y, board, legalMoves);
                setboard.ClearLegalMovesDots();
                setboard.ShowLegalMovesDots(legalMoves);
                isPieceSelected = true;
            }

            //Second click
            if (isPieceSelected == true)
            {
                //Second click is a fail
                if (getColor(start.x, start.y, board) == getColor(clickPos.x, clickPos.y, board))
                {
                    clearLegalMovesArray();
                    start = clickPos;
                    moves.GenerateLegalMovesArray(start.x, start.y, board, legalMoves);
                    setboard.ShowLegalMovesDots(legalMoves);
                }
                //Second click is a success
                else
                {
                    final = clickPos;
                    moves.ProcessMove(start, final, board, legalMoves);
                    isPieceSelected = false;
                    isWhiteMated = moves.checkForMate(board, 'w');
                    isBlackMated = moves.checkForMate(board, 'b');
                    clearLegalMovesArray();
                    setboard.ClearLegalMovesDots();
                    setboard.SetPieces(board);
                }
            }
        }
    }

    char getColor(int x, int y, int[,] board)
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

    public void nextTurn()
    {
        if (turn == 'w')
            turn = 'b';
        else if (turn == 'b')
            turn = 'w';
    }
}