using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    #region Objects
    private Animator animator;
    private EnemyState _state;
    private Vector2 screenBounds;

    public EnemyEvent Destroyed = new EnemyEvent();
    public int lives;
    public Model model;

    /// <summary>
    /// Estado actual del enemigo
    /// </summary>
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

    /// <summary>
    /// Numero de fila que tiene la nave en la grilla
    /// </summary>
    public int Row { get; set; }
    /// <summary>
    /// Numero de columna que tiene la nave en la grilla
    /// </summary>
    public int Column { get; set; }
    #endregion

    #region Unity Methods
    void Start()
    {
        State = EnemyState.Idle;
        animator = GetComponent<Animator>();

        var spriteSize = GetComponent<SpriteRenderer>().sprite.bounds.size;
        this.screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z)) - new Vector3(spriteSize.x / 2, spriteSize.y / 2);
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
    /// <summary>
    /// Determina si la nave se puede mover horizonalmente
    /// </summary>
    /// <param name="displacement"></param>
    /// <returns></returns>
    public bool Can_Move(float displacement)
    {
        Vector3 newPos = this.transform.position + (Vector3.right * displacement);
        return newPos.x > -this.screenBounds.x && newPos.x < this.screenBounds.x;
    }
    private void Invoke_DestroyEvent()
    {
        Destroyed.Invoke(this);
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
        Dying
    }
    public class EnemyEvent : UnityEvent<Enemy>
    {
    }
    #endregion
}

