using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyC : EnemyBase
{
    public enum eDirection
    {
        Left,
        Right
    }

    public eDirection MovementDirection = eDirection.Right;
    public float TimeToActivate = 20;

    private Vector2 DesiredDirection = Vector2.right;
    private float mRotationAngle = 0;

    protected override void Update()
    {
        base.Update();

        // Check if enough time to activate enemy
        if (mTimeLiving < TimeToActivate)
        {
            this.mRigidBody.velocity = Vector2.zero;
            return;
        }

        // Update Current Direction
        if (mTimeVisible < 1 || mTimeVisible > 3.5f)
        {
            this.DesiredDirection = this.MovementDirection == eDirection.Right ? Vector2.right : Vector2.left;
        }
        else
        {
            if (this.MovementDirection == eDirection.Right)
            {
                mRotationAngle += (360f * Time.deltaTime) / 3f;
            }
            else
            {
                mRotationAngle -= (360f * Time.deltaTime) / 3f;
            }
            this.DesiredDirection = Quaternion.Euler(0, 0, mRotationAngle) * Vector2.up;
        }

        // Make the direction slowly adapt to become the Desired Direction
        this.mDirection = Vector2.Lerp(this.mDirection, this.DesiredDirection, Time.deltaTime * 4f).normalized;
    }
}
