using UnityEngine;

/*
 * Coin instance manager
 */
public class Battery : MonoBehaviour
{
    [SerializeField] int rotationSpeed = 20;
    [SerializeField] GameObject particles = null;

    void Update()
    {
        transform.Rotate(new Vector3(rotationSpeed, rotationSpeed, rotationSpeed) * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Player>().currentRole.Equals(Role.Explorer))
        {
            onCollected();
        }
    }

    public void onCollected()
    {
        Instantiate(particles, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
