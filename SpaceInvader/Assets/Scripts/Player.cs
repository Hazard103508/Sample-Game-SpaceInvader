using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Objects
    private Weapon weapon;
    private Vector2 screenBounds;

    public float speed;
    #endregion

    #region Unity Events
    void Start()
    {
        weapon = GetComponent<Weapon>();
        var spriteSize = GetComponent<SpriteRenderer>().sprite.bounds.size;
        this.screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z)) - new Vector3(spriteSize.x / 2, spriteSize.y / 2);
    }
    void Update()
    {
        Move();
        Shoot();
    }
    #endregion

    #region Methods
    /// <summary>
    /// Mueve al jugador horizontalmente en el mapa
    /// </summary>
    public void Move()
    {
        this.transform.Translate(Vector2.right * Input.GetAxis("Horizontal") * this.speed * Time.deltaTime);

        if (this.transform.position.x < -this.screenBounds.x)
            this.transform.position = new Vector3(-this.screenBounds.x, this.transform.position.y, 0);

        if (this.transform.position.x > this.screenBounds.x)
            this.transform.position = new Vector3(this.screenBounds.x, this.transform.position.y, 0);
    }
    /// <summary>
    /// Dispara una bala
    /// </summary>
    private void Shoot()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            weapon.Shoot();
    }
    #endregion
}
