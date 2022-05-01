using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
	public static GameManager Instance;

	public static event Action<int> OnUpdateCoins;
	public static event Action OnSkinChange;

	public int Coins { get; private set; }
	public int CurrentSkin { get; private set; }
	public bool IsGameRunning { get { return _isGameRunning; } private set { _isGameRunning = value; } }
	public bool HasGameStarted { get { return _hasGameStarted; } private set { _hasGameStarted = value; } }

	[Header("References")]
	public Transform playerSpawn;
	public List<GameObject> skins = new List<GameObject>();

	[Header("Settings")]
	[SerializeField] private int _targetFrameRate = 60;

	private bool _isGameRunning = true;
	private bool _hasGameStarted = false;

	private void OnEnable() {
		UIManager.OnFirstTap += () => HasGameStarted = true;
		EndGameCollider.OnHit += (multiplier) => GameOver(multiplier);
		Blade.OnCut += IncreaseCoins;
		PlayerController.OnDeath += StopGame;
		UIManager.OnPause += (pauseState) => Pause(pauseState);
	}

	private void OnDisable() {
		UIManager.OnFirstTap -= () => HasGameStarted = true;
		EndGameCollider.OnHit -= (multiplier) => GameOver(multiplier);
		Blade.OnCut -= IncreaseCoins;
		UIManager.OnPause -= (pauseState) => Pause(pauseState);
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

	private void Start() => FindNewPlayerSpawn();

	public void ChangeSkin(int skinIndex) {
		CurrentSkin = skinIndex;

		GameObject lastPlayer = GameObject.FindGameObjectWithTag("Player");
		playerSpawn.GetComponent<SpawnPlayer>().SpawnNewPlayer();

		if (lastPlayer != null)
			Destroy(lastPlayer);

		OnSkinChange?.Invoke();
	}

	public void CameraFollowNewTarget(Transform target) {
		CameraController cameraController = Camera.main.GetComponent<CameraController>();
		if (cameraController != null)
			cameraController.SetNewTarget(target);
	}

	// returns true if the buy was sucessful
	public bool SpendCoins(int spendAmount) {
		if (Coins - spendAmount >= 0) {
			Coins -= spendAmount;
			OnUpdateCoins?.Invoke(Coins);

			return true;
		}
		else
			return false;
	}

	public void LoadLevel(int index) => StartCoroutine(LoadScene(index));

	private IEnumerator LoadScene(int index) {
		ResetGameState();

		AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync(index, LoadSceneMode.Single);
		while (!asyncLoadLevel.isDone)
			yield return null;

		yield return new WaitForEndOfFrame();

		FindNewPlayerSpawn();
	}

	private void ResetGameState() {
		UnfreezeGame();

		IsGameRunning = true;
		HasGameStarted = false;
	}

	private void FindNewPlayerSpawn() {
		GameObject spawn = GameObject.FindGameObjectWithTag("Player Spawn");
		if (spawn != null)
			playerSpawn = spawn.transform;
	}

	private void IncreaseCoins() {
		Coins++;
		OnUpdateCoins?.Invoke(Coins);
	}

	private void GameOver(int multiplier) {
		StopGame();
		MultiplyCoins(multiplier);
	}

	private void StopGame() {
		IsGameRunning = false;
		FreezeGame();
	}

	private void MultiplyCoins(int multiplier) {
		Coins *= multiplier;
		OnUpdateCoins?.Invoke(Coins);
	}

	private void Pause(bool pauseState) {
		if (pauseState)
			FreezeGame();
		else
			UnfreezeGame();
	}

	private void FreezeGame() => Time.timeScale = 0f;

	private void UnfreezeGame() => Time.timeScale = 1f;

	private void LimitFrameRate() {
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = _targetFrameRate;
	}
}
