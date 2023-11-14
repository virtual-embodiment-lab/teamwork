using System.Collections;
using System.Linq;
using Normal.Realtime;
using TMPro;
using UnityEngine;

/*
 * Coin instance manager
 */
public class Coin : RealtimeComponent<CoinModel>
{
    [SerializeField] GameObject xObject = null;
    [SerializeField] GameObject trianglePrefab = null;
    [SerializeField] GameObject spherePrefab = null;
    [SerializeField] GameObject cubePrefab = null;
    [SerializeField] int rotationSpeed = 20;
    [SerializeField] GameObject particles = null;
    [SerializeField] GameObject nextCoin = null;
    [SerializeField] string nextCoinColor = null;
    [SerializeField] Material coinHidden = null;
    [SerializeField] Material coinFound = null;
    [SerializeField] GameObject tmp = null;
    [SerializeField] bool isFirst = false;
    [SerializeField] CoinShape currentShape = CoinShape.None;
    [SerializeField] private float detectionDistance = 10f;
    [SerializeField] private string shapename = null;

    private GameManager gameManager;

    private bool shapeHasBeenSet = false;
    private GameObject currentShapeObject;

    protected void Start()
    {
        GetComponent<Renderer>().material = coinHidden;
        gameManager = FindObjectOfType<GameManager>();
    }
    public string ShapeName
    {
        get { return shapename; }
    }

    void Update()
    {
        transform.Rotate(new Vector3(0, rotationSpeed, 0) * Time.deltaTime);

        Player[] players = FindObjectsOfType<Player>();
        if (gameObject.layer == LayerMask.NameToLayer("Hidden"))
        {
            foreach (var player in players)
            {
                if (player.currentRole == Role.Explorer)
                {
                    float distanceToExplorer = Vector3.Distance(transform.position, player.transform.position);
                    if (distanceToExplorer <= detectionDistance)
                    {
                        gameObject.layer = LayerMask.NameToLayer("Default");
                        break;
                    }
                }
            }
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Player>().currentRole.Equals(Role.Explorer))
            onFound();
        if (other.gameObject.GetComponent<Player>().currentRole.Equals(Role.Collector) && model.found && !model.collected)
        {
            if (isFirst)
            {
                onCollected();
                other.gameObject.GetComponent<Player>().targetCoin = getShape(nextCoinColor);
                Debug.Log(nextCoinColor);
            }

            else if (other.gameObject.GetComponent<Player>().targetCoin == currentShape)
            {
                onCollected();
                other.gameObject.GetComponent<Player>().targetCoin = getShape(nextCoinColor);
                Debug.Log(nextCoinColor);
            }
        }
    }

    public void onFound()
    {
        if (!shapeHasBeenSet)
        {
            model.found = true;

            // Choose a random shape: 1 for Triangle, 2 for Sphere, 3 for Cube
            int randomShape = Random.Range(1, 4);

            // Switch the coin's shape based on the random choice
            switch (randomShape)
            {
                case 1:
                    SetShape(trianglePrefab);
                    shapename = "Triangle";
                    break;
                case 2:
                    SetShape(spherePrefab);
                    shapename = "Sphere";
                    break;
                case 3:
                    SetShape(cubePrefab);
                    shapename = "Cube";
                    break;
            }
            
            shapeHasBeenSet = true;
        }
        GetComponent<Renderer>().material = coinFound;

    }

    private void SetShape(GameObject prefab)
    {
            currentShapeObject = Realtime.Instantiate(prefab.name, new Vector3(0, 0, 0), Quaternion.identity, new Realtime.InstantiateOptions { });
            currentShapeObject.transform.SetParent(transform, false);
    }

    public void onCollected()
    {
        model.collected = true;
        gameManager.IncrementCoinsCollected();
        GetComponent<Renderer>().enabled = false;
        // Disable rendering for the child object (if it exists)
        if (currentShapeObject != null)
        {
            Renderer childRenderer = currentShapeObject.GetComponent<Renderer>();
            if (childRenderer != null)
            {
                childRenderer.enabled = false;
            }
        }
        _ = Realtime.Instantiate(particles.name, transform.position, Quaternion.identity, new Realtime.InstantiateOptions { });
        StartCoroutine(SetCoinTextAfterInstantiation(currentShapeObject));
    }

    private IEnumerator SetCoinTextAfterInstantiation(GameObject nextCoinShape)
    {
        GameObject nextCoinObject = Realtime.Instantiate(nextCoin.name, new Vector3(transform.position.x, 0, transform.position.z), Quaternion.identity, new Realtime.InstantiateOptions { });
        yield return null;
        tmp = nextCoinObject.transform.GetChild(0).GetChild(0).GetChild(0).gameObject;

        tmp.GetComponent<TextMeshProUGUI>().text = $"Next Coin:\n{ShapeName}";
    }

    protected override void OnRealtimeModelReplaced(CoinModel previousModel, CoinModel currentModel)
    {
        if (previousModel != null)
        {
            // Unregister from events on the previous model
            previousModel.foundDidChange -= FoundDidChange;
            previousModel.collectedDidChange -= CollectedDidChange;
        }

        if (currentModel != null)
        {
            // Register for events on the new model
            currentModel.foundDidChange += FoundDidChange;
            currentModel.collectedDidChange += CollectedDidChange;

            // Update the material to reflect the current state
            UpdateVisualState();
        }
    }

    private void FoundDidChange(CoinModel model, bool found)
    {
        // Update the material when the found state changes
        UpdateVisualState();
    }

    private void CollectedDidChange(CoinModel model, bool collected)
    {
        // Update the GameManager when the coin is collected
        if (collected)
        {
            GetComponent<Renderer>().enabled = false;
        }
    }

    private void UpdateVisualState()
    {
        if (model != null)
        {
            GetComponent<Renderer>().material = model.found ? coinFound : coinHidden;
            if (model.collected)
            {
                GetComponent<Renderer>().enabled = false;
                // Perform any additional cleanup if necessary
                //Destroy(gameObject);
            }
        }
    }

    private CoinShape getShape(string nextCoinColor)
    {
        switch (nextCoinColor)
        {
            case "Red":
                return CoinShape.Triangle;

            case "Blue":
                return CoinShape.Circle;

            case "Green":
                return CoinShape.Rectangle;
        }

        return CoinShape.None;
    }
}
