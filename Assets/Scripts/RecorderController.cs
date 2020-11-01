using easyar;
using System;
using System.IO;
using UnityEngine;

public class RecorderController : MonoBehaviour
{
    private VideoRecorder videoRecorder;
    private string filePath;
    private CameraRecorder cameraRecorder;
    private void Awake()
    {
        videoRecorder = FindObjectOfType<VideoRecorder>();
        videoRecorder.FilePathType = VideoRecorder.OutputPathType.PersistentDataPath;
        videoRecorder.StatusUpdate += (status, msg) =>
        {
            if (status == RecordStatus.OnStarted)
            {
                    //GUIPopup.EnqueueMessage("Recording start", 5);
                }
            if (status == RecordStatus.FailedToStart || status == RecordStatus.FileFailed || status == RecordStatus.LogError)
            {
                GUIPopup.EnqueueMessage("Recording Error: " + status + ", details: " + msg, 2);
            }
            Debug.Log("RecordStatus: " + status + ", details: " + msg);
        };
    }

    private void OnDestroy()
    {

    }

    public void RecorderStart()
    {
        if (!videoRecorder)
        {
            return;
        }
        if (!videoRecorder.IsReady)
        {
            return;
        }

        filePath = "EasyAR_Recording_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".mp4";
        videoRecorder.FilePath = filePath;
        videoRecorder.StartRecording();
        cameraRecorder = Camera.main.gameObject.AddComponent<CameraRecorder>();
        cameraRecorder.Setup(videoRecorder, null);
    }

    public void RecorderStop()
    {
        if (!videoRecorder)
        {
            return;
        }
        if (videoRecorder.StopRecording())
        {
            /* GUIPopup.EnqueueMessage("Recording finished, video saved to Unity Application.persistentDataPath" + Environment.NewLine +
                 "Filename: " + filePath + Environment.NewLine +
                 "PersistentDataPath: " + Application.persistentDataPath + Environment.NewLine +
                 "You can change sample code if you prefer to record videos into system album", 8);*/

            /*GUIPopup.EnqueueMessage(
               "Filename: " + filePath + Environment.NewLine +
               "Path: " + Application.persistentDataPath + Environment.NewLine, 3);*/
            AdjustVideoAndPlay();
        }
        else
        {
            GUIPopup.EnqueueMessage("Recording failed", 5);
        }
        if (cameraRecorder)
        {
            cameraRecorder.Destroy();
        }
    }

    private void AdjustVideoAndPlay()
    {
        //TODO: Костыль
        //Сохраняем в Галерею и удаляем из папки приложения
        NativeGallery.SaveVideoToGallery(Application.persistentDataPath + "/" + filePath, "NovgorodAR", filePath);
        File.Delete(Application.persistentDataPath + "/" + filePath);
    }
}