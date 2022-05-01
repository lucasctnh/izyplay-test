using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blade : MonoBehaviour {
	public static event Action<Rigidbody> OnStuck;
	public static event Action OnCut;

	[Header("Settings")]
	[Tooltip("The force to be applied to throw the halfs of the cuttable away")]
	[SerializeField] private float _cutForce = 7f;

	[Tooltip("The minimum distance from the player for the halfs of cuttable start to destroy themselves")]
	[SerializeField] private float _minDistanceToDestroy = 40f;

	public void OnCollisionEnter(Collision other) {
		if (other.gameObject.CompareTag("Stuckable"))
			OnStuck?.Invoke(other.rigidbody);

		if (other.gameObject.CompareTag("Cuttable"))
			Cut(other.transform);
	}

	private void Cut(Transform cuttable) {
		foreach (Transform child in cuttable) {
			Rigidbody rb = CreateRigidbody(child);
			rb.AddForce(-child.transform.forward * _cutForce, ForceMode.VelocityChange);

			CreateDestroyFarFromPlayer(child);
		}

		cuttable.DetachChildren();
		Destroy(cuttable.gameObject);

		OnCut?.Invoke();
	}

	private Rigidbody CreateRigidbody(Transform child) {
		Rigidbody rb = child.gameObject.GetComponent<Rigidbody>();
		if (rb == null)
			rb = child.gameObject.AddComponent<Rigidbody>();

		return rb;
	}

	private void CreateDestroyFarFromPlayer(Transform child) {
		DestroyFarFromPlayer far = child.gameObject.AddComponent<DestroyFarFromPlayer>();
		far.player = transform;
		far.minDistance = _minDistanceToDestroy;
	}
}
