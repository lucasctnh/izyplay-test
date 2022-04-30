using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cuttable : MonoBehaviour {
	[SerializeField] private Rigidbody _rigidbodySideA;
	[SerializeField] private Rigidbody _rigidbodySideB;

	private void OnEnable() => PlayerController.OnSlice += RagdollSides;

	private void OnDisable() => PlayerController.OnSlice -= RagdollSides;

	private void RagdollSides() {
		_rigidbodySideA.useGravity = true;
		_rigidbodySideB.useGravity = true;
	}
}
