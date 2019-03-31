using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollider : MonoBehaviour
{
    [SerializeField]
    BarrierSystem barrierSystem;

    void OnCollisionEnter2D(Collision2D other)
    {
        print("Colliding");
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            if (!other.gameObject.CompareTag(barrierSystem.tag))
            {
                print("Colliding");
                barrierSystem.die();
            }
        }
    }
}
