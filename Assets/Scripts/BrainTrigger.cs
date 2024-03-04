using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrainTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Zombie"))
        {
            Debug.Log("GameEnd!");
        }
        else
        {
            Debug.LogError("Unexpected something touch the Brain");
        }
    }
}
