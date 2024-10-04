using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class TaskCompleteTriggerExample : MonoBehaviour
{
    public NPCDialogue npcDialogue;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>().color = Color.red;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (npcDialogue.currentState == "Incomplete")
            {
                GetComponent<SpriteRenderer>().color = Color.green;
            }
        }
    }
}
