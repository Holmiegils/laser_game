using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Net.Mime.MediaTypeNames;

namespace VolumetricLines
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [ExecuteInEditMode]
    public class VolumetricLineBehavior : MonoBehaviour
    {
        static readonly Vector3 Average = new Vector3(1f / 3f, 1f / 3f, 1f / 3f);

        #region private variables
        [SerializeField]
        private AudioSource m_warningAudioSource;

        [SerializeField]
        private AudioSource m_blastAudioSource;

        [SerializeField]
        private bool playSound = false;  // Flag to determine if this instance should play sounds

        [SerializeField]
        public Material m_templateMaterial;

        [SerializeField]
        private bool m_doNotOverwriteTemplateMaterialProperties;

        [SerializeField]
        private Vector3 m_startPos;

        [SerializeField]
        private Vector3 m_endPos = new Vector3(0f, 0f, 100f);

        [SerializeField]
        private Color m_lineColor;

        [SerializeField]
        private float m_lineWidth;

        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float m_lightSaberFactor;

        [SerializeField]
        private float m_warningLaserWidth = 5.0f;

        [SerializeField]
        private float m_blastLaserWidth = 15.0f;

        [SerializeField]
        private float m_blinkSpeed = 0.2f;

        [SerializeField]
        private float m_timeTillBlast = 5.0f;

        [SerializeField]
        private float m_blastDuration = 2.0f;

        [SerializeField]
        private float m_timeBetweenBlasts = 2.0f;

        private float m_timer;
        private float m_blinkTimer;
        private bool m_isVisible = true;
        private bool isWarningSoundPlaying = false;

        private Material m_material;
        private MeshFilter m_meshFilter;

        [SerializeField]
        private BoxCollider m_boxCollider; // Serialized BoxCollider reference
        #endregion

        #region properties
        public Material TemplateMaterial
        {
            get { return m_templateMaterial; }
            set { m_templateMaterial = value; }
        }

        public bool DoNotOverwriteTemplateMaterialProperties
        {
            get { return m_doNotOverwriteTemplateMaterialProperties; }
            set { m_doNotOverwriteTemplateMaterialProperties = value; }
        }

        public Color LineColor
        {
            get { return m_lineColor; }
            set
            {
                CreateMaterial();
                if (null != m_material)
                {
                    m_lineColor = value;
                    m_material.color = m_lineColor;
                }
            }
        }

        public float LineWidth
        {
            get { return m_lineWidth; }
            set
            {
                CreateMaterial();
                if (null != m_material)
                {
                    m_lineWidth = value;
                    m_material.SetFloat("_LineWidth", m_lineWidth);
                }
                UpdateBounds();
                UpdateColliderState(); // Update collider state when line width changes
            }
        }

        public float LightSaberFactor
        {
            get { return m_lightSaberFactor; }
            set
            {
                CreateMaterial();
                if (null != m_material)
                {
                    m_lightSaberFactor = value;
                    m_material.SetFloat("_LightSaberFactor", m_lightSaberFactor);
                }
            }
        }

        public Vector3 StartPos
        {
            get { return m_startPos; }
            set
            {
                m_startPos = value;
                SetStartAndEndPoints(m_startPos, m_endPos);
            }
        }

        public Vector3 EndPos
        {
            get { return m_endPos; }
            set
            {
                m_endPos = value;
                SetStartAndEndPoints(m_startPos, m_endPos);
            }
        }
        #endregion

        #region methods
        private void CreateMaterial()
        {
            if (null == m_material || null == GetComponent<MeshRenderer>().sharedMaterial)
            {
                if (null != m_templateMaterial)
                {
                    m_material = Material.Instantiate(m_templateMaterial);
                    GetComponent<MeshRenderer>().sharedMaterial = m_material;
                    SetAllMaterialProperties();
                }
                else
                {
                    m_material = GetComponent<MeshRenderer>().sharedMaterial;
                }
            }
        }

        private void DestroyMaterial()
        {
            if (null != m_material)
            {
                DestroyImmediate(m_material);
                m_material = null;
            }
        }

        private float CalculateLineScale()
        {
            return Vector3.Dot(transform.lossyScale, Average);
        }

        public void UpdateLineScale()
        {
            if (null != m_material)
            {
                m_material.SetFloat("_LineScale", CalculateLineScale());
            }
        }

        private void SetAllMaterialProperties()
        {
            SetStartAndEndPoints(m_startPos, m_endPos);

            if (null != m_material)
            {
                if (!m_doNotOverwriteTemplateMaterialProperties)
                {
                    m_material.color = m_lineColor;
                    m_material.SetFloat("_LineWidth", m_lineWidth);
                    m_material.SetFloat("_LightSaberFactor", m_lightSaberFactor);
                }
                UpdateLineScale();
            }
        }

        private Bounds CalculateBounds()
        {
            var maxWidth = Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);
            var scaledLineWidth = maxWidth * LineWidth * 0.5f;

            var min = new Vector3(
                Mathf.Min(m_startPos.x, m_endPos.x) - scaledLineWidth,
                Mathf.Min(m_startPos.y, m_endPos.y) - scaledLineWidth,
                Mathf.Min(m_startPos.z, m_endPos.z) - scaledLineWidth
            );
            var max = new Vector3(
                Mathf.Max(m_startPos.x, m_endPos.x) + scaledLineWidth,
                Mathf.Max(m_startPos.y, m_endPos.y) + scaledLineWidth,
                Mathf.Max(m_startPos.z, m_endPos.z) + scaledLineWidth
            );

            return new Bounds
            {
                min = min,
                max = max
            };
        }

        public void UpdateBounds()
        {
            if (null != m_meshFilter)
            {
                var mesh = m_meshFilter.sharedMesh;
                UnityEngine.Debug.Assert(null != mesh);
                if (null != mesh)
                {
                    mesh.bounds = CalculateBounds();
                }
            }
        }

        public void SetStartAndEndPoints(Vector3 startPoint, Vector3 endPoint)
        {
            m_startPos = startPoint;
            m_endPos = endPoint;

            Vector3[] vertexPositions = {
                m_startPos,
                m_startPos,
                m_startPos,
                m_startPos,
                m_endPos,
                m_endPos,
                m_endPos,
                m_endPos,
            };

            Vector3[] other = {
                m_endPos,
                m_endPos,
                m_endPos,
                m_endPos,
                m_startPos,
                m_startPos,
                m_startPos,
                m_startPos,
            };

            if (null != m_meshFilter)
            {
                var mesh = m_meshFilter.sharedMesh;
                UnityEngine.Debug.Assert(null != mesh);
                if (null != mesh)
                {
                    mesh.vertices = vertexPositions;
                    mesh.normals = other;
                    UpdateBounds();
                }
            }
        }

        private void UpdateColliderState()
        {
            if (m_boxCollider != null)
            {
                m_boxCollider.enabled = Mathf.Approximately(m_lineWidth, m_blastLaserWidth);
            }
        }
        #endregion

        #region event functions
        void Start()
        {
            Mesh mesh = new Mesh();
            m_meshFilter = GetComponent<MeshFilter>();
            m_meshFilter.mesh = mesh;
            SetStartAndEndPoints(m_startPos, m_endPos);
            mesh.uv = VolumetricLineVertexData.TexCoords;
            mesh.uv2 = VolumetricLineVertexData.VertexOffsets;
            mesh.SetIndices(VolumetricLineVertexData.Indices, MeshTopology.Triangles, 0);
            CreateMaterial();

            // Get the BoxCollider component if not assigned in the Inspector
            if (m_boxCollider == null)
            {
                m_boxCollider = GetComponent<BoxCollider>();
            }

            // Ensure the AudioSources are assigned through the Inspector
            if (playSound)
            {
                if (m_warningAudioSource == null || m_blastAudioSource == null)
                {
                    UnityEngine.Debug.LogError("Please ensure there are at least two AudioSource components attached to this GameObject and assigned in the Inspector.");
                }
            }
        }

        void OnDestroy()
        {
            if (null != m_meshFilter)
            {
                if (UnityEngine.Application.isPlaying)
                {
                    Mesh.Destroy(m_meshFilter.sharedMesh);
                }
                else
                {
                    Mesh.DestroyImmediate(m_meshFilter.sharedMesh);
                }
                m_meshFilter.sharedMesh = null;
            }
            DestroyMaterial();
        }

        void Update()
        {
            // Ensure the logic runs only in play mode
            if (!UnityEngine.Application.isPlaying)
            {
                return;
            }

            m_timer += Time.deltaTime;
            m_blinkTimer += Time.deltaTime;

            if (m_blinkTimer >= m_blinkSpeed && m_timer >= m_timeBetweenBlasts)
            {
                m_blinkTimer = 0;
                m_isVisible = !m_isVisible;
                LineWidth = m_isVisible ? m_warningLaserWidth : 0;
                m_blinkSpeed -= 0.006f;
            }

            if (m_timer >= m_timeTillBlast)
            {
                LineWidth = m_blastLaserWidth;

                if (m_timer >= m_timeTillBlast + m_blastDuration)
                {
                    LineWidth = 0;
                    m_timer = 0;
                    m_blinkTimer = 0;
                    m_blinkSpeed = 0.2f;
                }
            }

            // Play sound when line width reaches specific values, if playSound is true
            if (playSound)
            {
                if (Mathf.Approximately(m_lineWidth, m_warningLaserWidth) && m_warningAudioSource != null && !isWarningSoundPlaying)
                {
                    StartCoroutine(PlayWarningSound());
                }
                if (Mathf.Approximately(m_lineWidth, m_blastLaserWidth) && m_blastAudioSource != null && !m_blastAudioSource.isPlaying)
                {
                    m_blastAudioSource.Play();
                }
            }

            if (transform.hasChanged)
            {
                UpdateLineScale();
                UpdateBounds();
                transform.hasChanged = false;
            }
        }

        IEnumerator PlayWarningSound()
        {
            isWarningSoundPlaying = true;
            m_warningAudioSource.Play();
            yield return new WaitForSeconds(m_warningAudioSource.clip.length);
            isWarningSoundPlaying = false;
        }

        void OnValidate()
        {
            if (string.IsNullOrEmpty(gameObject.scene.name) || string.IsNullOrEmpty(gameObject.scene.path))
            {
                return;
            }
            CreateMaterial();
            SetAllMaterialProperties();
            UpdateBounds();
            UpdateColliderState(); // Update collider state when validating
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(gameObject.transform.TransformPoint(m_startPos), gameObject.transform.TransformPoint(m_endPos));
        }
        #endregion
    }
}
