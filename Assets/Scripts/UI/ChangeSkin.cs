using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChangeSkin : MonoBehaviour {
	[Header("Settings")]
	[SerializeField] private int _skinIndex = 0;
	[SerializeField] private int _skinCost = 0;
	[SerializeField] private bool _isUnlocked = false;

	[Header("References")]
	[SerializeField] private TMP_Text _costText;

	private void Awake() {
		if (!_isUnlocked)
			_costText.text = "$" + _skinCost;
	}

    public void BuySkin() {
		if (!_isUnlocked) {
			bool canBuy = GameManager.Instance.SpendCoins(_skinCost);

			if (canBuy) {
				_isUnlocked = true;
				_costText.gameObject.SetActive(false);
				GameManager.Instance.ChangeSkin(_skinIndex);
			}
		}
		else
			GameManager.Instance.ChangeSkin(_skinIndex);
	}
}
