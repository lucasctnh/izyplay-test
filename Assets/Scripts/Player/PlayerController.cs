using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {
	public static event Action OnSlice;

	[SerializeField] private float _movementForce = 200f;

	private FixedJoint _FixedJoint {
		get {
			_fixedJoint = GetComponent<FixedJoint>();
			return _fixedJoint;
		}
	}

	private float _UpwardsForce {
		get {
			if (_isPlayerStuck)
				return _movementForce * 3f;
			else
				return _movementForce;
		}
	}

	private Rigidbody _rigibody;
	private FixedJoint _fixedJoint;
	private Quaternion _defaultRotation;
	private float _timeRotating = 0f;
	private int _rotationsAroundItself = 0;
	private bool _isRotating = false;
	private bool _canStuck = true;
	private bool _isPlayerStuck = false;
	private bool _canCountRotations = true;
	private bool _canRotate = false;

	private void Awake() {
		_rigibody = GetComponent<Rigidbody>();
		_defaultRotation = transform.rotation;
	}

	private void FixedUpdate() {
		if (_isRotating && _rotationsAroundItself >= 1 && IsRotationCloseToDefault(transform.rotation, GetPrecisionFromEulerAngles(0, 0, 3f))) {
			_isRotating = false;
			_rotationsAroundItself--;

			if (_rotationsAroundItself == 0)
				StopPlayer();
		}

		if (_canRotate)
			Rotate();
	}

	private void Update() {
		if (_isRotating && _canCountRotations) {
			_timeRotating += Time.deltaTime;
			CountRotations(GetFullAngleFromRotations(transform.rotation, _defaultRotation, Vector3.right));
		}
	}

	private void OnCollisionEnter(Collision other) {
		if (other.gameObject.CompareTag("Stuckable") && _canStuck) {
			_canRotate = false;
			_isRotating = false;
			_isPlayerStuck = true;
			_rotationsAroundItself = 0;

			if (_FixedJoint == null)
				CreateFixedJoint();

			StartCoroutine(ConfigureJoint(other.rigidbody));
		}

		if (other.gameObject.CompareTag("Cuttable"))
			OnSlice?.Invoke();
	}

	private void OnTriggerEnter(Collider other) {
	}

	private void OnCollisionExit(Collision other) {
		if (other.gameObject.CompareTag("Stuckable")) {
			_isPlayerStuck = false;
			StartCoroutine(WaitToGetStuckAgain());
		}
	}

	public void Move() {
		PrepareToRotate();
		AddMovementForces();
	}

	private void StopPlayer() {
		_canRotate = false;
		_rigibody.angularDrag = 100f;
		transform.rotation = _defaultRotation;
	}

	private void CountRotations(float angle) {
		if (angle >= 300 && _timeRotating >= .5f) {
			_rotationsAroundItself++;

			_timeRotating = 0f;

			_canCountRotations = false;
		}
	}

	private IEnumerator WaitToGetStuckAgain() {
		_canStuck = false;
		yield return new WaitForSeconds(1f);
		_canStuck = true;
	}

	private void CreateFixedJoint() => gameObject.AddComponent<FixedJoint>();

	private IEnumerator ConfigureJoint(Rigidbody rigidbody) {
		yield return new WaitWhile(() => _FixedJoint == null);

		_FixedJoint.connectedBody = rigidbody;
		_FixedJoint.breakForce = 500f;
		_FixedJoint.breakTorque = 500f;
	}

	private void PrepareToRotate() {
		_rigibody.angularDrag = .05f;
		_canRotate = true;
		_isRotating = true;
		_canCountRotations = true;
	}

	private void Rotate() {
		_rigibody.AddRelativeTorque(0, 0, -20f * Time.fixedDeltaTime, ForceMode.VelocityChange);
		BrakeTorqueNearDefaultPosition();
	}

	private void BrakeTorqueNearDefaultPosition() {
		if (IsRotationCloseToDefaultCounterclockwise(transform.rotation, GetPrecisionFromEulerAngles(0, 0, 60f)))
			_rigibody.angularDrag = 5f;
		else
			_rigibody.angularDrag = .05f;
	}

	private void AddMovementForces() {
		_rigibody.AddForce(Vector3.up * _UpwardsForce * Time.fixedDeltaTime, ForceMode.VelocityChange);
		_rigibody.AddForce(Vector3.right * _movementForce * Time.fixedDeltaTime, ForceMode.VelocityChange);
	}

	private bool IsRotationCloseToDefault(Quaternion rotation, float precision) {
		return Mathf.Abs(Quaternion.Dot(rotation, _defaultRotation)) >= precision;
	}

	private bool IsRotationCloseToDefaultCounterclockwise(Quaternion rotation, float precision) {
		return Quaternion.Dot(rotation, _defaultRotation) <= -precision;
	}

	private float GetPrecisionFromEulerAngles(float x, float y, float z) {
		float precision = Quaternion.Dot(Quaternion.identity, Quaternion.Euler(x, y, z));
		return precision;
	}

	private float GetFullAngleFromRotations(Quaternion quatA, Quaternion quatB, Vector3 axis) {
		Vector3 vectorA = quatA * axis;
		Vector3 vectorB = quatB * axis;
		float angle = Vector3.Angle(vectorA, vectorB);

		Vector3 cross = Vector3.Cross(vectorA, vectorB);
		if (cross.z < 0) {
			angle *= -1;
			angle += 360;
		}

		return angle;
	}
}
