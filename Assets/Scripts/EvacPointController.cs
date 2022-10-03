using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class EvacPointController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) {
        GameHandler.Instance.GetPlayer().GetComponent<PlayerHandler>().DisableInputs();
        StartCoroutine(EndGameSequence());
        AudioHandler.Instance.Play(AudioType.EVAC);
    }

    private IEnumerator EndGameSequence() {
        yield return new WaitForSeconds(1f);

        VFXHandler.Instance.FadeOut();
        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene("MenuScene");
    }
}
