using UnityEngine;

/*
 * Destroy the particles after instantiation
 */
public class ParticleTimer : MonoBehaviour
{
    [Header("Delay Time")]
    [SerializeField] public float delay = 2.0f;
    private void Start()
    {
        Destroy(gameObject, delay);
    }
}