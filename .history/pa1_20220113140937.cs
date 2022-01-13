#nullable enable
using System;
using static System.Console;

namespace Bme121
{
    record Player(string Colour, string Symbol, string Name);

    // The `record` is a kind of automatic class in the sense that the compiler generates
    // the fields and constructor and some other things just from this one line.
    // There's a rationale for the capital letters on the variable names (later).
    // For now you can think of it as roughly equivalent to a nonstatic `class` with three
    // public fields and a three-parameter constructor which initializes the fields.
    // It would be like the following. The 'readonly' means you can't change the field value
    // after it is set by the constructor method.

    //~ class Player
    //~ {
    //~ public readonly string Colour; //readonly for more security
    //~ public readonly string Symbol;
    //~ public readonly string Name;

    //~ public Player( string Colour, string Symbol, string Name )
    //~ {
    //~ this.Colour = Colour;
    //~ this.Symbol = Symbol;
    //~ this.Name = Name;
    //~ }
    //~ }

    static partial class Program
    {
        // Display common text for the top of the screen.

        static void Welcome()
        {
            WriteLine("Welcome to Othello. Good luck!");
        }

        // Collect a player name or default to form the player record. (holding onto the player info)

        static Player NewPlayer(string colour, string symbol, string defaultName) //returns new player object
        {

            Write("Type the {0} disc ({1}) player name (or <Enter> for '{2}'): ", colour, symbol, defaultName);
            string enteredName = ReadLine();

            if (enteredName.Length == 0)
            {
                enteredName = defaultName;
            }

            return new Player(colour, symbol, enteredName);
        }

        // Determine which player goes first or default.

        static int GetFirstTurn(Player[] players, int defaultFirst)
        {

            WriteLine("Choose which player will go first.");
            WriteLine("Enter '0' for player 1/black/X, Enter '1' for player 2/white/O = '1').");
            Write("or <Enter> for  {defaultFirst} to go first: ");
            string response = ReadLine();

            //check for invalid response and keep asking user to try again until a valid response is reached
            while (response.Length != 0 && response != "0" && response != "1")
            {
                Write("Invalid response. Please try again: ");
                response = ReadLine();
            }

            if (response.Length == 0)
            {
                return defaultFirst;
                WriteLine("By default, player 1 (black) will go first)");
            }

            int playerNumber = int.Parse(response) + 1;

            WriteLine($"Player {response} will go first.");

            return int.Parse(response);
        }

        // Get a board size (between 4 and 26 and even) or default, for one direction.

        static int GetBoardSize(string direction, int defaultSize)
        {
            WriteLine("Enter a number of {0} for the game board (must be an even number between 4 and 26)", direction);
            Write("or <Enter> for default # of {0}: ");
            string response = ReadLine();

            if (response.Length == 0)
            {
                return defaultSize;
            }
            else if (int.Parse(response) < 4 || int.Parse(response) > 26 || int.Parse(response) % 2 != 0)
            {
                Write("Invalid response. Please try again: ");
                response = ReadLine();
            }

            return int.Parse(response);
        }

        // Get a move from a player.

        static string GetMove(Player player)
        {
            WriteLine($"It is {player.Name}'s ({player.Symbol}) turn. Make a move ('row column') or enter 'quit' to exit the game.");
            string move = ReadLine();

            return move;
        }

        // Try to make a move. Return true if it worked.

        static bool TryMove(string[,] board, Player player, string move) //checks if the move is valid
        {
            if (move == "skip")
            {
                return true;
            }

            int row = IndexAtLetter(move[0].ToString());
            int col = IndexAtLetter(move[1].ToString());

            if (row == -1 || col == -1) return false; //if a letter is not entered, -1 is returned from the IndexAtLetter method

            int rowLimit = board.GetLength(0);
            int colLimit = board.GetLength(1);

            if (row >= rowLimit || col >= colLimit) //if entered a row/col that is greater than the size of the board, its invalid
            {
                return false;
            }

            //check for empty string
            if (board[row, col] != " ") return false; //if something is already there, you cant make a move to there

            //update the value of the variable if the TryDirection method works or not so the first direction the move works is the only
            //way the chips are flipped (according to the rules of the game)
            bool isValid = false;

            //for every move, need to check 8 directions
            //the following WriteLines were for testing/debugging

            //WriteLine("Top left");
            isValid = TryDirection(board, player, row, -1, col, -1, isValid); //Top left
                                                                              //WriteLine("Top");
            isValid = TryDirection(board, player, row, -1, col, 0, isValid); //Top
                                                                             //WriteLine("Top right");
            isValid = TryDirection(board, player, row, -1, col, 1, isValid); //Top right
                                                                             //WriteLine("Right");
            isValid = TryDirection(board, player, row, 0, col, 1, isValid); //Right
                                                                            //WriteLine("Bottom Right");
            isValid = TryDirection(board, player, row, 1, col, 1, isValid); //Bottom Right
                                                                            //WriteLine("Bottom");
            isValid = TryDirection(board, player, row, 1, col, 0, isValid); //Bottom
                                                                            //WriteLine("Bottom left");
            isValid = TryDirection(board, player, row, 1, col, -1, isValid); //Bottom Left
                                                                             //WriteLine("Left");
            isValid = TryDirection(board, player, row, 0, col, -1, isValid); //Left

            //the -1, -1, etc. are deltaRow and deltaCol

            return isValid;
        }

