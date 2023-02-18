using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Explosion : MonoBehaviour
{
  private InputManagement input;
  private Collision collision;
  private UIManager UIManager;
  private GameObject flag;
  private Movement movement;

  public int lives = 3;

  void Awake()
  {
    input = GetComponent<InputManagement>();
    collision = GetComponent<Collision>();
    movement = GetComponent<Movement>();
    UIManager = GameObject.Find("UI").GetComponent<UIManager>();
    flag = GameObject.Find("Flag");

    UIManager.SetLivesLabel(lives);
  }

  // Update is called once per frame
  void Update()
  {
    Action action = input.GetAction();

    if (action == Action.Explode)
    {
      Die();
    }
  }

  private bool DestroySurroundings()
  {
    bool destroyedFlag = false;
    List<Collider2D>[] objs = {
      collision.GetCollidersAt(new Vector3(0, 1, 0)),
      collision.GetCollidersAt(new Vector3(0, -1, 0)),
      collision.GetCollidersAt(new Vector3(-1, 0, 0)),
      collision.GetCollidersAt(new Vector3(1, 0, 0)),
      collision.GetCollidersAt(new Vector3(0, 0, 0)),
    };

    foreach (List<Collider2D> colliders in objs)
    {
      colliders.ForEach(delegate (Collider2D collider)
      {
        if (collider.gameObject.name == "Flag")
        {
          destroyedFlag = true;
        }

        Destroy(collider.gameObject);
      });
    }

    return destroyedFlag;
  }

  private void GameOver()
  {
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
  }

  private void Respawn()
  {
    gameObject.GetComponent<SpriteRenderer>().enabled = true;
    movement.target.position = flag.transform.position;
    transform.position = flag.transform.position;
  }

  private void Die()
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
    StartCoroutine(UIManager.RespawnSequence());
    Invoke("Respawn", 3);
  }
}
