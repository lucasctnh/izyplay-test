using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Handle : MonoBehaviour {
	public static event Action OnHit;

	public void OnCollisionEnter(Collision other) {
		if (other.gameObject.CompareTag("Stuckable") || other.gameObject.CompareTag("Cuttable"))
			OnHit?.Invoke();
	}
}
