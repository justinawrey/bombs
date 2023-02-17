using UnityEngine;

public class FlagToss : MonoBehaviour
{
  private InputManagement input;
  private Collision collision;
  private GameObject flag;

  void Awake()
  {
    input = GetComponent<InputManagement>();
    collision = GetComponent<Collision>();
    flag = GameObject.Find("Flag");
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
        flag.transform.SetParent(this.gameObject.transform);
        flag.transform.position += new Vector3(0, 0.25f, 0f);
        flag.GetComponent<Rigidbody2D>().simulated = false;
        gameObject.transform.Find("ThrowRange").gameObject.GetComponent<SpriteRenderer>().enabled = true;
      }
      // put it down if you have it
      else if (HasFlag() && CanPutDownFlag())
      {
        flag.transform.parent = null;
        flag.transform.position -= new Vector3(0, 0.25f, 0f);
        flag.GetComponent<Rigidbody2D>().simulated = true;
        gameObject.transform.Find("ThrowRange").gameObject.GetComponent<SpriteRenderer>().enabled = false;
      }
    }
  }

  private bool HasFlag()
  {
    return flag.transform.IsChildOf(this.gameObject.transform);
  }

  private bool CanPutDownFlag()
  {
    // Cant put it down if youre in the middle of a ladder but
    // you can if youre standing at the bottom of the ladder
    return !collision.IsOnLadder() || collision.BlockInDir(Direction.Down);
  }
}
