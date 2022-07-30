using UnityEngine;

namespace BezierCurves
{
    public class PathCreator : MonoBehaviour {

        [HideInInspector] [SerializeField]
        public Path path;

        public void CreatePath()
        {
            path = gameObject.AddComponent<Path>(); //path = new Path(transform.position); est interdit, on Add le component puis on appelle le constructeur nous même...
            path.Init(transform.position);
        }
    }
}