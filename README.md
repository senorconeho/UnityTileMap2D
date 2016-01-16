# UnityTileMap2D
The focus for this project is to provide a simple level creator for tile maps, using the top-down perspective, like seen on Zelda: A Link to the Past, Binding of Isaac and other games. The project is made using Unity3D 5.

## Map creation: how it works
In your scene, create an empty object and add the script "TileMapBuilder".

### TileMapBuilder options

#### Map file
The XML file with the map definition

#### Tile Settings
The `Tile Width` and the `Tile Height`, both in world units. This values are used to provide the right offset between the tile objects in the scene.

#### Tiles list
Here we have a list of characters (used in the map file), and the corresponding prefabs (which will be instantiated in the scene. For each entry in the list you must use only single characters. To add new entries, click on the "Add new tile button"


## Sprites Attribution
I'm using the "Cute Planet" sprite pack from Daniel Cook (http://www.lostgarden.com/2007/05/dancs-miraculously-flexible-game.html)

