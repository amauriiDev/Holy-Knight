using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    // Integers and our values
    private const string PARAM = "transition";
    private const int idle = 0, run = 1, attack = 2;

    //Triggers
    private const string death = "isDeath", takeHit = "takeHit";
    Animator _anim;
    void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    
    public void IdleAnimation(){
        _anim.SetInteger(PARAM, idle);
    }
    public void RunAnimation(){
        _anim.SetInteger(PARAM, run);
    }
    public void AttackAnimation(){
        _anim.SetInteger(PARAM, attack);
    }
    public void Attack(){
        //!Bad pratice
        GetComponentInParent<Enemy>().Attack();
    }

    public void DeathAnimation(){
        _anim.SetTrigger(death);
    }
    public void TakeHitAnimation(){
        _anim.SetTrigger(takeHit);
    }
}