        // Do the flips along a direction specified by the row and column delta for one step.

        static bool TryDirection(string[,] board, Player player,
            int moveRow, int deltaRow, int moveCol, int deltaCol, bool isValid)
        {
            if (isValid == true) return true; //this will stop this method right here and just keep returning true

            int nextRow = moveRow + deltaRow;
            int nextCol = moveCol + deltaCol;

            if (nextRow >= board.GetLength(0) || nextCol >= board.GetLength(1) || nextRow < 0 || nextCol < 0) //checks if goes above the bounds so that SPECIFIC direction makes this move invalid (other directions may keep this place valid)
            {
                return false;
            }
            else if (board[nextRow, nextCol] == player.Symbol) //invalid bc nothing in between to flip
            {
                return false;
            }

            while (nextRow < board.GetLength(0) && nextCol < board.GetLength(1) && nextRow >= 0 && nextCol >= 0)
            {
                WriteLine(nextRow + (",") + nextCol);

                if (board[nextRow, nextCol] == " ") //= is assignment == comparison
                {
                    return false;
                }
                else if (board[nextRow, nextCol] == player.Symbol) //this indicates a valid move, so then go through the process of flipping
                {
                    WriteLine(nextRow + "," + nextCol);
                    int currentRow = moveRow + deltaRow;
                    int currentCol = moveCol + deltaCol;

                    while (currentRow != nextRow || currentCol != nextCol)
                    {
                        board[currentRow, currentCol] = player.Symbol;
                        board[moveRow, moveCol] = player.Symbol;
                        currentRow += deltaRow;
                        currentCol += deltaCol;
                    }

                    return true;
                }
                nextRow += deltaRow;
                nextCol += deltaCol;
            }
            return false;
        }


        // Count the discs to find the score for a player.

        static int GetScore(string[,] board, Player player)
        {
            int score = 0;

            //checks each square on the board for the same symbol of the player to count the score
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (board[i, j] == player.Symbol)
                    {
                        score++;
                    }
                }
            }
            return score;
        }

        // Display a line of scores for all players.

        static bool DisplayScores(string[,] board, Player[] players)
        {
            int firstPlayerScore = GetScore(board, players[0]);
            int secondPlayerScore = GetScore(board, players[1]);

            WriteLine($"Points of first player (X): {firstPlayerScore}");
            WriteLine($"Points of second player (O): {secondPlayerScore}");

            //figure out if its the end of the game or not
            if (firstPlayerScore + secondPlayerScore == board.GetLength(0) * board.GetLength(1)) //if total X/Os is = to area of board
            {
                DisplayWinners(board, players);
                return true;
            }
            return false;
        }

        // Display winner(s) and categorize their win over the defeated player(s).

        static void DisplayWinners(string[,] board, Player[] players)
        {
            if (GetScore(board, players[0]) > GetScore(board, players[1]))
            {
                WriteLine($"{players[0].Name} (black, X) is the winner of this game ({players[1].Name}, better luck next time!). Thank you for playing!");
            }
            else
            {
                WriteLine($"{players[1].Name} (white, O) is the winner of this game ({players[0].Name}, better luck next time!). Thank you for playing!");
            }
        }

        static void Main()
        {
            // Set up the players and game.
            // Note: I used an array of 'Player' objects to hold information about the players.
            // This allowed me to just pass one 'Player' object to methods needing to use
            // the player name, colour, or symbol in 'WriteLine' messages or board operation.
            // The array aspect allowed me to use the index to keep track or whose turn it is.
            // It is also possible to use separate variables or separate arrays instead
            // of an array of objects. It is possible to put the player information in
            // global variables (static field variables of the 'Program' class) so they
            // can be accessed by any of the methods without passing them directly as arguments.

            Welcome();
            //player class, array of players
            Player[] players = new Player[] //allocate array with new player array expression w/following initial values
            {
                NewPlayer( colour: "black", symbol: "X", defaultName: "Black" ),
                NewPlayer( colour: "white", symbol: "O", defaultName: "White" ),
            };
            WriteLine();

            int turn = GetFirstTurn(players, defaultFirst: 0);

            WriteLine();
            int rows = GetBoardSize(direction: "rows", defaultSize: 8);
            int cols = GetBoardSize(direction: "columns", defaultSize: 8);

            string[,] game = NewBoard(rows, cols);

            // Play the game.

            bool gameOver = false;
            while (!gameOver)
            {
                Welcome();
                DisplayBoard(game);

                string move = GetMove(players[turn]);
                if (move == "quit")
                {
                    DisplayScores(game, players);
                    gameOver = true;
                }
                else
                {
                    bool madeMove = TryMove(game, players[turn], move);
                    if (madeMove) turn = (turn + 1) % players.Length;
                    else
                    {
                        Write(" Your choice didn't work!");
                        Write(" Press <Enter> to try again.");
                        ReadLine();
                    }

                    gameOver = DisplayScores(game, players);
                }

                WriteLine();
            }


        }
    };
}

