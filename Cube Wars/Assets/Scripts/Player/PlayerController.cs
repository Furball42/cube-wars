using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent (typeof(Rigidbody))]
[RequireComponent (typeof (WeaponController))]
public class PlayerController : NetworkBehaviour {

	public float acceleration = 2.0f;
	public float turnSpeed = 180f;
	public float maxSpeed = 25f;
	public float maxReverse = 10f;

	string movementAxisName;     
	string turnAxisName; 
	float movementInputValue;    
	float turnInputValue;
	float currentSpeed;
	Rigidbody rigidbody; 
	Animator anim;
		
	void Start () {
		movementAxisName = "Vertical";
		turnAxisName = "Horizontal";
		currentSpeed = 0.0f;
		rigidbody = GetComponent<Rigidbody>();
	}
		
	void Update () {

		if (!isLocalPlayer)
			return;

		//movement
		movementInputValue = Input.GetAxis(movementAxisName);
		turnInputValue = Input.GetAxis(turnAxisName);

		Move();
		Turn();
	}

	//movement
	void Move() {

		Vector3 movement = transform.forward * movementInputValue * currentSpeed * Time.deltaTime;
		rigidbody.MovePosition(rigidbody.position + movement);

		if (movementInputValue > 0) //forward
		{			
			currentSpeed += acceleration;

			if (currentSpeed > maxSpeed)
				currentSpeed = maxSpeed;
		}		
		else if (movementInputValue < 0) { //backward

			currentSpeed += acceleration;

			if (currentSpeed > maxReverse)
				currentSpeed = maxReverse;
		}
		else if (movementInputValue == 0) { //slow down - stop

			currentSpeed = 0;

			if (currentSpeed < 0)
				currentSpeed = 0;
		}
	}

	//turning
	void Turn() {

		float turn = turnInputValue * turnSpeed * Time.deltaTime;

		Quaternion turnRotation = Quaternion.Euler (0f, turn, 0f);

		rigidbody.MoveRotation (rigidbody.rotation * turnRotation);
	}
		
}
