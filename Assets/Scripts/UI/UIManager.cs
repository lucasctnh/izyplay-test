using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour {
	[SerializeField] private GameObject _initialMenu;
	[SerializeField] private TMP_Text _coinsText;

	private void OnEnable() {
		PlayerController.OnFirstTap += () => _initialMenu.SetActive(false);
		GameManager.OnUpdateCoins += (coins) => _coinsText.text = "x" + coins;
	}

	private void OnDisable() {
		PlayerController.OnFirstTap -= () => _initialMenu.SetActive(false);
		GameManager.OnUpdateCoins -= (coins) => _coinsText.text = "x" + coins;
	}
}
