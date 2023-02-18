using UnityEngine;

public class FlagToss : MonoBehaviour
{
  private InputManagement input;
  private Collision collision;
  private GameObject flag;
  private GameObject throwRange;
  private GameObject throwMarker;

  void Awake()
  {
    input = GetComponent<InputManagement>();
    collision = GetComponent<Collision>();
    flag = GameObject.Find("Flag");
    throwRange = gameObject.transform.Find("ThrowRange").gameObject;
    throwMarker = gameObject.transform.Find("ThrowMarker").gameObject;
  }

  // Update is called once per frame
  void Update()
  {
    Action action = input.GetAction();

    if (action == Action.Carry)
    {
      // pick it up if you dont have it
      if (!HasFlag() && collision.IsOnFlag())
      {
        PickupFlag();
      }
      // put it down if you have it
      else if (HasFlag() && CanPutDownFlag())
      {
        PutDownFlag();
      }
    }
    else if (action == Action.Throw && HasFlag())
    {
      PutDownFlag();
      flag.transform.position = throwMarker.transform.position;
    }

    if (HasFlag())
    {
      Vector3 mousePos = input.GetMousePosition();
      Vector3 rounded = new Vector3(Mathf.RoundToInt(mousePos.x), Mathf.RoundToInt(mousePos.y), 0);

      bool onBlock = collision.IsInDir(Direction.Down, collision.blockLayer | collision.ladderLayer, rounded);
      bool inAir = !collision.IsInDir(Direction.Center, collision.blockLayer | collision.ladderLayer, rounded);

      if (throwRange.GetComponent<SpriteRenderer>().bounds.Contains(rounded) && onBlock && inAir)
      {
        throwMarker.transform.position = rounded;
      }
    }
  }

  public bool HasFlag()
  {
    return flag.transform.IsChildOf(this.gameObject.transform);
  }

  private bool CanPutDownFlag()
  {
    // Cant put it down if youre in the middle of a ladder but
    // you can if youre standing at the bottom of the ladder
    return !collision.IsOnLadder() || collision.BlockInDir(Direction.Down);
  }

  private void PickupFlag()
  {
    flag.transform.SetParent(this.gameObject.transform);
    flag.transform.position += new Vector3(0, 0.25f, 0f);
    flag.GetComponent<Rigidbody2D>().simulated = false;
    throwRange.GetComponent<SpriteRenderer>().enabled = true;
    throwMarker.GetComponent<SpriteRenderer>().enabled = true;
  }

  private void PutDownFlag()
  {
    flag.transform.parent = null;
    flag.transform.position -= new Vector3(0, 0.25f, 0f);
    flag.GetComponent<Rigidbody2D>().simulated = true;
    throwRange.GetComponent<SpriteRenderer>().enabled = false;
    throwMarker.GetComponent<SpriteRenderer>().enabled = false;
  }
}
