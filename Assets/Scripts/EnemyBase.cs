using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    protected Animator mAnimator;
    protected SpriteRenderer mRender;
    protected Rigidbody2D mRigidBody;

    protected float mTimeLiving;
    protected float mTimeVisible;
    protected float mTimeForNextShot = 0;

    protected bool Visible;
    protected Vector2 mDirection = new Vector2(0.1f, -1);

    [Header("Properties")]
    public float Speed;
    public int ShotsRemaining = 0;
    public float ShotInterval = 3;
    public bool AlwaysLookForward = false;
    public int NumHitsToDie = 1;
    public int ScoreWhenDies = 100;

    [Header("Sounds")]
    public AudioClip SoundHitNoDie;
    public AudioClip SoundDie;

    [Header("Prefabs")]
    public GameObject ShotPrefab;
    public GameObject DeathAnimationPrefab;
    public GameObject PickupPrefab;


    protected virtual void Awake()
    {
        mAnimator = this.GetComponent<Animator>();
        mRender = this.GetComponent<SpriteRenderer>();
        mRigidBody = this.GetComponent<Rigidbody2D>();
    }

    protected virtual void Start()
    {
        // Randomize the time when the first shot will appear (if any)
        mTimeForNextShot = Random.Range(0, ShotInterval);
    }

    protected void OnTriggerEnter2D(Collider2D collider)
    {
        var player = collider.GetComponent<Player>();
        if (player != null)
        {
            player.HitByPlane();
        }
        var playerShot = collider.GetComponent<PlayerShot>();
        if (playerShot != null)
        {
            this.HitByPlayerShot(playerShot);
        }
    }

    public virtual void HitByPlayerShot(PlayerShot pShot)
    {
        if (pShot.DoubleShot)
        {
            NumHitsToDie -= 2;
        }
        else
        {
            NumHitsToDie--;
        }

        if (NumHitsToDie <= 0)
        {
            AudioSource.PlayClipAtPoint(SoundDie, this.transform.position, 1f);

            NumHitsToDie = 0;
            this.Destroy();

            GameManager.Player.Score += this.ScoreWhenDies;
        }
        else
        {
            AudioSource.PlayClipAtPoint(SoundHitNoDie, this.transform.position, 1f);
        }
    }

    public virtual void Destroy()
    {
        GameObject.Destroy(this.gameObject);

        if (DeathAnimationPrefab != null)
        {
            Debug.Log("Death");
            GameObject.Instantiate(DeathAnimationPrefab, this.transform.position, Quaternion.identity);

            if (PickupPrefab != null)
            {
                Debug.Log("Pickup");
                GameObject.Instantiate(PickupPrefab, this.transform.position, Quaternion.identity);
            }
        }
    }

    public void Fire()
    {
        ShotsRemaining--;

        // Instantiate new shot and assign the position of the enemy plane
        var newGO = GameObject.Instantiate(this.ShotPrefab);
        newGO.transform.position = this.transform.position;

        // Calculate a direction towards player
        var shot = newGO.GetComponent<EnemyShot>();
        shot.Direction = (GameManager.Player.transform.position - this.transform.position).normalized;
    }

    protected virtual void Update()
    {
        // keep track of amount of time flying
        mTimeLiving += Time.deltaTime;

        // Check if the enemy is already in the visible part of the screen, and update time counter if visible
        Visible = mRender.isVisible;
        if (Visible)
        {
            mTimeVisible += Time.deltaTime;
        }

        // Update rigidbody velocity
        this.mRigidBody.velocity = mDirection * this.Speed;

        // Check if we need to fire
        if (this.Visible && ShotsRemaining > 0)
        {
            mTimeForNextShot -= Time.deltaTime;
            if (mTimeForNextShot <= 0)
            {
                mTimeForNextShot = ShotInterval;
                Fire();
            }
        }

        // Make the plane rotate to look always in the direction of movement
        if (AlwaysLookForward && mDirection != Vector2.zero)
        {
            var angle = Mathf.Atan2(this.mDirection.y, this.mDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle + 90, Vector3.forward);
        }
    }
}
