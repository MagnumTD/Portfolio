Hey folks,

this is my project that I created in connection with my term paper about procedurally generating levels. The goal was to build corridor structures using Lindenmayer systems.

In the essay I tried to tackle different approaches to handle the problem of room collision. These different concepts can be selected on the LevelCreator GameObject.
	
	"No Solution" will often end in room collisions, which will result in unplayable levels.
	"Brute Force" tries to generate levels until there are no room collisions. This will often result in boring levels.
	"Multiple Floors" just moves the rooms above the one they collided with. This creates interesting levels, but has the downside that levels are not always playable, because rooms can still block the entrances to other rooms.
	"Crossways" are the best concept for the problem. Everytime a room collision comes up, the walls of the given rooms will be removed, which mostly has open and interesting playable levels as an outcome.

The other interesting variable to compare the usability of the different solutions is the minimum amount of rooms to be created.

The levels are built from different rooms that are colorcoded:

	The startroom is green, this is the room that the player will spawn in.
	The usual corridors are gray.
	There are locked rooms (black) that need to be unlocked by walking into a keyroom (blue), which will turn white once used and the lock will be opened and its color switches to violet.
	There are also right curves (orange) and left curves (yellow).
	The final room is the red goal room. Entering this room will generate a new level.

The project's gameplay elements are provided by Unity3Ds default Character Controller.


I also worked on two group projects. The first one was a 2D-Jump and Run game where you play a diver underwater. For that project I worked with two other programmers. My main responsibilities were to create a custom character controller, simple enemy behaviour and level obstacles like currents or jellyfish. 
The second project was a slot machine similar to the ones found in the town in Borderlands 2. The user can win different, on air generated, rifles, which were combined by several parts like a grip, a scope, a barrel and so on. 
Since these were group projects, I unfortunately cannot upload them to github. 
