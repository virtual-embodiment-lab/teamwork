using Normal.Realtime;
using UnityEngine;

/*
 * Coin instance manager
 */
public class BatterySample : MonoBehaviour
{
    [SerializeField] int rotationSpeed = 20;

    void Update()
    {
        transform.Rotate(new Vector3(rotationSpeed, rotationSpeed, rotationSpeed) * Time.deltaTime);
    }
}
