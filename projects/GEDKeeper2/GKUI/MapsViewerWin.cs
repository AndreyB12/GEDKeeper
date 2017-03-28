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

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using GKCommon;
using GKCommon.GEDCOM;
using GKCore;
using GKCore.Geocoding;
using GKCore.Interfaces;
using GKUI.Controls;

namespace GKUI
{
    /// <summary>
    /// 
    /// </summary>
    public sealed partial class MapsViewerWin : Form, ILocalization
    {
        private class PlaceRef
        {
            public readonly DateTime Date;
            public readonly GEDCOMCustomEvent Event;

            public PlaceRef(GEDCOMCustomEvent evt)
            {
                Event = evt;
                Date = (evt == null) ? new DateTime(0) : evt.Date.GetDateTime();
            }
        }

        private class MapPlace : BaseObject
        {
            public string Name;
            public readonly IList<GeoPoint> Points;
            public readonly ExtList<PlaceRef> PlaceRefs;

            public MapPlace()
            {
                Points = new List<GeoPoint>();
                PlaceRefs = new ExtList<PlaceRef>(false);
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    PlaceRefs.Dispose();
                }
                base.Dispose(disposing);
            }
        }

        private readonly TreeNode fBaseRoot;
        private readonly GKMapBrowser fMapBrowser;
        private readonly ExtList<GeoPoint> fMapPoints;
        private readonly ExtList<MapPlace> fPlaces;
        private readonly List<GEDCOMRecord> fSelectedPersons;
        private readonly IBaseWindow fBase;
        private readonly GEDCOMTree fTree;

        private bool IsSelected(GEDCOMRecord iRec)
        {
            bool res = (fSelectedPersons == null || (fSelectedPersons != null && fSelectedPersons.IndexOf(iRec) >= 0));
            return res;
        }

