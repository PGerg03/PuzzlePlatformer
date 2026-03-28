using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "CustomRuleTile", menuName = "Tiles/Custom Rule Tile")]
public class CustomRuleTile : RuleTile<CustomRuleTile.Neighbor>
{
    public enum Neighbor
    {
        ThisOrOther = 1,
        Empty = 2,
    }
    public override bool RuleMatch(int neighbor, TileBase tile)
    {
        Neighbor neighborType = (Neighbor)neighbor;

        switch (neighborType)
        {
            case Neighbor.ThisOrOther:
                return tile == this || (tile != null && tile != this);
            case Neighbor.Empty:
                return tile == null;
            default:
                return base.RuleMatch(neighbor, tile);
        }
    }
}
