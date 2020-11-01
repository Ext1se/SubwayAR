using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanoController : MonoBehaviour
{
    public GameObject sphere;
    public Material[] panoramas;

    private int activePosition = 0;

    private void Start()
    {
        if (panoramas.Length > 0)
        {
            activePosition = 0;
            sphere.GetComponent<MeshRenderer>().sharedMaterial = panoramas[0];
        }
    }

    public void Move(bool isNext)
    {
        if (isNext)
        {
            activePosition++;
        }
        else
        {
            activePosition--;
        }

        if (activePosition < 0)
        {
            activePosition = panoramas.Length - 1;
        }

        if (activePosition > panoramas.Length - 1)
        {
            activePosition = 0;
        }

        sphere.GetComponent<MeshRenderer>().sharedMaterial = panoramas[activePosition];
    }

    public void SetPano(int position)
    {
        if (position >= 0 && position < panoramas.Length)
        {
            activePosition = position;
        }
        else
        {
            activePosition = 0;
        }
        sphere.GetComponent<MeshRenderer>().sharedMaterial = panoramas[activePosition];
    }

    public void SetEnabled(bool isEnabled)
    {
        gameObject.SetActive(isEnabled);
    }
}
