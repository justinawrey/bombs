using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

public class Movement : MonoBehaviour
{
  private Vector2 input;
  public float speed = 5f;
  private Transform target;
  public LayerMask blockLayer;
  public LayerMask ladderLayer;
  public LayerMask flagLayer;
  private GameObject flag;
  private UIManager UIManager;

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

    // Get the flag
    flag = GameObject.Find("Flag");

    UIManager = GameObject.Find("UI").GetComponent<UIManager>();
    UIManager.SetLivesLabel(lives);
  }

  void Update()
  {
    input.x = Input.GetAxisRaw("Horizontal");
    input.y = Input.GetAxisRaw("Vertical");

    if (Vector3.Distance(transform.position, target.position) < 0.05f)
    {
      // User input left
      if (input.x == -1f && BlockOrLadderInDir(Direction.SE) && !BlockInDir(Direction.Left))
      {
        target.position += new Vector3(-1f, 0f, 0f);
      }

      // User input right
      else if (input.x == 1f && BlockOrLadderInDir(Direction.SW) && !BlockInDir(Direction.Right))
      {
        target.position += new Vector3(1f, 0f, 0f);
      }

      // User input up
      else if (input.y == 1f && IsOnLadder())
      {
        target.position += new Vector3(0f, 1f, 0f);
      }

      // User input down
      else if (input.y == -1f && LadderInDir(Direction.Down))
      {
        target.position += new Vector3(0f, -1f, 0f);
      }

      // Try to pick up / put down flag
      else if (Input.GetKeyDown(KeyCode.E))
      {
        // pick it up if you dont have it
        if (!HasFlag() && IsOnFlag())
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

      // Explode
      else if (Input.GetKeyDown(KeyCode.Q))
      {
        Die();
      }
    }

    transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
  }

  bool BlockInDir(Direction dir)
  {
    return hasOverlapBox(dir, blockLayer);
  }

  bool BlockOrLadderInDir(Direction dir)
  {
    return hasOverlapBox(dir, blockLayer | ladderLayer);
  }

  bool LadderInDir(Direction dir)
  {
    return hasOverlapBox(dir, ladderLayer);
  }

  bool IsOnLadder()
  {
    return hasOverlapBox(Direction.Center, ladderLayer);
  }

  bool IsOnFlag()
  {
    return hasOverlapBox(Direction.Center, flagLayer);
  }

  bool HasFlag()
  {
    return flag.transform.IsChildOf(this.gameObject.transform);
  }

  bool CanPutDownFlag()
  {
    // Cant put it down if youre in the middle of a ladder but
    // you can if youre standing at the bottom of the ladder
    return !IsOnLadder() || BlockInDir(Direction.Down);
  }

  bool CanDie()
  {
    return !HasFlag();
  }

  void GameOver()
  {
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
  }

  void Die()
  {
    lives -= 1;
    if (lives < 0)
    {
      UIManager.SetGameOverLabel();
      Invoke("GameOver", 1);
      return;
    }

    bool destroyedFlag = DestroySurroundings();
    if (destroyedFlag)
    {
      UIManager.SetGameOverLabel();
      Invoke("GameOver", 1);
      return;
    }

    UIManager.SetLivesLabel(lives);
    gameObject.GetComponent<SpriteRenderer>().enabled = false;
    UIManager.StartRespawnSequence();
    Invoke("Respawn", 3);
  }

  void Respawn()
  {
    gameObject.GetComponent<SpriteRenderer>().enabled = true;
    target.position = flag.transform.position;
    transform.position = flag.transform.position;
  }

  List<GameObject> CollectEverythingAt(Direction dir)
  {
    Vector3 offset = new Vector3(0, 0, 0);
    switch (dir)
    {
      case Direction.Up:
        offset = new Vector3(0, 1, 0);
        break;
      case Direction.Down:
        offset = new Vector3(0, -1, 0);
        break;
      case Direction.Left:
        offset = new Vector3(-1, 0, 0);
        break;
      case Direction.Right:
        offset = new Vector3(1, 0, 0);
        break;
    }

    ContactFilter2D contactFilter = new ContactFilter2D().NoFilter();
    contactFilter.layerMask = flagLayer | blockLayer | ladderLayer;
    contactFilter.useLayerMask = true;
    Collider2D[] results = new Collider2D[10];
    List<GameObject> gameObjects = new List<GameObject>();
    var numResults = Physics2D.OverlapBox(transform.position + offset, new Vector2(0.25f, 0.25f), 0f, contactFilter, results);

    for (int i = 0; i < results.Length; i++)
    {
      if (results[i])
      {
        gameObjects.Add(results[i].gameObject);
      }
    }

    return gameObjects;
  }

  bool DestroySurroundings()
  {

    bool destroyedFlag = false;
    List<GameObject>[] objs = {
      CollectEverythingAt(Direction.Up),
      CollectEverythingAt(Direction.Down),
      CollectEverythingAt(Direction.Left),
      CollectEverythingAt(Direction.Right),
      CollectEverythingAt(Direction.Center)
    };

    foreach (List<GameObject> items in objs)
    {
      items.ForEach(delegate (GameObject toDestroy)
      {
        if (toDestroy.name == "Flag")
        {
          destroyedFlag = true;
        }

        Destroy(toDestroy);
      });
    }

    return destroyedFlag;
  }

  bool hasOverlapBox(Direction dir, LayerMask layer)
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
}
