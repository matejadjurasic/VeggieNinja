using UnityEngine;

public class Veggies : MonoBehaviour
{
    public GameObject whole;
    public GameObject sliced;

    private Rigidbody veggieRigidbody;
    private Collider veggieCollider;
    private ParticleSystem juiceParticleEffect;
    public int points = 1;
    public float minHeight = 0f;
    private bool isFalling = false;
    private bool isSliced = false;

    private void Awake()
    {
        veggieRigidbody = GetComponent<Rigidbody>();
        veggieCollider = GetComponent<Collider>();
        juiceParticleEffect = GetComponentInChildren<ParticleSystem>();
    }

    private void Slice(Vector3 direction, Vector3 position, float force)
    {
        if (isSliced)
            return;
        isSliced = true;
        FindObjectOfType<GameManager>().IncreaseScore(points);
        veggieCollider.enabled = false;
        whole.SetActive(false);
        sliced.SetActive(true);

        juiceParticleEffect.Play();

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        sliced.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        Rigidbody[] slices = sliced.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody slice in slices)
        {
            slice.velocity = veggieRigidbody.velocity;
            slice.AddForceAtPosition(direction * force, position, ForceMode.Impulse);
        }
    }

    private void Update()
    {
        if (veggieRigidbody.velocity.y < 0f && !isFalling)
        {
            isFalling = true;
        }
        if (transform.position.y < minHeight && isFalling == true && isSliced != true)
        {
            FindObjectOfType<GameManager>().LoseHealth();
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Blade blade = other.GetComponent<Blade>();
            Slice(blade.direction, blade.transform.position, blade.sliceForce);
        }
    }
}
