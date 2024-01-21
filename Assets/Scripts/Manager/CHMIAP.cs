using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class CHMIAP : CHSingleton<CHMIAP>, IStoreListener
{
    public class IAPInfo
    {
        public string productName;
        public string productID;
        public ProductType productType;
    }

    public class PurchaseState
    {
        public string productName;
        public Defines.EPurchase state;
    }

    List<IAPInfo> productList = new List<IAPInfo>();

    IStoreController iStoreController; // ���� ������ �����ϴ� �Լ� ����
    IExtensionProvider iExtensionProvider; // ���� �÷����� ���� Ȯ�� ó�� ����

    public bool IsInitialized => iStoreController != null && iExtensionProvider != null;

    public Action<PurchaseState> purchaseState;

    public bool IsConsumableType(string productName)
    {
        var product = productList.Find(_ => _.productName == productName);
        if (product == null)
            return false;

        return product.productType == ProductType.Consumable;
    }

    public void Init()
    {
        if (IsInitialized)
            return;

        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        /*productList.Add(new IAPInfo
        {
            productName = CHMMain.String.Product_Name_RemoveAD,
            productID = CHMMain.String.Product_ID_RemoveAD,
            productType = ProductType.NonConsumable
        });

        productList.Add(new IAPInfo
        {
            productName = CHMMain.String.Product_Name_AddTime,
            productID = CHMMain.String.Product_ID_AddTime,
            productType = ProductType.Consumable
        });

        productList.Add(new IAPInfo
        {
            productName = CHMMain.String.Product_Name_AddMove,
            productID = CHMMain.String.Product_ID_AddMove,
            productType = ProductType.Consumable
        });*/

        for (int i = 0; i < productList.Count; ++i)
        {
            var product = productList[i];

            builder.AddProduct(product.productName, product.productType, new IDs
            {
                {product.productID, GooglePlay.Name }
            });
        }

        UnityPurchasing.Initialize(this, builder);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extension)
    {
        Debug.Log($"����Ƽ IAP �ʱ�ȭ ����");
        iStoreController = controller;
        iExtensionProvider = extension;
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogError($"����Ƽ IAP �ʱ�ȭ ���� {error}");
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.LogError($"����Ƽ IAP �ʱ�ȭ ���� {error}\n{message}");
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        var id = args.purchasedProduct.definition.id;
        Debug.Log($"���� ���� - ID : {id}");

        if (purchaseState != null)
            purchaseState.Invoke(new PurchaseState
            {
                productName = id,
                state = Defines.EPurchase.Success
            });

        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(UnityEngine.Purchasing.Product product, PurchaseFailureReason error)
    {
        var id = product.definition.id;
        Debug.LogWarning($"���� ���� - ID : {id}\n{error}");

        if (purchaseState != null)
            purchaseState.Invoke(new PurchaseState
            {
                productName = id,
                state = Defines.EPurchase.Failure
            });
    }

    public void Purchase(string productName)
    {
        if (false == IsInitialized)
            return;

        var product = GetProduct(productName);

        if (product != null && product.availableToPurchase)
        {
            Debug.Log($"���� �õ� - ID : {product.definition.id}");
            iStoreController.InitiatePurchase(product);
        }
        else
        {
            Debug.Log($"���� �õ� �Ұ� - ID : {productName}");
        }
    }

    public void RestorePurchase()
    {
        if (false == IsInitialized)
            return;

        if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer)
        {
            Debug.Log($"���� ���� �õ�");

            var appleExt = iExtensionProvider.GetExtension<IAppleExtensions>();
            appleExt.RestoreTransactions(result => Debug.Log($"���� ���� �õ� ��� - {result}"));
        }
    }

    public bool HadPurchased(string productID)
    {
        if (false == IsInitialized)
            return false;

        var product = GetProduct(productID);
        if (product == null)
            return false;

        return product.hasReceipt;
    }

    public UnityEngine.Purchasing.Product GetProduct(string productID)
    {
        return iStoreController.products.WithID(productID);
    }

    public decimal GetPrice(string productID)
    {
        var product = GetProduct(productID);
        if (product == null)
            return 0;

        return product.metadata.localizedPrice;
    }

    public string GetPriceUnit(string productID)
    {
        var product = GetProduct(productID);
        if (product == null)
            return "";

        return product.metadata.isoCurrencyCode;
    }
}