using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLevel : MonoBehaviour
{
    [SerializeField]
    private GameManager gameManager;

    void OnTriggerEnter2D()
    {
        gameManager.CompleteLevel();
    }
}
