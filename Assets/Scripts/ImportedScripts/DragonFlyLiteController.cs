using MalbersAnimations.Controller;
using MalbersAnimations.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonFlyLiteController : MonoBehaviour
{
    public MAnimal Dragon;
    public MEvent SetFly;
    public MEvent SetAttack2;

    private bool m_RandomAttack = false;
    private int m_Delay = 5;
    private int m_AttackPosition = 0;
    private DragonEventState m_DragonEventState;

    void Start()
    {
        //m_DragonEventState = DragonEventState.IDLE;
        m_DragonEventState = DragonEventState.READY;
        //StartCoroutine(DelayCoroutine());
        //Dragon.State_Activate(6);
        SetFly.Invoke(true);
    }

    void Update()
    {
        if (m_DragonEventState == DragonEventState.READY)
        {
            //SetAttack2.Invoke(2);
            //SetAttack2.Invoke(true);
            //StartCoroutine(AttackCoroutine());

            //Dragon.Mode_Activate(2, (int)DragonAttack2.FIRE_BREATH);
            //m_DragonEventState = DragonEventState.BUSY;
            //StartCoroutine(FireCoroutine());
        }
    }


    IEnumerator FireCoroutine()
    {
        yield return new WaitForSeconds(1);
        Dragon.Mode_Disable(2);
        yield return new WaitForSeconds(14);
        Dragon.Mode_Enable(2);
        m_DragonEventState = DragonEventState.READY;
    }

    IEnumerator AttackCoroutine()
    {
        m_DragonEventState = DragonEventState.BUSY;
        if (m_RandomAttack)
        {
            Attack();
        }
        else
        {
            Debug.Log(m_AttackPosition);
            switch (m_AttackPosition)
            {
                case 0:
                    Attack1(DragonAttack1.TAIL_LEFT);
                    m_Delay = 5;
                    break;
                case 1:
                    Attack1(DragonAttack1.GROUND_FIRE);
                    m_Delay = 8;
                    break;
                case 2:
                    Attack1(DragonAttack1.TAIL_RIGHT);
                    m_Delay = 5;
                    break;
                case 3:
                    Attack1(DragonAttack1.FLY_FIRE);
                    m_Delay = 14;
                    break;
                default:
                    Roar();
                    break;
            }
            m_AttackPosition++;
            if (m_AttackPosition >= 4)
            {
                m_AttackPosition = 0;
            }
            /*
            int randomAttack = Random.Range(0, 6);
            switch (randomAttack)
            {
                case 0:
                    Attack1(DragonAttack1.TAIL_LEFT);
                    break;
                case 1:
                    Attack1(DragonAttack1.TAIL_RIGHT);
                    break;
                case 2:
                    Attack1(DragonAttack1.FLY_FIRE);
                    break;
                case 3:
                    Attack1(DragonAttack1.GROUND_FIRE);
                    break;
                case 4:
                    Roar();
                    break;
            }
            */
        }
        yield return new WaitForSeconds(m_Delay);
        m_DragonEventState = DragonEventState.READY;
        Dragon.transform.position = new Vector3(0, Dragon.transform.position.y, 0);
    }

    IEnumerator DeathCoroutine()
    {
        Dragon.State_Activate(6);
        yield return new WaitForSeconds(4);
        Dragon.State_Activate(3);
    }

    IEnumerator DelayCoroutine()
    {
        yield return new WaitForSeconds(m_Delay);
        m_DragonEventState = DragonEventState.READY;
    }


    private void Attack()
    {
        Dragon.Mode_Activate(1);
    }

    private void Attack1(DragonAttack1 attack1)
    {
        Dragon.Mode_Activate(1, (int)attack1);
    }

    private void AttackTail()
    {
        Attack1(DragonAttack1.TAIL_RIGHT);
    }

    private void AttackFireFly()
    {
        Attack1(DragonAttack1.FLY_FIRE);
    }

    private void AttackFireGround()
    {
        Attack1(DragonAttack1.GROUND_FIRE);
    }

    private void AttackFireBreath()
    {
        Dragon.Mode_Activate(2, (int)DragonAttack2.FIRE_BREATH);
    }

    private void Roar()
    {
        Dragon.Mode_Activate(4, (int)DragonEventActionState.ROAR);

    }

    private void Death()
    {
        Dragon.State_Activate(10);
        //Dragon.Mode_Activate(4, (int)DragonEventActionState.SLEEP);
    }

    public void Reset()
    {
        StopCoroutine(AttackCoroutine());
        //gameObject.SetActive(false);
        //transform.position = new Vector3(0, 1, 0);
        //transform.rotation = Quaternion.EulerAngles(0, 0, 0);
        m_DragonEventState = DragonEventState.READY;
        Roar();
        //gameObject.SetActive(true);
    }

    enum DragonAttack1
    {
        TAIL_LEFT = 7,
        TAIL_RIGHT = 8,
        FLY_FIRE = 13,
        GROUND_FIRE = 14
    }

    enum DragonAttack2
    {
        FIRE_BALL = 1,
        FIRE_BREATH = 2
    }

    enum DragonEventState
    {
        IDLE,
        READY,
        BUSY
    }

    enum DragonEventActionState
    {
        ROAR = 20,
        SLEEP = 6,
        HIT,
        DEATH
    }
}
