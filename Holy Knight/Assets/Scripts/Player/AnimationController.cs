using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    const string PARAM = "transition";
    const int idle = 0;
    const int run = 1;
    const int attack = 2;
    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void RunStart(){
        anim.SetInteger(PARAM, run);
    }
    public void RunStop(){
        anim.SetInteger(PARAM, idle);
    }
    public void RunAttack(){
         anim.SetInteger(PARAM, attack);
    }
    public void StopAttack(){
         anim.SetInteger(PARAM, idle);
    }
}
