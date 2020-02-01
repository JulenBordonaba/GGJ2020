using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager current;

    [Header("Variables")]
    public float population = 100;
    public float resources = 100;
    public int happiness = 100;
    public float resourceGrowth = 0;
    public float locura = 0;

    public float boostTrabajosForzados = 10;
    [Tooltip("Pon valores positivos para restar")]
    public float debufDrogar = 10;
    public float bajadaLocura = 3f;
    public Event[] events;

    public float locuraDrogar = 50;
    public float locuraTrabajosForzados = 30;
    public Sprite[] eventSprites;
    public Image eventImage;



    [Header("Texts")]
    public TextMeshProUGUI populationText;
    public TextMeshProUGUI resourcesText;
    public TextMeshProUGUI happynessText;
    public TextMeshProUGUI resourceGrowthText;
    public TextMeshProUGUI populationGrowthText;
    public TextMeshProUGUI locuraText;
    public TextMeshProUGUI eventText;

    public GameObject gameOverObject;
    public Toggle drogarToggle, trabajosForzadosToggle;

    private bool canRecover = true;
    public bool trabajosForzados = false;
    public bool drogar = false;
    private float tiempoRelajados = 0;

    public int ordersToNewspaper = 0;


    private void Awake()
    {
        current = this;

    }

    // Start is called before the first frame update
    void Start()
    {
        drogar = drogarToggle.isOn;
        trabajosForzados = trabajosForzadosToggle.isOn;
        population = Random.Range(30, 70);
        resources = Random.Range(30, 70);
        happiness = 3;
        StartCoroutine(HappinessGrowCoroutine());
        ordersToNewspaper = Random.Range(2, 4);

    }

    // Update is called once per frame
    void Update()
    {
        ShowTexts();
        Growth();
        Limits();
        if (GameOver)
        {
            gameOverObject.SetActive(true);
        }
        if (canRecover)
        {
            locura -= bajadaLocura * Time.deltaTime;
            locura = Mathf.Clamp(locura, 0, 100);
        }
        Relajados();

    }



    public void Relajados()
    {
        if (!trabajosForzados)
        {
            tiempoRelajados += Time.deltaTime;
            if (tiempoRelajados >= 10f)
            {
                tiempoRelajados = 0;
                if (happiness < 3)
                {
                    happiness += 1;
                }
            }
        }
        else
        {
            tiempoRelajados = 0;
        }
    }

    public int HappinessGrow
    {
        get { return (drogar ? 1 : 0) + (trabajosForzados ? -1 : 0) + (((population > 90) || (population<30)) ? -1 : 0); }
    }

    public void ChangeDrogar()
    {
        drogar = drogarToggle.isOn;
        happiness = 5;
        locura += locuraDrogar;

        GameManager.current.ordersToNewspaper -= 1;
        if (GameManager.current.ordersToNewspaper <= 0)
        {
            GameManager.current.ThrowEvent();
            GameManager.current.ordersToNewspaper = Random.Range(2, 4);
        }
    }

    public void ChangeTrabajosForzados()
    {
        trabajosForzados = trabajosForzadosToggle.isOn;
        happiness -= 1;
        locura += locuraTrabajosForzados;

        GameManager.current.ordersToNewspaper -= 1;
        if (GameManager.current.ordersToNewspaper <= 0)
        {
            GameManager.current.ThrowEvent();
            GameManager.current.ordersToNewspaper = Random.Range(2, 4);
        }
    }


    public void Limits()
    {
        happiness = Mathf.Clamp(happiness, 0, 5);
        population = Mathf.Clamp(population, 0, 100);
    }

    public void Growth()
    {
        population += PopulationGrow * Time.deltaTime;
        resources += ResourceGrow * Time.deltaTime;
        resources -= ResourceConsumption * Time.deltaTime;

    }


    public void ThrowEvent()
    {
        eventText.transform.parent.gameObject.SetActive(true);
        eventText.text = events[Random.Range(0, events.Length)].text;
        eventImage.sprite = eventSprites[Random.Range(0, eventSprites.Length)];
    }

    public void ShowTexts()
    {
        populationText.text = "population: " + Mathf.FloorToInt(population).ToString();
        resourcesText.text = "resources: " + Mathf.FloorToInt(resources).ToString();
        happynessText.text = "happyness: " + Mathf.FloorToInt(happiness).ToString();
        locuraText.text = "Locura: " + Mathf.FloorToInt(locura).ToString();
        resourceGrowthText.text = "Índice de recursos: " + Mathf.FloorToInt(ResourceGrow-ResourceConsumption) + "(" + Mathf.FloorToInt(ResourceGrow) + " - " + Mathf.FloorToInt(ResourceConsumption) + ")";
        populationGrowthText.text = "Índice de crecimiento: " + Mathf.FloorToInt(PopulationGrow).ToString();
    }


    public float PopulationGrow
    {
        get { return Mathf.Sqrt(resources) * (happiness) * 0.1f; }
    }

    public float ResourceGrow
    {
        get { return Mathf.Clamp(resourceGrowth + (trabajosForzados ? 10 : 0) - (drogar ? 10 : 0) * (happiness < 3 ? 1 : happiness * 0.5f), 0, Mathf.Infinity); }
    }

    public float ResourceConsumption
    {
        get { return population * 0.1f; }
    }

    public bool GameOver
    {
        get { return ((population <= 0 ? true : false) || (resources <= 0 ? true : false) || (happiness <= 0 ? true : false) || (locura >= 100 ? true : false)); }
    }


    public bool isLoco
    {
        get { return locura >= 100; }
    }

    public void ExecuteOrder()
    {
        canRecover = false;
        StartCoroutine(ExecuteOrderCoroutine());
    }

    public IEnumerator ExecuteOrderCoroutine()
    {
        yield return new WaitForSeconds(1f);
        canRecover = true;
    }


    public IEnumerator HappinessGrowCoroutine()
    {
        while (true)
        {

            yield return new WaitForSeconds(5f);
            happiness += HappinessGrow;
        }
    }

}
