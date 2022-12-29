using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class setBoard : MonoBehaviour
{
    public GameObject blackSquare;
    public GameObject whiteSquare;
    public GameObject yellowCircle;
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
    void Start()
    {
    }

    //Sets the board Pieces
    public void SetPieces(int[,] board)
    {
        ClearPieces();
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (board[x, y] == whiteKing)
                    Instantiate(wKing, new Vector3(x, y, 0), Quaternion.identity);
                else if (board[x, y] == whiteQueen)
                    Instantiate(wQueen, new Vector3(x, y, 0), Quaternion.identity);
                else if (board[x, y] == whiteRook)
                    Instantiate(wRook, new Vector3(x, y, 0), Quaternion.identity);
                else if (board[x, y] == whiteBishop)
                    Instantiate(wBishop, new Vector3(x, y, 0), Quaternion.identity);
                else if (board[x, y] == whiteKnight)
                    Instantiate(wKnight, new Vector3(x, y, 0), Quaternion.identity);
                else if (board[x, y] == whitePawn)
                    Instantiate(wPawn, new Vector3(x, y, 0), Quaternion.identity);
                else if (board[x, y] == blackKing)
                    Instantiate(bKing, new Vector3(x, y, 0), Quaternion.identity);
                else if (board[x, y] == blackQueen)
                    Instantiate(bQueen, new Vector3(x, y, 0), Quaternion.identity);
                else if (board[x, y] == blackRook)
                    Instantiate(bRook, new Vector3(x, y, 0), Quaternion.identity);
                else if (board[x, y] == blackBishop)
                    Instantiate(bBishop, new Vector3(x, y, 0), Quaternion.identity);
                else if (board[x, y] == blackKnight)
                    Instantiate(bKnight, new Vector3(x, y, 0), Quaternion.identity);
                else if (board[x, y] == blackPawn)
                    Instantiate(bPawn, new Vector3(x, y, 0), Quaternion.identity);
            }
        }
    }

    void ClearPieces()
    {
        GameObject[] pieces = GameObject.FindGameObjectsWithTag("chessPiece");
        foreach (GameObject piece in pieces)
            GameObject.Destroy(piece);
    }

    //Creates the checkered pattern board
    public void createBoard()
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
    
    //Yellow Dots
    public void ShowLegalMovesDots(bool[,] legalMoves)
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

    //Clears the legal move dots from the board
    public void ClearLegalMovesDots()
    {
        GameObject[] circles = GameObject.FindGameObjectsWithTag("circle");
        foreach (GameObject circle in circles)
            GameObject.Destroy(circle);
    }

    //Sets the board array from a fen string
    public void SetPiecesToArray(string fen, int[,] board)
    {
        int x = 0;
        int y = 7;
        int length = fen.Length;

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

}
