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
using UnityEngine.Events;

/// <summary>
/// Control script to operate with all throwing objects.
/// </summary>
/// <remarks>
/// Unity Events are not effective. 
/// I use them to show you clearly in Unity Editor, where you can insert your code.
/// If you want more performance, you need to use C# Events: https://jacksondunstan.com/articles/3335 .
/// <para />
/// Because of coroutines behavior, it is impossible to place some methods inside ThrowingObject class.
/// OOP does not work in this case, but we have a stable throwing with Object Pool.
/// </remarks>
[HelpURL("https://makaka.org/unity-assets")]
[AddComponentMenu ("Scripts/Makaka Games/Throw Control/Throw Control")]
public class ThrowControl : MonoBehaviour 
{
	public RandomObjectPooler randomObjectPooler;
	public UnityEvent OnInitialized;

	private List<ThrowingObject> throwingObjectsRegistered;
	private ThrowingObject throwingObjectTempForRegistration;

	[Header("FPS (throw force takes into account the speed of the player's movement) ")]
	public CharacterController characterControllerFPS;
	private float characterControllerFPSSpeedCurrent = 0f;

	[Header("Camera")]
	public Camera cameraMain;

	[Header("Mode")]
	public Mode mode = Mode.ClickOrTap;
	
	public enum Mode
	{
		Flick
		,
		ClickOrTap
	}

	private bool isTouchForFlick = false;

	[Tooltip("If it's false then it allows fast flicks only." 
	+ "\n\nPositions in the last and previous frames are taken into account." 
	+ "\n\nPlay with params: sensivity = new Vector2(100f, 100f); force = 45f.")]
	public bool isFullPathForFlick = true;
	public float lerpTimeFactorOnTouchForFlick = 20f;

	[Header("Throw")]
	[Tooltip("Actual for FPS Controller" )]
	public bool isInputPositionFixed = false;

	[Range(0.01f, 1f)]
	public float inputPositionFixedScreenFactorX = 0.48f;

	[Range(0.01f, 1f)]
	public float inputPositionFixedScreenFactorY = 0.52f;
	public Vector2 inputSensitivity = new Vector2(1f, 100f);

	public float forceFactorExtra = 10f;
	public float torqueFactorExtra = 60f;
	public float torqueAngleExtra;
	
    public Transform parentOnThrow;

	public UnityEvent OnThrow;

	[Header("Next Throw")]
	[Range(0.1f, 10f)]
	public float nextThrowGettingDelay = 0.1f;

	/// <summary> Seconds for next try of coroutine call (min = 0.1f)</summary>
	private const float nextCoroutineCallDelay = 0.1f;
    private bool isNextThrowGetting;

	private GameObject gameObjectTemp;
	private ThrowingObject throwingObjectTemp;

	public UnityEvent OnNextThrowGetting;

	[Header("Tag")]
	public bool isTagCustomSetOnInit = false;

	[TagSelector]
    public string tagCustomOnInit = TagSelectorAttribute.Untagged;

	[Header("Layer Changing (actual for quick Throwing to neutralize mutual collisions)")]
    public bool isLayerChangingOn = true;
	
	public LayerMask layerMaskOnThrow;
	public LayerMask layerMaskOnReset;

	[Range(0f, 5f)]
	public float layerChangingDelay = 1f;

	private int layerIndexOnThrow;
	private int layerIndexOnReset;

	[Header("Reset (must be called after the end of Fade Out)")]
	[Range(0f, 10f)]
	public float resetDelay = 4f;
	public UnityEvent OnReset;

	private GameObject gameObjectCurrent;
	private ThrowingObject throwingObjectCurrent;

	private RaycastHit raycastHit;

	private bool isInputBegan = false;
	private bool isInputEnded = false;
	private bool isInputHeldDown = false;

    private Vector3 inputPositionCurrent;
	private Vector3 inputPositionPivot;

	[Header("Fade Out (must be completed before Reset)")]
	public bool isFadeOutOn = true;

	[Range(0f, 10f)]
	public float fadeOutDelay = 3f;

	[Range(0f, 1f)]
	public float fadeOutSpeed = 0.03f;

	public UnityEvent OnFadeOut;

	// ---------
	// DEBUGGING
	// ---------
	
	// #if DEBUG
	// private NumberDebugger positionDebugger = new NumberDebugger();
	// #endif

    /// <summary>Call after pool initialisation.</summary>
    public void InitThrowingObjects()
    {
		StartCoroutine(InitThrowingObjectsCoroutine());
	}

