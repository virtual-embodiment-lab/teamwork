// Copyright (c) Cornell University and Iowa State University
// Licensed under CC BY-NC-SA 4.0
// See CREDITS.md for a list of developers and contributors.

using System;
using Unity.VisualScripting;
using UnityEngine;

public class Flag : MonoBehaviour
{
    [SerializeField] private FlagColor flagColor = FlagColor.None;
    [SerializeField] private int removeOrder;
    [SerializeField] private float blinkingTime = 3.0f;
    [SerializeField] private float timeToRemove = 2.0f;
    [SerializeField] private float blinkSpeed = 1.0f;

    private float deltaTime = 0;

    private Color startColor;
    private Color endColor = new Color(1.0f, 1.0f, 1.0f, 0f);

    private GameManager gameManager;
    public bool removing = false;
    

    Renderer[] ren;

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager.experimentCondition == flagCondition.Sudden){
            removeOrder = 1;
        }

        ren = GetComponentsInChildren<Renderer>();
        switch (flagColor)
        {
            case FlagColor.Red:
                startColor = new Color(1.0f, 0.0f, 0.0f, 1.0f);
                break;
            case FlagColor.Blue:
                startColor = new Color(0.0f, 0.0f, 1.0f, 1.0f);
                break;
            case FlagColor.Green:
                startColor = new Color(0.0f, 0.45f, 0.0f, 1.0f);
                break;
            case FlagColor.Yellow:
                startColor = new Color(1.0f, 1.0f, 0.0f, 1.0f);
                break;
            case FlagColor.Purple:
                startColor = new Color(0.7f, 0.0f, 0.7f, 1.0f);
                break;
            case FlagColor.Cyan:
                startColor = new Color(0.0f, 1.0f, 1.0f, 1.0f);
                break;
            case FlagColor.LightGreen:
                startColor = new Color(0.0f, 1.0f, 0.0f, 1.0f);
                break;
            default:
                startColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                break;
        }

        foreach (Renderer r in ren)
        {
            r.material.color = startColor;
        }
    }

    void Update()
    {
        if (gameManager.flagNum == removeOrder) removing  = true;

        if (removing){
            if (deltaTime < blinkingTime)
            {
                foreach (Renderer r in ren)
                {
                    r.material.color = Color.Lerp(startColor, endColor, Mathf.PingPong(deltaTime * blinkSpeed, 1));
                }
                
                Color currentColor = Color.Lerp(endColor, startColor, (timeToRemove - blinkingTime) / blinkingTime);
                deltaTime += Time.deltaTime;
            }else if (deltaTime >= blinkingTime & deltaTime < (timeToRemove + blinkingTime)){
                float currentSpeed = Mathf.Lerp(6, blinkSpeed, ((timeToRemove + blinkingTime) - deltaTime)  / (timeToRemove + blinkingTime));

                foreach (Renderer r in ren)
                {
                    r.material.color = Color.Lerp(endColor, startColor, ((timeToRemove + blinkingTime) - deltaTime)  / timeToRemove);
                    // r.material.color = Color.Lerp(startColor, endColor, Mathf.PingPong(Time.time * currentSpeed, 1));
                }
                deltaTime += Time.deltaTime;

            }else if (deltaTime >= (timeToRemove + blinkingTime)){
                foreach (Renderer r in ren)
                {
                    r.material.color = endColor;
                }
                Destroy(gameObject);            
            }
        }

        // }else{
        //     foreach (Renderer r in ren)
        //     {
        //         r.material.color = Color.Lerp(startColor, endColor, Mathf.PingPong(Time.time * speed, 1));
        //     }

        //     float increment = (increaseSpeed * Time.deltaTime) / (3.5f - speed + 0.01f); // Adding a small value to prevent division by zero
        //     Debug.Log("speed: " + speed);
        //     // speed += increment;
        //     // if (speed > 0.9){
        //     //     speed += 0.0002f;
        //     // }else{
        //         // speed += 0.005f;
        //     // }
        
    }
}
