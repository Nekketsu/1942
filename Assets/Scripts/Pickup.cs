using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public enum ePickupType
    {
        POW,
        ExtraLife
    }

    public ePickupType Type = ePickupType.POW;
    public float LifeSeconds = 10;

    private void Update()
    {
        this.LifeSeconds -= Time.deltaTime;
        if (this.LifeSeconds <= 0)
        {
            this.Destroy();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.GetComponent<Player>();
        if (player != null)
        {
            player.TakePickup(this);
            this.Destroy();
        }
    }

    private void Destroy()
    {
        GameObject.Destroy(this.gameObject);
        GameManager.Player.Score += 200;
    }
}
