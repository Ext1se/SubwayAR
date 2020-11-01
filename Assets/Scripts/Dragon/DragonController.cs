using MalbersAnimations.Controller;
using MalbersAnimations.Events;
using Michsky.UI.ModernUIPack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonController : MonoBehaviour
{
    enum DragonState
    {
        IDLE,
        READY,
        BUSY,
        DEATH
    }

    enum DragonAttack
    {
        TAIL_LEFT = 7,
        TAIL_RIGHT = 8,
        FLY_FIRE = 13,
        GROUND_FIRE = 14
    }

    enum DragonFire
    {
        FIRE_BALL = 1,
        FIRE_BREATH = 2
    }

    enum DragonAction
    {
        ROAR = 20,
        SLEEP = 6
    }

    public MAnimal Dragon;
    public ProgressBar ProgressBarHealth;
    public GameObject Parent;
    public GameObject FireBreath;

    private DragonState state = DragonState.IDLE;
    private Vector3 startPosition = new Vector3();
    private int delay = 5;
    private int attackPosition = 0;
    private int health = 100;
    private float godTime = 0.4f;
    private bool isGod = false;

    private void Awake()
    {

    }

    private void Start()
    {
        state = DragonState.IDLE;
        //StartCoroutine(CoroutineStartGame());
    }

    public void StartGame()
    {
        startPosition = Dragon.transform.position;
        StartCoroutine(CoroutineStartGame());
    }

    private IEnumerator CoroutineStartGame()
    {
        yield return new WaitForSeconds(1);
        state = DragonState.READY;
    }

    private void Update()
    {
        if (state == DragonState.READY)
        {
            StartCoroutine(CheckAttackTime());
            StartCoroutine(AttackCoroutine());
        }
    }

    private IEnumerator CheckAttackTime()
    {
        yield return new WaitForSeconds(20);
        if (state == DragonState.BUSY)
        {
            state = DragonState.IDLE;
        }
    }

    private IEnumerator AttackCoroutine()
    {
        state = DragonState.BUSY;

        Debug.Log(attackPosition);
        switch (attackPosition)
        {
            case 0:
                Attack1(DragonAttack.TAIL_LEFT);
                delay = 5;
                break;
            case 1:
                Attack1(DragonAttack.GROUND_FIRE);
                delay = 8;
                break;
            case 2:
                Attack1(DragonAttack.TAIL_RIGHT);
                delay = 5;
                break;
            case 3:
                Attack1(DragonAttack.FLY_FIRE);
                delay = 14;
                break;
            default:
                Roar();
                break;
        }
        attackPosition++;
        if (attackPosition >= 4)
        {
            attackPosition = 0;
        }
        yield return new WaitForSeconds(delay);
        state = DragonState.READY;
        ////
        //Выравнивание позиции дракона, а то анимация немного смещает позицию...
        /*
        Dragon.transform.position = new Vector3(Mathf.Round(Dragon.transform.position.x), 
            Dragon.transform.position.y, 
            Mathf.Round(Dragon.transform.position.z));
            */
        //Dragon.transform.position = startPosition;
        if (Parent != null)
        {
            Dragon.transform.position = new Vector3(Parent.transform.position.x, Dragon.transform.position.y, Parent.transform.position.z);
        }
        //////////////
    }

    IEnumerator DelayCoroutine()
    {
        yield return new WaitForSeconds(delay);
        state = DragonState.READY;
    }


    private void Attack()
    {
        Dragon.Mode_Activate(1);
    }

    private void Attack1(DragonAttack attack1)
    {
        Dragon.Mode_Activate(1, (int)attack1);
    }

    private void AttackTail()
    {
        Attack1(DragonAttack.TAIL_RIGHT);
    }

    private void Roar()
    {
        Dragon.Mode_Activate(4, (int)DragonAction.ROAR);

    }

    private void Death()
    {
        //Sleep
        //Dragon.State_Activate(10);
        //Dragon.Mode_Activate(4, (int)DragonEventActionState.SLEEP);
        state = DragonState.DEATH;
        FireBreath.gameObject.SetActive(false);
        StopAllCoroutines();
        Dragon.Mode_Disable(1);
        Dragon.Mode_Stop();
        Dragon.State_Force(10);
        Dragon.State_Disable(1);
        Dragon.State_Enable(10);
        Dragon.State_Activate(10);
    }

    public void Reset()
    {
        Dragon.State_Disable(10);
        Dragon.Mode_Enable(1);
        Dragon.State_Enable(0);
        health = 100;
        StopCoroutine(AttackCoroutine());
        //gameObject.SetActive(false);
        //transform.position = new Vector3(0, 1, 0);
        transform.rotation = Quaternion.Euler(0, 0, 0);
        state = DragonState.IDLE;
        FireBreath.gameObject.SetActive(true);
        UpdateHealth();
    }

    private IEnumerator SetGodMode()
    {
        isGod = true;
        yield return new WaitForSeconds(godTime);
        isGod = false;
    }


    public void GetDamage()
    {
        if (isGod || state == DragonState.DEATH)
        {
            return;
        }
        health -= 8;
        if (health < 0)
        {
            state = DragonState.DEATH;
            Death();
        }
        else
        {
            StartCoroutine(SetGodMode());
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
