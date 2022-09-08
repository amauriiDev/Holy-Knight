using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rigid; AnimationController animationController; Collider2D coll;

    const float initialSpeed = 1.5f; float speed; float AxisX; float AxisY; bool canRun;

    [SerializeField] int attackCombo; const int initialAttackCombo = 0; const float time2Attack = 0.5f;
    private int life = 0; const int maxLife = 3;

    float jumpForce;
    [SerializeField] float floatHeight = 0.1f; bool canJump;
    private bool _canAttack, _isAttacking, attackAgain;

    public LayerMask groundLayer, ladderLayer;
    const float distance2 = 0.34f;  
    [SerializeField] float rayLadderDistance = 0.2f;
    private const float attackDellay = 0.4f;

    public bool CanAttack { get => _canAttack; set => _canAttack = value; }
    public bool IsAttacking { get => _isAttacking; set => _isAttacking = value; }
    bool isJumping;
    bool isClimbing;

    [SerializeField]private Transform attackPoint; 
    [SerializeField]private float radiusAttack = 0.4f; 
    [SerializeField]private LayerMask LMEnemy;

    private bool isRecoveryTime = false;

    private void Awake()
    {
        attackCombo = initialAttackCombo;
        speed = initialSpeed;

        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<CapsuleCollider2D>();
        animationController = GetComponent<AnimationController>();
    }

    void Start()
    {
        AxisX = 0;
        life = maxLife;

        canRun = true;
        canJump = true;
        CanAttack = true;
        isJumping = false;
        isClimbing = false;
        IsAttacking = false;
        attackAgain = true;
        isRecoveryTime = false;

    
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
        else if (isClimbing)
        {
            transform.position += Vector3.up * (AxisY * speed * Time.fixedDeltaTime);
        }

        //Dispara um RayCast para baixo partindo da origem do player
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, distance2, groundLayer);
        // Se NAO estiver colidindo com o chão
        if (hit.collider != null && !isJumping)
        {  // No chão

            float distance = Mathf.Abs(hit.point.y - transform.position.y);
            float heightError = floatHeight - distance;
            jumpForce = heightError - rigid.velocity.y; //A forca do pulo é proporcional ao error, menos a velocidade do objeto

            if (!canJump && !isClimbing)   // Se ele nao pode pular, quer dizer que está pulando
            {
                canJump = true;
                isJumping = true;
                rigid.AddForce(Vector2.up * jumpForce * Time.fixedDeltaTime);   // pulo
            }
        }
        else if (hit.collider != null && isJumping) // chegando no chao depois de um pulo
        {
            animationController.StopJump();
            isJumping = false;
            canJump = true;

            if (AxisX == 0)
                animationController.RunBase();
            else
                animationController.RunStart();

        }


    }
    void GroundChecker()
    {

        //Dispara um RayCast para baixo partindo da origem do player
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, distance2, groundLayer);
        // Se NAO estiver colidindo com o chão
        if (hit.collider == null)
        {
            isJumping = true;
            animationController.RunJump();
            return;
        }
        //animationController.StopJump(false);
        //!Debug.Log(hit.collider.name);
        // Calculate the distance from the surface and the "error" relative
        // to the floating height.



        // Apply the force to the rigidbody.

    }
    void OnDrawGizmosSelected()
    {
        // Draws a 5 unit long red line in front of the object
        Gizmos.color = Color.red;
        Vector3 direction = transform.TransformDirection(new Vector2(0, -distance2));
        Gizmos.DrawRay(transform.position, direction);
    }

    public void Movement(InputAction.CallbackContext context)
    {
        AxisX = context.ReadValue<float>();

        if (context.performed)
        {
            canRun = true;
            isClimbing = false;
            animationController.RunStart();
        }
        if (context.canceled)
        {
            canRun = false;
            animationController.RunBase();
        }
    }

    public void Attack(InputAction.CallbackContext context)
    {
        //SE ESTIVER PULANDO, ATAQUE AÉREO
        if (isJumping && context.started)
        {
            animationController.RunAirAttack();
        }
        //SE ESTIVER NO CHAO, ATAQUE PADRAO
        else if (context.started && !IsAttacking)
        {
            animationController.RunAttack1();
        }

        if (context.performed || context.canceled)
        {
            IsAttacking = false;
            animationController.RunBase();
        }
    }
    public void Damage(){
        Collider2D hit = Physics2D.OverlapCircle(attackPoint.position, radiusAttack, LMEnemy);

        if (hit != null)
        {
            //Debug.Log("TOME");
            hit.GetComponent<Enemy>().TakeHit();
            
        }
    }
    public void TakeHit(){

        if (!isRecoveryTime)
        {
            life--;

            if (life > 0)
            {
                StartCoroutine(RecoveryTime(1.5f));
            }
            else{
                Death();
            }
        }
    }
    private IEnumerator RecoveryTime(float time){
        isRecoveryTime = true;
        yield return new WaitForSeconds(time);
        isRecoveryTime = false;
    }
    private void Death(){
        //Debug.Log("Morri: "+ life.ToString());
    }

    public void Climb(InputAction.CallbackContext context){
        //! NAO ESTÁ FUNCIONANDO
        //TODO: FAZER FUNCIONAR
        // AxisY = context.ReadValue<float>();

        // if (context.performed && Physics2D.IsTouchingLayers(coll,ladderLayer))
        // {
        //     isClimbing = true;
        //     canJump = false;
        //     canRun = false;
        //     animationController.RunClimbLddr();
        //     Debug.Log("Escalandooo");
        // }
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

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.started && canJump)
        {
            // PULAR E NAO PERMITIR PULAR NOVAMENTE
            animationController.RunJump();
            canJump = false;
        }
    }

    IEnumerator Delay2Return(float time)
    {
        this.attackAgain = true;
        yield return new WaitForSeconds(time);
        this.attackAgain = false;
    }

    private void OnDrawGizmos() {
    
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(attackPoint.position, radiusAttack);
    }
}
