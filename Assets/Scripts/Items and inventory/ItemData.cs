using System.Text;
using UnityEngine;

# if UNITY_EDITOR
using UnityEditor;
# endif

public enum ItemType
{
    Material,
    Equipment
}

[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Item")]
public class ItemData : ScriptableObject
{
    public ItemType itemType;
    public string itemName;
    public Sprite itemIcon;
    public string itemID;
    [Range(0, 100)]
    public float dropChance;
    public StringBuilder sb = new StringBuilder();

    private void OnValidate()
    {
        // 标识每个物品的唯一ID
# if UNITY_EDITOR
        string path = AssetDatabase.GetAssetPath(this);
        itemID = AssetDatabase.AssetPathToGUID(path);
# endif
    }

    public virtual string GetItemDescription() => sb.ToString();
}