using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[CanEditMultipleObjects]
[CustomEditor(typeof(TileMapBuilder))]
public class TileMapInspector : Editor {

	Dictionary<char, GameObject> tilesDict = new Dictionary<char, GameObject>();
	private bool showFoldout = true;

	//
	private SerializedProperty	assets;
	private SerializedProperty	mapFile;
	private SerializedProperty	layerName;
	private SerializedProperty	tileWidth;
	private SerializedProperty	tileHeight;
	private Vector3 startingPosition;

	// GUI Text Messages
	private static GUIContent emptyContent = GUIContent.none;
	private static GUIContent insertContent = new GUIContent("Add new tile", "Add a symbol for the map and a prefab to the tile");
	private static GUIContent buildContent = new GUIContent("Build map", "Build the map in the scene according to the text file");
	private static GUIContent deleteContent = new GUIContent("X","Delete tile");
	private static GUIContent mapFileContent = new GUIContent("Map file");
	private static GUIContent layerNameContent = new GUIContent("Object name", "Map object name in the hierarchy");
	private static GUIContent tileWidthContent = new GUIContent("Tile Width", "Width of the tile in world units");
	private static GUIContent tileHeightContent = new GUIContent("Tile Height", "Width of the tile in world units");

	// GUI Formatting
	private static GUILayoutOption deleteButtonWidth = GUILayout.MaxWidth(30.0f);
	private static GUILayoutOption symbolFieldWidth = GUILayout.MaxWidth(40.0f);

	/* -----------------------------------------------------------------------------------------------------------
	 * UNITY'S METHODS
	 * -----------------------------------------------------------------------------------------------------------
	 */
	/// <summary>
	/// </summary>
	public void OnEnable() {

		assets = serializedObject.FindProperty("assets");
		mapFile = serializedObject.FindProperty("mapFile");
		layerName = serializedObject.FindProperty("layerName");
		tileWidth = serializedObject.FindProperty("tileWidth");
		tileHeight = serializedObject.FindProperty("tileHeight");
	}

	/// <summary>
	/// Here where the field are actually shown on the screen
	/// </summary>
	public override void OnInspectorGUI() {

		// Update the serializedProperty - always do this in the beginning of OnInspectorGUI
		serializedObject.Update();

		// Draws standart complete inspector layout and properties
		DrawDefaultInspector();

		// Show the custom GUI controls looking like controls
		EditorGUI.indentLevel = 0;
		EditorGUIUtility.LookLikeControls();

		// Draw select file with level
		EditorGUILayout.Space();
		EditorGUILayout.PropertyField(mapFile, mapFileContent);
		EditorGUILayout.Space();
		EditorGUILayout.PropertyField(layerName, layerNameContent);
		EditorGUILayout.Vector3Field("Starting position:", startingPosition);

		// Draw assets foldout
		EditorGUILayout.Space();
		EditorGUIUtility.LookLikeInspector();
		GUILayout.Label("Tile Settings", EditorStyles.boldLabel);

		EditorGUILayout.PropertyField(tileWidth, tileWidthContent);
		EditorGUILayout.PropertyField(tileHeight, tileHeightContent);


		if(showFoldout = EditorGUILayout.Foldout(showFoldout, "Tiles list")) {

			// Tab
			EditorGUIUtility.LookLikeControls();
			EditorGUI.indentLevel = 1;

			if(assets.hasMultipleDifferentValues) {

				EditorGUILayout.LabelField("Scenery prefab lists have different content!");
			}
			else {

				// Draw prefab and symbol fields for each item
				for(int nElement = 0; nElement < assets.arraySize; nElement++) {

					EditorGUILayout.BeginHorizontal();
					{
						// Grab link to array element button
						SerializedProperty asset = assets.GetArrayElementAtIndex(nElement);

						// Draw delete array element button
						if(GUILayout.Button(deleteContent, EditorStyles.miniButtonLeft, deleteButtonWidth)) {

							if(assets.arraySize > 1) {

								assets.DeleteArrayElementAtIndex(nElement);
								serializedObject.ApplyModifiedProperties();
							}
						}
						else {

							EditorGUILayout.PropertyField(asset.FindPropertyRelative("symbol"), emptyContent, symbolFieldWidth);
							EditorGUILayout.PropertyField(asset.FindPropertyRelative("prefab"), emptyContent);
						}
					}
					EditorGUILayout.EndHorizontal();
				}
			}

		}

		// Draws the tile list
		EditorGUILayout.Space();
		if(!assets.hasMultipleDifferentValues) {

			if(GUILayout.Button(insertContent)) {

				// Insert new array element and reload SerializedObject
				assets.InsertArrayElementAtIndex(assets.arraySize-1);
				serializedObject.ApplyModifiedProperties();
			}
		}

		// Draw the "build the level" button
		EditorGUILayout.Space();
		if(GUILayout.Button(buildContent)) {

			// DEBUG
			//Debug.Log("Call method to build the content");

			//string[] stLinesFromFile = ReadTextFile((TextAsset) mapFile.objectReferenceValue);
			
			//// DEBUG
			//foreach(string st in stLinesFromFile) {

			//	Debug.Log(st);
			//}

			//TileMapAsset[] tiles = CreateTileList();
			CreateTileTable();
			CreateTileMapFromXML((TextAsset) mapFile.objectReferenceValue);

			//CreateTileLevelFromTextLines(stLinesFromFile);
		}

		// Update SerializedObject
		serializedObject.ApplyModifiedProperties();
	}

