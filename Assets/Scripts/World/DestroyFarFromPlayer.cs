using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyFarFromPlayer : MonoBehaviour {
    public Transform player;
	public float minDistance;

	private void Update() {
		if (Vector3.Distance(transform.position, player.position) > minDistance)
			Destroy(gameObject);
	}
}
