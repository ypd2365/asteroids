using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    public new Rigidbody2D rigidbody { get; private set; }
    public Bullet bulletPrefab;

    private float thrustSpeed = 1f;
    public bool thrusting { get; private set; }

    public float turnDirection { get; private set; } = 0f;
    private float rotationSpeed = 0.1f;

    public float respawnDelay = 3f;
    public float respawnInvulnerability = 3f;

    public GameObject shield;

    private Bounds screenBounds;

    private int numberOfBullets = 3;
    private float spreadAngle = 20f;
    private int playerHealth=100;
    int storeInitialBullets;

    bool isBulletBoosterActivated=false;

    public playerData playerdata;
    


    private void Awake()
    {
       
        rigidbody = GetComponent<Rigidbody2D>();

        // Convert screen space bounds to world space bounds
        screenBounds = new Bounds();
        screenBounds.Encapsulate(Camera.main.ScreenToWorldPoint(Vector3.zero));
        screenBounds.Encapsulate(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f)));

        thrustSpeed = playerdata.thrustSpeed;
        rotationSpeed = playerdata.rotationSpeed;
        numberOfBullets = playerdata.numberOfBullets;
        playerHealth = playerdata.playerHealth;
        spreadAngle = playerdata.spreadAngle;


        if (numberOfBullets / 2 * 2 == numberOfBullets) numberOfBullets++; // Need an odd number of shots
        if (numberOfBullets < 3) numberOfBullets = 3;  // At least 3 shots for a fan

        storeInitialBullets = numberOfBullets;


    }

    private void OnEnable()
    {
        // Turn off collisions for a few seconds after spawning to ensure the
        // player has enough time to safely move away from asteroids
        gameObject.layer = LayerMask.NameToLayer("Ignore Collisions");
        Invoke(nameof(TurnOnCollisions), respawnInvulnerability);
        FindObjectOfType<GameManager>().SetLives(playerHealth);
    }

    private void Update()
    {
        thrusting = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
            turnDirection = 1f;
        } else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            turnDirection = -1f;
        } else {
            turnDirection = 0f;
        }

        if (Input.GetKeyDown(KeyCode.Space) && isBulletBoosterActivated==false) {
            Shoot();
        }
    }

    private void FixedUpdate()
    {
        if (thrusting) {
            rigidbody.AddForce(transform.up * thrustSpeed);
        }

        if (turnDirection != 0f) {
            rigidbody.AddTorque(rotationSpeed * turnDirection);
        }

        // Wrap to the other side of the screen if the player goes off screen
        if (rigidbody.position.x > screenBounds.max.x + 0.5f) {
            rigidbody.position = new Vector2(screenBounds.min.x - 0.5f, rigidbody.position.y);
        } else if (rigidbody.position.x < screenBounds.min.x - 0.5f) {
            rigidbody.position = new Vector2(screenBounds.max.x + 0.5f, rigidbody.position.y);
        } else if (rigidbody.position.y > screenBounds.max.y + 0.5f) {
            rigidbody.position = new Vector2(rigidbody.position.x, screenBounds.min.y - 0.5f);
        } else if (rigidbody.position.y < screenBounds.min.y - 0.5f) {
            rigidbody.position = new Vector2(rigidbody.position.x, screenBounds.max.y + 0.5f);
        }
    }

    private void Shoot()
    {
        var qAngle = Quaternion.AngleAxis((float)(-numberOfBullets / 2.0 * spreadAngle), transform.forward) * transform.rotation;
        var qDelta = Quaternion.AngleAxis(spreadAngle, transform.forward);

        for (var i = 0; i < numberOfBullets; i++)
        {
            Bullet bullet  = Instantiate(bulletPrefab, transform.position, qAngle);
            bullet.Project(bullet.transform.up);
           // go.rigidbody.AddForce(go.transform.forward * 1000.0);
            qAngle = qDelta * qAngle;
        }

       // Bullet bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
       // bullet.Project(transform.up);
    }

    private void TurnOnCollisions()
    {
        gameObject.layer = LayerMask.NameToLayer("Player");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Asteroid"))
        {
            if(shield.activeSelf)
            {
                shield.SetActive(false);
            }
            else
            {
                playerHealth -= 20;
                FindObjectOfType<GameManager>().SetLives(playerHealth);
                Destroy(collision.gameObject);
                if (playerHealth <= 0)
                {
                    rigidbody.velocity = Vector3.zero;
                    rigidbody.angularVelocity = 0f;
                    gameObject.SetActive(false);

                    FindObjectOfType<GameManager>().PlayerDeath(this);
                }
            }
            
           
        }
        if (collision.gameObject.CompareTag("shieldPowerUp"))
        {
            shield.SetActive(true);
            FindObjectOfType<GameManager>().isBoosterOnScreen = false;
            Destroy(collision.gameObject);

        }
        if (collision.gameObject.CompareTag("bulletPowerUp"))
        {
            isBulletBoosterActivated = true;
            FindObjectOfType<GameManager>().isBoosterOnScreen = false;
            Destroy(collision.gameObject);
            StartCoroutine(ActivateBulletBooster());

        }
    }

    int countSeconds;
    IEnumerator ActivateBulletBooster()
    {
        numberOfBullets = 9;
        while(countSeconds<=playerdata.blasterBoosterDuration*2)
        {
            Shoot();
            yield return new WaitForSeconds(0.5f);
            countSeconds++;
        }
        countSeconds = 0;

        DeactivateBulletBooster();
    }

    void DeactivateBulletBooster()
    {
        numberOfBullets = storeInitialBullets;
        CancelInvoke();
        isBulletBoosterActivated = false;
    }

}
