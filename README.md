# Midterm Project Proposal - VR Development Course
Team Members: Or Hershko and Adva Levin

# Introduction:
The midterm project will be a shameless clone of Super Mario Bros. (1985).

# Game Loop:
When the user enters the game, a main menu will be displayed where they can choose to start the game. This screen will also display the high scores (fastest completion + most points) as well as an exit button.

The game will initially support a single-player version.

After selecting the game mode single

## THERES AN EASTER EGG!
If you press shrek carachter your player is changing to the shrek character, else if you press mario or dont do a thing- you've got mario!

# Game loop
Once the game starts, the menu will disappear. During the game, the player will jump using the up arrow, causing the character to jump in place. There will be obstacles along the way that either cause the player to lose or delay their progress. The player's goal is to reach the castle as quickly as possible to save the princess, and to also collect the highest score by gathering coins.

We will add an enhanced feature for the "Question Mark Block." When the player hits this block, they will receive one of the following options:

Higher jump (good)
Lower jump (bad)
Bonus points (good)
Point deduction (bad)
Double coins collected for a limited time (good)
Coin value halved for a limited time (bad)
Game Levels:
Currently, we plan to implement one or two levels, depending on progress.

Difficulty Levels:
We will create two difficulty levels: Easy and Hard.
In Easy Mode:

There will be fewer enemies.
Question Mark Blocks that give good rewards will be green, while those that give bad rewards will be red.
In Hard Mode:

There will be more enemies.
There will be no indication of whether a Question Mark Block gives a good or bad reward.

Throughout the game, there will be monsters. If the player collides with a monster, they lose, and the game ends. However, if the player jumps on the monster's head, the monster dies, and the player earns points, allowing the game to continue.

If the player achieves a new high score, a UI element will congratulate them. The main menu will then be shown again.

# Implementation Details:
 # Scenes:
The entire game will take place within one scene.

# Packages:
The game will utilize Unity's 2D physics (with a 2D Rigidbody on the player).
The game's menus will use Unity's Canvas system.

# Target Device:
PC
# Estimated Submission Date:
23.09.2024
# References:
A short video of a level from the original game:
https://www.youtube.com/watch?v=DGQGvAwqpbE
Our hebrew Submission:
https://docs.google.com/document/d/1EZ9PgSreIArqBijmKz6ybsVsH37PtZwQg7LeI_iUXXE/edit