	/* -----------------------------------------------------------------------------------------------------------
	 * XML MAP BUILDING
	 * -----------------------------------------------------------------------------------------------------------
	 */
	/// <summary>
	/// Read all lines from a text file and returns a list of strings (one for each line read)
	/// </summary>
	/// <param name="stPath"> A string with the path to the file to be read</param>
	/// <returns> An array of strings. Each string is a line from the file</returns>
	//string[] ReadXMLTextFile(TextAsset taObject) {

	//	string stFileContents = taObject.text;
	//	
	//	// XML
	//	var readedTiles = MapContainer.LoadFromText(stFileContents);
	//	if(readedTiles == null) {
	//		Debug.LogError("Fail to process xml");
	//	}
	//	Debug.Log(string.Format("{0} has {1} layers", readedTiles.name, readedTiles.layers.Length ));
	//	foreach(LayerContainer layer in readedTiles.layers) {

	//		Debug.Log(string.Format("Layer {0} order {1} has {2} lines", layer.name, layer.order, layer.tiles.Length));
	//	}
	//}

	/// <summary>
	/// </summary>
	void CreateTileMapFromXML(TextAsset taObject) {

		string stFileContents = taObject.text;
		int nX = 0;

		float fTileWidth = tileWidth.floatValue;
		float fTileHeight = tileHeight.floatValue;
		var xmlMap = MapContainer.LoadFromText(stFileContents);
		
		// DEBUG
		Debug.Log(string.Format("Processing map {0} with {1} layers...", xmlMap.name, xmlMap.layers.Length));

		
		GameObject goMap = new GameObject();
		goMap.transform.name = xmlMap.name;

		// Process each layer
		foreach(LayerContainer layer in xmlMap.layers) {

			string stLayerName = layer.name;
			int nLayerOrderOnMap = layer.order;
			int nLine = layer.tiles.Length - 1; // Number of lines for this layer

			GameObject goLayer = new GameObject();
			goLayer.transform.parent = goMap.transform;
			goLayer.transform.name = stLayerName;

			// Process each line in the layer
			foreach(TileLineContainer line in layer.tiles) {
	
				// Base position for this layer		
				Vector3 v3StartingPosition = new Vector3(0, (float)nLayerOrderOnMap * fTileHeight * 0.5f, 0 ); 	
				int nCharCol = 0;

				// Process each character in the line
				foreach(char ch in line.text) {

					GameObject prefab;

					if(tilesDict.TryGetValue(ch, out prefab)) {

						// DEBUG
						//Debug.Log("For symbol: " + ch + " [" + nCharCol + "] deploying " + prefab);

						if(prefab != null) {

							// Calculates the tile position
							Vector3 v3Offset = new Vector3(nCharCol * fTileWidth, nLine * fTileHeight, 0) + v3StartingPosition;

							GameObject goTile = Instantiate(prefab, v3Offset, Quaternion.identity) as GameObject;

							// Set the 'Sorting Layer' on the 'Sprite RendererComponent'
							SpriteRenderer sr = goTile.GetComponent<SpriteRenderer>();
							if(sr != null) {

								//sr.sortingOrder = nLayerOrderOnMap + (layer.tiles.Length-1) - nLine;
								//sr.sortingOrder = nLayerOrderOnMap + ( ((layer.tiles.Length-1) - nLine) * 10);
								// 2014-12-20 New Sorting Order Rule:
								float fYPosition = Mathf.Max(v3Offset.y, 0.4f); // Don't let the y value be zero at division
								fYPosition = 100/fYPosition;
								sr.sortingOrder = (1000 * nLayerOrderOnMap)+ Mathf.CeilToInt(fYPosition);
							}
							// TODO: read a property from the prefab or the instantiated object that says if it will use full height
							// or half height
							goTile.transform.name = nLine.ToString("D2") + "_" + nCharCol.ToString("D2");
							goTile.transform.parent = goLayer.transform;
						}
					}

					nCharCol++;
				}

				nLine--;
			}
		}
	}

