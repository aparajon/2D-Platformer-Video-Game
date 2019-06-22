Thank you very much for purchasing the Retro Pixel Monsters!
Here's just a few notes to the package and its contents.

====SPRITESKIN SCRIPT INFORMATION====
SCRIPT INFO
The SpriteSkin script included in the pack is used for a few things.
It enables you to switch between colored and black outline styles.
It also saves on animation setup, since some monsters use the same animations.

For instance, all the color variations of Goblins and Slimes are set up the same way.
To prevent having to redo the animations for each color variation, the script
simply switches out the sprite in run-time instead.

RESOURCES FOLDER
The spritesheets need to be in the "Resources\Retro Pixel Monsters\Spritesheets" folder.
This is what makes the SpriteSkin script work, since it uses Resources.LoadAll().
The rest of the assets may be moved outside of the Resources folder.

OTHER USES
You could use the script for skinning your own sprites too.
It might need a bit of modification to work for your project.
If you're not using different outline styles for your own sprites, you could
remove the feature that offsets the frame index, for instance.

=====PREFABS & ANIMATIONS=====
All the monsters have complete animation sets and organized animator controller trees.
The animator controllers are currently set up to simply loop through down and rightward
facing animations, in most instances.

The prefabs are made to demonstrate how to set up the sprites. The SpriteSkin script
is attached to all prefabs. There are no AI scripts included or attached to prefabs.

=====INFO SCENES=====
The Info Scenes are included as a bonus, as they were mostly made for setting up
screenshots to use in the Asset Store.

The info scenes contain suggested attack patterns for the monsters, to give you an idea
on how you could set up their attacks in your project. No AI scripts are included.