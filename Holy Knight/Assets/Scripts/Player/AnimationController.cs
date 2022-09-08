using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    const string PARAM = "transition";
    const int idle = 0;
    const int run = 1;
    const int attack = 2;
    const int jump = 3;
    const int airAttack = 4;
    const int climbLadder = 5;
    Animator anim;    

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void RunStart()
    {
        anim.SetInteger(PARAM, run);
    }
    public void RunBase()
    {
        anim.SetInteger(PARAM, idle);
    }
    public void RunAttack1()
    {
        anim.SetInteger(PARAM, attack);
    }
    public void RunAirAttack()
    {
        //anim.SetInteger(PARAM, airAttack);
        anim.Play("air-attack1");
    }
    public void RunAttack2()
    {
        anim.SetInteger("attack", 1);
    }
    public void RunAttack3()
    {
        anim.SetInteger("attack", 2);
    }
    public void StopAttack()
    {
        anim.SetInteger(PARAM, idle);
        //anim.SetInteger("attack", 0);
    }
    public void RunJump()
    {
        anim.SetInteger(PARAM, jump);
        anim.SetBool("isJumping", true);
    }
    public void StopJump()
    {
        anim.SetInteger(PARAM, run);
        anim.SetBool("isJumping", false);
    }

    public void RunClimbLddr(){
        anim.SetInteger(PARAM,climbLadder);
        anim.SetBool("isClimbing", true);
    }

    public void Damage(){
        Master.Instance.playerController.Damage();
    }
}