        private void PlacesLoad()
        {
            try
            {
                fMapBrowser.InitMap();

                cmbPersons.BeginUpdate();
                tvPlaces.BeginUpdate();
                fBase.ProgressInit(LangMan.LS(LSID.LSID_LoadingLocations), fTree.RecordsCount);
                try
                {
                    fPlaces.Clear();
                    cmbPersons.Items.Clear();
                    cmbPersons.Sorted = false;
                    cmbPersons.Items.Add(new GKComboItem(LangMan.LS(LSID.LSID_NotSelected), null));

                    int num = fTree.RecordsCount;
                    for (int i = 0; i < num; i++) {
                        GEDCOMRecord rec = fTree[i];
                        bool res = rec is GEDCOMIndividualRecord && IsSelected(rec);

                        if (res) {
                            GEDCOMIndividualRecord ind = rec as GEDCOMIndividualRecord;
                            int pCnt = 0;

                            int num2 = ind.Events.Count;
                            for (int j = 0; j < num2; j++)
                            {
                                GEDCOMCustomEvent ev = ind.Events[j];
                                if (ev.Place.StringValue != "") {
                                    AddPlace(ev.Place, ev);
                                    pCnt++;
                                }
                            }

                            if (pCnt > 0) {
                                cmbPersons.Items.Add(new GKComboItem(GKUtils.GetNameString(ind, true, false) + " [" + pCnt.ToString() + "]", ind));
                            }
                        }

                        fBase.ProgressStep();
                    }

                    fBaseRoot.ExpandAll();
                    cmbPersons.Sorted = true;

                    btnSelectPlaces.Enabled = true;
                }
                finally
                {
                    fBase.ProgressDone();
                    tvPlaces.EndUpdate();
                    cmbPersons.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Logger.LogWrite("MapsViewerWin.PlacesLoad(): " + ex.Message);
            }
        }

        private void PreparePointsList(ExtList<GeoPoint> gmapPoints, bool byPerson)
        {
            fMapBrowser.BeginUpdate();
            try
            {
                fMapBrowser.ClearPoints();

                int num = gmapPoints.Count;
                for (int i = 0; i < num; i++)
                {
                    GeoPoint pt = gmapPoints[i];
                    string stHint = pt.Hint;
                    if (byPerson)
                    {
                        stHint = stHint + " [" + pt.Date.ToString() + "]";
                    }

                    fMapBrowser.AddPoint(pt.Latitude, pt.Longitude, stHint);
                }
                fMapBrowser.ZoomToBounds();
            }
            finally
            {
                fMapBrowser.EndUpdate();
            }
        }

        private void radTotal_Click(object sender, EventArgs e)
        {
            chkBirth.Enabled = radTotal.Checked;
            chkDeath.Enabled = radTotal.Checked;
            chkResidence.Enabled = radTotal.Checked;
            cmbPersons.Enabled = radSelected.Checked;
            chkLinesVisible.Enabled = radSelected.Checked;
        }

        private void MapsViewerWin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) Close();
        }

        // TODO: localize?
        private void btnSaveImage_Click(object sender, EventArgs e)
        {
            string filter1 = "Image files|*.jpg";

            string fileName = UIHelper.GetSaveFile("", "", filter1, 2, "jpg", "");
            if (!string.IsNullOrEmpty(fileName))
            {
                fMapBrowser.SaveSnapshot(fileName);
            }
        }

        private void btnSelectPlaces_Click(object sender, EventArgs e)
        {
            GEDCOMIndividualRecord ind = null;

            bool condBirth = false;
            bool condDeath = false;
            bool condResidence = false;

            if (radTotal.Checked) {
                condBirth = chkBirth.Checked;
                condDeath = chkDeath.Checked;
                condResidence = chkResidence.Checked;
            } else if (radSelected.Checked) {
                if (cmbPersons.SelectedIndex >= 0)
                {
                    GKComboItem item = (GKComboItem)cmbPersons.Items[cmbPersons.SelectedIndex];
                    ind = (item.Tag as GEDCOMIndividualRecord);
                }
            }

            fMapBrowser.ShowLines = (ind != null && chkLinesVisible.Checked);
            fMapPoints.Clear();

            int num = fPlaces.Count;
            for (int i = 0; i < num; i++)
            {
                MapPlace place = fPlaces[i];
                if (place.Points.Count < 1) continue;

                int num2 = place.PlaceRefs.Count;
                for (int j = 0; j < num2; j++)
                {
                    GEDCOMCustomEvent evt = place.PlaceRefs[j].Event;

                    if ((ind != null && (evt.Parent == ind)) || (condBirth && evt.Name == "BIRT") || (condDeath && evt.Name == "DEAT") || (condResidence && evt.Name == "RESI"))
                    {
                        CopyPoint(place.Points[0], place.PlaceRefs[j]);
                    }
                }
            }

            if (ind != null)
            {
                // sort points by date
                fMapPoints.QuickSort(MapPointsCompare);
            }

            PreparePointsList(fMapPoints, ind != null);
        }

        private static int MapPointsCompare(GeoPoint item1, GeoPoint item2)
        {
            return item1.Date.CompareTo(item2.Date);
        }

        private void TreePlaces_DoubleClick(object sender, EventArgs e)
        {
            GKTreeNode node = tvPlaces.SelectedNode as GKTreeNode;
            if (node == null) return;

            GeoPoint pt = node.Tag as GeoPoint;
            if (pt == null) return;

            fMapBrowser.SetCenter(pt.Latitude, pt.Longitude, -1);
        }

        public void ProcessMap()
        {
            Show();
            PlacesLoad();
        }

        public MapsViewerWin(IBaseWindow baseWin)
        {
            InitializeComponent();

            fMapBrowser = new GKMapBrowser();
            fMapBrowser.Dock = DockStyle.Fill;
            Panel1.Controls.Add(fMapBrowser);

            fBase = baseWin;
            fTree = baseWin.Tree;
            fSelectedPersons = baseWin.GetContentList(GEDCOMRecordType.rtIndividual);

            fMapPoints = new ExtList<GeoPoint>(true);
            fPlaces = new ExtList<MapPlace>(true);
            fBaseRoot = tvPlaces.Nodes.Add(LangMan.LS(LSID.LSID_RPLocations));
            radTotal.Checked = true;

            SetLang();
        }

        public void SetLang()
        {
            Text = LangMan.LS(LSID.LSID_MIMap);
            pagePlaces.Text = LangMan.LS(LSID.LSID_RPLocations);
            grpSelection.Text = LangMan.LS(LSID.LSID_MapSelection);
            radTotal.Text = LangMan.LS(LSID.LSID_MapSelOnAll);
            chkBirth.Text = LangMan.LS(LSID.LSID_MSBirthPlaces);
            chkDeath.Text = LangMan.LS(LSID.LSID_MSDeathPlaces);
            chkResidence.Text = LangMan.LS(LSID.LSID_MSResiPlace);
            radSelected.Text = LangMan.LS(LSID.LSID_MapSelOnSelected);
            btnSaveImage.Text = LangMan.LS(LSID.LSID_SaveImage);
            btnSelectPlaces.Text = LangMan.LS(LSID.LSID_Show);
            chkLinesVisible.Text = LangMan.LS(LSID.LSID_LinesVisible);
        }

        private TreeNode FindTreeNode(string place)
        {
            int num = fBaseRoot.Nodes.Count;
            for (int i = 0; i < num; i++) {
                TreeNode node = fBaseRoot.Nodes[i];

                if (node.Text == place) {
                    return node;
                }
            }

            return null;
        }

        private void AddPlace(GEDCOMPlace place, GEDCOMCustomEvent placeEvent)
        {
            try
            {
                GEDCOMLocationRecord locRec = place.Location.Value as GEDCOMLocationRecord;

                string placeName = (locRec != null) ? locRec.LocationName : place.StringValue;

                TreeNode node = FindTreeNode(placeName);
                MapPlace mapPlace;

                if (node == null) {
                    mapPlace = new MapPlace();
                    mapPlace.Name = placeName;
                    fPlaces.Add(mapPlace);

                    node = new GKTreeNode(placeName, mapPlace);
                    fBaseRoot.Nodes.Add(node);

                    if (locRec == null) {
                        MainWin.Instance.RequestGeoCoords(placeName, mapPlace.Points);

                        int num = mapPlace.Points.Count;
                        for (int i = 0; i < num; i++) {
                            GeoPoint pt = mapPlace.Points[i];
                            string ptTitle = pt.Hint + string.Format(" [{0:0.000000}, {1:0.000000}]", pt.Latitude, pt.Longitude);
                            node.Nodes.Add(new GKTreeNode(ptTitle, pt));
                        }
                    } else {
                        GeoPoint pt = new GeoPoint(locRec.Map.Lati, locRec.Map.Long, placeName);
                        mapPlace.Points.Add(pt);

                        string ptTitle = pt.Hint + string.Format(" [{0:0.000000}, {1:0.000000}]", pt.Latitude, pt.Longitude);
                        node.Nodes.Add(new GKTreeNode(ptTitle, pt));
                    }
                } else {
                    mapPlace = (((GKTreeNode) node).Tag as MapPlace);
                }

                mapPlace.PlaceRefs.Add(new PlaceRef(placeEvent));
            } catch (Exception ex) {
                Logger.LogWrite("MapsViewerWin.AddPlace(): " + ex.Message);
            }
        }

        private void CopyPoint(GeoPoint gmPt, PlaceRef placeRef)
        {
            GeoPoint pt;
            int num = fMapPoints.Count;
            for (int i = 0; i < num; i++) {
                pt = fMapPoints[i];
                if (pt.Hint == gmPt.Hint) {
                    return;
                }
            }

            pt = new GeoPoint(gmPt.Latitude, gmPt.Longitude, gmPt.Hint);
            pt.Date = placeRef.Date;
            fMapPoints.Add(pt);
        }
    }
}
