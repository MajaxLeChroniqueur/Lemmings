using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartLevel : MonoBehaviour {

    [SerializeField]
    private GameObject cameraPresentation;

    [SerializeField]
    private Vector3 cameraObjective;

    [SerializeField]
    private Animator fadeToBlack;

    [SerializeField]
    private Animator countToThree;

    [SerializeField]
    private float travellingSpeed;

    public bool canGameStart = false;

	// Use this for initialization
	void Start () {
        canGameStart = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PregameCameraTravel(Vector3 cameraWhereToGo)
    {
        cameraObjective = cameraWhereToGo;
        StartCoroutine("Travelling");
    }

    IEnumerator Travelling()
    {
        yield return new WaitForSeconds(2f);

        while (Vector3.Distance(cameraPresentation.transform.position, cameraObjective) >= 0.1f)
        {
            cameraPresentation.transform.position = Vector3.MoveTowards(cameraPresentation.transform.position, cameraObjective, travellingSpeed);
            yield return null;
        }

        yield return new WaitForSeconds(2f);

        while (Vector3.Distance(cameraPresentation.transform.position, Vector3.zero) >= 0.1f)
        {
            cameraPresentation.transform.position = Vector3.MoveTowards(cameraPresentation.transform.position, Vector3.zero, travellingSpeed);
            yield return null;
        }

        yield return null;

        fadeToBlack.Play("FonduNoir");

        yield return new WaitForSeconds(1f);

        countToThree.Play("Start");

        yield return new WaitForSeconds(6f);

        canGameStart = true;
    }
}
