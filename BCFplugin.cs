
// Read BCF comments file
//
// Emmanuel Maschas - 10-2020
//
// Version Navisworks 2021

#region "Usings"
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using Navis = Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.Plugins;
using BCFpanel;
#endregion

namespace BCFplugin {

  #region "BCF Dock Pane" 

  [Plugin("BCFpane", "SUEZ", DisplayName = "BCF Reader - by E. Maschas")] // Plugin name, Developer ID
  [Strings("BCFplugin.name")]
  [DockPanePlugin(800, 600, AutoScroll = false, FixedSize = false, MinimumHeight = 200, MinimumWidth = 200)]
  public class BCFDockPane : DockPanePlugin {

    /// <Summary> Copy the BCF Viewpoint in Navis Viewpoint </Summary>
    public Boolean CopyBCFcamera(BCFvisinfo.VisualizationInfo bvp, Navis.Viewpoint nvp) {
      double scale = BCFscale();
      if(bvp.PerspectiveCamera != null) {
        nvp.Position = new Navis.Point3D(bvp.PerspectiveCamera.CameraViewPoint.X / scale, bvp.PerspectiveCamera.CameraViewPoint.Y / scale, bvp.PerspectiveCamera.CameraViewPoint.Z / scale);
        nvp.AlignDirection(new Navis.Vector3D(bvp.PerspectiveCamera.CameraDirection.X, bvp.PerspectiveCamera.CameraDirection.Y, bvp.PerspectiveCamera.CameraDirection.Z));
        nvp.AlignUp(new Navis.Vector3D(bvp.PerspectiveCamera.CameraUpVector.X, bvp.PerspectiveCamera.CameraUpVector.Y, bvp.PerspectiveCamera.CameraUpVector.Z));
        nvp.Projection = Navis.ViewpointProjection.Perspective;
        nvp.HeightField = bvp.PerspectiveCamera.FieldOfView * Math.PI / 180.0;
        return true;
      } else if(bvp.OrthogonalCamera != null) {
        nvp.Position = new Navis.Point3D(bvp.OrthogonalCamera.CameraViewPoint.X / scale, bvp.OrthogonalCamera.CameraViewPoint.Y / scale, bvp.OrthogonalCamera.CameraViewPoint.Z / scale);
        nvp.AlignDirection(new Navis.Vector3D(bvp.OrthogonalCamera.CameraDirection.X, bvp.OrthogonalCamera.CameraDirection.Y, bvp.OrthogonalCamera.CameraDirection.Z));
        nvp.AlignUp(new Navis.Vector3D(bvp.OrthogonalCamera.CameraUpVector.X, bvp.OrthogonalCamera.CameraUpVector.Y, bvp.OrthogonalCamera.CameraUpVector.Z));
        nvp.Projection = Navis.ViewpointProjection.Orthographic;
        nvp.HeightField = bvp.OrthogonalCamera.ViewToWorldScale / scale;
        return true;
      } else {
        return false;
      }
    }

    /// <Summary> Copy tyhe BCF Topics in Navis Viewpoints </Summary>
    public void BCFexport() {
      String body;
      Navis.DocumentParts.DocumentSavedViewpoints vps = Navis.Application.MainDocument.SavedViewpoints;
      List<BCFclass.BCFfile> files = BCFcontent.getFiles();
      double scale = BCFscale();
      foreach(BCFclass.BCFfile file in files) {
        Navis.FolderItem fitm = new Navis.FolderItem();
        fitm.DisplayName = file.Name;
        foreach(BCFmarkup.Markup markup in file.MarkupsList) {
          Navis.Viewpoint nvp = new Navis.Viewpoint();
          if(markup.Viewpoints.Count > 0) {
            BCFmarkup.ViewPoint bvp = markup.Viewpoints[0];
            CopyBCFcamera(bvp.Bcfv, nvp);
          }
          Navis.SavedViewpoint svp = new Navis.SavedViewpoint(nvp);
          svp.DisplayName = markup.Topic.Title;
          body =
            "Topic: " + markup.Topic.Title + "\r\n" +
            "Description: " + markup.Topic.Description + "\r\n" +
            "Type: " + markup.Topic.TopicType + "\r\n" +
            "Status: " + markup.Topic.TopicStatus + "\r\n" +
            "Author: " + markup.Topic.CreationAuthor + "\r\n" +
            "Created: " + formatDate(markup.Topic.CreationDate);
          svp.Comments.Add(new Navis.Comment(body, Navis.CommentStatus.New, markup.Topic.CreationAuthor));
          foreach(BCFmarkup.Comment comment in markup.Comments) {
            body =
              "Comment: " + comment.CommentProperty + "\r\n" +
              "Author: " + comment.Author + "\r\n" +
              "Created: " + formatDate(comment.Date);
            svp.Comments.Add(new Navis.Comment(body, Navis.CommentStatus.New, comment.Author));
          }
          fitm.Children.Add(svp);
        }
        vps.AddCopy(fitm);
      }
    }

