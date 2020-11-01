using UnityEngine;

public class FireBall : MonoBehaviour
{
    public float radius = 2;
    public float life = 5f;
    public GameObject Explotion;

    void Start()
    {
        Destroy(gameObject, life);
    }

    void OnTriggerEnter(Collider other)
    {
        KnightLiteController knight = other.GetComponent<KnightLiteController>();
        if (knight != null)
        {
            knight.GetDamage();
        }

        if (other.tag.Equals("AnimalBone"))
        {
            //EnemyController dragon = other.GetComponent<EnemyController>();
            DragonController dragon = FindObjectOfType<DragonController>();
            if (dragon != null)
            {
                dragon.GetDamage();
            }
        }
        if (Explotion)
        {
            GameObject fireballexplotion = Instantiate(Explotion);
            fireballexplotion.transform.position = transform.position;
            Destroy(fireballexplotion, 2f);
        }
        Destroy(gameObject);
    }
}