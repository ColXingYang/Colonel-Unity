using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEditor;

public class ColonelDefaultControls {

    public static GameObject CreateDoubleClickButton(DefaultControls.Resources resources)
    {
        GameObject btn = DefaultControls.CreateButton(resources);
        btn.name = "DoubleClickButton";
        btn.transform.Find("Text").GetComponent<Text>().text = "DoubleClickButton";

        Image image = btn.GetComponent<Image>();
        image.sprite = resources.standard;
        image.type = Image.Type.Sliced;

        Object.DestroyImmediate(btn.GetComponent<Button>());
        btn.AddComponent<DoubleClickButton>();
        return btn;
    }

    public static GameObject CreateLongClickButton(DefaultControls.Resources resources)
    {
        GameObject btn = DefaultControls.CreateButton(resources);
        btn.name = "LongClickButton";
        btn.transform.Find("Text").GetComponent<Text>().text = "LongClickButton";

        Image image = btn.GetComponent<Image>();
        image.sprite = resources.standard;
        image.type = Image.Type.Sliced;

        Object.DestroyImmediate(btn.GetComponent<Button>());
        btn.AddComponent<LongClickButton>();
        return btn;
    }
}
