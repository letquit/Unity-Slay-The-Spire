using System;
using System.Collections;
using UnityEngine;

public class DamageSystem : MonoBehaviour
{
    [SerializeField] private GameObject damageVFX;

    private void OnEnable()
    {
        ActionSystem.AttachPerformer<DealDamageGA>(DealDamagePerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<DealDamageGA>();
    }

    private IEnumerator DealDamagePerformer(DealDamageGA dealDamageGA)
    {
        foreach (var target in dealDamageGA.Targets)
        {
            int finalDamage = dealDamageGA.ModifiedAmount;
            
            if (finalDamage > 0)
            {
                target.ApplyFinalDamage(finalDamage);
                Instantiate(damageVFX, target.transform.position, Quaternion.identity);
                yield return new WaitForSeconds(0.15f);
                
                if (target.CurrentHealth <= 0)
                {
                    //TODO: 复活
                    // if (HasReviveCondition(target))
                    // {
                    //     ReviveTarget(target);
                    // }
                    // else if (target is EnemyView enemyView)
                    if (target is EnemyView enemyView)
                    {
                        KillEnemyGA killEnemyGA = new(enemyView);
                        ActionSystem.Instance.AddReaction(killEnemyGA); 
                    }
                    else
                    {
                        // Game Over or other
                    }
                }
            }
            else
            {
                Instantiate(damageVFX, target.transform.position, Quaternion.identity);
                yield return new WaitForSeconds(0.15f);
            }
        }
    }
}
