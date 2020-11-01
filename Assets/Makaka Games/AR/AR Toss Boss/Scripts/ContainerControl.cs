/*
===================================================================
Unity Assets by MAKAKA GAMES: https://makaka.org/o/all-unity-assets
===================================================================

Online Docs (Latest): https://makaka.org/unity-assets
Offline Docs: You have a PDF file in the package folder.

=======
SUPPORT
=======

First of all, read the docs. If it didn’t help, get the support.

Web: https://makaka.org/support
Email: info@makaka.org

If you find a bug or you can’t use the asset as you need, 
please first send email to info@makaka.org (in English or in Russian) 
before leaving a review to the asset store.

I am here to help you and to improve my products for the best.
*/

using System;
using System.Collections.Generic;

using UnityEngine;

#pragma warning disable 649

[HelpURL("https://makaka.org/unity-assets")]
public class ContainerControl : MonoBehaviour
{
    [SerializeField]
    private SameGameObjectDetector sameGameObjectDetector;
    private GameObject currentObject;

    [Header("Tag For Triggering")]
    [SerializeField]
	private bool isTagCustomUsedForCollisionDetection = false;

    [TagSelector]
    [SerializeField]
	private string tagCustomForCollisionDetection = TagSelectorAttribute.Untagged;

    private event Action OnInitialized;
    private event Action<GameObject> OnCollisionSafe;
    
    public void Init(int countOfObjectsForInteraction, Action OnInitialized, Action<GameObject> OnCollisionSafe)
    {   
        this.OnInitialized += OnInitialized;
        this.OnCollisionSafe += OnCollisionSafe;

        sameGameObjectDetector.Init(countOfObjectsForInteraction, InitBase);
    }

    private void InitBase()
    {   
        if (OnInitialized != null)
        {
            OnInitialized.Invoke();
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        currentObject = other.gameObject;

        if (isTagCustomUsedForCollisionDetection && currentObject.tag != tagCustomForCollisionDetection)
        {
            return;
        }

        if (!sameGameObjectDetector.DetectOrRegister (currentObject))
        {
            if (OnCollisionSafe != null)
            {
                OnCollisionSafe.Invoke(currentObject);
            }
        }
    }
}
