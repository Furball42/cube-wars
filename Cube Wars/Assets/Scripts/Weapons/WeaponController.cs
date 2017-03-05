using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class WeaponController : NetworkBehaviour {

	public GameObject weaponAttachmentPoint;

	GameObject equippedWeaponObj;

	void Start() {

		if (!isLocalPlayer)
			return;

		CmdEquipWeapon();	
	}

	void Update() {

		if (!isLocalPlayer)
			return;

		//shooting
		if(Input.GetMouseButton(0)) {
			CmdOnTriggerHold();
		}

		if(Input.GetMouseButtonUp(0)) {
			CmdOnTriggerRelease();
		}
	}
		
	[Command]
	void CmdEquipWeapon() {

		if(equippedWeaponObj != null) {
			Destroy(equippedWeaponObj.gameObject);
		}
			
		equippedWeaponObj = Instantiate(Resources.Load("Weapons/auto_cannon"),weaponAttachmentPoint.transform.position, weaponAttachmentPoint.transform.rotation) as GameObject;
		//equippedWeaponObj = Instantiate(Resources.Load("Weapons/DualCannon"),weaponAttachmentPoint.transform.position, weaponAttachmentPoint.transform.rotation) as GameObject;
		//equippedWeaponObj = Instantiate(Resources.Load("Weapons/BurstCannon"),weaponAttachmentPoint.transform.position, weaponAttachmentPoint.transform.rotation) as GameObject;
		equippedWeaponObj.GetComponent<Weapon>().parentNetId = GetComponent<NetworkIdentity>().netId;
		equippedWeaponObj.transform.SetParent(weaponAttachmentPoint.transform);

		NetworkServer.Spawn(equippedWeaponObj);
		//RpcSetParent(equippedWeaponObj, gameObject);
	}

//	[ClientRpc]
//	void RpcSetParent(GameObject obj, GameObject parentObj) {
//		
//		if (!NetworkServer.active) {
//			obj.transform.SetParent(parentObj.transform);
//		}
//
//	}

//	[Command]
//	void CmdShoot() {
//		if(equippedWeaponObj != null) {
//			equippedWeaponObj.GetComponent<Weapon>().Shoot();
//		}
//	}

	[Command]
	void CmdOnTriggerHold() {
		if(equippedWeaponObj != null) {
			equippedWeaponObj.GetComponent<Weapon>().OnTriggerHold();
		}
	}

	[Command]
	void CmdOnTriggerRelease() {
		if(equippedWeaponObj != null) {
			equippedWeaponObj.GetComponent<Weapon>().OnTriggerRelease();
		}
	}
}
