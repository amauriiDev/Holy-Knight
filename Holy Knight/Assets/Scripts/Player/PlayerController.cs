using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rigid; AnimationController animationController;

    const float initialSpeed = 1.5f;float speed; float AxisX; bool canRun;

    [SerializeField]int attackCombo; const int initialAttackCombo = 0; const float time2Attack = 0.5f;

    float jumpForce;
    [SerializeField] float floatHeight = 0.1f; bool canJump;
    private bool _canAttack, _isAttacking, attackAgain;

    public LayerMask groundLayer;
    const float distance2 = 0.34f;
    private const float attackDellay = 0.4f;

    public bool CanAttack { get => _canAttack; set => _canAttack = value; }
    public bool IsAttacking { get => _isAttacking; set => _isAttacking = value; }


    private void Awake() {
        attackCombo = initialAttackCombo;
        speed = initialSpeed;
    }

    void Start()
    {
        AxisX = 0;

        canRun = true;
        canJump = true;
        CanAttack = true;
        IsAttacking = false;
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
            transform.position += Vector3.right * (AxisX * speed * Time.fixedDeltaTime);
        }

        //Dispara um Ray para baixo partindo da origem do player
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, distance2,groundLayer);
        // SE colidir com a layer 'ground'
        if (hit.collider != null)
        {
            //!Debug.Log(hit.collider.name);
            // Calculate the distance from the surface and the "error" relative
            // to the floating height.
            float distance = Mathf.Abs(hit.point.y - transform.position.y);
            //!PRINTDebug.Log(distance);
            float heightError = floatHeight - distance;

            //proporcional ao error menos a velocidade do objeto
            jumpForce = heightError - rigid.velocity.y;

            if (!canJump)
            {
                canJump= true;
                rigid.AddForce(Vector2.up * jumpForce * Time.fixedDeltaTime);
            }
                // Apply the force to the rigidbody.
        }
    }
    void OnDrawGizmosSelected()
    {
        // Draws a 5 unit long red line in front of the object
        Gizmos.color = Color.red;
        Vector3 direction = transform.TransformDirection(new Vector2(0,-distance2));
        Gizmos.DrawRay(transform.position, direction);
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
            animationController.RunBase();
        }
    }
    
    public void Atack(InputAction.CallbackContext content)
    {
        if (content.started && !IsAttacking)
        {
            animationController.RunAttack1();
        }
 
        if (content.performed || content.canceled)
        {
            IsAttacking = false;
            animationController.RunBase();
        }
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
        if (content.started && canJump)
        {
            // PULAR E NAO PERMITIR PULAR NOVAMENTE
            animationController.RunJump();
            canJump = false;
        }
        if (content.canceled)
        {
            // SE SOLTAR O BOTAO É PERMITIDO PULAR NOVAMENTE
            //TODO: remover essa opção
            canJump = true;
            animationController.StopJump();
        }
    }

    IEnumerator Delay2Return(float time){
        this.attackAgain = true;
        yield return new WaitForSeconds(time);
        this.attackAgain = false;
    }
}
