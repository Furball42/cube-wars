using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent (typeof (PlayerController))]
public class Player : DamageableEntity {

	public Slider hpSlider;
	public Color fullHPColor;
	public Color zeroHPColor;
	public Image hpFillImage;
	public Color playerColor;

	float respawnTime = 5f;
	protected NetworkStartPosition[] spawnPoints;
	PlayerManager playerManager;

	public override void OnStartLocalPlayer() {

		spawnPoints = FindObjectsOfType<NetworkStartPosition>();
		playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();

		playerColor = Color.blue;
		SetPlayerColor();

		//ui
		playerManager.SetupPlayerLives();
	}

	void Update() {
		SetHitpointsUI();
	}
		
	public void Respawn() {
		if(isLocalPlayer) {

			Vector3 spawnPoint = Vector3.zero;

			if(spawnPoints != null && spawnPoints.Length > 0) {
				spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
			}
				
			transform.position = spawnPoint;

			RemovePlayerLife();
			CheckIfToDisablePlayer();
		}
	}
		
	public void SetHitpointsUI() {
		if(isLocalPlayer) {
			hpSlider.value = base.hitpoints;
			float ratio = base.hitpoints/ base.startingHitpoints;
			hpFillImage.color = Color.Lerp(zeroHPColor, fullHPColor, ratio);
		}
	}

	public void RemovePlayerLife() {
		playerManager.RemovePlayerLife();
	}

	public void SetPlayerColor() {
		MeshRenderer[] childMeshes;
		childMeshes = GetComponentsInChildren<MeshRenderer>();
		foreach(MeshRenderer rend in childMeshes) {
			rend.material.color = playerColor;
		}
	}

	public void CheckIfToDisablePlayer() {
		if(playerManager.ReturnPlayerLives() <= 0) {
			gameObject.SetActive(false);
		}
	}
}
