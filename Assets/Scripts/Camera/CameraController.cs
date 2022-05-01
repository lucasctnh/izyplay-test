using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class CameraController : MonoBehaviour {
	[Header("Settings")]
	[SerializeField] private float _smoothSpeed = 0.125f;

	[Header("References")]
	[SerializeField] private Transform _target;

	private Vector3 _offset;

	private void Start() {
		if (_target == null)
			_target = GameManager.Instance.playerSpawn;

		_offset = transform.position - _target.position;
	}

	private void FixedUpdate() {
		Vector3 desiredPosition = _target.position + _offset;
		Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, _smoothSpeed);
		transform.position = smoothedPosition;
	}

	public void SetNewTarget(Transform target) {
		_target = target;
		GetComponent<LookAtConstraint>().SetSource(0, new ConstraintSource{ sourceTransform = target.transform, weight = 1f });
	}
}
