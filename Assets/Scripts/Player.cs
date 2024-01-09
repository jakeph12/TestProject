using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public partial class Player : MonoBehaviour
{
    [SerializeField]private float _Hp;
    [SerializeField] private Text m_txHp;
    public float Hp
    {
        get => _Hp;
        set
        {
            _Hp = value;
            if (_Hp > 100) _Hp = 100;
            if (m_txHp) m_txHp.text = $"Hp:{_Hp}";
            if (_Hp <= 0) Die();
        }
    }
    public float Damage;
    public float AtackSpeed;
    public float AttackRange = 2;

    private float lastAttackTime = 0;
    private bool _isDead = false;
    private bool isDead
    {
        get => _isDead;
        set
        {
            _isDead = value;
            if (_isDead)
            {
                enabled = false;
                m_btAttack.interactable = false;
                m_btSuperAttack.interactable = false;
            }
            else
            {
                enabled = true;
                m_btAttack.interactable = true;
            }
        }
    }
    public Animator AnimatorController;

    public void Start()
    {
        if(m_btAttack) m_btAttack.onClick.AddListener(() => Attack());
        if (m_btSuperAttack) m_btSuperAttack.onClick.AddListener(() => SuperAttack());
        if(m_txHp) m_txHp.text = $"Hp:{Hp}";
    }



    private void FixedUpdate()
    {
        Move();
        var enemies = SceneManager.Instance.Enemies;



        for (int i = 0; i < enemies.Count; i++)
        {
            var enemie = enemies[i];


            if (!enemie) continue;
            var distance = Vector3.Distance(transform.position, enemie.transform.position);

            if (!m_scrCloseEnemy && distance <= AttackRange * 2) 
            {
                m_scrCloseEnemy = enemie;
                return;
            }
            if (!m_scrCloseEnemy) return;
            var closestDistance = Vector3.Distance(transform.position, m_scrCloseEnemy.transform.position);

            if (distance <= closestDistance)
            {
                m_scrCloseEnemy = enemie;
            }

        }
        if (m_scrCloseEnemy)
        {
            var closestDistance = Vector3.Distance(transform.position, m_scrCloseEnemy.transform.position);
            if (AttackRange * 2 < closestDistance)
            {
                m_scrCloseEnemy = null;
                return;
            }

            transform.transform.rotation = Quaternion.LookRotation(m_scrCloseEnemy.transform.position - transform.position);
        }
    }

    private void Die()
    {
        isDead = true;
        AnimatorController.SetTrigger("Die");

        SceneManager.Instance.GameOver();
    }


}
public partial class Player
{
    [Min(0.001f)]
    [SerializeField] private float m_flSpees = 1;
    private Enemie _m_scrCloseEnemy;
    private Enemie m_scrCloseEnemy
    {
        get => _m_scrCloseEnemy;
        set
        {
            _m_scrCloseEnemy = value;
            if (m_btSuperAttack)
            {
                if (_m_scrCloseEnemy)
                {
                    if (!m_blinCooldownSuper && !isDead)
                        m_btSuperAttack.interactable = true;
                }
                else m_btSuperAttack.interactable = false;
            }
            else
            {
                if (_m_scrCloseEnemy && !isDead) m_btSuperAttack.interactable = false;
            }
        }
    }
    private bool m_blinCooldownSuper  = false;
    [SerializeField] private Button m_btAttack,m_btSuperAttack;
    public void Move()
    {
        var vk = new Vector3((int)Input.GetAxisRaw("Horizontal"), 0, (int)Input.GetAxisRaw("Vertical")) * m_flSpees;
        transform.position += vk;
        AnimatorController.SetFloat("Speed",vk.magnitude);
    }
    public async void Attack()
    {
        m_btAttack.interactable = false;
      
        AnimatorController.SetTrigger("Attack");
        if (m_scrCloseEnemy != null)
        {
            var distance = Vector3.Distance(transform.position, m_scrCloseEnemy.transform.position);
            if (distance <= AttackRange)
            {
                if (m_scrCloseEnemy.Hp > 0)
                {
                    m_scrCloseEnemy.Hp -= Damage;
                    if (m_scrCloseEnemy.Hp <= 0) m_scrCloseEnemy = null;
                }

            }
        }
        await UniTask.Delay((int)(AtackSpeed * 1000));

        if(!isDead) m_btAttack.interactable = true;
    }
    public async void SuperAttack()
    {
        m_btSuperAttack.interactable = false;
        m_blinCooldownSuper = true;

        AnimatorController.SetTrigger("SuperAttack");
        if (m_scrCloseEnemy != null)
        {
            var distance = Vector3.Distance(transform.position, m_scrCloseEnemy.transform.position);
            if (distance <= AttackRange)
            {
                if (m_scrCloseEnemy.Hp > 0)
                {
                    m_scrCloseEnemy.Hp -= Damage * 2;
                    if (m_scrCloseEnemy.Hp <= 0) m_scrCloseEnemy = null;
                }

            }
        }

        await UniTask.Delay(2000);

        m_blinCooldownSuper = false;
        if (!isDead) m_btSuperAttack.interactable = true;
    }
}