	/// <summary>Init physics correctly.</summary>
	private IEnumerator InitThrowingObjectsCoroutine()
	{
		if (randomObjectPooler)
		{
			if (cameraMain)
			{
				InitLayerIndexes ();

				throwingObjectsRegistered = new List<ThrowingObject> ();
			
				for (int i = 0; i < randomObjectPooler.pooledObjects.Count; i++)
				{
					gameObjectTemp = randomObjectPooler.pooledObjects[i];

					if (gameObjectTemp)
					{
						gameObjectTemp.SetActive(true);

						throwingObjectTemp = RegisterOrGetThrowingObject(gameObjectTemp);
						throwingObjectTemp.ResetPosition(cameraMain);
						throwingObjectTemp.ResetRotation(randomObjectPooler.poolParent);
						throwingObjectTemp.SetRendererEnabled(false);

						if (isTagCustomSetOnInit)
						{
							throwingObjectTemp.tag = tagCustomOnInit;
						}

						yield return new WaitForFixedUpdate ();

						StartCoroutine (Reset (nextCoroutineCallDelay, throwingObjectTemp));
						
						yield return new WaitForFixedUpdate ();
					}
				}

				yield return new WaitForFixedUpdate ();
                Debug.Log("Finished Init");
				OnInitialized.Invoke ();
			}
			else
			{
				Debug.LogError("Camera Main is Null. Assign it in the Editor.");
			}
		}
		else
		{
			Debug.LogError("Random Object Pooler is Null. Assign it in the Editor.");
		}
    }

	public void GetFirstThrow()
	{
		GetNextThrow(nextCoroutineCallDelay);
	}

	void Update() 
	{
		if (!isNextThrowGetting && gameObjectCurrent && throwingObjectCurrent && !throwingObjectCurrent.isThrown)
		{	
			#if UNITY_EDITOR || UNITY_STANDALONE

				isInputBegan = Input.GetMouseButtonDown(0);
				isInputEnded = Input.GetMouseButtonUp(0);
				isInputHeldDown = Input.GetMouseButton(0);

				inputPositionCurrent = 
					isInputPositionFixed 
					? GetInputPositionFixed() 
					: Input.mousePosition;

			#elif UNITY_IOS || UNITY_ANDROID

				if (Input.touchCount == 1)
				{
					isInputBegan = Input.GetTouch(0).phase == TouchPhase.Began;
					isInputEnded = Input.GetTouch(0).phase == TouchPhase.Ended;
					isInputHeldDown = true;

					inputPositionCurrent = 
						isInputPositionFixed 
						? GetInputPositionFixed() 
						: (Vector3) Input.GetTouch (0).position;
				}
				else
				{
					return;
				}

			#else

				Debug.LogWarning("Indicate your platform here for platform dependent compilation.");

			#endif

			if (isTouchForFlick)
			{
				//print("isTouchForFlick");
				
				StartCoroutine(OnTouchForFlick ());
			}

			//print("isThrown check: " + isThrown);
		
			if (isInputBegan)
			{
				//print("isInputBegan");

				if (Physics.Raycast (cameraMain.ScreenPointToRay (inputPositionCurrent), out raycastHit, 100f)) 
				{
					if (mode == Mode.Flick) 
					{
						if (raycastHit.rigidbody == throwingObjectCurrent.rigidbody3D)
						{
							isTouchForFlick = true;

							if(isFullPathForFlick)
							{
								inputPositionPivot = inputPositionCurrent;
							}
						}
						else // If click or tap outside of Throwing Game Object when Flick Mode => No Throw
						{
							if(isFullPathForFlick)
							{
								inputPositionPivot = Vector3.zero;
							}
						}
					}
				}
			}

			// Next Update()
			if(isInputEnded)
			{
				//print("isInputEnded");

				if (mode == Mode.Flick && inputPositionPivot != Vector3.zero) 
				{
					//print("Mode.Flick => Throw()");

					throwingObjectCurrent.isThrown = true;

					StartCoroutine(Throw (inputPositionPivot, inputPositionCurrent, throwingObjectCurrent));

					isTouchForFlick = false;
				}
			}

			if(isInputHeldDown)
			{
				//print("isInputHeldDown");

				//It allows fast flicks only. See ToolTip for isFullPathForFlick.
				if (mode == Mode.Flick && !isFullPathForFlick)
				{
					inputPositionPivot = inputPositionCurrent;
				}
				else if (mode == Mode.ClickOrTap && inputPositionPivot.y < inputPositionCurrent.y)
				{
					//print("Mode.ClickOrTap => Throw()");

					inputPositionPivot = 
						cameraMain.ViewportToScreenPoint(throwingObjectCurrent.positionInViewportOnReset);
					
					throwingObjectCurrent.isThrown = true;

					StartCoroutine(Throw (inputPositionPivot, inputPositionCurrent, throwingObjectCurrent));
				}
			}
		}
	}

