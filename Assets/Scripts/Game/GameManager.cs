using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public static event Action<int> OnUpdateCoins;

	private int _coins = 0;

	private void OnEnable() => Blade.OnBladeCut += IncreaseCoins;

	private void OnDisable() => Blade.OnBladeCut -= IncreaseCoins;

	private void IncreaseCoins() {
		_coins++;
		OnUpdateCoins?.Invoke(_coins);
	}
}
