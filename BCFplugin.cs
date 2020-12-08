
// Read BCF comments file
//
// Emmanuel Maschas - 10-2020
//
// Version Navisworks 2021

#region "Usings"
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.Plugins;
using Navis = Autodesk.Navisworks.Api.Application;
using BCFpanel;
#endregion

namespace BCFplugin {

  #region "BCF Dock Pane" 

  [Plugin("BCFpane", "SUEZ", DisplayName = "BCF Reader - by E. Maschas")] // Plugin name, Developer ID
  [DockPanePlugin(800, 600, AutoScroll = false, FixedSize = false, MinimumHeight = 200, MinimumWidth = 200)]
  public class BCFDockPane : DockPanePlugin {

    /// <Summary> Content of the DockPane </Summary>
    public BCFpanelContent BCFcontent = null;

    /// <Summary> Move the camera </Summary>
    public void MoveCamera(Double Cx, Double Cy, Double Cz, Double Dx, Double Dy, Double Dz, Double Ux, Double Uy, Double Uz, Double field, List<String> components) {
      // Remark : BCF units are METER and DEGREES
      double scale = 1.0;
      if(Navis.ActiveDocument.Units == Units.Millimeters) {
        scale = 0.001;
      } else if(Navis.ActiveDocument.Units == Units.Centimeters) {
        scale = 0.010;
      } else if(Navis.ActiveDocument.Units == Units.Meters) {
        scale = 1.000;
      } else if(Navis.ActiveDocument.Units == Units.Inches) {
        scale = 0.0254;
      } else if(Navis.ActiveDocument.Units == Units.Feet) {
        scale = 0.3048;
      } else {
        MessageBox.Show("UNHADLED UNITS : " + Navis.ActiveDocument.Units.ToString());
      }
      // Viewpoint-Camera-Position
      try {
        Autodesk.Navisworks.Api.Viewpoint V = Navis.ActiveDocument.CurrentViewpoint.CreateCopy();
        if(field > 0.0) { // If Field==0.0 : camera is unset
          // Move Camera
          V.Position = new Point3D(Cx / scale, Cy / scale, Cz / scale);
          Vector3D VD = new Vector3D(Dx, Dy, Dz);
          V.AlignDirection(VD);
          Vector3D VU = new Vector3D(Ux, Uy, Uz);
          V.AlignUp(VU);
          V.HeightField = field * Math.PI / 180.0;
          Navis.ActiveDocument.CurrentViewpoint.CopyFrom(V);
          // Search the components
          Navis.ActiveDocument.CurrentSelection.Clear();
          ModelItemCollection res = new ModelItemCollection();
          foreach(string guid in components) {
            Search s = new Search();
            s.Selection.SelectAll();
            s.SearchConditions.Add(SearchCondition.HasPropertyByDisplayName("IFC", "GLOBALID").EqualValue(VariantData.FromDisplayString(guid)));
            res.AddRange(s.FindAll(Autodesk.Navisworks.Api.Application.ActiveDocument, false));
          }
          Navis.ActiveDocument.CurrentSelection.CopyFrom(res);
        }
      } catch {
        MessageBox.Show("Camera Error !");
      }

    }

    // Create the control that will be used to display in the pane
    public override Control CreateControlPane() {
      UserControl pane = new UserControl();
      pane.SuspendLayout();
      // Panel
      Panel panel = new Panel();
      panel.Dock = DockStyle.Fill;
      pane.Controls.Add(panel);
      // Content
      BCFcontent = new BCFpanelContent(panel, MoveCamera);
      // BCF DocPane
      pane.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
      pane.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      pane.Name = "BCF Panel";
      pane.Dock = DockStyle.Fill;
      pane.ResumeLayout(false);
      pane.CreateControl();
      return pane;
    }

    public override void DestroyControlPane(Control pane) {
      pane.Dispose();
    }

  }
  #endregion

  #region "BCF Ribbon Handler"

  [Plugin("BCFRibbon", "SUEZ", DisplayName = "BCF")]
  [Strings("BCFribbon.name")]
  [RibbonLayout("BCFribbon.xaml")]
  [RibbonTab("ID_BCFTab", DisplayName = "BCF", LoadForCanExecute = true)]
  [Command("ID_BCF_Show", DisplayName = "Show BCF", LoadForCanExecute = true)]
  [Command("ID_BCF_Load", DisplayName = "Load BCF", LoadForCanExecute = true)]
  [Command("ID_BCF_Append", DisplayName = "Append BCF", LoadForCanExecute = true)]
  public class BCFRibbonHandler : CommandHandlerPlugin {

    public override int ExecuteCommand(string commandId, params string[] parameters) {
      BCFDockPane dockPane;
      if(!Navis.IsAutomated) {
        PluginRecord pluginRecord = Navis.Plugins.FindPlugin("BCFpane.SUEZ");
        if(pluginRecord is DockPanePluginRecord && pluginRecord.IsEnabled) {
          dockPane = (BCFDockPane)(pluginRecord.LoadedPlugin ?? pluginRecord.LoadPlugin());
          if(commandId == "ID_BCF_Show") {
            dockPane.ActivatePane();
          } else if(commandId == "ID_BCF_Load") {
            dockPane.ActivatePane();
            dockPane.BCFcontent.OpenFile();
          } else if(commandId == "ID_BCF_Append") {
            dockPane.BCFcontent.AppendFile();
          }
        }
      } else MessageBox.Show("Unknown Command : " + commandId);
      return 0;
    }

    public override bool TryShowCommandHelp(String commandId) {
      MessageBox.Show("This is command Id " + commandId);
      return true;
    }
    
    public override bool CanExecuteRibbonTab(String ribbonTabId) {
      return true;
    }
    
    public override CommandState CanExecuteCommand(String commandId) {
      CommandState state = new CommandState();
      state.IsEnabled = true;
      return state;
    }

  }
  #endregion

}
