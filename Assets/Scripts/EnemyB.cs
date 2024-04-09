using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyB : EnemyBase
{
    public enum eDirection
    {
        Down,
        Up,
        Left,
        Right,
    }

    private eDirection mCurrentDirection = eDirection.Down;
    private float mTimeInCurrentDirection = 0;
    private Vector2 DesiredDirection = Vector2.right;

    public eDirection CurrentDirection
    {
        get => mCurrentDirection;
        set
        {
            if (value != mCurrentDirection)
            {
                mTimeInCurrentDirection = 0;
            }

            mCurrentDirection = value;
        }
    }

    protected override void Update()
    {
        base.Update();

        UpdateCurrentDirection();

        // Make the direction slowly adapt to become the Desired Direction
        this.mDirection = Vector2.Lerp(this.mDirection, this.DesiredDirection, Time.deltaTime * 4f).normalized;
    }

    private void UpdateCurrentDirection()
    {
        // If still not visible, just travel down so it enters the visible area of the screen at some position
        if (!Visible)
        {
            CurrentDirection = eDirection.Down;
            DesiredDirection = Vector2.down;
            mTimeInCurrentDirection = 0;
            return;
        }

        mTimeInCurrentDirection += Time.deltaTime;

        switch (mCurrentDirection)
        {
            case eDirection.Left:
                if (this.transform.position.x < -4f)
                {
                    this.CurrentDirection = this.transform.position.y > 4.5f ? eDirection.Down : eDirection.Up;
                }
                break;
            case eDirection.Right:
                if (this.transform.position.x > 4f)
                {
                    this.CurrentDirection = this.transform.position.y > 4.5f ? eDirection.Down : eDirection.Up;
                }
                break;
            case eDirection.Up:
            case eDirection.Down:
                // Make sure the plane doesn't stay for too long in up/down directions
                if (mTimeInCurrentDirection > Random.Range(1.5f, 2.5f))
                {
                    this.CurrentDirection = this.transform.position.x > 0 ? eDirection.Left : eDirection.Right;
                }
                break;
            default:
                break;
        }

        // Update desired direction to match the CurrentDirection value
        this.DesiredDirection = TranslateDirectionToVector2(CurrentDirection);
    }

    private Vector2 TranslateDirectionToVector2(eDirection pDirection)
    {
        switch (pDirection)
        {
            case eDirection.Down:
                return Vector2.down;
            case eDirection.Up:
                return Vector2.up;
            case eDirection.Left:
                return Vector2.left;
            case eDirection.Right:
                return Vector2.right;
        }

        return Vector2.down;
    }
}
