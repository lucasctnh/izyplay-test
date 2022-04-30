using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public static event Action<int> OnUpdateCoins;

	private int _coins = 0;

	private void OnEnable() {
		Blade.OnBladeCut += IncreaseCoins;
		EndGame.OnEndGame += (multiplier) => MultiplyCoins(multiplier);
	}

	private void OnDisable() {
		Blade.OnBladeCut -= IncreaseCoins;
		EndGame.OnEndGame -= (multiplier) => MultiplyCoins(multiplier);
	}

	private void IncreaseCoins() {
		_coins++;
		OnUpdateCoins?.Invoke(_coins);
	}

	private void MultiplyCoins(int multiplier) {
		_coins *= multiplier;
		OnUpdateCoins?.Invoke(_coins);
	}
}
