using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Rigidbody2D _rigid; EnemyAnimation _animation; CapsuleCollider2D _collider;

    private int _life = 0; const int maxLife = 30;

    //Debug inspector
    [SerializeField] private float rangeCapsuleCast = 1.3f;
    private const float maxDistanceChase = 2.0f;
    private const float distance2Attack = 0.42f;
    [SerializeField] private LayerMask LMPlayer;
    [SerializeField] private Transform attackPoint;
    private const float radiusAttack = 0.27f;

    RaycastHit2D _playerHit;

    private bool isChasing = false;
    private bool canAttack = false;
    private float _speed = 0; const float initialSpeed = 1.0f;

    void Awake()
    {
        _rigid = GetComponent<Rigidbody2D>();
        _collider = GetComponent<CapsuleCollider2D>();
        _animation = GetComponentInChildren<EnemyAnimation>();

        _life = maxLife;
        _speed = initialSpeed;
    }

    void FixedUpdate()
    {

        if (DetectedPlayer())
        {
            Walking();
            if (canAttack)
            {
                canAttack = false;
                _animation.AttackAnimation();
            }
            else if (isChasing)
            {
                ChasePlayer();
            }
        }
        else
        {
            _rigid.velocity = Vector2.zero;
            _animation.IdleAnimation();
        }
    }
    
    private void ChasePlayer()
    {
        if (_playerHit.collider == null)
        {
            return;
        }

        float distance = _playerHit.transform.position.x - transform.position.x;

        this.transform.localScale = (distance > 0)
            ? new Vector3(1, 1, 1)
            : new Vector3(-1, 1, 1);

        if (Math.Abs(distance) < distance2Attack)
        {
            _rigid.velocity = Vector2.zero;
            isChasing = false;
            canAttack = true;
        }
        else
        {
            transform.position += new Vector3(distance * _speed * Time.fixedDeltaTime, 0, 0);
        }
    }

    bool DetectedPlayer()
    {
        _playerHit = Physics2D.CircleCast(this.transform.position, rangeCapsuleCast, transform.forward, Mathf.Infinity, LMPlayer);

        if (_playerHit.collider == null)
        {
            return false;
        }
        isChasing = true;
        float distance = _playerHit.transform.position.x - transform.position.x;

        return (Mathf.Abs(distance) < maxDistanceChase);
    }
    private void Walking()
    {
        _animation.RunAnimation();
    }

// Method called by Animation Event
    public void Attack()
    {
        Collider2D hit = Physics2D.OverlapCircle(attackPoint.position, radiusAttack, LMPlayer);

        if (hit != null)
        {
            //Debug.Log("TOME");
            Master.Instance.playerController.TakeHit();
        }
    }
    public void TakeHit()
    {
        _life--;
        if (_life > 0)
        {
            _animation.TakeHitAnimation();
            StartCoroutine(StabTime(0.5f));
        }
        else
        {
            _animation.DeathAnimation();
            _rigid.velocity = Vector2.zero;
            _speed = 0;
            isChasing = false;
            Invoke("Death", 1f);
        }
    }
    private IEnumerator StabTime(float time)
    {
        canAttack = false;
        _speed = 0;
        yield return new WaitForSeconds(time);
        _speed = initialSpeed;
    }
    private void Death()
    {
        Destroy(this.gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(this.transform.position, rangeCapsuleCast);
        Gizmos.DrawWireSphere(attackPoint.position, radiusAttack);
    }
}
