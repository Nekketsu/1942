using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShot : MonoBehaviour
{
    protected SpriteRenderer mRender;

    public float Speed = 10;
    public GameObject ShotVanishAnimation;
    public bool DoubleShot;

    [Header("Sprites")]
    public Sprite ShotNormal;
    public Sprite ShotDouble;

    private void Awake()
    {
        mRender = this.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (DoubleShot)
        {
            mRender.sprite = ShotDouble;
        }
        else
        {
            mRender.sprite = ShotNormal;
        }

        this.transform.position += new Vector3(0f, Speed * Time.deltaTime, 0f);
    }

    public void Destroy()
    {
        GameObject.Destroy(this.gameObject);

        if (ShotVanishAnimation != null)
        {
            GameObject.Instantiate(ShotVanishAnimation, this.transform.position, Quaternion.identity);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        this.Destroy();
    }
}
