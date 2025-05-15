using UnityEngine;
using UnityEngine.UI;

public class BookPuzzleSelector : MonoBehaviour
{
    public BookPuzzleSlot[] slots;
    public Image selectorVisual;
    public int currentIndex = 0;

    private BookPuzzleSlot selectedSlot = null;

    void Start()
    {
        UpdateSelectorPosition();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentIndex = Mathf.Max(0, currentIndex - 1);
            UpdateSelectorPosition();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentIndex = Mathf.Min(slots.Length - 1, currentIndex + 1);
            UpdateSelectorPosition();
        }

        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return))
        {
            HandleSelection();
        }
    }

    void UpdateSelectorPosition()
    {
        selectorVisual.transform.position = slots[currentIndex].transform.position;
    }

    void HandleSelection()
    {
        if (selectedSlot == null)
        {
            selectedSlot = slots[currentIndex];
            selectedSlot.image.color = Color.yellow;
        }
        else
        {
            BookPuzzleSlot target = slots[currentIndex];

            // Swap imágenes
            Sprite tempSprite = selectedSlot.image.sprite;
            selectedSlot.image.sprite = target.image.sprite;
            target.image.sprite = tempSprite;

            // Swap IDs
            string tempID = selectedSlot.bookID;
            selectedSlot.bookID = target.bookID;
            target.bookID = tempID;

            selectedSlot.image.color = Color.white;
            selectedSlot = null;

            BookPuzzleManager.Instance.CheckSolution();
        }
    }
}

