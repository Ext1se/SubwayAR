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
public class FailControl : MonoBehaviour
{   
    [Tooltip("To prevent failing several times.")]
    [SerializeField]
    private SameGameObjectDetector failingSeveralTimesDetector;
    private GameObject currentObject;

    [Tooltip("To Prevent Failing after Scoring.")]
    [SerializeField]
    private SameGameObjectDetector failingAfterScoringDetector;

    public Color failColor = Color.red;

    [Header("Tag For Collision Detection")]
    [SerializeField]
    private bool isTagCustomUsedForCollisionDetection = false;
    
    [TagSelector]
    [SerializeField]
    private string tagCustomForCollisionDetection = TagSelectorAttribute.Untagged;

    [Header("Audio")]
    [SerializeField]
	private AudioSource audioSource;
	
    [SerializeField]
	private AudioClip[] failSounds;
    
    private event Action OnInitialized;
    private event Action<GameObject> OnFailed;
    private event Action<GameObject> OnCollision;

    public void Init (
        int countOfObjectsForInteraction, 
        Action OnInitialized,
        Action<GameObject> OnCollision,
        Action<GameObject> OnFailed)
    {
        this.OnInitialized += OnInitialized;
        this.OnCollision += OnCollision;
        this.OnFailed += OnFailed;

        failingSeveralTimesDetector.Init(
            countOfObjectsForInteraction,() => InitFailingAfterScoringDetector(countOfObjectsForInteraction));
    }
    
    private void InitFailingAfterScoringDetector(int countOfObjectsForInteraction)
    {
        failingAfterScoringDetector.Init(countOfObjectsForInteraction, InitBase);
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

        if (OnCollision != null)
        {
            OnCollision.Invoke(currentObject);
        }

        if (!failingSeveralTimesDetector.DetectOrRegister (currentObject))
        {
            if (!failingAfterScoringDetector.DetectOrRegister (currentObject))
            {  
                PlayFailSound();

                if (OnFailed != null)
                {
                    //print ("Fail");

                    OnFailed.Invoke(currentObject);
                }
            } 
        }
    }

    private void PlayFailSound()
    {
        audioSource.PlayOneShot(failSounds[UnityEngine.Random.Range(0, failSounds.Length)]);
    }

    /// <summary>
    ///  To Register Events happening outside.
    /// </summary>
    public void RegisterAfterScoring(GameObject scoredObject)
    {
        failingAfterScoringDetector.DetectOrRegister(scoredObject);
    }
}
