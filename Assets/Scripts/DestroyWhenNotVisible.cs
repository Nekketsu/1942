using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyWhenNotVisible : MonoBehaviour
{
    private SpriteRenderer mRender;
    private bool HasBeenVisible = false;

    private void Awake()
    {
        mRender = this.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (mRender.isVisible)
        {
            HasBeenVisible = true;
        }
        else
        {
            if (HasBeenVisible)
            {
                GameObject.Destroy(this.gameObject);
            }
        }
    }
}
