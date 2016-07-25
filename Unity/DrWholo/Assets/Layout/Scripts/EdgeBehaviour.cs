using EpForceDirectedGraph.cs;
using UnityEngine;

namespace DrWholo.Layout
{
    /// <summary>
    /// A behaviour that encapsulates a EpForceDirectedGraph Edge.
    /// </summary>
    public class EdgeBehaviour : MonoBehaviour
    {
        #region Member Variables
        private Edge edge;
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
        #endregion // Behaviour Overrides

        #region Public Properties
        /// <summary>
        /// Gets the node that the behavior contains.
        /// </summary>
        public Edge Edge { get { return edge; } set { edge = value; } }
        #endregion // Public Properties
    }
}