using System;
using UnityEngine;
//BEGIN_IN_APP_PURCHASE
/*
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
*/

//END_IN_APP_PURCHASE

public class IAP_Controller : MonoBehaviour
//BEGIN_IN_APP_PURCHASE
/*
, IStoreListener, IDetailedStoreListener
*/

//END_IN_APP_PURCHASE
{
    public static IAP_Controller Instance { get; private set; }
    //BEGIN_IN_APP_PURCHASE
/*
[SerializeField] bool UseUnityServices = false;
#if Admob_Simple_Rizwan || Max_Mediation_Rizwan || UnityAds_Rizwan

#else
    [SerializeField] bool TestMode = true;
#endif
    public static Action OnPurchaseSuccessful;
    private IStoreController StoreController;
    private IExtensionProvider ExtensionProvider;
    bool DebugMode = false;

    private async void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
            if (transform.parent == null) { DontDestroyOnLoad(gameObject); }
        }
        if (UseUnityServices)
        {
            InitializationOptions options = new InitializationOptions()
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                      .SetEnvironmentName("test");
#else
                            .SetEnvironmentName("production");
#endif
            await UnityServices.InitializeAsync(options);
        }

        ResourceRequest operation = Resources.LoadAsync<TextAsset>("IAPProductCatalog");
        operation.completed += HandleIAPCatalogLoaded;
    }

    private void HandleIAPCatalogLoaded(AsyncOperation Operation)
    {
        ResourceRequest request = Operation as ResourceRequest;
        ProductCatalog catalog = JsonUtility.FromJson<ProductCatalog>((request.asset as TextAsset).text);
        //PrintStatus($"Loaded catalog with {catalog.allProducts.Count} items", true);
        Debug.LogError($"Loaded catalog with {catalog.allProducts.Count} items");
#if Admob_Simple_Rizwan || Max_Mediation_Rizwan || UnityAds_Rizwan
        if (AdsController.Instance.TestMode)
        {
            StandardPurchasingModule.Instance().useFakeStoreUIMode = FakeStoreUIMode.StandardUser;
            StandardPurchasingModule.Instance().useFakeStoreAlways = true;
        }
        else
        {
            StandardPurchasingModule.Instance().useFakeStoreAlways = false;
        }
#else
        if (TestMode)
        {
            StandardPurchasingModule.Instance().useFakeStoreUIMode = FakeStoreUIMode.StandardUser;
            StandardPurchasingModule.Instance().useFakeStoreAlways = true;
        }
        else
        {
            StandardPurchasingModule.Instance().useFakeStoreAlways = false;
        }
#endif

#if UNITY_ANDROID
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(
            StandardPurchasingModule.Instance(AppStore.GooglePlay)
        );
#elif UNITY_IOS
                ConfigurationBuilder builder = ConfigurationBuilder.Instance(
                    StandardPurchasingModule.Instance(AppStore.AppleAppStore)
                );
#else
                ConfigurationBuilder builder = ConfigurationBuilder.Instance(
                    StandardPurchasingModule.Instance(AppStore.NotSpecified)
                );
#endif

        foreach (ProductCatalogItem item in catalog.allProducts)
        {
            builder.AddProduct(item.id, item.type);
        }

        PrintStatus($"Initializing Unity IAP with {builder.products.Count} products", false);
        UnityPurchasing.Initialize(this, builder);
        DebugMode = AdsController.Instance.DebugMode;
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        StoreController = controller;
        ExtensionProvider = extensions;
        PrintStatus($"Successfully Initialized Unity IAP. Store Controller has {StoreController.products.all.Length} products", false);
    }

    public void BuyProduct_Index_Number(int IndexNumber, Action onPurchaseSuccessful)
    {
        if (IndexNumber < 0 || IndexNumber >= StoreController.products.all.Length)
        {
            PrintStatus($"IAP product number #{IndexNumber} does not exist", true);
            return;
        }

        Product productToBuy = StoreController.products.all[IndexNumber];
        if (productToBuy == null)
        {
            PrintStatus($"IAP product #{IndexNumber} is null", true);
            return;
        }

        OnPurchaseSuccessful = onPurchaseSuccessful;
        StoreController.InitiatePurchase(productToBuy);
    }

    public void BuyProduct_ID_Name(string ID_Name, Action onPurchaseSuccessful)
    {
        Product productToBuy = StoreController.products.WithID(ID_Name);
        if (productToBuy == null)
        {
            PrintStatus($"IAP product named {ID_Name} is null", true);
            return;
        }

        OnPurchaseSuccessful = onPurchaseSuccessful;
        StoreController.InitiatePurchase(productToBuy);
    }

    public void RestorePurchase()
    {
#if UNITY_IOS
                ExtensionProvider.GetExtension<IAppleExtensions>().RestoreTransactions(OnRestore);
#endif
    }

    private void OnRestore(bool success, string message)
    {
        if (success)
        {
            PrintStatus($"Purchases successfully restored. {message}", false);
        }
        else
        {
            PrintStatus($"Failed to restore purchases. {message}", true);
        }
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        PrintStatus($"Error initializing IAP because of {error}. Show a message to the player depending on the error.", true);
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        PrintStatus($"Failed to purchase {product.definition.id} because {failureReason}", true);
        OnPurchaseSuccessful = null;
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        PrintStatus($"Successfully purchased {purchaseEvent.purchasedProduct.definition.id}", false);

        OnPurchaseSuccessful?.Invoke();
        OnPurchaseSuccessful = null;

        return PurchaseProcessingResult.Complete;
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        PrintStatus($"OnInitializeFailed: Failed to initialize due to {error} - {message}", true);
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        PrintStatus($"OnPurchaseFailed: Failed to purchase {product.definition.id} due to {failureDescription.reason}", true);
        OnPurchaseSuccessful = null;
    }

    void PrintStatus(string message, bool isError)
    {
        if (DebugMode)
        {
#if UNITY_EDITOR
            if (isError)
            {
                Debug.LogError($"<color=red><b>#IAP# </b></color> <b>{message}</b> <color=red><b>#IAP# </b></color>");
            }
            else
            {
                Debug.Log($"<color=green><b>#IAP# </b></color> <b>{message}</b> <color=green><b>#IAP# </b></color>");
            }
#elif UNITY_ANDROID || UNITY_IOS
                    Debug.Log(message);
#endif
        }
    }
*/
  //END_IN_APP_PURCHASE
}
