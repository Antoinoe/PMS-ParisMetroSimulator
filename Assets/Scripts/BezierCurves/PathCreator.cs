using UnityEngine;

namespace BezierCurves
{
    public class PathCreator : MonoBehaviour {

        [HideInInspector] [SerializeField]
        public Path path;

        public void CreatePath()
        {
            path = new Path(transform.position);
        }
    }
}