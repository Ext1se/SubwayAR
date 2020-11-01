using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coins : MonoBehaviour
{
    public TMPro.TextMeshProUGUI text;
    public static int CoinsCount=0;

    private void Update()
    {
        text.SetText(CoinsCount.ToString());
    }
}
