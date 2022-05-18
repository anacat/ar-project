using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractableObject : MonoBehaviour
{
    public Canvas canvas;
    public Button button;
    public GameObject selectionVisualizer;
    public TextMeshPro text;

    private void Awake()
    {
        Deselect();
        canvas.worldCamera = Camera.main;
    }

    public void Select()
    {
        selectionVisualizer.SetActive(true);
        button.gameObject.SetActive(true);
    }

    public void Deselect()
    {
        selectionVisualizer.SetActive(false);
        button.gameObject.SetActive(false);
    }

    public void ButtonClick()
    {
        Destroy(gameObject);
    }
}