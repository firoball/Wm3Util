# Wm3Util
.wm3 importer for Unity

## What is this?

Wm3Util is a set of import scripts for the Unity Engine in order to load .wm3 files created by A3Tools.
A3Tools is a separate tool based on the Acknex8 engine, which converts Acknex3 raycaster levels to 3d meshes.
The .wm3 format was developed with Acknex8 use in mind, but it theoretically can be used in any engine. 

As Acknex8 does not feature a WYSIWYG development environment, it is very tedious to refine .wm3 levels.
To overcome this, Wm3Util was developed, since Unity allows for more comfortable scene/level editing.


## What is A3Tools?

A3Tools is a converter for raycaster sector/wall based levels (similar to 1993 Doom) created with the Acknex3 engine.
This toolset is by far not completed, but still allows to convert many A3 levels (static only).
A3Tools can export .wm3 files and .bmp textures, which allows for import into pretty much any 3d engine of choice.

The .wm3 format was developed having usage in Acknex8 in mind, though. Format is documented in A3Tools manual.

A3Tools utilizes the Acknex8 engine to operate, Windows binaries are included. The A3Tools project can be obtained from Github: 
[https://github.com/firoball/A3Tools](https://github.com/firoball/A3Tools)


## Why should I use Wm3Util?

Since you came here somehow, I expect you are aware of the existence of Acknex3, Acknex8 and Unity.
So let's keep this short.

Use this utility to pretty much whatever you want:
* Convert your old Acknex3 levels for fun
* Use converted level as base for some retro project
* Try to break the tool by importing awkward levels
* Pure nostalgia
* Learning reasons


## How does it work?

Step-by-step guide:
* Use A3Tools to convert a level of choice (refer to A3Tools documentation)
* Fork Wm3Util project
* Copy all generated files from A3Tools output directory to *Assets/Import* of your Wm3Util fork
* Rename all .wm3 files to .wm3.bytes in order to make sure Unity reads the contents properly
* Open your Wm3Util fork in Unity
* Open *Window -> Wm3 Importer*

The Wm3 Importer panel will show up. Configuration must be done for template materials and level.

![Wm3 Importer Panel](wm3panel.jpg)

Drag any .wm3.bytes file of choice to the Wm3 Importer dialog.
The template materials can be found in *Assets/Wm3Util/Materials*. Drag them to the dialog as shown.

They are built in a way to imitate the Acknex3 look as close as possible. Customization is possible, though.


## Give me Samples!

Kandoria is a sample project with several .wm3 files imported into several scenes.

Some manual changes were done:
* Doors and enemies were made passable
* Pitfalls were covered with invisible planes
* Levels spread over several .wm3 files were merged
* Simple level hub system was added
* Fog and sky shader setup
* FPSController for easy navigation added

Github: [https://github.com/firoball/Wm3Util_sample](https://github.com/firoball/Wm3Util_sample)


## Future Development

This was a Unity learning project for me and is finished as such.
Of course there are many improvements possible (like animated textures and player positioning), 
but usually they also require an A3Tools and .wm3 format update.

This quickly adds up to a lot of work for little use. Acknex3 was not used very commonly at its time.
In case you are one of the very few former Acknex3 users and have a special request, feel free to contact me.


## Compatibility

Wm3Util was developed and tested with Unity Version 2018.3.0f2 Personal.
Downwards compatibility was not tested.


## Legal stuff

Please respect [license.txt](license.txt) (Attribution-NonCommercial 4.0 International)

-firoball

[http://firoball.de](http://firoball.de)
