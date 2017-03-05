using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

	public LayerMask collisionMask;

	float damage = 1;
	float speed = 10;

	void Update () {
		float moveDistance = speed * Time.deltaTime;
		CheckCollisions(moveDistance);

		transform.Translate(Vector3.forward * Time.deltaTime * speed);
	}

	public void SetSpeed(float newSpeed) {
		speed = newSpeed;
	}

	public void SetDamage(float newDamage) {
		damage = newDamage;
	}

	void CheckCollisions(float moveDistance) {
		Ray ray = new Ray(transform.position, transform.forward);
		RaycastHit hit;

		if(Physics.Raycast(ray, out hit, moveDistance, collisionMask)) {
			OnHitObject(hit);
		}
	}

	void OnHitObject(RaycastHit hit) {		
		IDamageable damageableObj = hit.collider.GetComponent<IDamageable>();
		if(damageableObj != null ) {
			damageableObj.TakeHit(damage, hit);
		}

		GameObject.Destroy(gameObject);
	}

//	void OnCollisionEnter(Collision collision) {
//		GameObject hit = collision.gameObject;
//
//		Debug.Log(hit.name);
//	}

}
