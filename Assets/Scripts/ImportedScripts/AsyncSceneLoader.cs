//This script lets you load a Scene asynchronously. It uses an asyncOperation to calculate the progress and outputs the current progress to Text (could also be used to make progress bars).

//Attach this script to a GameObject
//Create a Button (Create>UI>Button) and a Text GameObject (Create>UI>Text) and attach them both to the Inspector of your GameObject
//In Play Mode, press your Button to load the Scene, and the Text changes depending on progress. Press the space key to activate the Scene.
//Note: The progress may look like it goes straight to 100% if your Scene doesn’t have a lot to load.

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class AsyncSceneLoader : MonoBehaviour
{
    public TextMeshProUGUI m_Text;
    public float Delay = 0.0f;
    
    void Start()
    {
        //Call the LoadButton() function when the user clicks this Button
        //m_Button.onClick.AddListener(LoadButton);
        //StartCoroutine(LoadSceneCoroutine());
        //SceneManager.LoadScene("SampleScene");
        StartCoroutine(LoadSceneCoroutine());
    }   



    public void LoadScene()
    {
        StartCoroutine(LoadSceneCoroutine());
    }

    IEnumerator LoadSceneCoroutine()
    {
        yield return null;

        //Begin to load the Scene you specify
        yield return new WaitForSeconds(Delay);
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("Main");
        //Don't let the Scene activate until you allow it to 
        
        Debug.Log("Pro :" + asyncOperation.progress);
        bool tempbool = true;
      
        //When the load is still in progress, output the Text and progress bar
        while (!asyncOperation.isDone)
        {            
            if (asyncOperation.progress > 0.48 && tempbool)
            {
                tempbool = false;
               
            }
            //Output the current progress
            m_Text.SetText("Загрузка\n" + (Mathf.Round(asyncOperation.progress * 100f)) + "%");            
           

            yield return null;
        }
        asyncOperation.allowSceneActivation = true;


        //asyncOperation.allowSceneActivation = true;
    }
}