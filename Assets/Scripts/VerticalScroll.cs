using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalScroll : MonoBehaviour
{
    public float ScrollSpeed = 1f;

    private void Update()
    {
        this.transform.position += new Vector3(0, -ScrollSpeed * Time.deltaTime, 0);
    }
}
