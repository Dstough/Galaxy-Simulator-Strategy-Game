# Galaxy-Simulator-Strategy-Game

Inspired by a love of strategy games, programming, and the desire to learn more about how to make video games, 
this project aims to create a simple strategy game that uses procedural generation to define content.
Changes to the content are maintained in seperate list and replayed over what is generated.
Players can fly fleets of ships through the galaxy and interact with stars, planets, moons, etc.
The ultimate goal is to explore the galaxy and leave some kind of an impact upon it.

## Gameplay

Players can select their ships in groups of one or more and move them to squares around a system.
They will have a series of commands they can issue at any time similar to how rogue-likes handle user command.
Even if the command is not valid for a given target, the player can still try it.
This allows for exploration of game mechanics and encourages players to think about how to solve certain problems.

To keep the game from getting too overwhelming, the player will only be able to support a fleet of up to 9 ships.
This allows them to spread out in the galaxy or travel in a safer cluster of ships.

## Graphics

The graphic style will feature isometric pixel drawings of ships placed on a grid overlay in open space.
All the sprites are meant to be high level representations of the objects in the game and have the same scale.
Even though a star is much larger than a planet, they both will take up one square on the grid.

## Hud

The hud functions similar to real time strategy games by showing all the units a player has under his control.
They are bound to hotkeys 1-9 and multiple can be assigned to control groups.
Every possible command that a ship can be given are displayed on the hud at all times.
Only the available commands will provide any effect when used.
This may change as the command list grows.
