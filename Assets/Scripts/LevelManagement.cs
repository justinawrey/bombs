using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManagement : MonoBehaviour
{
  private void OnTriggerEnter2D(Collider2D other)
  {
    if (other.GetType() == typeof(CircleCollider2D) && other.name == "Bomb")
    {
      Scene scene = SceneManager.GetActiveScene();
      int nextLevel = Int32.Parse(scene.name.Substring(6)) + 1;
      SceneManager.LoadScene("Level " + nextLevel);
    }
  }
}
