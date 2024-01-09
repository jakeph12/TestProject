using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class SecondEnemy : Enemie
{
    [SerializeField] private Enemie SpawningEnemy;
    protected async override void Die()
    {
        var d = Damage / 2;
        var s = transform.localScale / 2;
        for(int i = 0;i < 2; i++)
        {
            var ne = Instantiate(SpawningEnemy.gameObject);
            var e = ne.GetComponent<Enemie>();
            var posX = Random.Range(transform.position.x - 1, transform.position.x + 1);
            var posZ = Random.Range(transform.position.z - 1, transform.position.z + 1);

            ne.transform.position = new Vector3(posX, 0, posZ);
            e.Hp = 1;
            e.Damage = d;
            e.transform.localScale = s;
        }

        isDead = true;
        AnimatorController.SetTrigger("Die");

        await UniTask.Delay(500);

        SceneManager.Instance.RemoveEnemie(this);
    }
}
