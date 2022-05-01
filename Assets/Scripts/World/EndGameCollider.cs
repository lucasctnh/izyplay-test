using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameCollider : MonoBehaviour {
	public static event Action<int> OnHit;

	[Tooltip("The multiplier that will be applied to the current coins if player hits this object")]
    [SerializeField] private int _coinsMultiplier = 1;

	private void OnCollisionEnter(Collision other) {
		if (other.collider.gameObject.CompareTag("Blade"))
			OnHit?.Invoke(_coinsMultiplier);
	}
}
