using UnityEngine;

namespace BezierCurves
{
    public class PathCreator : MonoBehaviour {

        [HideInInspector] [SerializeField]
        public Path path;

        public void CreatePath()
        {
            path = new Path(transform.position); //path = new Path(transform.position); est interdit, on Add le component puis on appelle le constructeur nous même...
        }
    }
}