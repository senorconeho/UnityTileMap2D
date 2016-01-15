using UnityEngine;
using System.Collections;

/// <summary>
/// Check in which tile the character (or other movable object) is in and adjust the sprite layer order
/// </summary>
public class CheckCharacterTile : MonoBehaviour {

	/* ==========================================================================================================
	 * CLASS VARIABLES
	 * ==========================================================================================================
	 */
	// PUBLIC
	public float fTileHeight = 1f;	// Default setting. Must set from the inspector
	public float fOffsetY = 0f; // Default setting. Must set from the inspector

	public float fTileY;
	public int nTileY;

	SpriteRenderer sr;
	
	public int nLayerInTheMap = 1; //< In which layer of the map the player is on?

	// PROTECTED


	/* ==========================================================================================================
	 * UNITY METHODS
	 * ==========================================================================================================
	 */

	/// <summary>
	/// <\summary>
	void Awake() {

		sr = gameObject.GetComponent<SpriteRenderer>();
	}
	
	/// <summary>
	/// Use this for initialization
	/// <\summary>
	void Start () {
	
	}
	
	/// <summary>
	/// Update is called once per frame
	/// <\summary>
	void Update () {
	
		fTileY = (fOffsetY + transform.position.y) / fTileHeight;

		// FIXME
		int nTotalTileLinesInTheMap = 5;
		//nTileY = (nTotalTileLinesInTheMap - Mathf.RoundToInt(fTileY)) * 10 + 5;

		// 2014-12-20
		float fYPosition = Mathf.Max(transform.position.y, 0.4f);
		fYPosition = 100/fYPosition;
		sr.sortingOrder = (1000 * nLayerInTheMap) + Mathf.CeilToInt(fYPosition);
		//sr.sortingOrder = nTileY + 1;
	}

	/* ==========================================================================================================
	 * CLASS METHODS
	 * ==========================================================================================================
	 */
	/// <summary>
	/// <\summary>

}
