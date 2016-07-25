using EpForceDirectedGraph.cs;
using UnityEngine;

namespace DrWholo.Layout
{
    /// <summary>
    /// A behaviour that encapsulates a EpForceDirectedGraph Node.
    /// </summary>
    public class NodeBehaviour : MonoBehaviour
    {
        #region Member Variables
        private Node node;
        #endregion // Member Variables

        #region Behaviour Overrides
        //// Use this for initialization
        //void Start()
        //{

        //}

        //// Update is called once per frame
        //void Update()
        //{

        //}
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the node that the behavior contains.
        /// </summary>
        public Node Node { get { return node; } set { node = value; } }
        #endregion // Public Properties
    }
}