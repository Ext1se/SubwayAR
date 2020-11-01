using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBallCollision : MonoBehaviour
{
    public ParticleSystem particle;
    public AudioSource source;
    public ThrowControl control;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "ThrowingObject")
        {
            collision.transform.position = transform.position;
            source.Play();
            particle.Play();
            Coins.CoinsCount++;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        other.transform.position = transform.position;
        source.Play();
        particle.Play();
        Coins.CoinsCount++;
    }
}
