using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.FPS.Game;
public class DamageZoneTrigger : MonoBehaviour
{
    [SerializeField] private float m_DamageAmount;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var damagable = other.GetComponent<Damageable>();
            if (damagable != null)
            {
                damagable.InflictDamage(m_DamageAmount, false, this.gameObject);
            }
        }
    }
}
