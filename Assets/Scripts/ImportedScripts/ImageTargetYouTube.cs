using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace easyar
{
    public class ImageTargetYouTube : MonoBehaviour
{
        public YoutubePlayer player;
        ImageTargetController imageTargetController;
        
        
    // Start is called before the first frame update
    void Start()
    {
            imageTargetController = GetComponentInParent<ImageTargetController>();
            imageTargetController.TargetFound += () =>
            {               
                player.Play();
                if(player.startFromSecond)
                player.videoPlayer.time = player.startFromSecondTime;
            };
        }

    // Update is called once per frame
    void Update()
    {

    }
}
}
