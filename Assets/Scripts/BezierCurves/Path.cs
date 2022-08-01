using System.Collections.Generic;
using UnityEngine;

namespace BezierCurves
{
    [System.Serializable]
    public class Path
    {
        [SerializeField, HideInInspector] 
        private List<Vector2> points;

        public Path(Vector2 center)
        {
            points = new List<Vector2>
            {
                center + Vector2.left,
                center + (Vector2.left + Vector2.up) * .5f,
                center + (Vector2.right + Vector2.down) * .5f,
                center + Vector2.right
            };
        }

        public void Init(Vector2 center)
        {
            points = new List<Vector2>
            {
                center + Vector2.left,
                center + (Vector2.left + Vector2.up) * .5f,
                center + (Vector2.right + Vector2.down) * .5f,
                center + Vector2.right
            };
        }
        
        public Vector2 this[int i] => points[i]; //indexer, permet d'utiliser "[]" sur la classe, ici : la pos du point souhaité

        public int NumPoints => points.Count;

        public int NumSegments => (points.Count - 4) / 3 + 1; // -4 pour enlever les 2 points du start et les 2 points de l'arrivée, /3 parce que 3 points par segments, +1 pour compter le segment initial -> ex : 13 points en tout : 13 - 4 -> 9, /3 -> 3, +1 -> 4 segments

        public void AddSegment(Vector2 anchorPos)
        {
            points.Add(points[^1] * 2 - points[^2]); //points.Add(points[points.Count - 1] * 2 - points[points.Count - 2]);-> ^1 = dernier element, ^2 = avant dernier element
            points.Add((points[^1] + anchorPos) * .5f);
            points.Add(anchorPos);
        }

        public Vector2[] GetPointsInSegment(int i)
        {
            return new[] {points[i * 3], points[i * 3 + 1], points[i * 3 + 2], points[i * 3 + 3]};
        }

        public void MovePoint(int i, Vector2 pos)
        {
            Vector2 deltaMove = pos - points[i];
            points[i] = pos;
            if (i % 3 == 0)
            {// on bouge un anchor point

                if (i + 1 < points.Count)
                {
                    points[i + 1] += deltaMove;
                }

                if (i - 1 >= 0)
                {
                    points[i + -1] += deltaMove;   
                }
            }
            else
            {
                bool nextPointIsAnchor = (i + 1) % 3 == 0;
                int correspondingControlIndex = nextPointIsAnchor ? i + 2 : i - 2;
                int anchorIndex = nextPointIsAnchor ? i + 1 : i - 1;

                if (correspondingControlIndex >= 0 && correspondingControlIndex < points.Count)
                {
                    float dist = (points[anchorIndex] - points[correspondingControlIndex]).magnitude;
                    Vector2 dir = (points[anchorIndex] - pos).normalized;
                    points[correspondingControlIndex] = points[anchorIndex] + dir * dist;
                }
            }
        }
    }
}
