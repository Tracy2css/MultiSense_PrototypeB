using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace VRUIP.Drawing
{
    public class Pen : A_Grabbable
    {
        [Header("Pen Properties")]
        [SerializeField] private float lineWidth = 0.02f;
        [SerializeField] private float drawRate = 0.01f;

        [Header("Components")]
        [SerializeField] private Transform penTip;
        [SerializeField] private Material penMaterial;
        [SerializeField] private Material drawingMaterial;
        
        private Transform _drawingContainer;
        private List<Vector3> _positions = new();
        private bool _draw;
        private LineRenderer _currentDrawing;
        private Color _color = Color.red;

        public Color Color
        {
            get => _color;
            set => SetColor(value);
        }

        protected override void Start()
        {
            base.Start();
            SetupPen();
        }

        protected override void Update()
        {
            base.Update();
            if (_draw && _currentDrawing != null)
            {
                // Get the position of the pen tip from the controller
                Vector3 penPos = penTip.transform.position;

                // Check if we need to add a new position to the list
                if (_positions.Count == 0 || Vector3.Distance(_positions[_positions.Count - 1], penPos) > drawRate)
                {
                    // Add the new position to the list
                    _positions.Add(penPos);

                    // Update the line renderer
                    _currentDrawing.positionCount = _positions.Count;
                    _currentDrawing.SetPositions(_positions.ToArray());
                }
            }
            else if (_draw && _currentDrawing == null)
            {
                StartNewDrawing();
            }
        }

        private void SetupPen()
        {
            // Register the actions
            RegisterActivated(() => _draw = true);
            RegisterDeactivated(() =>
            {
                _draw = false;
                if (_currentDrawing != null) SetDrawingCollider(_currentDrawing);
                _currentDrawing = null;
            });
            // Setup the initial color
            SetColor(Color.red);
            if (_drawingContainer == null) _drawingContainer = new GameObject("Drawing Container").transform;
            _drawingContainer.SetParent(transform.parent);
        }

        private void StartNewDrawing()
        {
            _positions.Clear();
            _currentDrawing = new GameObject().AddComponent<LineRenderer>();
            _currentDrawing.transform.SetParent(_drawingContainer);
            _currentDrawing.name = "Drawing";
            _currentDrawing.startWidth = _currentDrawing.endWidth = lineWidth;
            _currentDrawing.material = drawingMaterial;
            _currentDrawing.startColor = _currentDrawing.endColor = _color;
        }

        private void OnTriggerEnter(Collider other)
        {
            var colorChanger = other.GetComponent<PenColorChanger>();
            if (colorChanger != null)
            {
                SetColor(colorChanger.color);
            }
        }

        private void OnDisable()
        {
            if (_drawingContainer != null) _drawingContainer.gameObject.SetActive(false);
        }
        
        private void OnEnable()
        {
            if (_drawingContainer != null) _drawingContainer.gameObject.SetActive(true);
        }

        // Set the color of this pen.
        private void SetColor(Color color)
        {
            _color = color;
            penMaterial.color = color;
        }

        /// <summary>
        /// Add a collider to the line renderer/drawing.
        /// </summary>
        /// <param name="lineRenderer"></param>
        private void SetDrawingCollider(LineRenderer lineRenderer)
        {
            var meshCollider = new GameObject("DrawingCollider").AddComponent<MeshCollider>();
            meshCollider.sharedMesh = new Mesh();
            lineRenderer.BakeMesh(meshCollider.sharedMesh);
            meshCollider.transform.SetParent(lineRenderer.gameObject.transform, true);
            meshCollider.transform.position = new Vector3(0, 0, 0);
            meshCollider.AddComponent<Drawing>();
        }
    }
}
