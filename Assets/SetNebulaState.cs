using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetNebulaState : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player") {
            this.gameObject.transform.GetChild(0).gameObject.GetComponent<CosmicFogGenerator>().SetEnableGeneration(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player") {
            this.gameObject.transform.GetChild(0).gameObject.GetComponent<CosmicFogGenerator>().SetEnableGeneration(false);
        }
    }
}
