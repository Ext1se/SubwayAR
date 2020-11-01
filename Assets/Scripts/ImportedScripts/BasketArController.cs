using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace easyar
{
    public class BasketArController : MonoBehaviour
    {
        public ThrowControl foo;
        public RandomObjectPooler bar;
        public ImageTargetController imageTargetController;
       

        void Start()
        {
            //imageTargetController = GetComponentInParent<ImageTargetController>();
            imageTargetController.TargetFound += () =>
            {
              
                foo.enabled = true;
                bar.enabled = true;
              
            };
        }

    }
}
