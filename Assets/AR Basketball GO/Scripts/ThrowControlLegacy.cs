using UnityEngine;

public class ThrowControlLegacy : MonoBehaviour 
{
    public GameObject parentGame;
	public Vector2 sensivity = new Vector2(8f, 100f);

	public float speed = 5f;
	public float resetBallAfterSeconds = 5f;
	public float lerpTimeFactorOnTouch = 7f;
	public float cameraNearClipPlaneFactor = 7.5f;

	public bool isThrowBackAvailable = false;
    public Transform possibleParent;

	// if (isFullPathThrow == false)
	// sensivity = new Vector2(100f, 100f);
	// speed = 45f;
	public bool isFullPathThrow = true;

	private Vector3 direction;

	private Vector3 inputPositionCurrent;
	private Vector2 inputPositionPivot;
	private Vector2 inputPositionDifference;

	private Vector3 newBallPosition;
	private BallControl ballControl;
	private Rigidbody _rigidbody;
	private RaycastHit raycastHit;

	private bool isThrown; 
	private bool isHolding;

	private bool isInputBegan = false;
	private bool isInputEnded = false;
	private bool isInputLast = false;

    public void ChangeSense(float f)
    {
        sensivity = new Vector2(sensivity.x, sensivity.y + f);
    }


	void Start() 
	{
		_rigidbody = GetComponent<Rigidbody> ();
		ballControl = GetComponent<BallControl>();

		Reset ();
        Invoke("Reset", 0);
    }

	void Update() 
	{
		#if UNITY_EDITOR

			isInputBegan = Input.GetMouseButtonDown(0);
			isInputEnded = Input.GetMouseButtonUp(0);
			isInputLast = Input.GetMouseButton(0);

			inputPositionCurrent = Input.mousePosition;

		#else

			isInputBegan = Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began;
			isInputEnded = Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended;
			isInputLast = Input.touchCount == 1;

			inputPositionCurrent = Input.GetTouch (0).position;

		#endif

		if (isHolding)
			OnTouch ();

		if (isThrown)
			return;
			
		if (isInputBegan)
		{
			if (Physics.Raycast (Camera.main.ScreenPointToRay (inputPositionCurrent), out raycastHit, 100f)) 
			{
				if (raycastHit.transform == transform) 
				{
					isHolding = true;
					transform.SetParent (null);

					if(isFullPathThrow)
					{
						inputPositionPivot = inputPositionCurrent;
					}
				}
			}
		}

		if(isInputEnded)
		{
			if (isThrowBackAvailable) 
			{
				Throw (inputPositionCurrent);
			}
			else
			{
				if(inputPositionPivot.y < inputPositionCurrent.y)
				{ 
					Throw (inputPositionCurrent);
				}
			}
		}

		if(isInputLast && !isFullPathThrow) 
		{
			inputPositionPivot = inputPositionCurrent;
		}

        gameObject.SetActive(parentGame.activeInHierarchy);
        
    }

	void Reset()
	{
		CancelInvoke ();

		transform.position = Camera.main.ViewportToWorldPoint (
			new Vector3 (0.5f, 0.3f, 1f)
		);
		
		newBallPosition = transform.position;

		isThrown = isHolding = false;

		_rigidbody.useGravity = false;
		_rigidbody.velocity = Vector3.zero;
		_rigidbody.angularVelocity = Vector3.zero;

		transform.rotation = Quaternion.Euler (0f, 200f, 0f);
		transform.SetParent (Camera.main.transform);
	}

	void OnTouch() 
	{
		inputPositionCurrent.z = 1f;

		newBallPosition = Camera.main.ScreenToWorldPoint (inputPositionCurrent);

		transform.localPosition = Vector3.Lerp (
			transform.localPosition, 
			newBallPosition, 
			Time.deltaTime * lerpTimeFactorOnTouch
		);
	}


    void Throw(Vector2 inputPosition) 
	{
		ballControl.SetThrown();

		_rigidbody.useGravity = true;

		inputPositionDifference.y = (inputPosition.y - inputPositionPivot.y) / Screen.height * sensivity.y;

		inputPositionDifference.x = (inputPosition.x - inputPositionPivot.x) / Screen.width;
		inputPositionDifference.x = 
			Mathf.Abs (inputPosition.x - inputPositionPivot.x) / Screen.width * sensivity.x * inputPositionDifference.x;

		direction = new Vector3 (inputPositionDifference.x, 0f, 1f);
		direction = Camera.main.transform.TransformDirection (direction);

		_rigidbody.AddForce((direction + Vector3.up) * speed * inputPositionDifference.y);

		isHolding = false;
		isThrown = true;

		Invoke ("Reset", resetBallAfterSeconds);
	}
}