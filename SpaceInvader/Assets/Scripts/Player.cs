using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    private List<Bullet> bullets;
    private int bulletIndex;
    private float speed = 10f;

    public int bulletCount;

    void Start()
    {
        bullets = new List<Bullet>();
        for (int i = 0; i < bulletCount; i++)
        {
            var obj = Instantiate(bulletPrefab, this.transform.parent);
            obj.SetActive(false);
            bullets.Add(obj.GetComponent<Bullet>());
        }
        this.enabled = false;
    }


    void Update()
    {
        Move();
        Shoot();
    }

    /// <summary>
    /// Mueve al jugador horizontalmente en el mapa
    /// </summary>
    public void Move()
    {
        this.transform.Translate(Vector2.right * Input.GetAxis("Horizontal") * this.speed * Time.deltaTime);
    }
    /// <summary>
    /// Dispara una bala
    /// </summary>
    private void Shoot()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var bullet = this.bullets[bulletIndex];
            if (!bullet.gameObject.activeSelf)
            {
                bullet.gameObject.SetActive(true);
                bullet.transform.position = this.transform.position;

                bulletIndex++;
                if (bulletIndex == this.bullets.Count)
                    bulletIndex = 0;
            }
        }
    }
}
