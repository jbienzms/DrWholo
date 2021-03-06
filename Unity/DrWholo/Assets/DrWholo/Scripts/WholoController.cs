﻿using UnityEngine;
using System.Collections;
using DrWholo.Layout;
using EpForceDirectedGraph.cs;
using Adept;

#if WINDOWS_UWP
using DrWholoLib;
using System.Threading.Tasks;
#endif

namespace DrWholo
{
    /// <summary>
    /// Main logic controller for the Dr. Wholo app.
    /// </summary>
    public class WholoController : MonoBehaviour
    {
        #region Inspector Variables
        [SerializeField]
        [Tooltip("The Layout Renderer for displaying the graph.")]
        private LayoutRenderer layoutRenderer;
        #endregion // Inspector Variables

        private void LoadDemoGraph()
        {
            // Create graph
            var graph = new Graph();

            // Add nodes
            var a = graph.AddNode(new Node("A"));
            var b = graph.AddNode(new Node("B"));
            var c = graph.AddNode(new Node("C"));

            // Add edges
            var ab = graph.AddEdge(new Edge("A-B", a, b, new EdgeData() { length = 0.33f }));
            var ac = graph.AddEdge(new Edge("A-C", a, c, new EdgeData() { length = 0.33f }));

            // Create force directed
            var fdg = new ForceDirected3D(graph, 150, 10, 0.5f);

            // Set it to the renderer
            layoutRenderer.ForceDirected = fdg;
        }

        #if WINDOWS_UWP
        private async Task LoadDataAsync()
        {
            var service = DrWholoService.Instance;

            // Authenticate
            try
            {
                service.AllowInteractiveSignIn = false;
                await service.SignInAync();
            }
            catch
            {
                // Switch to Auth view (replacing current view)
                await AppViewManager.Views["Auth"].SwitchAsync();
            }
        }
        #endif

        private void LoadRealGraph()
        {
            #if WINDOWS_UWP
            var t = LoadDataAsync();
            #endif
        }

        #region Behaviour Overrides
        // Use this for initialization
        void Start()
        {
            LoadDemoGraph();
            LoadRealGraph();
        }

        //// Update is called once per frame
        //void Update()
        //{

        //}
        #endregion // Behaviour Overrides


    }
}