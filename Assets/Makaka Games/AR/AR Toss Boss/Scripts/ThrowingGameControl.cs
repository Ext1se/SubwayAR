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

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using TMPro;

#pragma warning disable 649

[HelpURL("https://makaka.org/unity-assets")]
public class ThrowingGameControl : MonoBehaviour
{
    [SerializeField]
	private Button buttonStart;
    
    [SerializeField]
	private FailControl failControl;

    [Header("Throwing")]
    [SerializeField]
	private ThrowControl throwControl;
    
    [SerializeField]
	private ThrowingObject.SoundSettings floorSounds = new ThrowingObject.SoundSettings();
    
    [SerializeField]
	private ThrowingObject.SoundSettings containerSounds = new ThrowingObject.SoundSettings();

    private Transform cameraMain;
    
    [Header("Arrow")]
    [SerializeField]
	private bool isArrowEnabled = true;
    
    [SerializeField]
	private Transform arrow;
    private Vector3 arrowDirection;
    private Vector3 arrowDirectionLocalEulerAngles = new Vector3(0f, 180f, 0f);

    [Header("Task")]
    [SerializeField]
    [Range(1, 10)]
	private int minTask = 1;
    
    [SerializeField]
    [Range(1, 10)]
	private int maxTask = 5;

    [Header("Score")]
    [SerializeField]
	private ScoreBestControl scoreBestControl;
    
    [SerializeField]
	private ScoreCurrentControl scoreCurrentControl;
    
    [SerializeField]
    private ScoreControl[] scoreControls;
    private ScoreControl scoreControlTemp;
    private ScoreControl scoreControlsTempCurrent;

    [Header("Events")]
    [SerializeField]
	private UnityEvent OnUnityStart;
    
    [SerializeField]
	private UnityEvent OnInitialized;
    
    private int initedScoreControls = 0;
	private int scoreControlsValidCount = 0;
    private int maxNumberOfAttemptsToSetTaskForNewScoreControl = 11;

    private bool isGameStarted = false;

    void Start ()
    {
        OnUnityStart.Invoke();

        arrow.gameObject.SetActive(false);
        
        buttonStart.onClick.AddListener(StartGame);
        buttonStart.onClick.AddListener(throwControl.GetFirstThrow);

        throwControl.OnInitialized.AddListener(InitScoreControls);
        throwControl.gameObject.SetActive (true);
    }

    private void InitScoreControls ()
    {
        cameraMain = throwControl.cameraMain.transform;
        
        for (int i = 0; i < scoreControls.Length; i++)
        {
            scoreControlTemp = scoreControls [i];

            if (scoreControlTemp)
            {
                scoreControlsValidCount++;
            }
        }

        for (int i = 0; i < scoreControls.Length; i++)
        {
            scoreControlTemp = scoreControls [i];

            if (scoreControlTemp)
            {
                scoreControlTemp.Init (
                    throwControl.GetObjectCount(), 
                    CountInitedScoreControl,
                    SetNextTask,
                    AddScore,
                    TouchContainer);
            }
        }
    }

    private void CountInitedScoreControl()
	{
		initedScoreControls++;

		//Debug.Log("Inited: #" + var);

		if (initedScoreControls == scoreControlsValidCount)
		{
            failControl.Init(
                throwControl.GetObjectCount(), 
                InitStartButton, 
                TouchFailPlane,
                Fail);

			//Debug.Log("All Items Inited");
		}
	}

    private void InitStartButton()
    {
        OnInitialized.Invoke();

        buttonStart.interactable = true;
    }
    
    private void Update ()
    {
        if (isGameStarted && isArrowEnabled)
        {
            if (!arrow.gameObject.activeSelf)
            {
                 arrow.gameObject.SetActive(true);
                 return;
            }

            SetArrowDirection ();
        }
    }

    private void StartGame ()
    {
         SetNextTask();

         isGameStarted = true;
    }

    private void TouchContainer (GameObject touchingObject)
    {
        throwControl.PlayRandomSoundDependingOnSpeed (containerSounds, touchingObject);
    }

    private void TouchFailPlane (GameObject failedObject)
    {
        throwControl.PlayRandomSoundDependingOnSpeed (floorSounds, failedObject);
    }

    private void Fail (GameObject failedObject)
    {
        scoreCurrentControl.Reset();

        throwControl.SetColor (failControl.failColor, failedObject);
    }

    private void AddScore (GameObject scoredObject)
    {
        // Memorize Time of Scoring
        failControl.RegisterAfterScoring(scoredObject);

        scoreCurrentControl.Add(1);

        //print(scoreCurrent);   

        if (scoreCurrentControl.GetValue () > scoreBestControl.GetValue())
        {   
            scoreBestControl.SaveAndShow (scoreCurrentControl.GetValue());

            //print(scoreBest);   
        }
    }

    private void SetArrowActive(bool value)
    {
        isArrowEnabled = value;

        arrow.gameObject.SetActive(value);
    }

    private void SetArrowDirection ()
    {
        if (scoreControlsTempCurrent)
        {
            arrowDirection = cameraMain.InverseTransformPoint(scoreControlsTempCurrent.transform.parent.position);
            
            arrowDirectionLocalEulerAngles.z =
                Mathf.Atan2(arrowDirection.x, arrowDirection.z) * Mathf.Rad2Deg + arrowDirectionLocalEulerAngles.y;
            
            arrow.localEulerAngles = arrowDirectionLocalEulerAngles;
        }
    }

    private void SetNextTask ()
    {
        for (int i = 0; i <= maxNumberOfAttemptsToSetTaskForNewScoreControl; i++)
        {
            scoreControlTemp = scoreControls[Random.Range (0, scoreControls.Length)];
            
            if (i < maxNumberOfAttemptsToSetTaskForNewScoreControl)
            {
                if (scoreControlTemp && !MonoBehaviour.ReferenceEquals(scoreControlTemp, scoreControlsTempCurrent))
                {
                    SetTaskTo(scoreControlTemp);

                    scoreControlsTempCurrent = scoreControlTemp;

                    return;
                }
            }
            else if (scoreControlsTempCurrent)
            {
                SetTaskTo(scoreControlsTempCurrent);

                //print ("Set Task for Previous Score Control");
            }
            else
            {
                print("Oops. No more Score Controls for Task Setting");
            }
        }
    }

    private void SetTaskTo (ScoreControl scoreControl)
    {
        scoreControl.SetTask (Random.Range (minTask, maxTask + 1));
    }

}
