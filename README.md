LunaTMX
===========

A small library to import and render tilemap, only support TMX file format.

### Features

1. Import TMX Map into a Texture2D
2. Support Auto-Detect Collision with Polygon Collider 2D
3. Support as many as you want tilesets for a layer


### Usage

#### Tiles
Draw and naming your layers, be careful with naming, layer with *[C]* prefix will have a collider component.

#### Collision
Name your layer with *[C]* prefix for automatically detecting collision.

#### Save TMX
Use CSV data type and save with *.TMX* extension, then change that extension into *.XML*.

#### Unity Usage
1. Create a folder inside Asset directory named *Resources*
2. Import all scripts, also import *TMX* files
3. Import all Tilesets image into a folder inside *Resources* directory we created above, also set those Texture Type to Advanced and tick Read/Write Enabled box
4. Add *TileMap.cs* to an empty Game Object and follow the instruction.


### License and Credit
© 2014, SuppaYami - Cuong Nguyen
Released under the MIT license
Check LICENSE for more details.