using UnityEngine;
using System;
using System.Collections;

[Serializable] public class TileMapAsset {

	public string symbol;
	public GameObject prefab;
}

public class TileMapBuilder : MonoBehaviour {

	[HideInInspector] public TileMapAsset[] assets = new TileMapAsset[] { new TileMapAsset() };

	[HideInInspector] public TextAsset mapFile = null;

	[HideInInspector] public string layerName = "Layer_name";

	[HideInInspector] public float	tileWidth;	// Usually 'sprite width in pixels/Pixel to units'
	[HideInInspector] public float	tileHeight;
}
