using UnityEngine;

enum Action
{
  MoveLeft,
  MoveRight,
  MoveUp,
  MoveDown,
  Carry,
  Explode,
  None
}

class InputManagement : MonoBehaviour
{
  public Action GetAction()
  {
    float x = Input.GetAxisRaw("Horizontal");
    if (Mathf.Abs(x) == 1)
    {
      return x == 1 ? Action.MoveRight : Action.MoveLeft;
    }

    float y = Input.GetAxisRaw("Vertical");
    if (Mathf.Abs(y) == 1)
    {
      return y == 1 ? Action.MoveUp : Action.MoveDown;
    }

    if (Input.GetKeyDown(KeyCode.E))
    {
      return Action.Carry;
    }

    if (Input.GetKeyDown(KeyCode.Q))
    {
      return Action.Explode;
    }

    return Action.None;
  }
}