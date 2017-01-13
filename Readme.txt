Hey folks,

this is my project that I created for an essay about procedurally generating levels. The goal was to build corridorstructures using Lindenmayer-systems.

In the essay I tried to tackle different approaches to handle the problem of room collision. These different concepts can be selected on the LevelCreator GameObject.
	
	No solution will often end in roomcollisions, which will result in unplayable levels.
	Brute Force tries to generate levels until there are no room collisions. This will often result in boring levels.
	Multiple floors just moves the rooms above the one it collided with. This concludes in interesting levels, but has the downside that levels are not always playable, rooms can still block the entrances to other rooms.
	Crossways are the best concept for the problem. Everytime a roomcollision comes up, the walls of the given rooms will be removed, which mostly has open and interesting playable levels as an outcome.

The other interesting variable would be the minimum amount of rooms to be created, since this is important to compare the usability of the different solutions. 

The levels are build from different rooms that are colorcoded:

	The startroom is green, this is the room, that the player will spawn in.
	The usual corridors are gray.
	There are locked rooms (black) that need to be unlocked by walking into a keyroom (blue), which will turn white once used and the lock will be opened and it's color switch to violet.
	There are also right curves (orange) and left curves (yellow).
	The final room is the red goal room. Entering this room will generate a new level.

The projects gameplay elements are provided by Unity3Ds default Character Controller.


I also worked on two group projects. The first one was a 2D-Jump and Run game where you play a diver underwater. For that project I worked with two other programmers. My main responsibilites were to create a custom character controller, simple enemy behaviour and level obstacles like currents or jellyfishes. 
The second project was a slot machine similar to the ones found in the town in Borderlands 2. The user can win different on air genereated rifles, that were combined by several parts like a grip, a scope, a barrel and so on. 
Since these projects were group projects I sadly can not upload them to github. 
