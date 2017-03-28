﻿/*
 *  "GEDKeeper", the personal genealogical database editor.
 *  Copyright (C) 2009-2017 by Sergey V. Zhdanovskih.
 *
 *  This file is part of "GEDKeeper".
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System.Collections.Generic;

namespace GKCommon.SmartGraph
{
    /// <summary>
    /// 
    /// </summary>
    public class Graph : BaseObject, IGraph
    {
        #region Private members

        private sealed class DefaultDataProvider : IDataProvider
        {
            public IVertex CreateVertex()
            {
                return new Vertex();
            }

            public IEdge CreateEdge(IVertex u, IVertex v, int cost, object value)
            {
                return new Edge((Vertex)u, (Vertex)v, cost, value);
            }
        }

        private class PathCandidate
        {
            public readonly IVertex Node;
            public readonly PathCandidate Next;

            public PathCandidate(IVertex node, PathCandidate next)
            {
                Node = node;
                Next = next;
            }
        }

        private readonly IDataProvider fProvider;
        private readonly List<IEdge> fEdgesList;
        private readonly List<IVertex> fVerticesList;
        private readonly Dictionary<string, IVertex> fVerticesDictionary;

        #endregion

        #region Properties

        public IEnumerable<IVertex> Vertices
        {
            get { return fVerticesList; }
        }

        public IEnumerable<IEdge> Edges
        {
            get { return fEdgesList; }
        }

        #endregion

        #region Instance control

        public Graph() : this(new DefaultDataProvider())
        {
        }

        public Graph(IDataProvider provider)
        {
            fProvider = provider;
            fVerticesList = new List<IVertex>();
            fEdgesList = new List<IEdge>();
            fVerticesDictionary = new Dictionary<string, IVertex>();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Clear();
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Data management

        public bool IsEmpty()
        {
            return fVerticesList.Count == 0 && fEdgesList.Count == 0;
        }

        public void Clear()
        {
            foreach (IVertex vertex in fVerticesList)
            {
                vertex.EdgeIn = null;
                vertex.EdgesOut.Clear();
            }

            fEdgesList.Clear();
            fVerticesList.Clear();
            fVerticesDictionary.Clear();
        }

        public IVertex AddVertex(object data)
        {
            IVertex result = fProvider.CreateVertex();
            result.Value = data;
            fVerticesList.Add(result);

            return result;
        }

        public IVertex AddVertex(string sign, object data)
        {
            IVertex result;
            if (fVerticesDictionary.TryGetValue(sign, out result)) {
                return result;
            }

            result = AddVertex(data);
            result.Sign = sign;
            fVerticesDictionary.Add(sign, result);

            return result;
        }

        public bool AddUndirectedEdge(IVertex source, IVertex target, int cost, object srcValue, object tgtValue)
        {
            IEdge edge1 = AddDirectedEdge(source, target, cost, srcValue);
            IEdge edge2 = AddDirectedEdge(target, source, cost, tgtValue);

            return (edge1 != null && edge2 != null);
        }

        public IEdge AddDirectedEdge(string sourceSign, string targetSign, int cost, object edgeValue)
        {
            IVertex source = FindVertex(sourceSign);
            IVertex target = FindVertex(targetSign);

            return AddDirectedEdge(source, target, cost, edgeValue);
        }

        public IEdge AddDirectedEdge(IVertex source, IVertex target, int cost, object edgeValue)
        {
            if (source == null || target == null || source == target) return null;

            IEdge resultEdge = fProvider.CreateEdge(source, target, cost, edgeValue);
            source.EdgesOut.Add(resultEdge);
            fEdgesList.Add(resultEdge);

            return resultEdge;
        }

        public void DeleteVertex(IVertex vertex)
        {
            if (vertex == null) return;

            for (int i = fEdgesList.Count - 1; i >= 0; i--)
            {
                IEdge edge = fEdgesList[i];

                if (edge.Source == vertex || edge.Target == vertex)
                {
                    DeleteEdge(edge);
                }
            }

            fVerticesList.Remove(vertex);
        }

        public void DeleteEdge(IEdge edge)
        {
            if (edge == null) return;

            IVertex src = edge.Source;
            src.EdgesOut.Remove(edge);

            fEdgesList.Remove(edge);
        }

        public IVertex FindVertex(string sign)
        {
            IVertex result;
            fVerticesDictionary.TryGetValue(sign, out result);
            return result;
        }

        #endregion

        #region Pathes search

        public void FindPathTree(IVertex root)
        {
            if (root == null) return;

            // reset path tree
            foreach (IVertex node in fVerticesList)
            {
                node.Dist = int.MaxValue;
                node.Visited = false;
                node.EdgeIn = null;
            }

            // init root
            root.Dist = 0;
            root.Visited = true;
            root.EdgeIn = null;

            PathCandidate topCandidate = new PathCandidate(root, null);

            // processing
            while (topCandidate != null)
            {
                IVertex topNode = topCandidate.Node;
                topCandidate = topCandidate.Next;

                int nodeDist = topNode.Dist;
                topNode.Visited = false;

                foreach (IEdge link in topNode.EdgesOut)
                {
                    IVertex target = link.Target;
                    int newDist = nodeDist + link.Cost;

                    if (newDist < target.Dist)
                    {
                        target.Dist = newDist;
                        target.EdgeIn = link;

                        if (!target.Visited)
                        {
                            target.Visited = true;
                            topCandidate = new PathCandidate(target, topCandidate);
                        }
                    }
                }
            }
        }

        public IEnumerable<IEdge> GetPath(IVertex target)
        {
            List<IEdge> result = new List<IEdge>();

            if (target != null)
            {
                IEdge edge = target.EdgeIn;
                while (edge != null)
                {
                    result.Insert(0, edge);
                    edge = edge.Source.EdgeIn;
                }
            }

            return result;
        }

        #endregion
    }
}
