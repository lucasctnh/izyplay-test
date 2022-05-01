using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour {
	public static event Action OnTapTouchableArea;
	public static event Action OnFirstTap;

	[Header("Initial Menu")]
	[SerializeField] private GameObject _initialMenu;
	[SerializeField] private TMP_Text _coinsText;

	[Header("Levels Menu")]
	[SerializeField] private GameObject _levelsMenu;

	[Header("Game Over Menu")]
	[SerializeField] private GameObject _gameOverMenu;
	[SerializeField] private TMP_Text _gameOverCoinsText;

	private void Awake(){
		if (GameManager.Instance.HasGameStarted)
			_initialMenu.SetActive(false);

		UpdateCoins(GameManager.Instance.Coins);
	}

	private void OnEnable() {
		OnFirstTap += () => _initialMenu.SetActive(false);
		GameManager.OnUpdateCoins += (coins) => UpdateCoins(coins);
		EndGame.OnEndGame += (mult) => _gameOverMenu.SetActive(true);
	}

	private void OnDisable() {
		OnFirstTap -= () => _initialMenu.SetActive(false);
		GameManager.OnUpdateCoins -= (coins) => UpdateCoins(coins);
		EndGame.OnEndGame -= (mult) => _gameOverMenu.SetActive(true);
	}

	public void TapTouchableArea() {
		OnTapTouchableArea?.Invoke();

		if (!GameManager.Instance.HasGameStarted)
			OnFirstTap?.Invoke();
	}

	private void UpdateCoins(int coins) {
		_coinsText.text = "x" + coins;
		_gameOverCoinsText.text = "x" + coins;
	}
}
