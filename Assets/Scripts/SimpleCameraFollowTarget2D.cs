using UnityEngine;
using System.Collections;

/// <summary>
/// Makes a camera follow an object. When the camera hits the game screen limits it's stops following
/// </summary>
public class SimpleCameraFollowTarget2D : MonoBehaviour {

	public Transform	trTarget;				//< target to follow 
	Camera cam;
	Transform tr;

	void Awake() {

		cam = gameObject.GetComponent<Camera>();	
		tr = this.transform;
	}

	/// Change the camera position only after the target object has been updated
	/// </summary>
	void LateUpdate() {

		if(trTarget == null)
			return;

		// Follow target
		Vector3 vNewPosition = new Vector3(trTarget.position.x, trTarget.position.y, tr.position.z);
		tr.position = vNewPosition;
	}
 }
