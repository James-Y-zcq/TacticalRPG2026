using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldObjectPreviewUI : MonoBehaviour
{
    public static WorldObjectPreviewUI i;

    [SerializeField] GameObject graphicsParent;
    [SerializeField] Image portraitRenderer;
    [SerializeField] TMP_Text headerText;
    [SerializeField] TMP_Text descriptionText;

    void Awake()
    {
        if(i==null)
            i = this;
        else
            Destroy(this.gameObject);
    }

    public void displayObject(MapObject mapObject)
    {
        Sprite portrait;
        string description;

        headerText.text = mapObject.exposeObjectInfo(out portrait, out description); //assigns all datatypes without constructing a new class or multiple getters

        portraitRenderer.sprite = portrait;
        descriptionText.text = description;

        graphicsParent.gameObject.SetActive(true);
    }

    public void hideMenu()
    {
        graphicsParent.gameObject.SetActive(false);
    }
}