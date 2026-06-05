using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class MapmodeButton : MonoBehaviour
{
    [SerializeField] Mapmode mapmode;
    public event Action<Mapmode> onMapmodeSelected;
    private Button attachedButton;
    void Awake()
    {
        attachedButton = GetComponent<Button>();
    }
    void OnEnable()
    {
        attachedButton = GetComponent<Button>();

        if (attachedButton != null)
        {
            attachedButton.onClick.AddListener(invokeMapmode);
        }
    }

    void OnDisable()
    {
        if (attachedButton != null)
        {
            attachedButton.onClick.RemoveListener(invokeMapmode);
        }
    }

    private void invokeMapmode()
    {
        onMapmodeSelected?.Invoke(mapmode);
    }
}