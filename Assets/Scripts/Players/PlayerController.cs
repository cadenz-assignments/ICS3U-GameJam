using UnityEngine;

namespace Players
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float speed = 6;
        private Rigidbody2D _rigidbody2D;

        private void Start()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            var dir = Vector2.zero;
            
            if (Input.GetKey(KeyCode.W))
            {
                dir += Vector2.up;
            }

            if (Input.GetKey(KeyCode.A))
            {
                dir += Vector2.left;
            }
            
            if (Input.GetKey(KeyCode.S))
            {
                dir += Vector2.down;
            }
            
            if (Input.GetKey(KeyCode.D))
            {
                dir += Vector2.right;
            }

            dir.Normalize();
            
            _rigidbody2D.MovePosition(_rigidbody2D.position + dir * (Time.deltaTime * speed));
        }
    }
}
