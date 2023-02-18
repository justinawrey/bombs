using UnityEngine;

public class Movement : MonoBehaviour
{
  public float speed = 5f;
  public Transform target;
  private InputManagement input;
  private Collision collision;
  private FlagToss flagToss;

  public int lives = 3;

  void Awake()
  {
    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
    cube.name = "MovePoint";
    cube.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
    target = cube.transform;
    target.position = transform.position;

    // Destroy the auto gen'd mesh renderer and mesh filter
    Destroy(cube.GetComponent<MeshRenderer>());
    Destroy(cube.GetComponent<MeshFilter>());

    collision = GetComponent<Collision>();
    flagToss = GetComponent<FlagToss>();
    input = GetComponent<InputManagement>();
  }

  void Update()
  {
    Action action = input.GetAction();

    if (Vector3.Distance(transform.position, target.position) < 0.05f)
    {
      // User input left
      if (action == Action.MoveLeft && collision.BlockOrLadderInDir(Direction.SE) && !collision.BlockInDir(Direction.Left))
      {
        target.position += new Vector3(-1f, 0f, 0f);
      }

      // User input right
      else if (action == Action.MoveRight && collision.BlockOrLadderInDir(Direction.SW) && !collision.BlockInDir(Direction.Right))
      {
        target.position += new Vector3(1f, 0f, 0f);
      }

      // User input up
      else if (action == Action.MoveUp && collision.IsOnLadder() && !flagToss.HasFlag())
      {
        target.position += new Vector3(0f, 1f, 0f);
      }

      // User input down
      else if (action == Action.MoveDown && collision.LadderInDir(Direction.Down) && !flagToss.HasFlag())
      {
        target.position += new Vector3(0f, -1f, 0f);
      }
    }

    transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
  }
}
