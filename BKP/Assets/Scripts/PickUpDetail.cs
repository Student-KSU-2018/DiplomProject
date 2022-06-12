using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpDetail : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        RubyController controller = other.GetComponent<RubyController>();

        if (controller != null)
        {
            controller.Details += 1;
            controller.CopyDetails += 1;
            Destroy(gameObject);
        }
    }
}
