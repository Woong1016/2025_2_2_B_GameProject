using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UIElements;



public enum BlockType
{
    Air,    //°ø±â

    Grass,  // ÀÜµð
    Dirt,   // Èë
    Stone,  // µ¹
    Bedrock,    // ±â¹Ý¾Ï
    Wood,// ³ª¹«  
    Leaf,   // ³ª¹µÀÙ
    Water,  // ¹°
    Sand,   // ¸ð·¡ 
    CoalOre,    // ¼®Åº±¤¸Æ
    IronOre,    // Ã¶±¤¸Æ
    GoldOre,    // ±Ý±¤¸Æ
    DiamondOre, // ´ÙÀÌ¾Æ±¤¸Æ
}



[System.Serializable]
public class BlockData
{
    public BlockType blocktype  ;
    public Color blockColor;
    public bool isSolid;

    public BlockData(BlockType type)
    {
        blocktype = type;
        isSolid = type != BlockType.Air;

        switch(type)
        {
            case BlockType.Grass:
                blockColor = new Color(0.2f, 0.8f, 0.2f);
                break;

            case BlockType.Dirt:
                blockColor = new Color(0.6f, 0.4f, 0.2f);
                break;
            case BlockType.Stone:
                blockColor = new Color(0.5f, 0.5f, 0.5f);
                break;
            case BlockType.Bedrock:
                blockColor = new Color(0.2f, 0.2f, 0.2f);
                break;

            case BlockType.Wood:
                blockColor = new Color(0.6f, 0.3f, 0.1f);
                break;
            case BlockType.Leaf:
                blockColor = new Color(0.1f, 0.6f, 0.1f);
                break;
            case BlockType.Water:
                blockColor = new Color(0.2f, 0.4f, 0.9f);
                isSolid = false;
                break;
            case BlockType.Sand:
                blockColor = new Color(0.9f, 0.85f, 0.6f);
                break;
            case BlockType.CoalOre:
                blockColor = new Color(0.3f, 0.3f, 0.3f);
                break;
            case BlockType.IronOre:
                blockColor = new Color(0.7f, 0.6f, 0.5f);
                break;
            case BlockType.GoldOre:
                blockColor = new Color(0.9f, 0.8f, 0.2f);
                break;
            case BlockType.DiamondOre:
                blockColor = new Color(0.3f, 0.8f, 0.9f);
                break;
        }  
    }
    
}

