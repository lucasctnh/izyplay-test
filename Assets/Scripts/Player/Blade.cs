using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blade : MonoBehaviour {
	public static event Action<Rigidbody> OnBladeStuck;
	public static event Action OnBladeCut;

	private bool _canStuck = true;

	public void OnCollisionEnter(Collision other) {
		if (other.gameObject.CompareTag("Stuckable") && _canStuck)
			OnBladeStuck?.Invoke(other.rigidbody);

		if (other.gameObject.CompareTag("Cuttable")) {
			OnBladeCut?.Invoke();
			RagdollCuttableChildren(other.transform);
		}
	}

	private void RagdollCuttableChildren(Transform cuttable) {
		foreach (Transform child in cuttable) {
			Rigidbody rb = child.gameObject.AddComponent<Rigidbody>();
			rb.AddExplosionForce(10f, child.transform.position, .5f, 0f, ForceMode.VelocityChange);
		}

		cuttable.DetachChildren();
		Destroy(cuttable.gameObject);
	}

	private IEnumerator WaitToGetStuckAgain() {
		_canStuck = false;
		yield return new WaitForSeconds(.5f);
		_canStuck = true;
	}
}
