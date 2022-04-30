using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour {
	[Header("Initial Menu")]
	[SerializeField] private GameObject _initialMenu;
	[SerializeField] private TMP_Text _coinsText;

	[Header("Levels Menu")]
	[SerializeField] private GameObject _levelsMenu;

	[Header("Game Over Menu")]
	[SerializeField] private GameObject _gameOverMenu;
	[SerializeField] private TMP_Text _gameOverCoinsText;

	private void OnEnable() {
		PlayerController.OnFirstTap += () => _initialMenu.SetActive(false);
		GameManager.OnUpdateCoins += (coins) => UpdateCoins(coins);
		EndGame.OnEndGame += (mult) => _gameOverMenu.SetActive(true);
	}

	private void OnDisable() {
		PlayerController.OnFirstTap -= () => _initialMenu.SetActive(false);
		GameManager.OnUpdateCoins -= (coins) => UpdateCoins(coins);
		EndGame.OnEndGame -= (mult) => _gameOverMenu.SetActive(true);
	}

	private void UpdateCoins(int coins) {
		_coinsText.text = "x" + coins;
		_gameOverCoinsText.text = "x" + coins;
	}
}
