using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeLevel : MonoBehaviour {
	[Tooltip("The level index according to build settings")]
	[SerializeField] private int _levelIndex = 0;

    public void SelectLevel() => GameManager.Instance.LoadLevel(_levelIndex);
}
