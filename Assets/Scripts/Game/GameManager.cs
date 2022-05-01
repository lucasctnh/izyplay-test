using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
	public static GameManager Instance;

	public static event Action<int> OnUpdateCoins;

	public int Coins { get { return _coins; } }
	public bool IsGameRunning { get { return _isGameRunning; } private set { _isGameRunning = value; } }
	public bool HasGameStarted { get { return _hasGameStarted; } private set { _hasGameStarted = value; } }

	[SerializeField] private int _targetFrameRate = 60;
	[SerializeField] private Transform _playerSpawn;
	[SerializeField] private GameObject _skinPrefab;

	private int _coins = 0;
	private bool _isGameRunning = true;
	private bool _hasGameStarted = false;

	private void OnEnable() {
		UIManager.OnFirstTap += () => HasGameStarted = true;
		EndGame.OnEndGame += (multiplier) => GameOver(multiplier);
		Blade.OnBladeCut += IncreaseCoins;
	}

	private void OnDisable() {
		UIManager.OnFirstTap -= () => HasGameStarted = true;
		EndGame.OnEndGame -= (multiplier) => GameOver(multiplier);
		Blade.OnBladeCut -= IncreaseCoins;
	}

	private void Awake() {
		if (Instance != null) {
			Destroy(gameObject);
			return;
		}

		Instance = this;
		DontDestroyOnLoad(gameObject);

		LimitFrameRate();
	}

	public void ChangeSkin() {
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		GameObject newPlayer = Instantiate(_skinPrefab, _playerSpawn.position, _skinPrefab.transform.rotation);
		Camera.main.GetComponent<FollowTarget>().SetNewTarget(newPlayer.transform);
		Destroy(player);
	}

	public void LoadNextLevel() {
		ResetGameState();
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1, LoadSceneMode.Single);
	}

	private void ResetGameState() => _isGameRunning = true;

	private void LimitFrameRate() {
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = _targetFrameRate;
	}

	private void IncreaseCoins() {
		_coins++;
		OnUpdateCoins?.Invoke(_coins);
	}

	private void GameOver(int multiplier) {
		IsGameRunning = false;
		MultiplyCoins(multiplier);
	}

	private void MultiplyCoins(int multiplier) {
		_coins *= multiplier;
		OnUpdateCoins?.Invoke(_coins);
	}
}
