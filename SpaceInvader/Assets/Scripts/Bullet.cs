using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private SpriteRenderer sr;
    private Vector2 screenBounds;
    public float speed;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        this.screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z)) + new Vector3(sr.size.x / 2, sr.size.y / 2);
    }
    void Update()
    {
        this.transform.Translate(Vector3.up * this.speed * Time.deltaTime, Space.World);
        if (this.transform.position.y > screenBounds.y || this.transform.position.y < -screenBounds.y)
            this.gameObject.SetActive(false);
    }
}
