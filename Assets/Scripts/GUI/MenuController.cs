using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    bool started;
    void FixedUpdate()
    {
        if (InputHandler.Instance.any.pressed && !started) {
            started = true;
            StartCoroutine(SceneTransition());
        }
    }

    private IEnumerator SceneTransition() {
        VFXHandler.Instance.FadeOut();
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("GameScene");
    }
}
