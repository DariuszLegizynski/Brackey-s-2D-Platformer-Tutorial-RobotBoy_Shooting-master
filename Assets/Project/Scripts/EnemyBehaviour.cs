/* When using a health bar for the enemy
 * 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    [System.Serializable]
    public class EnemyStats
    {
        public int maxHealth = 30;

        private int _currentHealth;
        public int currentHealth
        {
            get { return _currentHealth; }
            set { _currentHealth = Mathf.Clamp(value, 0, maxHealth); }
        }

        //sets our current health (or whatever stats are used) to the max health value at the start of the game
        public void Init()
        {
            currentHealth = maxHealth;
        }
    }

    public EnemyStats stats = new EnemyStats();

    [SerializeField]
    StatusIndicator statusIndicator;

    private void Start()
    {
        stats.Init();

        if(statusIndicator == null)
        {
            Debug.LogError("ENEMYBEHAVIOUR: No statusIndicator found on Enemy");
        }

        else
        {
            statusIndicator.SetHealth(stats.currentHealth, stats.maxHealth);
        }
    }

    public void TakeDamage(int damage)
    {
        stats.currentHealth -= damage;

        if (stats.currentHealth <= 0)
        {
            GameMaster.KillEnemy(this);
        }

        statusIndicator.SetHealth(stats.currentHealth, stats.maxHealth);
    }
}
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    [System.Serializable]
    public class EnemyStats
    {
        public int health = 30;
        public int damage = 40;
    }

    public EnemyStats enemyStats = new EnemyStats();

    public void TakeDamage(int damage)
    {
        enemyStats.health -= damage;

        if (enemyStats.health <= 0)
        {
            GameMaster.KillEnemy(this);
        }
    }

    private void OnCollisionEnter2D(Collision2D _collisionInfo)
    {
        Player _player = _collisionInfo.collider.GetComponent<Player>();

        if(_player != null)
        {
            _player.DamagePlayer(enemyStats.damage);
        }
    }
}
