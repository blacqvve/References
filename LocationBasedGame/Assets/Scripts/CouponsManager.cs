using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CouponsManager : MonoBehaviour
{
    [System.Serializable]
    public class Coupons
    {
        public string companyName { get; set; }
        public string couponDescription { get; set; }
        public Sprite companyLogo { get; set; }
        public int stock { get; set; }
        public int cost { get; set; }
        public string id { get; set; }
    }
    [SerializeField]
    private Text pointsText;
    [SerializeField]
    private Transform contentPanel;
    [SerializeField]
    private GameObject subLoading;
    List<Coupons> items = new List<Coupons>();
    public SimpleObjectPool buttonObjectPool;
    DatabaseManager databaseManager;
    // Start is called before the first frame update
    private void Awake()
    {
        databaseManager = FindObjectOfType<DatabaseManager>();
    }
    void OnEnable()
    {
        subLoading.SetActive(true);
        pointsText.text = "Kuponlarım";
        databaseManager.SendGetPlayerCoupons();
        databaseManager.OnGetPlayerCouponsFinished += Instance_OnGetPlayerCouponsFinished;
    }

    private void Instance_OnGetPlayerCouponsFinished()
    {

        items = PlayerData.Instance.couponItems;
        RefreshDisplay();
        subLoading.SetActive(false);
    }

    private void Start()
    {
       
        RefreshDisplay();
    }
    public void RefreshDisplay()
    {
        AddButtons();
    }

    void AddButtons()
    {
        for (int i = 0; i < items.Count; i++)
        {
            Coupons item = items[i];
            GameObject newButton = buttonObjectPool.GetObject();
            newButton.transform.SetParent(contentPanel);

            CouponItemButton sampleButton = newButton.GetComponent<CouponItemButton>();

            sampleButton.Setup(item, this);
        }
    }
}

