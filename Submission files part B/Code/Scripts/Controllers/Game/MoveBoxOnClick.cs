using System.Collections;
using UnityEngine;

public class MoveBoxOnClick : MonoBehaviour
{
    public Transform modelTarget;
    public Commoditie boxCommodite;
    public float duration = 2.0f;
    private Stage3Controller stage3Controller;
    private bool isMoved = false;
    private void OnEnable()
    {
        stage3Controller = FindAnyObjectByType<Stage3Controller>();
    }
    private void OnMouseDown()
    {
        TriggerMoveBox();
    }



    public void TriggerMoveBox()
    {
        if (isMoved || transform.childCount != 6 || !stage3Controller.OnDropCommo(boxCommodite)) return;  // 6 is the text as child
        isMoved = true;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = new Vector3(modelTarget.position.x, modelTarget.position.y + 2, modelTarget.position.z);
        transform.parent = null;
        StartCoroutine(MoveCube(startPosition, targetPosition, duration));
    }

    private IEnumerator MoveCube(Vector3 start, Vector3 end, float duration)
    {
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;

            // Update the position
            transform.position = Vector3.Lerp(start, end, t);

            // Move it in an arc to look like it's flying
            transform.position += new Vector3(0, Mathf.Sin(t * Mathf.PI) * 2, 0);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = end;
    }
}
