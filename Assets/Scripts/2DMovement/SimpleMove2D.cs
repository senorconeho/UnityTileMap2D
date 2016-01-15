using UnityEngine;
using System.Collections;

/// <summary>
/// The simplest way to move a character around a top down level. No velocity check, nothing.
/// The character still needs a rigidbody and a collider
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]

public class SimpleMove2D : MonoBehaviour
{

	// PUBLIC
	[HideInInspector] public Transform trSprite;

	public float fMoveForce = 40f;		

	float 	fH;	//< Horizontal movement input
	float 	fV;	//< Vertical movement input

	Rigidbody2D rb;		//< The rigidbody component of this object

	//
	// Use this for initialization
	void Start ()
	{

		trSprite = this.transform;
		rb = GetComponent<Rigidbody2D>();
	}

	// Update is called once per frame
	void Update ()
	{

		// Get input
		fH = Input.GetAxis ("Horizontal");
		fV = Input.GetAxis("Vertical");
	}

	// Physics
	void FixedUpdate ()
	{
		Vector2 vInputDirection = new Vector2(fH, fV);
		rb.AddForce(vInputDirection * fMoveForce);
	}
}

