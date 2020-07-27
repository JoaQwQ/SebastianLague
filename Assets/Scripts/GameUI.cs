using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public Image fadePlane;
    public GameObject gameOverUI;

    private void Start()
    {
        FindObjectOfType<Player>().OnDeath += OnGameOver;
        gameOverUI.SetActive(false);
    }

    void OnGameOver()
    {
        StartCoroutine(Fade(Color.clear, Color.black, 1));
        gameOverUI.SetActive(true);
        Cursor.visible = true;
    }

    IEnumerator Fade(Color from,Color to,float time)
    {
        float speed = 1 / time;
        float precent=0;

        while (precent<1)
        {
            precent += Time.deltaTime;
            fadePlane.color = Color.Lerp(from,to,precent);
            yield return null;
        }
    }

    public void StarNewGame()
    {
        SceneManager.LoadScene("Main");
    }
}
