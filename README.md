UnityTetris
====================

A simple tetris game built in Unity.

Controls
--------

Left Arrow - Move Left
Right Arrow - Move Right
Down Arrow - Move Down
Up Arrow - Rotate
A - turn on AI
Esc - Pause / Menu

Leaderboard
--------
Currently a simple JSON file on disk. For now, the computer name is used as the player's name for the high scores.

Implementation Details
--------

AI heurestic:
 - Each neighboring block / bottom of grid = +1
 - Left/Right edges of grid = +.5 (prevents shapes clumping up in the middle)
 - Penalties for higher rows = -.1 per row (prevents a column from being ignored too long and other parts of the grid building up)
 - Penalties for covering up unfilled blocks = -1
 
Possible checking the score of each column the shape could fall in, and going with the highest score.

Keyboard input class publishes key presses to subscribers
Menu class publishes state information to subscribers
UnityTetris class sends messages to the SceneManager (translation, rotation, game state change requests)