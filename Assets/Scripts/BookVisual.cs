using UnityEngine;

public class BookVisual : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;  // Usa SpriteRenderer en lugar de Image
    public string bookID;

    public void SetBook(string id, Sprite sprite)
    {
        bookID = id;
        spriteRenderer.sprite = sprite;
    }
}
