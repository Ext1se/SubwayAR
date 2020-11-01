using UnityEngine;

namespace MalbersAnimations
{
    public class HealthBall : MonoBehaviour
    {
        public float radius = 2;
        public float life = 10f;
        public GameObject explotion;

        void Start()
        {
            Destroy(gameObject, life);
        }

        void OnTriggerEnter(Collider other)
        {
            KnightLiteController knight = other.GetComponent<KnightLiteController>();
            if (knight != null)
            {
                knight.GetHelp();
            }

            Destroy(gameObject);
            if (explotion)
            {
                GameObject fireballexplotion = Instantiate(explotion);
                fireballexplotion.transform.position = transform.position;
                Destroy(fireballexplotion, 2f);
            }
        }
    }
}