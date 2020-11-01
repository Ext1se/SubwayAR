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
using System.Collections;

using UnityEngine;

using TMPro;

#pragma warning disable 649

[HelpURL("https://makaka.org/unity-assets")]
public class ScoreControl : MonoBehaviour
{
    [SerializeField]
	private SameGameObjectDetector sameGameObjectDetector;
    private GameObject currentObject;
    
    [SerializeField]
	private ContainerControl containerControl;
    
    [SerializeField]
	private GameObject explosionOnScoring;

    [Header("Tag For Triggering")]
    [SerializeField]
    private bool isTagCustomUsedForTriggering = false;

    [TagSelector]
    [SerializeField]
    private string tagCustomForTriggering = TagSelectorAttribute.Untagged;
    
    [Header("Text")]
    [SerializeField]
    private TextMeshPro scoreText;
    private int currentScore = 0;

    [SerializeField]
    private Animator scoreAnimator;

    [SerializeField]
    private float scoringAnimationDelay = 0f;
    private string scoringAnimationTrigger = "Scoring";
    
    [SerializeField]
    private float completeTaskAnimationDelay = 0f;
    private string completeTaskAnimationTrigger = "CompleteTask";
    
    [SerializeField]
    private string messageOnTaskCompleted = "X";
    private bool isTaskCompleted  = true;
    
    [Header("Audio")]
	[SerializeField]
    private AudioSource audioSource;
	
    [SerializeField]
    private AudioClip[] scoringSounds;

    private event Action OnInitialized;
    private event Action OnTaskCompleted;
    private event Action<GameObject> OnScored;
    
    public void Init (
        int countOfObjectsForInteraction, 
        Action OnInitialized,
        Action OnTaskCompleted,
        Action<GameObject> OnScored, 
        Action<GameObject> OnCollisionSafe)
    {
        this.OnInitialized += OnInitialized;        
        this.OnTaskCompleted += OnTaskCompleted;
        this.OnScored += OnScored;
      
        sameGameObjectDetector.Init(
            countOfObjectsForInteraction, () => InitContainerControl(OnCollisionSafe, countOfObjectsForInteraction));
    }

    private void InitContainerControl(Action<GameObject> OnCollisionSafe, int countOfObjectsForInteraction)
    {
        containerControl.Init(countOfObjectsForInteraction, InitBase, OnCollisionSafe);
    }

    private void InitBase()
    {
        if (OnInitialized != null)
        {
            OnInitialized.Invoke();
        }
    }

    public void SetTask (int score)
    {
        currentScore = score;
        scoreText.text = currentScore.ToString();

        isTaskCompleted = false;

        //print("New Task was set !: " + score);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isTaskCompleted)
        {   
            currentObject = other.gameObject;

            if (isTagCustomUsedForTriggering && currentObject.tag != tagCustomForTriggering)
            {
                return;
            }

            if (!sameGameObjectDetector.DetectOrRegister (currentObject))
            {
                Score();
            }
        }
    }

    private void Score()
    {
        if (OnScored != null)
        {
            OnScored.Invoke(currentObject);
        }

        audioSource.PlayOneShot(scoringSounds[UnityEngine.Random.Range(0, scoringSounds.Length)]);

        if (explosionOnScoring && !explosionOnScoring.activeSelf)
        {
            StartCoroutine(ShowExplosion());
        }

        if (scoreAnimator)
        {
            PlayScoringAnimation();
        }

        if (currentScore > 1)
        {
            scoreText.text = (--currentScore).ToString();
        }
        else
        {
            CompleteTask();
        }
    }

    private void PlayCompleteTaskAnimation ()
    {
         StartCoroutine (PlayCompleteTaskAnimationCoroutine(completeTaskAnimationDelay));
    }

    private IEnumerator PlayCompleteTaskAnimationCoroutine (float delay)
    {
        yield return new WaitForSeconds(delay);

        scoreAnimator.SetTrigger(completeTaskAnimationTrigger);
    }
    
    private void PlayScoringAnimation ()
    {
         StartCoroutine (PlayScoringAnimationCoroutine(scoringAnimationDelay));
    }

    private IEnumerator PlayScoringAnimationCoroutine (float delay)
    {
        yield return new WaitForSeconds(delay);

        scoreAnimator.SetTrigger(scoringAnimationTrigger);
    }

    private IEnumerator ShowExplosion()
    {
        explosionOnScoring.SetActive(true);

        yield return new WaitForSeconds(2f);

        explosionOnScoring.SetActive(false);
    }

    private void CompleteTask ()
    {
        //print("Win for this Volume!");

        PlayCompleteTaskAnimation ();

        scoreText.text = messageOnTaskCompleted;

        isTaskCompleted = true;

        if (OnTaskCompleted != null)
        {
            OnTaskCompleted.Invoke();
        }   
    }
}
