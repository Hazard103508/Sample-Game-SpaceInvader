using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector2 screenBounds;
    public float speed;
    public bool destroy;

    private void Start()
    {
        var spriteSize = GetComponent<SpriteRenderer>().sprite.bounds.size;
        this.screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z)) + new Vector3(spriteSize.x / 2, spriteSize.y / 2);
    }
    void Update()
    {
        this.transform.Translate(Vector3.up * this.speed * Time.deltaTime, Space.World);
        if (this.transform.position.y > screenBounds.y || this.transform.position.y < -screenBounds.y)
        {
            if (destroy)
                Destroy(this.gameObject);
            else
                this.gameObject.SetActive(false);
        }
    }
}
