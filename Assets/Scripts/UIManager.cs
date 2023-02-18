using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIManager : MonoBehaviour
{
  private Label livesLabel;
  private Label respawnLabel;
  private Label levelLabel;

  private void Awake()
  {
    VisualElement root = GetComponent<UIDocument>().rootVisualElement;
    livesLabel = root.Q<Label>("lives");
    respawnLabel = root.Q<Label>("respawn");
    levelLabel = root.Q<Label>("level");

    levelLabel.text = SceneManager.GetActiveScene().name;
    respawnLabel.visible = false;
  }

  private void SetRemainingRespawnTime(int num)
  {
    respawnLabel.text = "Respawning in " + num + "...";
  }

  public void SetLivesLabel(int num)
  {
    livesLabel.text = "♡ x " + num;
  }

  public void SetGameOverLabel()
  {
    livesLabel.text = "Game over :( Retrying...";
  }

  public IEnumerator RespawnSequence()
  {
    respawnLabel.visible = true;
    SetRemainingRespawnTime(3);
    yield return new WaitForSeconds(1);
    SetRemainingRespawnTime(2);
    yield return new WaitForSeconds(1);
    SetRemainingRespawnTime(1);
    yield return new WaitForSeconds(1);
    respawnLabel.visible = false;
  }
}
