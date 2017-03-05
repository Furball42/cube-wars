using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Weapon : NetworkBehaviour {

	public enum FireMode{ Auto, Burst, Single };

	public FireMode fireMode;
	public Transform[] muzzlePoints;
	public Projectile projectile;
	public float msBetweenShots = 100;
	public float muzzleVelocity = 35;
	[SyncVar] public NetworkInstanceId parentNetId;
	public int burstAmount;
	public float weaponShellDamage = 1;

	float nextShotTime;
	bool triggerReleased;
	int burstShotsRemaining;

	public override void OnStartClient() {
		
		GameObject parentObj = ClientScene.FindLocalObject(parentNetId);
		transform.SetParent(parentObj.transform.Find("Body/Turret/WeaponAttachmentPoint"));

		MeshRenderer[] childMeshes;
		childMeshes = GetComponentsInChildren<MeshRenderer>();
		foreach(MeshRenderer rend in childMeshes) {
			rend.material.color = Color.blue;
		}

		burstShotsRemaining = burstAmount;
	}

//	void Start() {
//		burstShotsRemaining = burstAmount;
//	}

	public void OnTriggerHold() {
		Shoot();
		triggerReleased = false;
	}

	public void OnTriggerRelease() {
		triggerReleased = true;
		burstShotsRemaining = burstAmount;
	}

	//public
	void Shoot() {

		if(Time.time > nextShotTime) {

			if(fireMode == FireMode.Burst) {
				if(burstShotsRemaining == 0) {					
					return;
				}
				burstShotsRemaining--;
			}
			else if(fireMode == FireMode.Single) {
				if(!triggerReleased) {
					return;
				}
			}

			for(int i = 0; i < muzzlePoints.Length; i++) {
				nextShotTime = Time.time + msBetweenShots / 1000;
				Projectile newProj = Instantiate(projectile, muzzlePoints[i].position, muzzlePoints[i].rotation) as Projectile;
				newProj.SetSpeed(muzzleVelocity);
				newProj.SetDamage(weaponShellDamage);

				Quaternion newProjRot = muzzlePoints[i].rotation;
				NetworkServer.Spawn(newProj.gameObject);
				Destroy(newProj, 2.0f);

				RpcSetProjectile(newProj.gameObject, muzzleVelocity, newProjRot);
			}			

			//weapon fire effects here
		}			
	}

	[ClientRpc]
	void RpcSetProjectile(GameObject projectile, float muzzleVelocity, Quaternion rot) {
		projectile.transform.rotation = rot;
		projectile.GetComponent<Projectile>().SetSpeed(muzzleVelocity);
		Destroy(projectile, 2.0f);
	}

}
