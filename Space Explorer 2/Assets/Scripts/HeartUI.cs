using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class HeartUI : MonoBehaviour
{
    [Header("Prefabs & Sprites")]
    public GameObject heartPrefab;
    public Transform heartContainer;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    

    private List<GameObject> hearts = new List<GameObject>();

    void Start()
    {
        //DrawHearts();
    }

    void Update()
    {
        
    }
    public void UpdateHearts(int currentHearts, int maxHearts)
    {
        // Xóa trái tim cũ
        foreach (GameObject heart in hearts)
        {
            Destroy(heart);
        }
        hearts.Clear();

        // Vẽ lại trái tim
        for (int i = 0; i < maxHearts; i++)
        {
            GameObject newHeart = Instantiate(heartPrefab, heartContainer);
            Image img = newHeart.GetComponent<Image>();
            img.sprite = (i < currentHearts) ? fullHeart : emptyHeart;
            hearts.Add(newHeart);
        }
    }
    //void DrawHearts()
    //{
    //    // Xóa trái tim cũ nếu có
    //    foreach (GameObject heart in hearts)
    //    {
    //        Destroy(heart);
    //    }
    //    hearts.Clear();

    //    for (int i = 0; i < maxHearts; i++)
    //    {
    //        GameObject newHeart = Instantiate(heartPrefab, heartContainer);
    //        Image img = newHeart.GetComponent<Image>();

    //        if (i < currentHearts)
    //            img.sprite = fullHeart;
    //        else
    //            img.sprite = emptyHeart;

    //        hearts.Add(newHeart);
    //    }
    //}

    //public void LoseHeart()
    //{
    //    if (currentHearts > 0)
    //    {
    //        currentHearts--;
    //        DrawHearts();
    //    }
    //}

    //public void GainHeart()
    //{
    //    if (currentHearts < maxHearts)
    //    {
    //        currentHearts++;
    //        DrawHearts();
    //    }
    //}
}
