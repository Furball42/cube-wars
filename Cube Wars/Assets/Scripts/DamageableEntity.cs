using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class DamageableEntity : NetworkBehaviour, IDamageable {

	public float startingHitpoints;
	public bool invulnerable;

	[SyncVar] protected float hitpoints;
	protected bool dead;
	Player player;

	protected virtual void Start() {
		hitpoints = startingHitpoints;
		player = GetComponent<Player>();
	}
		
	public void TakeHit(float damage, RaycastHit hit) {

		if(!isServer) return;

		RpcTakeHit(damage, hit);
	}

	[ClientRpc]
	public void RpcTakeHit(float damage, RaycastHit hit) {
		if(!invulnerable) {
			hitpoints -= damage;

			player.SetHitpointsUI();

			if(hitpoints <= 0 && !dead) {
				Die();
			}
		}
	}

	protected void Die() {
		dead = true;

		if (player != null) {
			player.Respawn();
			ResetHitPoints();
		}
	}

	public void ResetHitPoints() {
		dead = false;
		hitpoints = startingHitpoints;
		player.SetHitpointsUI();
	}
		





	[Command]
	void CmdDestroyObstacle(GameObject destroyedObj) {
		NetworkServer.Destroy(destroyedObj);
		RpcDestroyObstacle(destroyedObj);
	}

	[ClientRpc]
	void RpcDestroyObstacle(GameObject destroyedObj) {

		NetworkServer.Destroy(destroyedObj);
		//Destroy(destroyedObj);
	}
}
