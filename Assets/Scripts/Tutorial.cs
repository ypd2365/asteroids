using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{
    public GameObject player;
    public GameObject asteroids;
    public GameObject startGame;

    // Start is called before the first frame update
    void Start()
    {

        StartCoroutine(Animation());


    }

    IEnumerator Animation()
    {
        StartCoroutine(SmoothLerp(player, new Vector3(0, -0.9f, 0), 3.0f));
        yield return new WaitForSeconds(3.01f);
        StartCoroutine(ZoomIn());
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(SmoothLerp(asteroids, new Vector3(0, 3f, 0), 2.0f));
        yield return new WaitForSeconds(2.01f);
        startGame.SetActive(true);

    }

    private IEnumerator SmoothLerp(GameObject obj,Vector3 pos, float time)
    {
        Vector3 startingPos = obj.transform.position;
        Vector3 finalPos = pos;
        float elapsedTime = 0;

         while(elapsedTime < time)
         {
            obj.transform.position = Vector3.Lerp(startingPos, finalPos, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
         }
    }

    IEnumerator ZoomIn()
    {
        while (Camera.main.orthographicSize < 6)
        {
            yield return new WaitForSeconds(0.01f);
            Camera.main.orthographicSize += 0.1f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && startGame.activeSelf)
        {
            SceneManager.LoadScene("gameScreen");
        }
    }
}
