using System;
using BezierCurves;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Editor
{
    [CustomEditor(typeof(PathCreator))]
    public class PathEditor : UnityEditor.Editor
    {
        private PathCreator _creator;
        private Path _path;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            EditorGUI.BeginChangeCheck();
            if (GUILayout.Button("Create new"))
            {
                Undo.RecordObject(_creator, "Create new");
                _creator.CreatePath();
                _path = _creator.path;
                SceneView.RepaintAll();
            }
            if (GUILayout.Button("Toggle closed"))
            {
                Undo.RecordObject(_creator, "Toggle closed");
                _path.ToggleClosed();
                SceneView.RepaintAll();
            }

            bool autoSetControlPoint = GUILayout.Toggle(_path.AutoSetControlPoints, "Auto Set Control Points");
            if (autoSetControlPoint != _path.AutoSetControlPoints)
            {
                Undo.RecordObject(_creator, "Toggle Auto Set Controls");
                _path.AutoSetControlPoints = autoSetControlPoint;
            }

            if (EditorGUI.EndChangeCheck())
            {
                SceneView.RepaintAll();
            }
            
        }

        private void OnSceneGUI()
        {
            Draw();
            Input();
        }

        private void Input()
        {
            Event guiEvent =  Event.current;
            Vector2 mousePos = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition).origin;

            if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.shift)
            {
                Undo.RecordObject(_creator, "Add segment");
                _path.AddSegment(mousePos);
            }
        }

        private void Draw()
        {
            for (int i = 0; i < _path.NumSegments; i++)
            {
                Vector2[] points = _path.GetPointsInSegment(i);
                Handles.color = Color.black;
                Handles.DrawLine(points[1], points[0]);
                Handles.DrawLine(points[2], points[3]);
                Handles.DrawBezier(points[0], points[3], points[1], points[2], Color.green, null, 2);
            }
            
            
            for (int i = 0; i < _path.NumPoints; i++)
            {
                bool isAnchor = i % 3 == 0;
                Handles.color = isAnchor? Color.red : Color.yellow;
                Vector2 newPos = Handles.FreeMoveHandle(_path[i], Quaternion.identity, isAnchor ? .1f : .05f, Vector2.zero, Handles.CylinderHandleCap);
                if (_path[i] != newPos)
                {
                    Undo.RecordObject(_creator, "Move point"); // Le undo ne marche pas ??  
                    _path.MovePoint(i,newPos);
                }
            }
        }
        private void OnEnable()
        {
            _creator = (PathCreator) target;
            if (_creator.path == null)
            {
                _creator.CreatePath();
            }

            _path = _creator.path;
        }
    }
}
