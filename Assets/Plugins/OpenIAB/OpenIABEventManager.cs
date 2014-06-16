using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using OnePF;

public class OpenIABEventManager : MonoBehaviour
{
    public static event Action billingSupportedEvent;
    public static event Action<string> billingNotSupportedEvent;
    public static event Action<Inventory> queryInventorySucceededEvent;
    public static event Action<string> queryInventoryFailedEvent;
    public static event Action<Purchase> purchaseSucceededEvent;
    public static event Action<int, string> purchaseFailedEvent;
    public static event Action<Purchase> consumePurchaseSucceededEvent;
    public static event Action<string> consumePurchaseFailedEvent;
    public static event Action<string> transactionRestoredEvent;
    public static event Action<string> restoreFailedEvent;
    public static event Action restoreSucceededEvent;

    private void Awake()
    {
        // Set the GameObject name to the class name for easy access from native plugin
        gameObject.name = GetType().ToString();
        DontDestroyOnLoad(this);
    }

#if UNITY_ANDROID
    private void OnBillingSupported(string empty)
    {
        if (billingSupportedEvent != null)
            billingSupportedEvent();
    }

    private void OnBillingNotSupported(string error)
    {
        if (billingNotSupportedEvent != null)
            billingNotSupportedEvent(error);
    }

    private void OnQueryInventorySucceeded(string json)
    {
        if (queryInventorySucceededEvent != null)
        {
            Inventory inventory = new Inventory(json);
            queryInventorySucceededEvent(inventory);
        }
    }

    private void OnQueryInventoryFailed(string error)
    {
        if (queryInventoryFailedEvent != null)
            queryInventoryFailedEvent(error);
    }

    private void OnPurchaseSucceeded(string json)
    {
        if (purchaseSucceededEvent != null)
            purchaseSucceededEvent(new Purchase(json));
    }

    private void OnPurchaseFailed(string message)
    {
        var tokens = message.Split('|');
        string errorMessage = tokens[1];

        int errorCode;
        if (!Int32.TryParse(tokens[0], out errorCode))
            errorCode = -1;

        if (purchaseFailedEvent != null)
            purchaseFailedEvent(errorCode, errorMessage);
    }

    private void OnConsumePurchaseSucceeded(string json)
    {
        if (consumePurchaseSucceededEvent != null)
            consumePurchaseSucceededEvent(new Purchase(json));
    }

    private void OnConsumePurchaseFailed(string error)
    {
        if (consumePurchaseFailedEvent != null)
            consumePurchaseFailedEvent(error);
    }

    public void OnTransactionRestored(string sku)
    {
        if (transactionRestoredEvent != null)
        {
            transactionRestoredEvent(sku);
        }
    }

    public void OnRestoreTransactionFailed(string error)
    {
        if (restoreFailedEvent != null)
        {
            restoreFailedEvent(error);
        }
    }

    public void OnRestoreTransactionSucceeded(string message)
    {
        if (restoreSucceededEvent != null)
        {
            restoreSucceededEvent();
        }
    }
#endif

#if UNITY_IOS
    private void OnBillingSupported(string inventory)
    {
        OpenIAB_iOS.CreateInventory(inventory);

        if (billingSupportedEvent != null)
        {
            billingSupportedEvent();
        }
    }

    private void OnBillingNotSupported(string error)
    {
        if (billingNotSupportedEvent != null)
            billingNotSupportedEvent(error);
    }

    private void OnQueryInventorySucceeded(string json)
    {
        if (queryInventorySucceededEvent != null)
        {
            Inventory inventory = new Inventory(json);
            queryInventorySucceededEvent(inventory);
        }
    }

    public static void OnQueryInventorySucceeded(Inventory inventory)
    {
        if (queryInventorySucceededEvent != null)
        {
            queryInventorySucceededEvent(inventory);
        }
    }

    private void OnQueryInventoryFailed(string error)
    {
        if (queryInventoryFailedEvent != null)
            queryInventoryFailedEvent(error);
    }

    private void OnPurchaseSucceeded(string sku)
    {
        if (purchaseSucceededEvent != null)
        {
            purchaseSucceededEvent(Purchase.CreateFromSku(OpenIAB_iOS.StoreSku2Sku(sku)));
        }
    }

    private void OnPurchaseFailed(string error)
    {
        if (purchaseFailedEvent != null)
        {
            purchaseFailedEvent(-1, error);
        }
    }

    private void OnConsumePurchaseSucceeded(string json)
    {
        if (consumePurchaseSucceededEvent != null)
            consumePurchaseSucceededEvent(new Purchase(json));
    }

    private void OnConsumePurchaseFailed(string error)
    {
        if (consumePurchaseFailedEvent != null)
            consumePurchaseFailedEvent(error);
    }

    public void OnPurchaseRestored(string sku)
    {
        if (transactionRestoredEvent != null)
        {
            transactionRestoredEvent(sku);
        }
    }

    public void OnRestoreFailed(string error)
    {
        if (restoreFailedEvent != null)
        {
            restoreFailedEvent(error);
        }
    }

    public void OnRestoreFinished(string message)
    {
        if (restoreSucceededEvent != null)
        {
            restoreSucceededEvent();
        }
    }
#endif

#if UNITY_WP8
    public void OnBillingSupported()
    {
        if (billingSupportedEvent != null)
            billingSupportedEvent();
    }

    public void OnBillingNotSupported(string error)
    {
        if (billingNotSupportedEvent != null)
            billingNotSupportedEvent(error);
    }

    private void OnQueryInventorySucceeded(Inventory inventory)
    {
        if (queryInventorySucceededEvent != null)
            queryInventorySucceededEvent(inventory);
    }

    private void OnQueryInventoryFailed(string error)
    {
        if (queryInventoryFailedEvent != null)
            queryInventoryFailedEvent(error);
    }

    private void OnPurchaseSucceeded(Purchase purchase)
    {
        if (purchaseSucceededEvent != null)
            purchaseSucceededEvent(purchase);
    }

    private void OnPurchaseFailed(string error)
    {
        if (purchaseFailedEvent != null)
            purchaseFailedEvent(-1, error);
    }

    private void OnConsumePurchaseSucceeded(Purchase purchase)
    {
        if (consumePurchaseSucceededEvent != null)
            consumePurchaseSucceededEvent(purchase);
    }

    private void OnConsumePurchaseFailed(string error)
    {
        if (consumePurchaseFailedEvent != null)
            consumePurchaseFailedEvent(error);
    }
#endif
}
