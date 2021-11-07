using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    #region Objects
    private Weapon weapon;
    private Animator animator;

    [HideInInspector] public Vector2Int location;
    public int lives;
    public bool canShoot;
    #endregion

    #region Unity Methods
    void Start()
    {
        animator = GetComponent<Animator>();
        weapon = GetComponent<Weapon>();

        StartCoroutine(Shoot());
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (lives > 0 && collision.CompareTag("Player_Bullet"))
        {
            collision.gameObject.SetActive(false);
            lives--;

            if (lives == 0)
                animator.SetTrigger("Die");
        }
    }
    #endregion


    #region Methods
    /// <summary>
    /// Al completarse la animacion y desaparecer la nave se detruye el objeto
    /// </summary>
    public void Die()
    {
        Destroy(gameObject);
    }
    /// <summary>
    /// Dispara una bala al player
    /// </summary>
    private IEnumerator Shoot()
    {
        while (true)
        {
            if (canShoot)
                weapon.Shoot();

            yield return new WaitForSeconds(Random.Range(1f, 3f));
        }
    }
    #endregion
}