	private Vector3 GetInputPositionFixed()
	{
		return new Vector3(
			Screen.width * inputPositionFixedScreenFactorX,
			Screen.height * inputPositionFixedScreenFactorY,
			0f);
	}

	private IEnumerator OnTouchForFlick() 
	{
		yield return new WaitForEndOfFrame();

		inputPositionCurrent.z = cameraMain.nearClipPlane * throwingObjectCurrent.cameraNearClipPlaneFactorOnReset;

		throwingObjectCurrent.rigidbody3D.position = Vector3.Lerp (
			throwingObjectCurrent.rigidbody3D.position, 
			cameraMain.ScreenToWorldPoint (inputPositionCurrent), 
			Time.fixedDeltaTime * lerpTimeFactorOnTouchForFlick
		);
	}

	private IEnumerator Throw(
		Vector2 inputPositionFirst, 
		Vector2 inputPositionLast, 
		ThrowingObject throwingObject)
    {
		throwingObject.transform.parent = parentOnThrow;	

		if (mode == Mode.ClickOrTap)
		{
			yield return new WaitForFixedUpdate();

			throwingObject.SetCollidersEnabled(true);

			//print (throwingObject.gameObject.name + " : SetCollidersEnabled(true)");
		}

		yield return new WaitForFixedUpdate();

		if (isLayerChangingOn)
		{
			StartCoroutine(ChangeLayer(layerChangingDelay, throwingObject.gameObject, layerIndexOnThrow));
		}

		if (characterControllerFPS)
		{
			characterControllerFPSSpeedCurrent = 
				characterControllerFPS.transform.InverseTransformDirection(characterControllerFPS.velocity).z;

				//print(characterControllerFPSSpeedCurrent);
		}

		//print (characterControllerFPSSpeedCurrent);

        throwingObject.ThrowBase(
			inputPositionFirst, 
			inputPositionLast, 
			inputSensitivity, 
			cameraMain.transform, 
			Screen.height,
			forceFactorExtra + characterControllerFPSSpeedCurrent,
			torqueFactorExtra,
			torqueAngleExtra);

		//print (throwingObject.gameObject.name + " : IEnumerator Throw()");

		if (isFadeOutOn)
        {
            StartCoroutine(throwingObject.FadeOut(fadeOutDelay, fadeOutSpeed, OnFadeOut));
        }

		// Wait for physics changing
		yield return new WaitForFixedUpdate();

		throwingObject.PlayWhooshSound();

        OnThrow.Invoke();

		StartCoroutine(Reset(resetDelay, throwingObject));

		GetNextThrow(nextThrowGettingDelay);
    }

    public void DirtyReset(ThrowingObject throwObj)
    {       
        StartCoroutine(Reset(0, throwObj));
    }


    private IEnumerator Reset(float delay, ThrowingObject throwingObject)
	{       
            yield return new WaitForSeconds(delay);
            yield return new WaitForFixedUpdate();

            if (isLayerChangingOn)
            {
                ChangeLayerNow(throwingObject.gameObject, layerIndexOnReset);
            }

            throwingObject.isThrown = false;
            throwingObject.ResetPhysicsBase();

            if (mode == Mode.ClickOrTap)
            {
                throwingObject.SetCollidersEnabled(false);

                //print (throwingObject.gameObject.name + " : SetCollidersEnabled(false)");
            }
            else
            {
                throwingObject.ActivateTriggersOnColliders(true);

                //print (throwingObject.gameObject.name + " : ActivateTriggersOnColliders(true)");
            }

            throwingObject.SetRendererEnabled(false);

            yield return new WaitForFixedUpdate();

            throwingObject.ResetPosition(cameraMain);
            throwingObject.ResetRotation(randomObjectPooler.poolParent);

            yield return new WaitForFixedUpdate();

            if (isFadeOutOn)
            {
                throwingObject.ResetFade();
            }

            throwingObject.transform.parent = randomObjectPooler.poolParent;
            throwingObject.gameObject.SetActive(false);

            OnReset.Invoke();
       
	}

	private void GetNextThrow(float delay)
	{	
		isNextThrowGetting = true;
		StartCoroutine(GetNextThrowCoroutine(delay));
	}

