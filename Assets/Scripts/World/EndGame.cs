using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGame : MonoBehaviour {
	public static event Action<int> OnEndGame;

    [SerializeField] private int _coinsMultiplier = 1;

	private void OnCollisionEnter(Collision other) {
		if (other.collider.gameObject.CompareTag("Blade"))
			OnEndGame?.Invoke(_coinsMultiplier);
	}
}
