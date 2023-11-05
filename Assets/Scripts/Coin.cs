using Normal.Realtime;
using UnityEngine;

/*
 * Coin instance manager
 */
public class Coin : MonoBehaviour
{
    [SerializeField] int rotationSpeed = 20;
    [SerializeField] GameObject particles = null;
    [SerializeField] GameObject Mesh = null;
    //[SerializeField] SpriteRenderer Target = null;
    [SerializeField] public bool found = false;
    [SerializeField] public bool collected = false;
    [SerializeField] Material Coin_Hidden = null;
    [SerializeField] Material Coin_Found = null;

    void Start()
    {
        Mesh.GetComponent<Renderer>().material = Coin_Hidden;
    }

    void Update()
    {
        transform.Rotate(new Vector3(0, rotationSpeed, 0) * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Player>().currentRole.Equals(Role.Explorer))
        {
            onFound();
        }

        if (other.gameObject.GetComponent<Player>().currentRole.Equals(Role.Collector) && found)
        {
            onCollected();
        }
    }

    public void onFound()
    {
        found = true;
        Mesh.GetComponent<Renderer>().material = Coin_Found;
    }

    public void onCollected()
    {
        _ = Realtime.Instantiate(particles.name, transform.position, Quaternion.identity, new Realtime.InstantiateOptions { });
        collected = true;
        gameObject.SetActive(false);
    }
}
