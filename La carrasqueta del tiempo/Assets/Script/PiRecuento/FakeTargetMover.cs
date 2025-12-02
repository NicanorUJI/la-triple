using UnityEngine;

public class FakeTargetMover : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        float move = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        transform.position += new Vector3(move, 0, 0);
    }
}

