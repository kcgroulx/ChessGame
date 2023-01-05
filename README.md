# ChessGame
A fully functioning Chess Game and AI I developed using Unity and C#. The program supports all chess moves including pawn promotion, castling and en passent. It also has legal moves generation, which displays all legal moves a piece is able to make, taking into account moves disallowed due to king safety.  
I added a very basic AI opponent, however, I'm currently working on a more complex version which will be able to see multiple moves ahead using Alpha-Beta pruning and level order searching algorithms.

# :hammer: How it works

In the program, the board is stored as a 2D array of ints, where numbers 0-12 represent the different pieces.  
0 = empty  
1 = whitePawn  
2 = whiteKnight  
...  
11 = blackQueen  
12 = blackKing  
A simple script takes this 2D array of ints and then draws the resulting board onto the screen every turn.
