using UnityEngine;
using UnityEngine.Purchasing;
using Unity.VisualScripting;

public class IAPTesting : MonoBehaviour
{
    public void Purchased(UnityEngine.Purchasing.Product product)
    {
        if (product.definition.id.Equals("1"))
        {
            EventBus.Trigger("add1kcoins");
            Debug.Log("Triggered add1kcoins");
        }

        else if (product.definition.id.Equals("2"))
        {
            EventBus.Trigger("add5kcoins");
            Debug.Log("Triggered add5kcoins");
        }
    }
}
