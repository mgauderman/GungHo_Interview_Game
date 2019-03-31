using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierSystem : MonoBehaviour
{
    [SerializeField]
    private Collider2D playerCollider;
    [SerializeField]
    private float alphaAmount;
    [SerializeField]
    private string startColorTag;
    [SerializeField]
    private GameManager gameManager;
    [SerializeField]
    private Color[] colors;
    [SerializeField]
    private string[] colorNames;

    private SpriteRenderer SR;

    void Start() 
    {
        SR = GetComponent<SpriteRenderer>();
        ChangeColor(startColorTag);
    }

    public void die() {
        gameManager.StartGame();
    }

    public void ChangeColor(string colorName) 
    {
        for( int i = 0; i < colors.Length; i++ ) {
            if(colorNames[i] == colorName) {
                SR.color = new Color(colors[i].r, colors[i].g, colors[i].b, alphaAmount);
                tag = colorName;
            }
        }
    }
}
