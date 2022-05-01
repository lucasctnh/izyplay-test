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
		EndGame.OnEndGame += (multiplier) => GameOver(multiplier);
		Blade.OnBladeCut += IncreaseCoins;
		PlayerController.OnDeath += StopGame;
		UIManager.OnPause += (pauseState) => Pause(pauseState);
	}

	private void OnDisable() {
		UIManager.OnFirstTap -= () => HasGameStarted = true;
		EndGame.OnEndGame -= (multiplier) => GameOver(multiplier);
		Blade.OnBladeCut -= IncreaseCoins;
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
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		GameObject newPlayer = Instantiate(skins[skinIndex], playerSpawn.position, skins[skinIndex].transform.rotation);
		CameraFollowNewTarget(newPlayer.transform);

		if (player != null)
			Destroy(player);

		CurrentSkin = skinIndex;
		OnSkinChange?.Invoke();
	}

	public void CameraFollowNewTarget(Transform target) {
		FollowTarget followScript = Camera.main.GetComponent<FollowTarget>();
		if (followScript != null)
			followScript.SetNewTarget(target);
	}

	public void LoadLevel(int index) => StartCoroutine(LoadScene(index));

	public bool SpendCoins(int spendAmount) { // returns true if the buy was sucessful
		if (Coins - spendAmount >= 0) {
			Coins -= spendAmount;
			OnUpdateCoins?.Invoke(Coins);

			return true;
		}
		else
			return false;
	}

	private IEnumerator LoadScene(int index) {
		ResetGameState();

		AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync(index, LoadSceneMode.Single);
		while (!asyncLoadLevel.isDone)
			yield return null;

		yield return new WaitForEndOfFrame();

		FindNewPlayerSpawn();
	}

	private void FindNewPlayerSpawn() {
		GameObject spawn = GameObject.FindGameObjectWithTag("Player Spawn");
		if (spawn != null)
			playerSpawn = spawn.transform;
	}

	private void ResetGameState() {
		UnfreezeGame();

		IsGameRunning = true;
		HasGameStarted = false;
	}

	private void FreezeGame() => Time.timeScale = 0f;

	private void UnfreezeGame() => Time.timeScale = 1f;

	private void LimitFrameRate() {
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = _targetFrameRate;
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
}
