using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {
	public static event Action OnDeath;

#region Inspector Fields

	[Header("Movement")]
	[Tooltip("The force that will be applied to the movement vertically")]
	[SerializeField] private float _upwardsForce = 200f;

	[Tooltip("The force that will be applied to the movement horizontally")]
	[SerializeField] private float _forwardsForce = 200f;

	[Tooltip("A multiplier that will be applied to the vertical force when the rotation is backwards")]
	[SerializeField] private float _backwardsVerticalMultiplier = .6f;

	[Tooltip("A multiplier that will be applied to the horizontal force when the rotation is backwards")]
	[SerializeField] private float _backwardsHorizontalMultiplier = 1.2f;

	[Tooltip("A multiplier that will be applied to gravity when in a sequence of cuts")]
	[SerializeField] private float _cutSequenceMultiplier = 2f;

	[Header("Rotation")]
	[Tooltip("The torque to be applied every frame if a move action was detected")]
	[SerializeField] private float _rotationTorque = 20f;

	[Tooltip("The minimum time in seconds to count a complete rotation around itself")]
	[SerializeField] private float _rotationTimeToDefault = 1f;

	[Tooltip("The angular drag that will be applied when one rotation is near complete")]
	[SerializeField] private float _brakeAngularDrag = 8f;

	[Tooltip("The default angular drag")]
	[SerializeField] private float _defaultAngularDrag = .05f;

	[Header("Components References")]
	[SerializeField] private Blade _blade;
	[SerializeField] private Handle _handle;

#endregion

#region Private Properties & Fields

	private FixedJoint _FixedJoint {
		get {
			if (this != null)
				_fixedJoint = GetComponent<FixedJoint>();
			return _fixedJoint;
		}
	}

	private Rigidbody _rigidbody;
	private FixedJoint _fixedJoint;
	private Quaternion _defaultRotation;
	private float _timeRotating = 0f;
	private float _lastCutTime = 0f;
	private int _rotationsAroundItself = 0;
	private int _direction = -1;
	private bool _isRotating = false;
	private bool _canCountRotations = true;
	private bool _canRotate = false;
	private bool _isOnCutSequence = false;

#endregion

#region Unity Lifecycle

	private void OnEnable() {
		UIManager.OnTapTouchableArea += Move;
		Blade.OnStuck += (rigidbody) => GetStuckOn(rigidbody);
		Blade.OnCut += HandleCutSequence;
		Handle.OnHit += RotateBackwards;
	}

	private void OnDisable() {
		UIManager.OnTapTouchableArea -= Move;
		Blade.OnStuck -= (rigidbody) => GetStuckOn(rigidbody);
		Blade.OnCut -= HandleCutSequence;
		Handle.OnHit -= RotateBackwards;
	}

	private void Awake() {
		_rigidbody = GetComponent<Rigidbody>();
		_defaultRotation = transform.rotation;
	}

	private void FixedUpdate() {
		if (!GameManager.Instance.IsGameRunning)
			return;

		if (_isRotating && _rotationsAroundItself >= 1 && IsRotationCloseToDefault(transform.rotation, GetPrecisionFromEulerAngles(0, 0, 3f))) {
			_rotationsAroundItself--;

			if (_rotationsAroundItself == 0)
				CapRotation();
		}

		if (_canRotate)
			Rotate();

		if (!_isRotating && _isOnCutSequence)
			ApplyGravityHelper();
	}

	private void Update() {
		if (!GameManager.Instance.IsGameRunning)
			return;

		if (_isRotating && _canCountRotations)
			CountRotations(GetFullAngleFromRotations(transform.rotation, _defaultRotation, Vector3.right));
	}

	private void OnCollisionEnter(Collision other) {
		if (other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Spikes"))
			OnDeath?.Invoke();

		ContactPoint contact = other.GetContact(0);
		if (contact.thisCollider.gameObject.CompareTag("Blade"))
			_blade.OnCollisionEnter(other);

		if (contact.thisCollider.gameObject.CompareTag("Handle"))
			_handle.OnCollisionEnter(other);
	}

#endregion

#region Movement

	private void CapRotation() {
		StopRotation();

		_rigidbody.angularDrag = 100f;
		transform.rotation = _defaultRotation;
	}

	private void StopRotation() {
		_canRotate = false;
		_isRotating = false;
	}

	private void ApplyGravityHelper() {
		_rigidbody.velocity += Vector3.up * Physics.gravity.y * _cutSequenceMultiplier * Time.deltaTime;
		_rigidbody.velocity = new Vector3(0, _rigidbody.velocity.y, 0); // enforce player to go only down
	}

	private void Move() {
		if (!GameManager.Instance.IsGameRunning)
			return;

		PrepareToRotate();
		AddMovementForces();
	}

	private void RotateBackwards() {
		PrepareToRotate("counterclockwise");
		AddMovementForces(_backwardsVerticalMultiplier, _backwardsHorizontalMultiplier);
	}

	private void PrepareToRotate(string direction = "clockwise") {
		ResetRotationState();

		_direction = direction == "counterclockwise" ? 1 : -1;

		RemoveFixedJoint();
		InitiateRotation();
	}

	private void ResetRotationState() {
		_rigidbody.angularDrag = _defaultAngularDrag;
		_isOnCutSequence = false;
		_canCountRotations = true;
		_timeRotating = 0f;
	}

	private void RemoveFixedJoint() {
		if (_FixedJoint != null)
			Destroy(_FixedJoint);
	}

	private void InitiateRotation() {
		_canRotate = true;
		_isRotating = true;
	}

	private void AddMovementForces(float upwardsMultiplier = 1f, float forwardsMultiplier = 1f) {
		_rigidbody.velocity = Vector3.zero;
		_rigidbody.AddForce(Vector3.up * _upwardsForce * upwardsMultiplier * Time.fixedDeltaTime, ForceMode.VelocityChange);
		_rigidbody.AddForce(Vector3.right * _forwardsForce * (_direction * -1) * forwardsMultiplier * Time.fixedDeltaTime, ForceMode.VelocityChange);
	}

	private void Rotate() {
		_rigidbody.AddRelativeTorque(0, 0, _rotationTorque * _direction * Time.fixedDeltaTime, ForceMode.VelocityChange);
		BrakeTorqueNearDefaultPosition();
	}

	private void BrakeTorqueNearDefaultPosition() {
		if (IsRotationCloseToDefaultCounterclockwise(transform.rotation, GetPrecisionFromEulerAngles(0, 0, 60f)))
			_rigidbody.angularDrag = _brakeAngularDrag;
		else
			_rigidbody.angularDrag = _defaultAngularDrag;
	}

	private void CountRotations(float angle) {
		_timeRotating += Time.deltaTime;

		bool angleCondition = angle >= 300;
		if (_direction > 0)
			angleCondition = angle <= 60;

		if (angleCondition && _timeRotating >= _rotationTimeToDefault)
			IncreaseRotationsAroundItselfOnce();
	}

	private void IncreaseRotationsAroundItselfOnce() {
		_rotationsAroundItself++;
		_canCountRotations = false;

		_timeRotating = 0f;
	}

#endregion

#region Stuck Logic

	private void GetStuckOn(Rigidbody rigidbody) {
		StopRotation();
		ResetRotationsAroundItself();

		if (this != null) { // workaround missing reference exception
			if (_FixedJoint == null)
				CreateFixedJoint();

			StartCoroutine(ConfigureJoint(rigidbody));
		}
	}

	private void ResetRotationsAroundItself() => _rotationsAroundItself = 0;

	private void CreateFixedJoint() => gameObject.AddComponent<FixedJoint>();

	private IEnumerator ConfigureJoint(Rigidbody rigidbody) {
		yield return new WaitWhile(() => _FixedJoint == null); // wait joint creation

		if (_fixedJoint != null)
			_fixedJoint.connectedBody = rigidbody;
	}

#endregion

	private void HandleCutSequence() {
		float timeDiff = Time.time - _lastCutTime;
		_lastCutTime = Time.time;

		_isOnCutSequence = timeDiff < .6f;
	}

#region Math Utils

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

#endregion
}