    /// <summary> Format an ISO date in readable string </summary>
    /// <param name="isodate">Date in ISO format. Ex : 2014-10-16T14:35:29+00:00</param>
    private string formatDate(DateTime isodate) {
      string res;
      try {
        res = isodate.ToString();
      } catch { res = "-"; }
      return res;
    }

    /// <Summary> Content of the DockPane </Summary>
    public double BCFscale() {
      double scale = 1.0;
      if(Navis.Application.ActiveDocument.Units == Navis.Units.Millimeters) {
        scale = 0.001;
      } else if(Navis.Application.ActiveDocument.Units == Navis.Units.Centimeters) {
        scale = 0.010;
      } else if(Navis.Application.ActiveDocument.Units == Navis.Units.Meters) {
        scale = 1.000;
      } else if(Navis.Application.ActiveDocument.Units == Navis.Units.Inches) {
        scale = 0.0254;
      } else if(Navis.Application.ActiveDocument.Units == Navis.Units.Feet) {
        scale = 0.3048;
      } else {
        MessageBox.Show("UNHADLED UNITS : " + Navis.Application.ActiveDocument.Units.ToString());
      }
      return scale;
    }

    /// <Summary> Content of the DockPane </Summary>
    public BCFpanelContent BCFcontent = null;

    /// <Summary> Move the camera </Summary>
    public void MoveCamera(BCFvisinfo.VisualizationInfo viewpoint) {
      // Remark : BCF units are METER and DEGREES
      double scale = BCFscale();
      // Viewpoint-Camera-Position
      try {
        Navis.Viewpoint nvp = Navis.Application.ActiveDocument.CurrentViewpoint.CreateCopy();
        if(CopyBCFcamera(viewpoint, nvp)) {
          Navis.Application.ActiveDocument.CurrentViewpoint.CopyFrom(nvp);
          // Search the components
          Navis.Application.ActiveDocument.CurrentSelection.Clear();
          Navis.ModelItemCollection res = new Navis.ModelItemCollection();
          foreach(BCFvisinfo.Component cmp in viewpoint.Components.Selection) {
            Navis.Search s = new Navis.Search();
            s.Selection.SelectAll();
            s.SearchConditions.Add(Navis.SearchCondition.HasPropertyByDisplayName("IFC", "GLOBALID").EqualValue(Navis.VariantData.FromDisplayString(cmp.IfcGuid)));
            res.AddRange(s.FindAll(Navis.Application.ActiveDocument, false));
          }
          Navis.Application.ActiveDocument.CurrentSelection.CopyFrom(res);
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
      // Picture ToolTip
      String pictTip = TryGetString("Pict.ToolTip") ?? "Double click to move the camera";
      // Content
      BCFcontent = new BCFpanelContent(panel, MoveCamera, pictTip);
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
  [Strings("BCFplugin.name")]
  [RibbonLayout("BCFribbon.xaml")]
  [RibbonTab("ID_BCFTab", LoadForCanExecute = true)]
  [Command("ID_BCF_Show", LoadForCanExecute = true)]
  [Command("ID_BCF_Load", LoadForCanExecute = true)]
  [Command("ID_BCF_Append", LoadForCanExecute = true)]
  [Command("ID_BCF_Export", LoadForCanExecute = true)]
  public class BCFRibbonHandler : CommandHandlerPlugin {

    public override int ExecuteCommand(string commandId, params string[] parameters) {
      BCFDockPane dockPane;
      if(!Navis.Application.IsAutomated) {
        PluginRecord pluginRecord = Navis.Application.Plugins.FindPlugin("BCFpane.SUEZ");
        if(pluginRecord is DockPanePluginRecord && pluginRecord.IsEnabled) {
          dockPane = (BCFDockPane)(pluginRecord.LoadedPlugin ?? pluginRecord.LoadPlugin());
          if(commandId == "ID_BCF_Show") {
            dockPane.ActivatePane();
          } else if(commandId == "ID_BCF_Load") {
            dockPane.ActivatePane();
            dockPane.BCFcontent.OpenFile();
          } else if(commandId == "ID_BCF_Append") {
            dockPane.BCFcontent.AppendFile();
          } else if(commandId == "ID_BCF_Export") {
            String question = TryGetString("Export.Question") ?? "Export BCF as Viewpoints";
            MessageBox.Show(question, "BCF Plugin", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            dockPane.BCFexport();
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
