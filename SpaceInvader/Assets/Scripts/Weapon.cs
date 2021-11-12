using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    private List<Bullet> bullets;
    private int bulletIndex;

    public int ammunition;
    public float bulletSpeed;

    #region Unity Events
    void Start()
    {
        Load_Bullets();
    }
    private void OnDestroy()
    {
        bullets.ForEach(bullet =>
        {
            if (bullet.gameObject.activeSelf)
                bullet.destroy = true; // al destruir el componente marco las balas para que se destruyan al salir de la pantalla
            else
                Destroy(bullet.gameObject); // destruyo las balas inactivas
        });
    }
    #endregion

    #region Methods
    /// <summary>
    /// crea las intancias de las balas de la nave
    /// </summary>
    private void Load_Bullets()
    {
        var folder = GameObject.Find("Bullets");

        bullets = new List<Bullet>();
        for (int i = 0; i < ammunition; i++)
        {
            var obj = Instantiate(bulletPrefab, folder.transform);
            obj.SetActive(false);
            Bullet bullet = obj.GetComponent<Bullet>();
            bullet.speed = bulletSpeed;

            bullets.Add(bullet);
        }
    }

    /// <summary>
    /// Dispara una bala
    /// </summary>
    public bool Shoot(Vector3 startPosition)
    {
        var bullet = this.bullets[bulletIndex];
        if (!bullet.gameObject.activeSelf)
        {
            bullet.gameObject.SetActive(true);
            bullet.transform.position = startPosition;

            bulletIndex++;
            if (bulletIndex == this.bullets.Count)
                bulletIndex = 0;

            return true;
        }

        return false;
    }
    #endregion
}
