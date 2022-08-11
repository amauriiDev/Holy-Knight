using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rigid;
    AnimationController animationController;
    
    const float speed = 1.5f;
    float AxisX;

    bool canRun;

    private int attackCombo;

    void Start()
    {
        attackCombo = 0;
        AxisX = 0;

        canRun = true;

        rigid = GetComponent<Rigidbody2D>();
        animationController = GetComponent<AnimationController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (AxisX != 0)
        {
            transform.localScale = new Vector3(AxisX,1,1);
        }
    }

    void FixedUpdate() {

        if(canRun)
        {
            transform.position += new Vector3(AxisX * speed * Time.fixedDeltaTime,0,0);
        }
    }

    public void Movement(InputAction.CallbackContext content){
        AxisX = content.ReadValue<float>();

        if (content.performed){
            canRun = true;
            animationController.RunStart();
        }
        if (content.canceled){
            canRun = false;
            animationController.RunStop();
        }
    }
    public void Atack(InputAction.CallbackContext content){
        if (content.performed){
            animationController.RunAttack();
        }
        if (content.canceled){
            canRun = false;
            animationController.StopAttack();
        }
    }
    
}
