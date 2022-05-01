using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayer : MonoBehaviour {
	private void Start() => SpawnNewPlayer();

	public void SpawnNewPlayer() {
		List<GameObject> skins = GameManager.Instance.skins;
		int currentSkin = GameManager.Instance.CurrentSkin;

		GameObject newPlayer = Instantiate(skins[currentSkin], transform.position, skins[currentSkin].transform.rotation);
		GameManager.Instance.CameraFollowNewTarget(newPlayer.transform);
	}
}
