using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject tutorialParent;
    bool readyForInput = true;
    int state = 0;
    void FixedUpdate()
    {
        if (InputHandler.Instance.any.pressed && readyForInput) {
            AudioHandler.Instance.Play(AudioType.SELECT);
            readyForInput = false;
            if (state == 0)
                StartCoroutine(TutorialTransition());
            if (state == 1) {
                StartCoroutine(SceneTransition());
            }
        }
    }

    private IEnumerator TutorialTransition() {
        VFXHandler.Instance.FadeOut();
        yield return new WaitForSeconds(1f);
        state++;
        tutorialParent.SetActive(true);
        readyForInput = true;
    }

    private IEnumerator SceneTransition() {
        tutorialParent.GetComponent<Animator>().SetTrigger("fade");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("GameScene");
    }
}
