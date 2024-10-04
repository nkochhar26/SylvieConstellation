using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSpaceToolTip : MonoBehaviour
{
    [SerializeField] private GameObject interactToolTip;
    private bool isPlayer = false;
    private bool isDone = false;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        isDone = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (!isDone && isPlayer && Input.GetKeyDown(KeyCode.Space)) {
            interactToolTip.SetActive(false);
            isDone = true;
        }
        if (!isDone && isPlayer) {
            interactToolTip.SetActive(true);
        }
    }

    /// <summary>
    /// Sent when another object enters a trigger collider attached to this
    /// object (2D physics only).
    /// </summary>
    /// <param name="other">The other Collider2D involved in this collision.</param>
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player") {
            isPlayer = true;
        }
    }
}
