using UnityEngine;

namespace Blocks
{
    public class PistonSection : MonoBehaviour
    {
        public float SectionSize;
    
        private Rigidbody2D myRigidbody;
        private BoxCollider2D myCollider;

        private Vector2 myCurrentTarget;
        private float mySpeed = 0;
    
        public void MoveToPostion(Vector2 position, float speed)
        {
            myCurrentTarget = position;
            mySpeed = speed;
        }

        private void Awake()
        {
            myRigidbody = GetComponent<Rigidbody2D>();
            myCollider = GetComponent<BoxCollider2D>();
            SectionSize = myCollider.size.x;
        }

        private void FixedUpdate()
        {
            if (mySpeed == 0)
            {
                return;
            }
        
            if (myCurrentTarget.Equals(myRigidbody.position))
            {
                mySpeed = 0;
                return;
            }

            var newPos = Vector2.MoveTowards(myRigidbody.position, myCurrentTarget, mySpeed * Time.deltaTime);
            myRigidbody.MovePosition(newPos);
        }
    }
}
