using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class SetupLocalPlayer : NetworkBehaviour {

	void Start () {
		
		if(isLocalPlayer)
		{
			Canvas[] uiCanvas = GetComponentsInChildren<Canvas>();
			foreach(Canvas c in uiCanvas) {
				c.enabled = true;
			}
				
			GetComponent<PlayerController>().enabled = true;
			GetComponent<WeaponController>().enabled = true;
			SmoothCameraFollow.target = this.transform;
		}
	}
}
