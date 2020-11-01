using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

	public static GameController data;

    [SerializeField]
    private bool panelStartOn;
    [SerializeField]
    private bool panelVictoryOn;
    [SerializeField]
    private bool panelGamveOverOn;

    public GameObject ball;

    public GameObject panelStart;
	public GameObject panelVictory;
	public GameObject panelGameOver;

	public bool isPlaying;
	public enum State {InGame, Paused, Complete, StartUp}
	public State gameState;

	public UnityEvent OnGameStart;
	public UnityEvent OnGameComplete;
	private float timeScale = 1.5f;
	
	void Awake () 
	{
		data = this;
		Time.timeScale = timeScale;
		gameState = State.StartUp;

		ShowStartPanel();
	}

	void ShowStartPanel()
	{ 
        //if(panelStartOn)
		//panelStart.SetActive(true); 
	}

	void HideStartPanel()
	{
		if(panelStart.activeInHierarchy)
		{
			panelStart.SetActive(false);
		}
	}

	public void StartPlay()
	{
		isPlaying = true;
		gameState = State.InGame;

		HideStartPanel();

		OnGameStart.Invoke();
	}

    void OnEnable()
    {        
        ball.transform.SetParent(Camera.main.transform);
        ball.SetActive(true);
    }
  

    void Complete()
	{
		isPlaying = false;
		gameState = State.Complete;
		SoundController.data.playGameOver();


		OnGameComplete.Invoke();
	}

	public void Victory()
	{
		if (gameState != State.Complete) 
		{
			Complete();
            if (panelVictoryOn)
                panelVictory.SetActive(true);
		}
	}

	public void GameOver()
	{
		if (gameState != State.Complete) 
		{
			Complete();
            if (panelGamveOverOn)
                panelGameOver.SetActive(true);
		}
	}
	
	public void Restart()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}
}
