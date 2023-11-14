using System.Collections;
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

    public string ShapeName
    {
        get { return shapename; }
    }

    protected void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        transform.Rotate(new Vector3(rotationSpeed, rotationSpeed, rotationSpeed) * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Player>().currentRole.Equals(Role.Explorer))
            onFound();
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

    public void onFound()
    {
        if (currentShapeObject == null)
        {
            model.found = true;
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
        currentShapeObject = Realtime.Instantiate(prefab.name, new Vector3(0, 0, 0), Quaternion.identity, new Realtime.InstantiateOptions { });
        currentShapeObject.transform.SetParent(transform, false);
    }

    public void onCollected(Player player)
    {
        model.collected = true;
        player.targetCoin = nextShape;
        gameManager.IncrementCoinsCollected();
        if (currentShapeObject != null)
        {
            Renderer childRenderer = currentShapeObject.GetComponent<Renderer>();
            if (childRenderer != null)
                childRenderer.enabled = false;
        }
        _ = Realtime.Instantiate(particles.name, transform.position, Quaternion.identity, new Realtime.InstantiateOptions { });
        StartCoroutine(SetCoinTextAfterInstantiation(nextShape));
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
        }

        if (currentModel != null)
        {
            currentModel.foundDidChange += FoundDidChange;
            currentModel.collectedDidChange += CollectedDidChange;
            UpdateVisualState();
        }
    }

    private void FoundDidChange(CoinModel model, bool found)
    {
        UpdateVisualState();
    }

    private void CollectedDidChange(CoinModel model, bool collected)
    {
        if (collected)
        {
            //Renderer childRenderer = currentShapeObject.GetComponent<Renderer>();
            //if (childRenderer != null)
            //    childRenderer.enabled = false;
        }
    }

    private void UpdateVisualState()
    {
        if (model != null)
        {
            if (model.collected)
            {
                //Renderer childRenderer = currentShapeObject.GetComponent<Renderer>();
                //if (childRenderer != null)
                //    childRenderer.enabled = false;
            }
        }
    }
}
