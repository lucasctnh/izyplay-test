using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour {
	[Header("Settings")]
	[SerializeField] private float _smoothSpeed = 0.125f;
	[SerializeField] private Transform _target;

	private Vector3 _offset;

	private void Awake() => _offset = transform.position - _target.position;

	private void FixedUpdate() {
		Vector3 desiredPosition = _target.position + _offset;
		Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, _smoothSpeed);
		transform.position = smoothedPosition;
	}
}
