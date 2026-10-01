// BridgeController.cs
using UnityEngine;

public class BridgeController : MonoBehaviour
{
    [Tooltip("Перетащи сюда объект твердого моста (Bridge_Solid) из иерархии")]
    public GameObject solidBridgeObject;
    public GameObject voidZone;


    /// <summary>
    /// Этот метод делает мост активным (видимым и твердым).
    /// </summary>
    public void ShowBridge()
    {
        if (solidBridgeObject != null)
        {
            voidZone.SetActive(false);
            solidBridgeObject.SetActive(true);
        }
    }

    /// <summary>
    /// Этот метод прячет мост.
    /// </summary>
    public void HideBridge()
    {
        if (solidBridgeObject != null)
        {
            voidZone.SetActive(true);
            solidBridgeObject.SetActive(false);
        }
    }
}