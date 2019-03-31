# GungHo_Interview_Game
This is the game I created for my interview with GungHo. This readme should give you
a good idea of all/most of the things I worked on. If you have any questions, feel
free to email me at mgauderm@usc.edu 

Name of game: Grappler

Description: a 2D side-scrolling platformer in which the player has a grappling hook
they must use to traverse the level. They are always surrounded by a colored barrier,
and the player will die (and the level will restart) if the player lands on blocks
of a different color than their barrier. Once the player reaches the end, the game
will automatically fade back to the Start menu.

Controls: 
A/D - move left and right
Space - jump
Left click - shoot your grappling hook to a grappling point, or propel yourself towards
a grappling point if you are already attached
Right click - detach from a grappling point
Esc - pause the game

Notes on the outline shader:
- the outline shader I wrote (can be accessed in Custom/SpriteOutline) can be seen
in game on the grappling points, and the shader itself can be found in Assets/Shaders
- the shader takes 3 custom inputs whicih change the border: 
	- Outline Color: color of the outline
	- Alpha Threshold: the minimum alpha value a pixel can have to be considered solid
	(and therefore not be set as part of the outline)
	- Outline Thickness: the approximate thickness in texels of the outline (this was
	pretty tricky to implement, but it made the outline look a lot nicer)
- it may take some trial and error, but these three inputs allow for a lot of 
customization, and can usually achieve a desired result
- there are also two additional checkboxes in the inspector for a material using
my SpriteOutline shader. They allow you to choose which part of the shader gets rendered
(outline, diffuse, both, or none)
- for sprites which have opaque pixels all the way to the edge of the sprite, the
shader sets those opaque pixels on the edge to be part of the border (so there
isn't a gap in the border)
- it should be noted that for the best results, there should be a buffer of at least
a few pixels on all sides of a sprite since the shader only renders pixels in the sprite


Other Noteworthy notes: 
- there is a main menu in the Main_Menu scene. It is also the first scene to show
up if you play in a build rather than in editor
- if a grappling point is close enough in front of the player, left clicking will 
automatically shoot towards the closest grappling point. 
- if there was no grappling point in range at the start of grapple, it will shoot
out straight and hold straight for a moment. If the extended grapple comes into
contact with a grappling point, it will latch onto the nearest point. If it hits a non-
grappling point object, it retracts immediately
- when the player is swinging from a grappling point, they have 2 choices: left click
to propel themselves to the grappling point or right click to detach completely. Sometimes
one option is better than the other
- the barrier system forces the player to think about their next move rather than 
always propel themselves forward as fast as possible, because otherwise they have 
to restart the level
- in all level transitions, the screen fades in and out so it is not abrupt.
- I also was somewhat detailed in my commit messages, so those should provide some insights
into the game


Thank you for taking the time to read this and review my game! It was a lot of fun
to make! 



