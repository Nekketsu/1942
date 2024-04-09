using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyA : EnemyBase
{
    public enum eState
    {
        GoingDown,
        GoingUp,
        Turning
    }

    public eState State = eState.GoingDown;

    public GameObject Propeller;

    [Header("Sprites")]
    public Sprite SpriteNormal;
    public Sprite SpriteUpsideDown;

    public float MovementX = 0.15f;
    public float TimeToTurn = 4;

    protected override void Awake()
    {
        base.Awake();
        Propeller = this.transform.Find("Propeller").gameObject;
    }

    protected override void Start()
    {
        // Initially set state as going down
        this.ChangeState(eState.GoingDown);

        base.Start();
    }

    /// <summary>
    /// Called when the plane state needs to change
    /// </summary>
    /// <param name="goingDown"></param>
    private void ChangeState(eState pState)
    {
        this.State = pState;

        switch (this.State)
        {
            case eState.GoingDown:
                // Set still sprite and disable animation
                mRender.sprite = SpriteNormal;
                mAnimator.enabled = false;

                // Make it point downwards and calculate direction of movement
                this.transform.rotation = Quaternion.Euler(0, 0, 180);

                // Enable propeller visuals
                this.Propeller.SetActive(true);
                break;
            case eState.GoingUp:
                // Set still sprite and disable animation
                mRender.sprite = SpriteNormal;
                mAnimator.enabled = false;

                // Make it point upwards and calculate direction of movement
                this.transform.rotation = Quaternion.Euler(0, 0, 0);

                // Enable propeller visuals
                this.Propeller.SetActive(true);
                break;
            case eState.Turning:
                // Enable animation
                mAnimator.enabled = true;

                // Make it point downwards and calculate direction of movement
                this.transform.rotation = Quaternion.Euler(0, 0, 180);

                // Disable propeller visual while turning
                this.Propeller.SetActive(false);
                break;
        }
    }

    protected override void Update()
    {
        base.Update();

        switch (this.State)
        {
            case eState.GoingDown:
                // Check if it's time to turn
                if (mTimeVisible > TimeToTurn)
                {
                    this.ChangeState(eState.Turning);
                }

                mDirection = new Vector2(Visible ? MovementX : 0, -1);
                break;
            case eState.GoingUp:
                mDirection = new Vector2(MovementX, 1);
                break;
            case eState.Turning:
                // Grab animation state info
                var currentState = mAnimator.GetCurrentAnimatorStateInfo(0);

                // Check if the animation is finished to switch state, by checking if the animation normalizedTime
                var playbackTime = currentState.normalizedTime;
                if (playbackTime >= 1)
                {
                    this.ChangeState(eState.GoingUp);
                }
                else
                {
                    // If the turn animation is not finished, adapt movement to make the plane graduatelly
                    // Get the playbacktime in the 0..1 range
                    var playbackTime01 = currentState.normalizedTime % 1;

                    // Make the plane slow down until reaching the midpoint, and then gain speed again
                    mDirection = new Vector2(MovementX, Mathf.Clamp(playbackTime01 - 0.5f, -1f, 1f));
                }
                break;
            default:
                break;
        }
    }
}
