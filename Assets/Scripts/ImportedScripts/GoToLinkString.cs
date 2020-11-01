using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToLinkString : MonoBehaviour {
    public string url;
    public void OpenUrl()
    {
        if(url!=null)
        Application.OpenURL(url);
    }
}
