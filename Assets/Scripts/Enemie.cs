using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemie : MonoBehaviour
{
    [SerializeField]private float _Hp;
    public float Hp
    {
        get => _Hp;
        set
        {
            _Hp = value;
            if (_Hp > 0) return;
            Die();
            Agent.isStopped = true;
        }
    }
    public float Damage;
    public float AtackSpeed;
    public float AttackRange = 2;


    public Animator AnimatorController;
    public NavMeshAgent Agent;

    private float lastAttackTime = 0;
    private bool _isDead = false;
    protected bool isDead
    {
        get => _isDead;
        set
        {
            _isDead = value;
            if (_isDead) enabled = false;
        }
    }


    private void Start()
    {
        SceneManager.Instance.AddEnemie(this);
        Agent.SetDestination(SceneManager.Instance.Player.transform.position);
        isDead = false;
    }

    private void FixedUpdate()
    {
        if (SceneManager.Instance.Player.Hp <= 0) enabled = false;
        var distance = Vector3.Distance(transform.position, SceneManager.Instance.Player.transform.position);
     
        if (distance <= AttackRange)
        {
            Agent.isStopped = true;
            if (Time.time - lastAttackTime > AtackSpeed)
            {
                lastAttackTime = Time.time;
                SceneManager.Instance.Player.Hp -= Damage;
                AnimatorController.SetTrigger("Attack");
            }
        }
        else
        {
            Agent.isStopped = false;
            Agent.SetDestination(SceneManager.Instance.Player.transform.position);
        }
        AnimatorController.SetFloat("Speed", Agent.speed); 

    }



    protected virtual void Die()
    {
        isDead = true;
        AnimatorController.SetTrigger("Die");
        SceneManager.Instance.RemoveEnemie(this);
    }

}
