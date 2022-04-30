using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blade : MonoBehaviour {
	public static event Action<Rigidbody> OnBladeStuck;
	public static event Action OnBladeCut;

	[SerializeField] private float _cutForce = 7f;
	[SerializeField] private float _minDistanceToDestroy = 40f;

	private bool _canStuck = true;

	public void OnCollisionEnter(Collision other) {
		if (other.gameObject.CompareTag("Stuckable") && _canStuck)
			OnBladeStuck?.Invoke(other.rigidbody);

		if (other.gameObject.CompareTag("Cuttable"))
			Cut(other.transform);
	}

	private void Cut(Transform cuttable) {
		foreach (Transform child in cuttable) {
			Rigidbody rb = child.gameObject.AddComponent<Rigidbody>();
			rb.AddForce(-child.transform.forward * _cutForce, ForceMode.VelocityChange);

			DestroyFarFromPlayer far = child.gameObject.AddComponent<DestroyFarFromPlayer>();
			far.player = transform;
			far.minDistance = _minDistanceToDestroy;
		}

		cuttable.DetachChildren();
		Destroy(cuttable.gameObject);

		OnBladeCut?.Invoke();
	}

	private IEnumerator WaitToGetStuckAgain() {
		_canStuck = false;
		yield return new WaitForSeconds(.5f);
		_canStuck = true;
	}
}
