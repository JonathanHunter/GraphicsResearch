namespace GraphicsResearch.Util
{
    using UnityEngine;

    public class Wall : MonoBehaviour
    {
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.GetComponent<DartThrowing.CircleDart>() != null)
            {
                Rigidbody rgbdy = collision.gameObject.GetComponent<Rigidbody>();
                rgbdy.velocity = -rgbdy.velocity;
            }
        }
    }
}
