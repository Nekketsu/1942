using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D mRigidBody;
    private SpriteRenderer mSpriteRenderer;
    private GameObject mPropeller;

    [Header("Prefabs")]
    public GameObject PlayerShotPrefab;
    public GameObject DeathAnimationPrefab;

    [Header("Sounds")]
    public AudioClip PickupSound;
    public AudioClip ShotSound;
    public AudioClip DieSound;

    /// <summary>
    /// Direction of movement, in 2D
    /// </summary>
    private Vector2 mDirection;
    /// <summary>
    /// Speed in X and Y
    /// </summary>
    public Vector2 PlaneSpeed = new Vector2(5f, 10f);

    [Header("Sprites")]
    public Sprite SpriteNormal;
    public Sprite SpriteTurn01;
    public Sprite SpriteTurn02;
    public Sprite SpriteTurn03;

    public int Lives = 3;
    public int Score = 0;
    public bool DoubleWeapon = false;

    private float mTimeDying = 0;
    private float mTimeInGodMode = 0f;
    private float mTimeTurning;
    private bool mFlipSprite;

    public bool IsDying => mTimeDying > 0;
    public bool IsGodMode => mTimeInGodMode > 0;


    private void Awake()
    {
        GameManager.Player = this;

        mPropeller = this.transform.Find("Propeller").gameObject;
        mRigidBody = this.GetComponent<Rigidbody2D>();
        mSpriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    public void HitByShot()
    {
        this.Die();
    }

    public void HitByPlane()
    {
        this.Die();
    }

    public void Die()
    {
        // Do not die twice, if we are already dying
        if (IsDying || IsGodMode)
        {
            return;
        }

        // Decrease number of lives (and do nothing by now, this will be evaluated with the death animation
        this.Lives--;

        // Set god mode for the next 5 seconds;
        this.mTimeInGodMode = 5;

        // Allow some time for the death animation to play
        mTimeDying = 0.75f;

        // Play death animation and audio
        if (DeathAnimationPrefab != null)
        {
            GameObject.Instantiate(DeathAnimationPrefab, this.transform.position, Quaternion.identity);
            AudioSource.PlayClipAtPoint(this.DieSound, this.transform.position, 1f);
        }
    }

    public void ShowHide(bool pVisible)
    {
        this.mSpriteRenderer.enabled = pVisible;
        this.mPropeller.SetActive(pVisible);
    }

    private void Fire()
    {
        GameObject newGO = GameObject.Instantiate(this.PlayerShotPrefab);
        newGO.transform.position = this.transform.position;
        PlayerShot shot = newGO.GetComponent<PlayerShot>();
        shot.DoubleShot = this.DoubleWeapon;

        AudioSource.PlayClipAtPoint(ShotSound, this.transform.position, 1f);
    }

    public void TakePickup(Pickup pPickup)
    {
        AudioSource.PlayClipAtPoint(PickupSound, this.transform.position, 1f);

        switch (pPickup.Type)
        {
            case Pickup.ePickupType.POW:
                DoubleWeapon = true;
                break;
            case Pickup.ePickupType.ExtraLife:
                this.Lives++;
                break;
        }
    }

    private void Update()
    {
        InputResponse();
        UpdatePhysics();
        UpdateGraphics();
    }

    private void InputResponse()
    {
        mDirection = Vector2.zero;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            mDirection += Vector2.up;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            mDirection += Vector2.down;
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            Fire();
        }

        // Update direction and flip of sprite while we keep pressing left or right
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            mDirection += Vector2.left;
            mFlipSprite = true;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            mDirection += Vector2.right;
            mFlipSprite = false;
        }

        // Reset time turning each time we hit the turn key for the first time (GetKeyDown)
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            mTimeTurning = 0;
        }
    }

    private void UpdatePhysics()
    {
        mRigidBody.velocity = new Vector2(mDirection.x * PlaneSpeed.x, mDirection.y * PlaneSpeed.y);

        // Keep track of how long we've been turning
        if (Mathf.Abs(mRigidBody.velocity.x) > 1)
        {
            mTimeTurning += Time.deltaTime;
            mTimeTurning = Mathf.Min(mTimeTurning, 0.5f);
        }
        else
        {
            mTimeTurning -= Time.deltaTime * 2f;
            mTimeTurning = Mathf.Min(mTimeTurning, 0f);
        }
    }

    private void UpdateGraphics()
    {
        // Make player blink while in god mode
        if (IsGodMode)
        {
            mTimeInGodMode -= Time.deltaTime;
            mTimeInGodMode = Mathf.Max(0f, this.mTimeInGodMode);

            // Make the player blink while it's in GodMode
            mSpriteRenderer.enabled = ((int)(mTimeInGodMode * 10f)) % 2 == 0;
        }
        else
        {
            mSpriteRenderer.enabled = true;
        }


        // Update sprite rendered, based on amount of time turning
        if (mTimeTurning >= 0.5f)
        {
            mSpriteRenderer.sprite = this.SpriteTurn03;
        }
        else if (mTimeTurning > 0.25f)
        {
            mSpriteRenderer.sprite = this.SpriteTurn02;
        }
        else if (mTimeTurning > 0)
        {
            mSpriteRenderer.sprite = this.SpriteTurn01;
        }
        else
        {
            mSpriteRenderer.sprite = this.SpriteNormal;
        }

        // Check if we are turning left, to flip the sprites horizontally
        mSpriteRenderer.flipX = mFlipSprite;
    }
}
