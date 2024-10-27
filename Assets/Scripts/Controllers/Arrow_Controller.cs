using UnityEngine;
using Random = UnityEngine.Random;

public class Arrow_Controller : MonoBehaviour
{
    public int damage { get; private set; } = 25;
    [SerializeField] private string targetLayerName = "Player";
    [SerializeField] private float xVelocity = 4f;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private bool canMove = true;
    [SerializeField] private bool flipped;
    private CharacterStats stats;

    private void Update()
    {
        if (canMove)
            rb.velocity = new Vector2(xVelocity, rb.velocity.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(targetLayerName))
        {
            int arrowHitDir = SetupArrowHitDir(collision.transform);
            stats.DoDamage(collision.GetComponent<CharacterStats>(), arrowHitDir);
            StuckInto(collision);
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            StuckInto(collision);
    }

    public void SetupArrow(float _arrowSpeed, CharacterStats _stats)
    {
        xVelocity = _arrowSpeed;
        stats = _stats;
    }

    private int SetupArrowHitDir(Transform target) => transform.position.x > target.position.x ? -1 : 1;

    private void StuckInto(Collider2D collision)
    {
        GetComponentInChildren<ParticleSystem>().Stop();
        GetComponent<CapsuleCollider2D>().enabled = false;
        canMove = false;
        rb.isKinematic = true; // true => 不受物理引擎影响
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        transform.parent = collision.transform;

        Destroy(gameObject, Random.Range(5, 7));
    }

    public void FlipArrow()
    {
        if (flipped)
            return;

        xVelocity *= -1;
        flipped = true;
        targetLayerName = "Enemy";
        transform.Rotate(0, 180, 0);
    }
}