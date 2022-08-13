using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rigid; AnimationController animationController;

    const float speed = 1.5f; float AxisX; bool canRun;

    [SerializeField]int attackCombo; const int initialAttackCombo = 0; const float time2Attack = 0.5f;

    [SerializeField] float jumpForce = 2.0f; bool isJumping;
    private bool _canAttack, _attackPressed, attackAgain;
    private const float attackDellay = 0.4f;

    public bool CanAttack { get => _canAttack; set => _canAttack = value; }
    public bool AttackPressed { get => _attackPressed; set => _attackPressed = value; }

    void Start()
    {
        attackCombo = initialAttackCombo;
        AxisX = 0;

        canRun = true;
        isJumping = false;
        CanAttack = true;
        AttackPressed = false;
        attackAgain = true;

        rigid = GetComponent<Rigidbody2D>();
        animationController = GetComponent<AnimationController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (AxisX != 0)
        {
            transform.localScale = new Vector3(AxisX, 1, 1);
        }
    }

    void FixedUpdate()
    {
        if (canRun)
        {
            transform.position += new Vector3(AxisX * speed * Time.fixedDeltaTime, 0, 0);
        }

        if (isJumping)
        {
            rigid.AddForce(new Vector2(0, jumpForce * Time.fixedDeltaTime));
        }
    }

    public void Movement(InputAction.CallbackContext content)
    {
        AxisX = content.ReadValue<float>();

        if (content.performed)
        {
            canRun = true;
            animationController.RunStart();
        }
        if (content.canceled)
        {
            canRun = false;
            animationController.RunStop();
        }
    }
    
    public void Atack(InputAction.CallbackContext content)
    {
        if (content.performed)
        {
            Debug.Log(attackAgain);
            StartCoroutine(Delay2Return(time2Attack));
            if (CanAttack && attackAgain)
            {
                CanAttack = false;
                AttackPressed = true;
                animationController.RunAttack1();
            }
        }
        //? OLD CODE
        // if (content.performed && attackCombo == 0)
        // {
        //     attackCombo++;
        //     StartCoroutine(ComboAttack());
        // }
        // else if (content.performed && attackCombo < 4)
        // {
        //     attackCombo++;
        // }
        // // else
        // // {
        // //     animationController.StopAttack();
        // //     attackCombo = initialAttackCombo;
        // // }
    }

    public void AttackInputManager(){
        CanAttack = !CanAttack;
    }

    //TODO: REMOVER E COLOCAR NO ANIMATION
    // Method called by attack2 animation
    public void ResetComboAttack()
    {
        attackCombo = initialAttackCombo;
    }
    IEnumerator ComboAttack()
    {
        float counter = 0;
        while (counter < time2Attack)
        {
            counter += Time.deltaTime;

            //Wait for a frame so that Unity doesn't freeze
            //Check if a second attack will be executed
            if (attackCombo > 1)
            {
                animationController.RunAttack2();
                float counter_2 = 0;
                while (counter_2 < time2Attack)
                {
                    counter_2 += Time.deltaTime;

                    //Wait for a frame so that Unity doesn't freeze
                    //Check if a third attack will be executed
                    if (attackCombo > 2)
                    {
                        animationController.RunAttack3();
                        yield break;
                    }
                    // else
                    // {
                    //     animationController.StopAttack();
                    // }
                }
                attackCombo = initialAttackCombo;
                yield break;
            }
            
            yield return null;
        }
        animationController.StopAttack();
        attackCombo = initialAttackCombo;
    }

    public void Jump(InputAction.CallbackContext content)
    {
        if (content.performed)
        {
            isJumping = true;
        }
        if (content.canceled)
        {

            isJumping = false;
            
        }
    }

    IEnumerator Delay2Return(float time){
        this.attackAgain = true;
        yield return new WaitForSeconds(time);
        this.attackAgain = false;
    }
}
