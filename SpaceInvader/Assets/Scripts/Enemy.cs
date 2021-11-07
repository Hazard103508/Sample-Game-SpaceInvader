using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    #region Objects
    private Weapon weapon;
    private Animator animator;
    private EnemyState _state;

    [HideInInspector] public Vector2Int location;
    public EnemyEvent Destroyed = new EnemyEvent();
    public int lives;
    public Model model;

    public EnemyState State
    {
        get => _state;
        set
        {
            _state = value;
            if (value == EnemyState.Dying)
            {
                animator.SetTrigger("Destroy");
                Invoke("Invoke_DestroyEvent", 0.1f);
            }
        }
    }
    #endregion

    #region Unity Methods
    void Start()
    {
        State = EnemyState.Idle;
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
                this.State = EnemyState.Dying;
            else
                animator.SetTrigger("Hit");
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
    private void Invoke_DestroyEvent()
    {
        Destroyed.Invoke(this);
    }
    /// <summary>
    /// Dispara una bala al player
    /// </summary>
    private IEnumerator Shoot()
    {
        while (true)
        {
            if (this.State == EnemyState.Shooting)
                weapon.Shoot();

            yield return new WaitForSeconds(Random.Range(1f, 3f));
        }
    }
    #endregion

    #region Structures
    public enum Model
    {
        Blue,
        Red,
        Yellow,
        Green
    }
    public enum EnemyState
    {
        Idle,
        Shooting,
        Dying
    }
    public class EnemyEvent : UnityEvent<Enemy>
    {
    }
    #endregion
}

