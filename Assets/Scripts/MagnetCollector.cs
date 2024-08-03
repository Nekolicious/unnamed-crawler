using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetCollector : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out PickableItem item))
        {
            item.SetTarget(transform.parent.position);
        }
    }
}
