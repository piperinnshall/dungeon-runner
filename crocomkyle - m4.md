### Kylen Crocombe Reflection
## Student ID: 300666024


### Milestone 4:

Good Code:

https://github.com/piperinnshall/dungeon-runner/commit/b94ef415b6c921652118585f707372fd204a7a74#diff-7a7fe141ead67779bd6abc89015f53e8c5dee8ffc92b14eee6ba37cbc387cd92

This is a piece of code I'm proud of to make camera clipping work. In a function that is called in the LateUpdate() function (so it's called after all update functions have been called) the camera shoots out a raycast in the form of a straight line, anything that collides with the line that's on the wall layer, it brings the camera foward to the point of collision, prevent the camera from phasing through walls.

https://github.com/piperinnshall/dungeon-runner/commit/324d6d7d1e6b17e5b097fedce62fef3b3cceceb7#diff-fc539d4eeec465b08d3a3ad2dc4dfe9ee01f3abd6fa21c9c5705a1ba27f8ed99

This piece of code is very important for the player and is my biggest code contribution. The player states are records that are attached to the playerstate interface, and the behavior of the player and what state the player can/will switch to is determined by the function that the state is attached to i.e Idle=>HandleIdle(). This allows for nice modular code and makes it easier to change and add new states, as all you have to do is create and connect a new player state to the playerstate interface and a handle function so that it can control the player.

https://github.com/piperinnshall/dungeon-runner/commit/fd32b886e534e139d45a58ba0280a2d59c062dd2

The three scripts, Interact.cs, InteractableObject.cs and Torch.cs are setup in a way that makes them very flexible for future use. InteractableObject.cs can be attached to any game object. The Interact.cs script (which is attached to the player) triggers the InteractableObject.cs script when the player presses the 'e' key, this causes InteractableObject.cs to send out a ping that can be detected by a script that is attached to that game object, i.e Torch.cs, this trigger can then be attached to a function so that's it called whenever the ping is sent out it calls that function, in the case of Torch.cs, it calls the handle interaction function, which then determines whether to extinguish or ignite the torch.

https://github.com/piperinnshall/dungeon-runner/commit/e86bf9fe553eb7a472cebcd313607d96d15a4b4f#diff-7c85acb8198034d7a00f7439b19fd868f6eaff6a6925800e58fe0d1f3d5f1e01

The boomerang projectile script is needed for when the player throws the boomerang. When the player throws the boomerang, it spawns a boomerangprojectile game object in fron of the player and it travels in the direction the player was facing when they threw it, it then travels for a set distance and then returns to the player, deleting itself. If it hits an object that is on the wall layer it returns, but if it hit an enemy, it damages them and returns.

Bad Code:

https://github.com/piperinnshall/dungeon-runner/commit/007bb5473560b9832e160badf70b57d14b8fc069#diff-2a17f276d9dcd6eca93c68f0d88b30e7a83bbbb8fbd16238e62fdc46f5b6efc2

This code is responible for the player's equipment, it's frame work is somewhat setup for future coding, as it can equip any game object into the player's hands with the equip left and right hand functions. However the code needs to have the position of the equipment to be stored and applied within the equip functions, which makes it harder to work with, it would've been better to perhaps have a seperate class/container for equipment which has its own positions stored within.

https://github.com/piperinnshall/dungeon-runner/commit/f2c8399887feb3cee2a0e3b67a32934e335511a2

The bird eye view function is meant to let the player have more view of the terrain by makng the camera zoom out. In this implentation, the camera's fov is increased, which allows for more viewing, but it might've been better to instead just move the camera further away from the player instead.

https://github.com/piperinnshall/dungeon-runner/commit/24c85b3ba7d79c957860c8375280e8061f9c18c5#diff-db8d253a269f958e693ae789e68cc9043fee29e9fd1b4dc09fd3bd1e58682d34

The combat script for the player had a nice structure. It uses enums as states, but wasn't coded to be flexible with other player states and wasn't as modular as the current player state machine that is more modular, thus I intergated some of this code into the main player script and scrapped this script.



Video showing the player character's unity animations:
https://drive.google.com/file/d/1O8oQQL32jp6bDa1OS-JqsOSTJkaS_Ex8/view?usp=sharing

Reflection:
Throughout this course, the two key concepts I learnt during this course was time management and person managment. Working on the first assignment and the group project, time management was key, ensuring I was on track with the project whilst balancing other things in my life was difficult at first, but I found what worked best for me, was to ensure that, no matter how small, everyday I worked on something in the project, I found that this helped to keep me on track without letting work pile up and me having to rush to finish my designated work. During the first half, the development was slow going but on track. However, halfway through, my teammates and I started getting lax with meetings and communications, which slowed down development. During the later end I suggested we meet and got more involved with communicating with my group to make sure we could meet up, this helped us get back on track and taught me that I don't just have to do what others are doing, that I can step up and help organise a team I'm apart of.
In the future I will use the lessons I've learnt on time management to better make sure I'm on track with my tasks by ensuring I do at least a little bit of work everyday, whether it's univerisity work, job work or personal/passion work. I will also be more confident when it comes to communicating and organising a team I'm in, I'll be sure to step up if I believe it's neccesary and appropriate.
If I had to do this project again, I would be sure to manage my time better and do at least a little bit of work on the project everyday and be more involved with the group and step up more to organise the team and ensure we're all on track and motivated. 