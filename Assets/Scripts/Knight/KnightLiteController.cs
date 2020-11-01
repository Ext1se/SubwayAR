using Michsky.UI.ModernUIPack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightLiteController : MonoBehaviour
{
    enum KnightState
    {
        IDLE = 0,
        ATTACK = 1,
        DEFENCE = 2,
        HIT = 3,
        DEATH = 4,
        GOD = 5
    }

    public GameObject Knight;
    public ProgressBar ProgressBarHealth;

    private Animator animator;
    private KnightState state;
    private int health = 100;
    private float godTime = 0.6f;

    private void Start()
    {
        animator = Knight.GetComponent<Animator>();
        //state = KnightState.DEFENCE;
        state = KnightState.GOD;
    }

    private void Update()
    {

    }

    public void StartGame()
    {
        state = KnightState.DEFENCE;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Knight OnTriggerEnter = " + other.name);
        if (other.gameObject.tag.Equals("Weapon") || other.gameObject.tag.Equals("Ground"))
        {
            return;
        }
        //Костыль. Но почему-то постоянно приходит триггер от головы, хотя она и не близко
        if (other.gameObject.name.Equals("Head") || other.gameObject.name.StartsWith("Neck") || other.gameObject.name.StartsWith("Spine"))
        {
            return;
        }
        GetDamage();
    }

    private IEnumerator SetGodMode()
    {
        state = KnightState.GOD;
        yield return new WaitForSeconds(godTime);
        state = KnightState.DEFENCE;
    }

    public void Attack()
    {
        animator.SetTrigger("attack");
    }

    public void GetDamage()
    {
        if (state == KnightState.DEATH)
        {
            return;
        }
        if (state != KnightState.GOD)
        {
            health -= 10;
            if (health <= 0)
            {
                state = KnightState.DEATH;
                animator.SetTrigger("death");
            }
            else
            {
                animator.SetTrigger("hit");
                state = KnightState.HIT;
                StartCoroutine(SetGodMode());
            }
            UpdateHealth();
        }
    }

    public void GetHelp()
    {
        if (state == KnightState.DEATH)
        {
            return;
        }
        health += 10;
        if (health >= 100)
        {
            health = 99;
            return;
        }
        UpdateHealth();
    }

    private void UpdateHealth()
    {
        if (health >= 0 && health <= 100)
        {
            ProgressBarHealth.currentPercent = health;
        }
    }
}
