using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EnemyShot : MonoBehaviour
{
    [HideInInspector]
    public Vector2 Direction = Vector2.up;
    public float Speed = 10;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        var player = collider.GetComponent<Player>();
        if (player != null && !player.IsGodMode)
        {
            player.HitByShot();
            this.Destroy();
        }
    }

    public void Destroy()
    {
        GameObject.Destroy(this.gameObject);
    }

    public void Update()
    {
        this.transform.position = (Vector2)this.transform.position + ((Direction * Speed) * Time.deltaTime);
    }
}
