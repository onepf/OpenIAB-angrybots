using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace OnePF
{
    public class OpenIAB
    {
        #region Events

        public delegate void InitFinishedDelegate(bool success, string errorDescription);

#pragma warning disable 0067
        // Fired when init is finished
        public static event InitFinishedDelegate InitFinished;
        // Fired when the inventory and purchase history query has returned
        public static event Action<Inventory> QueryInventorySucceeded;
        // Fired when the inventory and purchase history query fails
        public static event Action<string> QueryInventoryFailed;
        // Fired when a purchase of a product or a subscription succeeds
        public static event Action<Purchase> PurchaseSucceeded;
        // Fired when a purchase fails
        public static event Action<int, string> PurchaseFailed;
        // Fired when a call to consume a product succeeds
        public static event Action<Purchase> ConsumeSucceeded;
        // Fired when a call to consume a product fails
        public static event Action<string> ConsumeFailed;
        // Fired when transaction was restored
        public static event Action<string> TransactionRestored;
        // Fired when transaction restoration process failed
        public static event Action<string> RestoreFailed;
        // Fired when transaction restoration process succeeded
        public static event Action RestoreSucceeded;
#pragma warning restore 0067

        #endregion

        public static OpenIABEventManager EventManager { get; private set; }

        static IOpenIAB _billing;

        static OpenIAB()
        {
#if UNITY_ANDROID
            _billing = new OpenIAB_Android();
            Debug.Log("********** Android OpenIAB plugin initialized **********");
#elif UNITY_IOS
			_billing = new OpenIAB_iOS();
            Debug.Log("********** iOS OpenIAB plugin initialized **********");
#elif UNITY_WP8
            _billing = new OpenIAB_WP8();
            Debug.Log("********** WP8 OpenIAB plugin initialized **********");
#else
			Debug.LogError("OpenIAB billing currently not supported on this platform. Sorry.");
#endif
        }

        // Must be only called before init
        public static void mapSku(string sku, string storeName, string storeSku)
        {
            _billing.mapSku(sku, storeName, storeSku);
        }

        // Starts up the billing service. This will also check to see if in app billing is supported and fire the appropriate event
        public static void init(Options options)
        {
            if (EventManager == null)
            {
                // Avoid duplication
                EventManager = GameObject.FindObjectOfType<OpenIABEventManager>();

                if (EventManager == null)
                    EventManager = new GameObject(typeof(OpenIABEventManager).ToString()).AddComponent<OpenIABEventManager>();

                OpenIABEventManager.billingSupportedEvent += BillingSupportedEvent;
                OpenIABEventManager.billingNotSupportedEvent += BillingNotSupportedEvent;
                OpenIABEventManager.purchaseSucceededEvent += PurchaseSucceededEvent;
                OpenIABEventManager.purchaseFailedEvent += PurchaseFailedEvent;
                OpenIABEventManager.consumePurchaseSucceededEvent += ConsumePurchaseSucceededEvent;
                OpenIABEventManager.consumePurchaseFailedEvent += ConsumePurchaseFailedEvent;
                OpenIABEventManager.transactionRestoredEvent += TransactionRestoredEvent;
                OpenIABEventManager.restoreSucceededEvent += RestoreSucceededEvent;
                OpenIABEventManager.restoreFailedEvent += RestoreFailedEvent;
            }

            _billing.init(options);
        }

        #region Internal event handlers

        static void BillingSupportedEvent()
        {
            if (InitFinished != null)
                InitFinished(true, "");
        }

        static void BillingNotSupportedEvent(string error)
        {
            if (InitFinished != null)
                InitFinished(false, error);
        }

        static void PurchaseSucceededEvent(Purchase purchase)
        {
            if (PurchaseSucceeded != null)
                PurchaseSucceeded(purchase);
        }

        static void PurchaseFailedEvent(int errorCode, string errorMessage)
        {
            if (PurchaseFailed != null)
                PurchaseFailed(errorCode, errorMessage);
        }

        static void ConsumePurchaseSucceededEvent(Purchase purchase)
        {
            if (ConsumeSucceeded != null)
                ConsumeSucceeded(purchase);
        }

        static void ConsumePurchaseFailedEvent(string error)
        {
            if (ConsumeFailed != null)
                ConsumeFailed(error);
        }

        static void RestoreSucceededEvent()
        {
            if (RestoreSucceeded != null)
                RestoreSucceeded();
        }

        static void RestoreFailedEvent(string error)
        {
            if (RestoreFailed != null)
                RestoreFailed(error);
        }

        static void TransactionRestoredEvent(string sku)
        {
            if (TransactionRestored != null)
                TransactionRestored(sku);
        }

        #endregion

        // Unbinds and shuts down the billing service
        public static void unbindService()
        {
            _billing.unbindService();
        }

        public static bool areSubscriptionsSupported()
        {
            return _billing.areSubscriptionsSupported();
        }

        // Sends a request to get all completed purchases
        public static void queryInventory()
        {
            _billing.queryInventory();
        }

        // Sends a request to get all completed purchases and specified skus information
        public static void queryInventory(string[] skus)
        {
            _billing.queryInventory(skus);
        }

        // Purchases the product with the given sku and developerPayload
        public static void purchaseProduct(string sku, string developerPayload = "")
        {
            _billing.purchaseProduct(sku, developerPayload);
        }

        // Purchases the subscription with the given sku and developerPayload
        public static void purchaseSubscription(string sku, string developerPayload = "")
        {
            _billing.purchaseSubscription(sku, developerPayload);
        }

        // Sends out a request to consume the product
        public static void consumeProduct(Purchase purchase)
        {
            _billing.consumeProduct(purchase);
        }

        public static void restoreTransactions()
        {
            _billing.restoreTransactions();
        }

        public static bool isDebugLog()
        {
            return _billing.isDebugLog();
        }

        public static void enableDebugLogging(bool enabled)
        {
            _billing.enableDebugLogging(enabled);
        }

        public static void enableDebugLogging(bool enabled, string tag)
        {
            _billing.enableDebugLogging(enabled, tag);
        }
    }
}
