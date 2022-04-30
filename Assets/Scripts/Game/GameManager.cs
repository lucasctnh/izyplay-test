using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
	public static GameManager Instance;

	public static event Action<int> OnUpdateCoins;

	public bool IsGameRunning { get { return _isGameRunning; } private set { _isGameRunning = value; } }
	public bool IsFirstTap { get { return _isFirstTap; } private set { _isFirstTap = value; } }

	[SerializeField] private int _targetFrameRate = 60;

	private int _coins = 0;
	private bool _isGameRunning = true;
	private bool _isFirstTap = true;

	private void OnEnable() {
		PlayerController.OnFirstTap += () => IsFirstTap = false;
		EndGame.OnEndGame += (multiplier) => GameOver(multiplier);
		Blade.OnBladeCut += IncreaseCoins;
	}

	private void OnDisable() {
		PlayerController.OnFirstTap -= () => IsFirstTap = false;
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

	public void LoadNextLevel() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1, LoadSceneMode.Single);

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
