// Copyright (c) Cornell University and Iowa State University
// Licensed under CC BY-NC-SA 4.0
// See CREDITS.md for a list of developers and contributors.

using System.Collections;
using Normal.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;

/*
 * Coin instance manager
 */
public class Coin : RealtimeComponent<CoinModel>
{
    [SerializeField] GameObject xObject = null;
    [SerializeField] GameObject trianglePrefab = null;
    [SerializeField] GameObject spherePrefab = null;
    [SerializeField] GameObject cubePrefab = null;
    [SerializeField] private GameObject currentShapeObject;
    [SerializeField] bool isFirst = false;
    [SerializeField] int rotationSpeed = 20;
    [SerializeField] CoinShape thisShape = CoinShape.None;

    [SerializeField] GameObject particles = null;
    [SerializeField] GameObject nextCoin = null;
    [SerializeField] CoinShape nextShape = CoinShape.None;

    [SerializeField] GameObject tmp = null;
    [SerializeField] private string shapename = null;
    private GameManager gameManager;
    private Logger_new gl;
    private Transform thisTransform;

    public string ShapeName
    {
        get { return shapename; }
    }

    protected void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        thisTransform = transform;
    }

    void Update()
    {
        transform.Rotate(new Vector3(rotationSpeed, rotationSpeed, rotationSpeed) * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (!realtimeView.isOwnedLocallyInHierarchy) return;
        RealtimeView realtimeView = other.GetComponent<RealtimeView>();
        if (!realtimeView.isOwnedLocallySelf) return;

        gl = other.GetComponent<Logger_new>();
        Debug.Log(other);
        if (other.gameObject.GetComponent<Player>().currentRole.Equals(Role.Explorer)){
            if (other.GetComponent<Player>().CurrentEnergy >= 33)
                onFound(other.GetComponent<Player>());
        }
        if (other.gameObject.GetComponent<Player>().currentRole.Equals(Role.Collector) && model.found && !model.collected)
        {
            Debug.Log(thisShape);
            Debug.Log(other.gameObject.GetComponent<Player>().targetCoin);
            if (isFirst)
                onCollected(other.gameObject.GetComponent<Player>());
            else if (other.gameObject.GetComponent<Player>().targetCoin.Equals(thisShape))
                onCollected(other.gameObject.GetComponent<Player>());
        }
    }

    public void onFound(Player player)
    {
        if (currentShapeObject == null)
        {
            if(player != null)
            {
                gl.AddLine("coin:activated");
                model.found = true;
                player.HandleEnergyConsumption(player);
            }

            xObject.transform.GetChild(0).GetComponent<Renderer>().enabled = false;
            xObject.transform.GetChild(1).GetComponent<Renderer>().enabled = false;
            xObject.transform.GetChild(2).GetComponent<Renderer>().enabled = false;

            switch (thisShape)
            {
                case CoinShape.Triangle:
                    SetShape(trianglePrefab);
                    shapename = "Triangle";
                    break;
                case CoinShape.Circle:
                    SetShape(spherePrefab);
                    shapename = "Sphere";
                    break;
                case CoinShape.Rectangle:
                    SetShape(cubePrefab);
                    shapename = "Cube";
                    break;
            }
        }
    }

    private void SetShape(GameObject prefab)
    {
        //currentShapeObject = Realtime.Instantiate(prefab.name, new Vector3(0, 0, 0), Quaternion.identity, new Realtime.InstantiateOptions {});
        currentShapeObject = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
        currentShapeObject.transform.SetParent(thisTransform, false);
    }

    public void onCollected(Player player, bool me = true)
    {
        if(me)
        {
            model.collected = true;
            gameManager.IncrementCoinsCollected();
            player.collectCoin();
            gl.AddLine("coin:collected");
            player.targetCoin = nextShape;
        }

        GameObject ShapeObject;
        foreach (Transform child in transform)
        {
            // Exclude the child with the constant name "x"
            if (child.name != "X") {
                ShapeObject = child.gameObject;
                Renderer childRenderer = ShapeObject.GetComponent<Renderer>();
                if (childRenderer != null)
                    childRenderer.enabled = false;
            }
        }
    }

    private IEnumerator SetCoinTextAfterInstantiation(CoinShape shape)
    {
        GameObject nextCoinObject = Realtime.Instantiate(nextCoin.name, new Vector3(transform.position.x, 0, transform.position.z), Quaternion.identity, new Realtime.InstantiateOptions { });
        yield return null;
        tmp = nextCoinObject.transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
        tmp.GetComponent<TextMeshProUGUI>().text = $"Next find a:\n{shape}";
    }

    protected override void OnRealtimeModelReplaced(CoinModel previousModel, CoinModel currentModel)
    {
        if (previousModel != null)
        {
            previousModel.foundDidChange -= FoundDidChange;
            previousModel.collectedDidChange -= CollectedDidChange;
            previousModel.parentIDDidChange -= parentIDDidChange;
        }

        if (currentModel != null)
        {
            currentModel.foundDidChange += FoundDidChange;
            currentModel.collectedDidChange += CollectedDidChange;
            //UpdateVisualState();
            currentModel.parentIDDidChange += parentIDDidChange;
        }
    }

    private void FoundDidChange(CoinModel model, bool found)
    {
        UpdateVisualState();
    }

    private void parentIDDidChange(CoinModel model, int value)
    {
        if (model.parentID == -1) {
            transform.SetParent(null);
        } else {
            transform.SetParent(this.transform);
            GameObject test = GameObject.Find(model.parentID.ToString());
            Transform newParent = GameObject.Find(model.parentID.ToString()).transform;
            if (newParent != null) {
                transform.SetParent(newParent);
            }
        }    
    }

    private void CollectedDidChange(CoinModel model, bool collected)
    {
        if (collected)
        {
            onCollected(null, false);
        }
    }

    private void UpdateVisualState()
    {
        if (model != null)
        {
            if (model.found)
            {
                onFound(null);
            }
        }
    }
}
