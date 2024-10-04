using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Test class. Remove when applicable;
/// </summary>
public class SpawnTextIfComplete : MonoBehaviour {

    [SerializeField] private GameObject text;

    void Awake() {
        PuzzleManagement.PuzzleProc.OnPuzzleComplete.AddListener(() => text.SetActive(true));
    }
}
