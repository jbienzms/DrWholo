using UnityEngine;
using EpForceDirectedGraph.cs;
using System.Collections.ObjectModel;

#if UNITY_UWP
#endif

namespace DrWholo.Layout
{
    /// <summary>
    /// A behavior that implements the EpForceDirectedGraph IRenderer interface.
    /// </summary>
    public class LayoutRenderer : MonoBehaviour, IRenderer, IGraphEventListener
    {
        #region Nested Classes
        private class EdgeBehaviorLookup : KeyedCollection<Edge, EdgeBehaviour>
        {
            protected override Edge GetKeyForItem(EdgeBehaviour item)
            {
                return item.Edge;
            }
        }

        private class NodeBehaviorLookup : KeyedCollection<Node, NodeBehaviour>
        {
            protected override Node GetKeyForItem(NodeBehaviour item)
            {
                return item.Node;
            }
        }
        #endregion // Nested Classes

        #region Inspector Variables
        [SerializeField]
        [Tooltip("The prefab to instantiate for each edge.")]
        private GameObject edgePrefab;

        [SerializeField]
        [Tooltip("The prefab to instantiate for each node.")]
        private GameObject nodePrefab;

        [SerializeField]
        [Tooltip("The parent that the graph will be a child of.")]
        private Transform parentTransform;
        #endregion // Inspector Variables

        #region Member Variables
        private EdgeBehaviorLookup edgeLookups = new EdgeBehaviorLookup();
        private NodeBehaviorLookup nodeLookups = new NodeBehaviorLookup();
        protected IForceDirected forceDirected;
        #endregion // Member Variables


        #region Internal Methods
        static private Vector3 ConvertVector(AbstractVector vector)
        {
            return new Vector3(vector.x, vector.y, vector.z);
        }

        private EdgeBehaviour CreateUEdge(Edge edge)
        {
            // Create prefab
            var eo = Instantiate(edgePrefab);

            // Create behavior
            var eb = eo.AddComponent<EdgeBehaviour>();

            // Link behavior to edge
            eb.Edge = edge;

            // Parent it
            eo.transform.SetParent(parentTransform, false);

            // Done
            return eb;
        }

        private NodeBehaviour CreateUNode(Node node)
        {
            // Create prefab
            var no = Instantiate(nodePrefab);

            // Create behavior
            var nb = no.AddComponent<NodeBehaviour>();

            // Link behavior to edge
            nb.Node = node;

            // Parent it
            no.transform.SetParent(parentTransform, false);

            // Done
            return nb;
        }

        private void HandleGraphChange(IForceDirected oldGraph, IForceDirected newGraph)
        {
            // Update event subscriptions
            if (oldGraph != null)
            {
                oldGraph.graph.RemoveGraphListener(this);
            }

            if (newGraph != null)
            {
                newGraph.graph.AddGraphListener(this);
            }
        }

        private void HandleGraphContentChanged()
        {
            // Remove stale edges
            for (int i = edgeLookups.Count - 1; i > 0; i--)
            {
                var el = edgeLookups[i];
                if ((forceDirected == null) || (!forceDirected.graph.edges.Contains(el.Edge)))
                {
                    edgeLookups.RemoveAt(i);
                    Destroy(el.gameObject);
                }
            }

            // Remove stale nodes
            for (int i = nodeLookups.Count - 1; i > 0; i--)
            {
                var nl = nodeLookups[i];
                if ((forceDirected == null) || (!forceDirected.graph.nodes.Contains(nl.Node)))
                {
                    nodeLookups.RemoveAt(i);
                    Destroy(nl.gameObject);
                }
            }

            // If no graph, done
            if (forceDirected == null) { return; }

            // Add new nodes
            foreach (var node in forceDirected.graph.nodes)
            {
                if (!nodeLookups.Contains(node))
                {
                    var nn = CreateUNode(node);
                    nodeLookups.Add(nn);
                }
            }

            // Add new edges
            foreach (var edge in forceDirected.graph.edges)
            {
                if (!edgeLookups.Contains(edge))
                {
                    var ne = CreateUEdge(edge);
                    edgeLookups.Add(ne);
                }
            }
        }
        #endregion // Internal Methods

        #region IGraphEventListener Members
        void IGraphEventListener.GraphChanged()
        {
            HandleGraphContentChanged();
        }
        #endregion // IGraphEventListener Members

        #region IRenderer Members
        void IRenderer.Clear()
        {
            // Clear is not used in Unity as the image is automatically cleared by the renderer
        }

        void IRenderer.Draw(float iTimeStep)
        {
            // Cannot continue if we don't have a directed graph
            if (forceDirected == null) { return; }

            // Calculate the graph
            forceDirected.Calculate(iTimeStep);

            // Update all nodes
            forceDirected.EachNode(delegate (Node node, Point point)
            {
                if (nodeLookups.Contains(node))
                {
                    // Get the game object
                    var no = nodeLookups[node].gameObject;

                    // Position
                    no.transform.localPosition = ConvertVector(point.position);
                }
            });

            // Update all edges
            forceDirected.EachEdge(delegate (Edge edge, Spring spring)
            {
                if (edgeLookups.Contains(edge))
                {
                    // Get the game object
                    var eo = edgeLookups[edge].gameObject;

                    // Get line renderer
                    var lr = eo.GetComponent<LineRenderer>();

                    // Position points
                    var positions = new Vector3[] { ConvertVector(spring.point1.position), ConvertVector(spring.point2.position) };
                    lr.SetPositions(positions);
                }
            });
        }
        #endregion // IRenderer Members

        #region Behaviour Overrides
        // Use this for initialization
        void Start()
        {
            // If no parent specified, use whatever object this behavior is attached to.
            if (parentTransform == null)
            {
                parentTransform = gameObject.transform;
            }
        }

        // Update is called once per frame
        void Update()
        {
            ((IRenderer)this).Draw(Time.deltaTime);
        }
        #endregion // Behaviour Overrides

        #region Public Properties
        /// <summary>
        /// Gets or sets the <see cref="IForceDirected"/> graph to be rendered.
        /// </summary>
        public IForceDirected ForceDirected
        {
            get
            {
                return forceDirected;
            }
            set
            {
                if (forceDirected != value)
                {
                    HandleGraphChange(forceDirected, value);
                    forceDirected = value;
                    HandleGraphContentChanged();
                }
            }
        }
        #endregion // Public Properties
    }

}