	private IEnumerator GetNextThrowCoroutine(float delay)
	{	
		gameObjectCurrent = null;
		throwingObjectCurrent = null;	

		yield return new WaitForSeconds(delay);

		gameObjectTemp = randomObjectPooler.GetPooledObject();

		if (gameObjectTemp)
        {
			gameObjectTemp.SetActive(true);

            throwingObjectTemp = RegisterOrGetThrowingObject(gameObjectTemp);
            throwingObjectTemp.ResetPosition(cameraMain);
            //throwingObjectTemp.ResetPosition();
            throwingObjectTemp.ResetRotation(randomObjectPooler.poolParent);

			//print(throwingObjectTemp.rigidbody3D.rotation.eulerAngles);

            if (mode == Mode.Flick)
            {
                yield return new WaitForFixedUpdate();

                throwingObjectTemp.ActivateTriggersOnColliders(false);

                //print (throwingObjectTemp.name + " : ActivateTriggersOnColliders(false)");
            }

            yield return new WaitForFixedUpdate();

        	throwingObjectTemp.SetRendererEnabled(true);

            // ---------------------
			// DEBUGGING OF POSITION
			// ---------------------

			// #if DEBUG
			// positionDebugger.DebugFloatAbsChanging(2f, throwingObjectTemp.rigidbody3D.position.x);
			// #endif

            throwingObjectCurrent = throwingObjectTemp;
            gameObjectCurrent = gameObjectTemp;

            isNextThrowGetting = false;

            OnNextThrowGetting.Invoke();
        }
        else
		{
			//print ("GetNextThrowBase() => false");

			StartCoroutine(GetNextThrowCoroutine(nextCoroutineCallDelay));
		}
	}

	public int GetObjectCount()
	{
		return randomObjectPooler.pooledObjects.Count;
	}

    /// <summary>For initial registration and subsequent getting ThrowingObject component</summary>
    public ThrowingObject RegisterOrGetThrowingObject(GameObject gameObject)
    {
		throwingObjectTempForRegistration = null;

		// Search of cached ThrowingObject 
		for (int i = 0; i < throwingObjectsRegistered.Count; i++)
		{
			throwingObjectTempForRegistration = throwingObjectsRegistered[i];

			if (throwingObjectTempForRegistration)
			{
				if (throwingObjectTempForRegistration.gameObject == gameObject)
				{	
					//print(i);
					
					break;
				}
				else
				{
					throwingObjectTempForRegistration = null;
				}
			}
			else // Game Object is null
			{
				throwingObjectsRegistered.RemoveAt(i);

				//print("Remove null ThrowingObject from List");
			}
		}

		if (!throwingObjectTempForRegistration)
		{
			throwingObjectTempForRegistration = gameObject.GetComponent<ThrowingObject>();

			if (throwingObjectTempForRegistration)
			{
				throwingObjectsRegistered.Add(throwingObjectTempForRegistration);
			}
			
			//print("Register New ThrowingObject");
		}

		return throwingObjectTempForRegistration;
    }

	public void PlayRandomSoundDependingOnSpeed(ThrowingObject.SoundSettings audioSettings, GameObject to)
	{
		ThrowingObject throwingObjectTemp = RegisterOrGetThrowingObject(to);

		if (throwingObjectTemp)
		{
			throwingObjectTemp.PlayRandomSoundDependingOnSpeed(audioSettings);
		}
	}

	public void SetColor (Color color, GameObject to)
	{
		ThrowingObject throwingObjectTemp = RegisterOrGetThrowingObject(to);

        if (throwingObjectTemp)
        {
            throwingObjectTemp.SetColor(color);
        }
	}

	private IEnumerator ChangeLayer(float delay, GameObject to, int layerIndex)
	{
		yield return new WaitForSeconds(delay);

		ChangeLayerNow(to, layerIndex);
	}

	private void ChangeLayerNow(GameObject to, int layerIndex)
	{
		to.layer = layerIndex;

		//print(layerIndex);
	}

	private int LayerMaskValueToIndex(int value)
	{
		return Mathf.RoundToInt(Mathf.Log(value, 2));
	}

    private void InitLayerIndexes()
    {
        layerIndexOnThrow = LayerMaskValueToIndex(layerMaskOnThrow.value);
        layerIndexOnReset = LayerMaskValueToIndex(layerMaskOnReset.value);
    }

	#if DEBUG
	public void TestEvent(int i)
	{
		Debug.Log("Event Call: " + i + ", " + System.DateTime.Now.TimeOfDay);
	}
	#endif
}