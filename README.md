UnityTetris
====================

A simple tetris game built in Unity. Can run the game in Unity, or use the .exe in the Builds folder.

Controls
--------

- Left Arrow = Move Left
- Right Arrow = Move Right
- Down Arrow = Move Down
- Up Arrow = Rotate
- A = Toggle AI
- Esc = Pause / Menu

Leaderboard
--------
Currently a simple JSON file on disk. For now, the computer name is used as the player's name for the high scores.

Implementation Details
--------

AI heurestic:
 - Each neighboring block / bottom of grid = +1
 - Left/Right edges of grid = +.75 (prevents shapes clumping up in the middle)
 - Penalties for higher rows = -.1 per row (prevents a column from being ignored too long and other parts of the grid building up)
 - Penalties for covering up unfilled blocks = -1
 
AI checks the possible score of each column the shape could fall in, and going with the highest score.

Publishers / Subscribers
- Input -  AI, Menu, UnityTetris classes subscribe keyboard input changes (gameplay inputs, turn on AI, show menu, etc)
- Menu - UnityTetris class subscribes to menu selections (pause/resume, start a new game, etc)
- ClassicTetris - AI, Menu, UnityTetris classes subscribe to game state changes (a block is placed, game has ended, etc)

The TetrisGrid class is a wrapper around the C# BitArray class and helps with things like bit shifting since those aren't natural operators on BitArrays.
The AI class has to touch some of the lower level classes to compute the score of possible moves.

TODOs / Things I didn't get around to
--------
- Capping frames after a certain time (such as cutting AI calculations short and doing the rest in the next frame)
- Seperating out the UI better from my classes
- Create a new Tetris ruleset (I tried to design my classes so this would be easy. Didn't quite get there fully, but I don't think it'd be hard)
- Create automated testing for the game. I could have done it, it just wasn't my focus with this project.