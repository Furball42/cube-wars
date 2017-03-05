using UnityEngine;
using System.Collections;

public class Obstacle : DamageableEntity {

	protected override void Start () {
		base.Start();
		//controller = GetComponent<PlayerController>();
	}
}
