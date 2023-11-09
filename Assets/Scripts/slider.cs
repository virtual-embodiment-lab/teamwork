using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class slider : MonoBehaviour
{
    // Start is called before the first frame update
    public Slider slid;
    [SerializeField] GameObject target_player;
    private Vector3 prev_pos;
    private float regen_rate = 1;
    private float energy_use_rate = 0.5f;

    void Start()
    {
        slid = gameObject.GetComponent<Slider>();
        slid.value = slid.maxValue;
        prev_pos = target_player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 sliderpos = new Vector3(target_player.transform.position.x+1, target_player.transform.position.y+1, target_player.transform.position.z+1);
        transform.SetPositionAndRotation(sliderpos, target_player.transform.rotation);
        slid.value -= Vector3.Distance(target_player.transform.position, prev_pos)*energy_use_rate;
        //Debug.Log(slid.value);
        prev_pos = target_player.transform.position;
        if (Input.GetKeyDown(KeyCode.E))
        {
            slid.value += regen_rate;
        }
    }

    public GameObject getPlayer()
    {
        return target_player;
    }

    public float getValue()
    {
        Debug.Log(slid);
        return slid.value;
    }
}
