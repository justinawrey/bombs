using System.Collections.Generic;
using UnityEngine;

enum Direction
{
  Left,
  Right,
  Up,
  Down,
  Center,
  NE,
  NW,
  SW,
  SE,
}

class Collision : MonoBehaviour
{
  public LayerMask blockLayer;
  public LayerMask ladderLayer;
  public LayerMask flagLayer;

  private bool IsInDir(Direction dir, LayerMask layer)
  {
    switch (dir)
    {
      case Direction.Left:
        return Physics2D.OverlapBox(transform.position + new Vector3(-1f, 0f, 0f), new Vector2(0.25f, 0.25f), 0f, layer);
      case Direction.Right:
        return Physics2D.OverlapBox(transform.position + new Vector3(1f, 0f, 0f), new Vector2(0.25f, 0.25f), 0f, layer);
      case Direction.Up:
        return Physics2D.OverlapBox(transform.position + new Vector3(0f, 1f, 0f), new Vector2(0.25f, 0.25f), 0f, layer);
      case Direction.Down:
        return Physics2D.OverlapBox(transform.position + new Vector3(0f, -1f, 0f), new Vector2(0.25f, 0.25f), 0f, layer);
      case Direction.Center:
        return Physics2D.OverlapBox(transform.position + new Vector3(0f, 0f, 0f), new Vector2(0.25f, 0.25f), 0f, layer);
      case Direction.NE:
        return Physics2D.OverlapBox(transform.position + new Vector3(-1f, 1f, 0f), new Vector2(0.25f, 0.25f), 0f, layer);
      case Direction.NW:
        return Physics2D.OverlapBox(transform.position + new Vector3(1f, 1f, 0f), new Vector2(0.25f, 0.25f), 0f, layer);
      case Direction.SW:
        return Physics2D.OverlapBox(transform.position + new Vector3(1f, -1f, 0f), new Vector2(0.25f, 0.25f), 0f, layer);
      case Direction.SE:
        return Physics2D.OverlapBox(transform.position + new Vector3(-1f, -1f, 0f), new Vector2(0.25f, 0.25f), 0f, layer);
      default:
        return false;
    }
  }

  public bool BlockInDir(Direction dir)
  {
    return IsInDir(dir, blockLayer);
  }

  public bool BlockOrLadderInDir(Direction dir)
  {
    return IsInDir(dir, blockLayer | ladderLayer);
  }

  public bool LadderInDir(Direction dir)
  {
    return IsInDir(dir, ladderLayer);
  }

  public bool IsOnLadder()
  {
    return IsInDir(Direction.Center, ladderLayer);
  }

  public bool IsOnFlag()
  {
    return IsInDir(Direction.Center, flagLayer);
  }

  public List<Collider2D> GetCollidersAt(Vector3 offset)
  {
    ContactFilter2D contactFilter = new ContactFilter2D().NoFilter();
    contactFilter.layerMask = flagLayer | blockLayer | ladderLayer;
    contactFilter.useLayerMask = true;

    List<Collider2D> colliders = new List<Collider2D>();
    Physics2D.OverlapBox(transform.position + offset, new Vector2(0.25f, 0.25f), 0f, contactFilter, colliders);
    return colliders;
  }
}