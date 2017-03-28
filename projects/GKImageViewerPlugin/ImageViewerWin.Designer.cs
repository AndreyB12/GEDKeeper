﻿using System;

namespace GKImageViewerPlugin
{
	partial class ImageViewerWin
	{
		private System.Windows.Forms.ToolStripButton tbFileLoad;
		private System.Windows.Forms.ToolStrip ToolBar1;

		private void InitializeComponent()
		{
		    this.ToolBar1 = new System.Windows.Forms.ToolStrip();
		    this.tbFileLoad = new System.Windows.Forms.ToolStripButton();
		    this.ToolBar1.SuspendLayout();
		    this.SuspendLayout();
		    // 
		    // ToolBar1
		    // 
		    this.ToolBar1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
		    this.ToolBar1.ImageScalingSize = new System.Drawing.Size(20, 20);
		    this.ToolBar1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
		    this.ToolBar1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
		    		    		    this.tbFileLoad});
		    this.ToolBar1.Location = new System.Drawing.Point(0, 0);
		    this.ToolBar1.Name = "ToolBar1";
		    this.ToolBar1.Size = new System.Drawing.Size(792, 25);
		    this.ToolBar1.TabIndex = 1;
		    // 
		    // tbFileLoad
		    // 
		    this.tbFileLoad.Name = "tbFileLoad";
		    this.tbFileLoad.Size = new System.Drawing.Size(23, 22);
		    this.tbFileLoad.Click += new System.EventHandler(this.ToolBar1_ButtonClick);
		    // 
		    // ImageViewerWin
		    // 
		    this.AutoScaleBaseSize = new System.Drawing.Size(7, 17);
		    this.ClientSize = new System.Drawing.Size(792, 573);
		    this.Controls.Add(this.ToolBar1);
		    this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
		    this.KeyPreview = true;
		    this.Name = "ImageViewerWin";
		    this.ShowInTaskbar = false;
		    this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		    this.Text = "ImageViewerWin";
		    this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ImageViewerWin_FormClosed);
		    this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ImageViewerWin_KeyDown);
		    this.ToolBar1.ResumeLayout(false);
		    this.ToolBar1.PerformLayout();
		    this.ResumeLayout(false);
		    this.PerformLayout();
		}
	}
}
