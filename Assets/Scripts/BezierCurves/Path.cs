using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Path {

    [SerializeField, HideInInspector]
    private List<Vector2> points;
    [SerializeField, HideInInspector]
    private bool isClosed;
    [SerializeField, HideInInspector]
    private bool autoSetControlPoints;

    public Path(Vector2 centre)
    {
        points = new List<Vector2>
        {
            centre+Vector2.left,
            centre+(Vector2.left+Vector2.up)*.5f,
            centre + (Vector2.right+Vector2.down)*.5f,
            centre + Vector2.right
        };
    }

    public Vector2 this[int i] => points[i];

    public bool AutoSetControlPoints
    {
        get => autoSetControlPoints;
        set
        {
            if (autoSetControlPoints == value) return;
            
            autoSetControlPoints = value;
            if (autoSetControlPoints)
            {
                AutoSetAllControlPoints();
            }
        }
    }

    public int NumPoints => points.Count;

    public int NumSegments => points.Count/3;

    public void AddSegment(Vector2 anchorPos)
    {
        points.Add(points[^1] * 2 - points[^2]);
        points.Add((points[^1] + anchorPos) * .5f);
        points.Add(anchorPos);

        if (autoSetControlPoints)
        {
            AutoSetAllAffectedControlPoints(points.Count - 1);
        }
    }

    public Vector2[] GetPointsInSegment(int i)
    {
        return new[] { points[i * 3], points[i * 3 + 1], points[i * 3 + 2], points[LoopIndex(i * 3 + 3)] };
    }

    public void MovePoint(int i, Vector2 pos)
    {
        var deltaMove = pos - points[i];

        if (i % 3 != 0 && autoSetControlPoints) return;
        points[i] = pos;

        if (autoSetControlPoints)
        {
            AutoSetAllAffectedControlPoints(i);
        }
        else
        {
            if (i % 3 == 0)
            {
                if (i + 1 < points.Count || isClosed)
                {
                    points[LoopIndex(i + 1)] += deltaMove;
                }
                if (i - 1 >= 0 || isClosed)
                {
                    points[LoopIndex(i - 1)] += deltaMove;
                }
            }
            else
            {
                var nextPointIsAnchor = (i + 1) % 3 == 0;
                var correspondingControlIndex = (nextPointIsAnchor) ? i + 2 : i - 2;
                var anchorIndex = (nextPointIsAnchor) ? i + 1 : i - 1;

                if ((correspondingControlIndex < 0 || correspondingControlIndex >= points.Count) && !isClosed) return;
                
                var dst = (points[LoopIndex(anchorIndex)] - points[LoopIndex(correspondingControlIndex)]).magnitude;
                var dir = (points[LoopIndex(anchorIndex)] - pos).normalized;
                points[LoopIndex(correspondingControlIndex)] = points[LoopIndex(anchorIndex)] + dir * dst;
            }
        }
    }

    public void ToggleClosed()
    {
        isClosed = !isClosed;

        if (isClosed)
        {
            points.Add(points[^1] * 2 - points[^2]);
            points.Add(points[0] * 2 - points[1]);
            if (!autoSetControlPoints) return;
            AutoSetAnchorControlPoints(0);
            AutoSetAnchorControlPoints(points.Count - 3);
        }
        else
        {
            points.RemoveRange(points.Count - 2, 2);
            if (autoSetControlPoints)
            {
                AutoSetStartAndEndControls();
            }
        }
    }

    private void AutoSetAllAffectedControlPoints(int updatedAnchorIndex)
    {
        for (var i = updatedAnchorIndex-3; i <= updatedAnchorIndex +3; i+=3)
        {
            if (i >= 0 && i < points.Count || isClosed)
            {
                AutoSetAnchorControlPoints(LoopIndex(i));
            }
        }

        AutoSetStartAndEndControls();
    }

    private void AutoSetAllControlPoints()
    {
        for (var i = 0; i < points.Count; i+=3)
        {
            AutoSetAnchorControlPoints(i);
        }

        AutoSetStartAndEndControls();
    }

    private void AutoSetAnchorControlPoints(int anchorIndex)
    {
        var anchorPos = points[anchorIndex]; //pos du points ciblé
        var dir = Vector2.zero; //direction par défaut
        var neighbourDistances = new float[2]; //voisin du point

        if (anchorIndex - 3 >= 0 || isClosed) //si il est dans la boucle sans être le premier point
        {
            var offset = points[LoopIndex(anchorIndex - 3)] - anchorPos;
            dir += offset.normalized;
            neighbourDistances[0] = offset.magnitude;
        }
		if (anchorIndex + 3 >= 0 || isClosed)
		{
			var offset = points[LoopIndex(anchorIndex + 3)] - anchorPos; //si il est dans la boucle sans être le dernier
			dir -= offset.normalized;
			neighbourDistances[1] = -offset.magnitude;
		}

        dir.Normalize();

        for (var i = 0; i < 2; i++)
        {
            var controlIndex = anchorIndex + i * 2 - 1;
            if (controlIndex >= 0 && controlIndex < points.Count || isClosed)
            {
                points[LoopIndex(controlIndex)] = anchorPos + dir * (neighbourDistances[i] * .5f);
            }
        }
    }

    private void AutoSetStartAndEndControls()
    {
        if (isClosed) return;
        points[1] = (points[0] + points[2]) * .5f;
        points[^2] = (points[^1] + points[^3]) * .5f;
    }

    private int LoopIndex(int i)
    {
        return (i + points.Count) % points.Count;
    }

}