using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Weapon") || other.gameObject.tag.Equals("Ground"))
        {
            return;
        }
        if (other.tag.Equals("AnimalBone"))
        {
            DragonController dragon = FindObjectOfType<DragonController>();
            if (dragon != null)
            {
                dragon.GetDamage();
            }
        }
    }
}