	/* -----------------------------------------------------------------------------------------------------------
	 * MAP BUILDING
	 * -----------------------------------------------------------------------------------------------------------
	 */
	/// <summary>
	/// Read all lines from a text file and returns a list of strings (one for each line read)
	/// </summary>
	/// <param name="stPath"> A string with the path to the file to be read</param>
	/// <returns> An array of strings. Each string is a line from the file</returns>
	string[] ReadTextFile(TextAsset taObject) {

		string stFileContents = taObject.text;
		
		string[] stLines = stFileContents.Split("\n"[0]);
		
		List<string> lLinesCleaned = new List<string>();
		// Clear empty lines
		foreach(string st in stLines) {

			if(st.Length > 1)
				lLinesCleaned.Add(st);
		}

		return lLinesCleaned.ToArray();
	}

	/// <summary>
	/// </summary>
	void CreateTileLevelFromTextLines(string[] stLines) {

		int nLineIdx = 0;

		// DEBUG
	/*	foreach(string stLine in stLines) {
			
			// FIXME: testing only
			CreateTileLineFromString(stLine, nLineIdx);
			nLineIdx++;
		}
		*/
		
		for(int nIdx = stLines.Length-1; nIdx >= 0; nIdx--) {

			CreateTileLineFromString(stLines[nIdx], nLineIdx, stLines.Length-1);
			nLineIdx++;
		}
	}

	/// <summary>
	/// </summary>
	TileMapAsset[] CreateTileList() {

		// Create a new array with the same size as the assets array
		TileMapAsset[] tiles = new TileMapAsset[assets.arraySize];
		int nIdx = 0;

		// Now, copy each element of the assets array to this new array
		foreach(SerializedProperty asset in assets) {

			tiles[nIdx] = new TileMapAsset();

			tiles[nIdx].symbol = asset.FindPropertyRelative("symbol").stringValue;
			tiles[nIdx].prefab = (GameObject)asset.FindPropertyRelative("prefab").objectReferenceValue;

			nIdx++;
		}

		return tiles;
	}

	/// <summary>
	/// </summary>
	void CreateTileTable() {

		tilesDict.Clear();

		// Now, copy each element of the assets array to this new array
		foreach(SerializedProperty asset in assets) {

			tilesDict[asset.FindPropertyRelative("symbol").stringValue[0]] = 
				(GameObject)asset.FindPropertyRelative("prefab").objectReferenceValue;
		}

		// DEBUG
		//foreach(KeyValuePair<char, GameObject> pair in tilesDict) {

		//	Debug.Log(pair.Key + " = " + pair.Value);
		//}
	}


	/// <summary>
	/// Lays down the level from the string. Actually, instantiate the prefab associated with the symbol in 
	/// a given position
	/// </summary>
	/// <param name="stLevelText"> The string to be parsed and transformed into tiles </param>
	/// <param name="nLineIdx"> The line index in the text file. </param>
	void CreateTileLineFromString(string stLevelText, int nLineIdx, int nTotalLines) {

		int nX = 0;

		float fTileWidth = tileWidth.floatValue;
		float fTileHeight = tileHeight.floatValue;
		
		// TODO: check if it's a full tile or a half height tile
		Vector3 v3StartingPosition = new Vector3(0, nLineIdx*fTileHeight, 0 ); // TODO: nLineIdx * tileHeight


		// DEBUG
		//Debug.Log(nLineIdx + " Line: " + stLevelText);


		foreach(char ch in stLevelText) {

			GameObject prefab;

			if(tilesDict.TryGetValue(ch, out prefab)) {

				// DEBUG
				//Debug.Log("For symbol: " + ch + " [" + nX + "] deploying " + prefab);

				if(prefab != null) {

					GameObject goTile = Instantiate(prefab, v3StartingPosition, Quaternion.identity) as GameObject;

					// Calculates the tile position
					Vector3 v3Offset = new Vector3(nX * fTileWidth, 0, 0);

					// Set the 'Sorting Layer' on the 'Sprite RendererComponent'
					SpriteRenderer sr = goTile.GetComponent<SpriteRenderer>();
					if(sr != null) {

						sr.sortingOrder = nTotalLines - nLineIdx;
					}
					// TODO: read a property from the prefab or the instantiated object that says if it will use full height
					// or half height

					goTile.transform.position += v3Offset;
					goTile.transform.name = "tile_" + nLineIdx.ToString("D2") + "_" + nX.ToString("D2");

				}
			}
		
			nX++;
		}
	}
}
