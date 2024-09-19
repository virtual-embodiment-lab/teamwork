// Copyright (c) Cornell University and Iowa State University
// Licensed under CC BY-NC-SA 4.0
// See CREDITS.md for a list of developers and contributors.

using Normal.Realtime;
using UnityEngine;

/*
 * Coin instance manager
 */
public class Battery : MonoBehaviour
{
    [SerializeField] int rotationSpeed = 20;
    [SerializeField] GameObject particles = null;
    [SerializeField] float lifeTime = 20f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Rotate(new Vector3(rotationSpeed, rotationSpeed, rotationSpeed) * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other);
        Debug.Log(other.gameObject.GetComponent<Player>().currentRole);
        if (other.gameObject.GetComponent<Player>().currentRole.Equals(Role.Explorer))
   
        {
            onCollected();
        }
    }

    public void onCollected()
    {
        _ = Realtime.Instantiate(particles.name, transform.position, Quaternion.identity, new Realtime.InstantiateOptions { });
        Destroy(gameObject);
    }
}
