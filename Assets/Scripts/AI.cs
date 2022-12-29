using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    public Moves move;
    public gameManger gameManger;

    bool[,] legalMoves = new bool[8, 8];

    public void RandomAIMove(char color)
    {
        Vector2Int start = new Vector2Int();
        Vector2Int final = new Vector2Int();
        bool IsAIPieceSelected = new bool();
        IsAIPieceSelected = false;

        //selectes random piece (start)
        while (IsAIPieceSelected == false)
        {
            start = new Vector2Int(Random.Range(0, 8), Random.Range(0, 8));
            clearLegalMovesArray();
            move.GenerateLegalMovesArray(start.x, start.y, gameManger.board, legalMoves);
            if (getColor(start.x, start.y, gameManger.board) == color)
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
        clearLegalMovesArray();
        move.GenerateLegalMovesArray(start.x, start.y, gameManger.board, legalMoves);

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (legalMoves[x, y] == true)
                {
                    moves.Add(new Vector2Int(x, y));
                }
            }
        }
        final = moves[Random.Range(0, moves.Count)];

        //Processes move
        move.ProcessMove(start, final, gameManger.board, legalMoves);
    }
    
    char getColor(int x, int y, int[,] board)
    {
        if (board[x, y] > 0 && board[x, y] < 7)
            return 'w';
        if (board[x, y] > 6 && board[x, y] < 13)
            return 'b';
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
}
