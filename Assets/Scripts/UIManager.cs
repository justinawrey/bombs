using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
  private Label livesLabel;
  private Label respawnLabel;
  private Label levelLabel;

  private void Awake()
  {
    VisualElement root = GetComponent<UIDocument>().rootVisualElement;
    livesLabel = root.Q<Label>("livesremaining");
    respawnLabel = root.Q<Label>("respawn");
    levelLabel = root.Q<Label>("level");

    levelLabel.text = SceneManager.GetActiveScene().name;
    HideRespawnText();
  }

  private void HideRespawnText()
  {
    respawnLabel.visible = false;
  }

  private void SetRemainingRespawnTime(int num)
  {
    respawnLabel.text = "Respawning in " + num + "...";
  }

  private void Respawn3()
  {
    SetRemainingRespawnTime(3);
  }

  private void Respawn2()
  {
    SetRemainingRespawnTime(2);
  }

  private void Respawn1()
  {
    SetRemainingRespawnTime(1);
  }

  public void SetLivesLabel(int num)
  {
    livesLabel.text = "♡ x " + num;
  }

  public void SetGameOverLabel()
  {
    livesLabel.text = "Game over :( Retrying...";
  }

  public void StartRespawnSequence()
  {
    respawnLabel.visible = true;
    Invoke("Respawn3", 0);
    Invoke("Respawn2", 1);
    Invoke("Respawn1", 2);
    Invoke("HideRespawnText", 3);
  }
}
