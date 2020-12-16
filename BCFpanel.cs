using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using BCFclass;

namespace BCFpanel {

  /// <summary> 
  /// BCFTreeNode is a class derivated from TreeNode with the following additional properties : <br/>
  ///   - NodeFile    : BCFfile reference if the TReeNodes refers to this type of element <br/>
  ///   - NodeTopic   : Topic reference if the TReeNodes refers to this type of element <br/>
  ///   - NodeComment : Comment reference if the TReeNodes refers to this type of element <br/>
  /// Only one of them shall be defined, the other shall be null
  /// </summary>
  public class BCFTreeNode : TreeNode {
    /// <summary>Added property : BCFfile NodeFile</summary>
    public BCFfile NodeFile;
    /// <summary>Added property : Topic NodeTopic</summary>
    public BCFmarkup.Markup NodeTopic;
    /// <summary>Added property : Comment NodeComment</summary>
    public BCFmarkup.Comment NodeComment;
  }

  /// <Summary> Main class of the BCFpanel </Summary>
  public class BCFpanelContent {

    // Components
    private SplitContainer Hsplit, Vsplit;
    private TreeView tree;
    private ListView list;
    private PictureBox pict;
    private ToolTip tooltip;
    // Image ratio
    private double ratio = 0.0;
    // BCF file list object
    private BCFfileList bcfs = new BCFfileList();

    /// <summary> Add a File in the tree </summary>
    /// <param name="file">BCF file to be added in tree</param>
    private BCFTreeNode AddFile(BCFfile file) {
      BCFTreeNode tn = new BCFTreeNode();
      tn.Text = file.Name;
      tn.NodeFile = file;
      tn.NodeTopic = null;
      tn.NodeComment = null;
      tree.Nodes.Add(tn);
      return tn;
    }

    /// <summary> Add a Topic in the tree </summary>
    /// <param name="parent">File owning this topic</param>
    /// <param name="topic">Topic to be added in tree</param>
    private BCFTreeNode AddTopic(BCFTreeNode parent, BCFmarkup.Markup topic) {
      BCFTreeNode tn = new BCFTreeNode();
      tn.Text = topic.Topic.Title;
      tn.NodeFile = null;
      tn.NodeTopic = topic;
      tn.NodeComment = null;
      parent.Nodes.Add(tn);
      return tn;
    }

    /// <Summary> Add a Comment in the tree node of the Topic </Summary>
    /// <param name="parent">Topic owning this comment</param>
    /// <param name="comment">Comment to be added in tree node of a Topic</param>
    private BCFTreeNode AddComment(BCFTreeNode parent, BCFmarkup.Comment comment) {
      BCFTreeNode tn = new BCFTreeNode();
      tn.Text = comment.CommentProperty;
      tn.NodeFile = null;
      tn.NodeTopic = null;
      tn.NodeComment = comment;
      parent.Nodes.Add(tn);
      return tn;
    }

    /// <summary> Add a line with (name, value) in the list view </summary>
    /// <param name="name">Name of the property</param>
    /// <param name="value">Value of the property</param>
    private void AddProperty(string name, string value) {
      ListViewItem newItem = new ListViewItem(name, 0);
      newItem.SubItems.Add(value);
      list.Items.Add(newItem);
    }

    /// <summary> Node sorter to sort BCF by File Index, Topics Index, and Comment Date (ascending)</summary>
    public class BCFsorter : IComparer {
      public int Compare(object x, object y) {
        int result = 0;
        BCFTreeNode tx = x as BCFTreeNode;
        BCFTreeNode ty = y as BCFTreeNode;
        if(tx.NodeFile != null && ty.NodeFile != null) {
          result = tx.NodeFile.Index - ty.NodeFile.Index;
        } else if(tx.NodeTopic != null && ty.NodeTopic != null) {
          result = tx.NodeTopic.Topic.Index - ty.NodeTopic.Topic.Index;
        } else if(tx.NodeComment != null && ty.NodeComment != null) {
          DateTime dx = tx.NodeComment.ModifiedDate;
          DateTime dy = ty.NodeComment.ModifiedDate;
          result = DateTime.Compare(dx, dy);
        } else // should not happen !
          result = string.Compare(tx.Text, ty.Text);
        return result;
      }
    }

    /// <summary> Delegate for the method provided by the user to move the camera </summary>
    public delegate void CameraMover(BCFvisinfo.VisualizationInfo viewpoint);

    /// <summary> The method provided by the user to move the camera (can be null if not used) </summary>
    public CameraMover MoveCamera;

    /// <summary> BCFpanel constructor with CameraMover</summary>
    public BCFpanelContent(Panel parent, CameraMover cameramethod, String pictToolText) {
      parent.SuspendLayout();
      MoveCamera = cameramethod;
      String exe = Application.ExecutablePath;
      String pat = Path.GetDirectoryName(exe);
      parent.Padding = new Padding(5, 5, 5, 5);
      // Tool tip
      // Set up the delays for the ToolTip.
      tooltip = new ToolTip();
      tooltip.AutoPopDelay = 5000;
      tooltip.InitialDelay = 1000;
      tooltip.ReshowDelay = 500;
      //tooltip.ShowAlways = true;
      // Tree View
      tree = new TreeView();
      tree.TabIndex = 2;
      tree.Dock = DockStyle.Fill;
      tree.CheckBoxes = false;
      tree.AfterSelect += new TreeViewEventHandler(ShowDetails);
      // List View
      list = new ListView();
      list.TabIndex = 3;
      list.Dock = DockStyle.Fill;
      list.View = View.Details;
      list.FullRowSelect = true;
      list.GridLines = false;
      // Image
      pict = new PictureBox();
      pict.Dock = DockStyle.Fill;
      pict.SizeMode = PictureBoxSizeMode.Zoom;
      pict.BorderStyle = BorderStyle.FixedSingle;
      pict.BackColor = Color.Beige;
      // pict click handlers
      pict.DoubleClick += new EventHandler(pict_DoubleClick);
      if(pictToolText != "") tooltip.SetToolTip(pict, pictToolText);
      // Vertical Split Container
      Vsplit = new SplitContainer();
      Vsplit.TabIndex = 1;
      Vsplit.TabStop = false;
      Vsplit.Dock = DockStyle.Fill;
      Vsplit.ForeColor = SystemColors.Control;
      Vsplit.Name = "Vertical Split";
      Vsplit.Orientation = Orientation.Horizontal;
      Vsplit.SplitterDistance = 295;
      Vsplit.SplitterWidth = 10;
      Vsplit.IsSplitterFixed = true;
      Vsplit.Panel1.Name = "Image";
      Vsplit.Panel2.Name = "Properties";
      Vsplit.Panel2MinSize = 150;
      // Horizontal Split Container
      Hsplit = new SplitContainer();
      Hsplit.TabIndex = 0;
      Hsplit.Dock = DockStyle.Fill;
      Hsplit.ForeColor = SystemColors.Control;
      Hsplit.Name = "Horizontal Split";
      Hsplit.Orientation = Orientation.Vertical;
      Hsplit.SplitterDistance = 80;
      Hsplit.SplitterWidth = 10;
      Hsplit.SplitterMoved += new SplitterEventHandler(HSplit_Moved);
      Hsplit.SplitterMoving += new SplitterCancelEventHandler(HSplit_Moving);
      Hsplit.Panel1.Name = "Topics List";
      Hsplit.Panel2.Name = "Details";
      // Fill the panel
      Vsplit.Panel1.Controls.Add(pict);
      Vsplit.Panel2.Controls.Add(list);
      Hsplit.Panel1.Controls.Add(tree);
      Hsplit.Panel2.Controls.Add(Vsplit);
      parent.Controls.Add(Hsplit);
      // Panel
      parent.MinimumSize = new Size(400, 400);
      // Panel resize handler
      parent.Resize += new EventHandler(BCFpanel_Resize);
      // Resize picture
      ResizeImage();
      // Set tree Sorter
      tree.TreeViewNodeSorter = new BCFsorter();
      parent.ResumeLayout();
    }

    /// <summary> BCFpanel constructor without CameraMover</summary>
    public BCFpanelContent(Panel parent) : this(parent, null, "") { }

    /// <summary> Fills the details of the selected File in the list view and the image </summary>
    /// <param name="file">Selected Topic</param>
    private void ShowFileDetails(BCFfile file) {
      list.Clear();
      list.Columns.Add("BCF FILE", -1, HorizontalAlignment.Left);
      list.Columns.Add("", -1, HorizontalAlignment.Left);
      AddProperty("BCF Version", file.Version.VersionId);
      AddProperty("Name", file.Name);
      AddProperty("Full Name", file.FullName);
      pict.Image = null;
      ratio = 0;
      ResizeImage();
    }

    /// <summary> Fills the details of the selected Topic in the list view and the image </summary>
    /// <param name="markup">Selected Topic</param>
    private void ShowTopicDetails(BCFmarkup.Markup markup) {
      list.Clear();
      list.Columns.Add("TOPIC", -1, HorizontalAlignment.Left);
      list.Columns.Add("", -1, HorizontalAlignment.Left);
      AddProperty("Title", markup.Topic.Title);
      AddProperty("Description", markup.Topic.Description);
      AddProperty("Topic Type", markup.Topic.TopicType);
      AddProperty("Topic Status", markup.Topic.TopicStatus);
      AddProperty("Topic Index", markup.Topic.Index.ToString());
      String tags = "";
      foreach(string label in markup.Topic.Labels) {
        tags += label + " ";
      }
      tags = tags.Trim();
      AddProperty("Topic Labels", tags);
      AddProperty("Creation Date", formatDate(markup.Topic.CreationDate));
      AddProperty("Creation Author", markup.Topic.CreationAuthor);
      if(markup.Topic.ModifiedAuthor != null && markup.Topic.ModifiedDate != null) {
        if(markup.Topic.ModifiedAuthor != markup.Topic.CreationAuthor || markup.Topic.ModifiedDate != markup.Topic.ModifiedDate) {
          AddProperty("Modified Date", formatDate(markup.Topic.ModifiedDate));
          AddProperty("Modified Author", markup.Topic.ModifiedAuthor);
        }
      }
      AddProperty("Due Date", (markup.Topic.DueDate.Equals(DateTime.MinValue) ? "Unset" : formatDate(markup.Topic.DueDate)));
      AddProperty("Assigned to", markup.Topic.AssignedTo);
      AddProperty("Stage", markup.Topic.Stage);
      int vc = markup.Viewpoints.Count;
      AddProperty("Viewpoints", vc > 0 ? vc.ToString() + " viewpoint" + (vc > 1 ? "s" : "") : "None");
      // Viewpoint
      if(vc > 0) {
        BCFmarkup.ViewPoint vp = markup.Viewpoints[0];
        // Viewpoint-Image
        if(vp.Image != null) {
          pict.Image = vp.Image;
          ratio = (double)vp.Image.Height / (double)vp.Image.Width;
        } else { // No Image
          pict.Image = null;
          ratio = 0.0;
        }
      } else { // No viewpoint -> No Image
        pict.Image = null;
        ratio = 0.0;
      }
      ResizeImage();
    }

    /// <summary> Fills the details of the selected Comment in the list view and the image </summary>
    /// <param name="comment">Selected Comment</param>
    /// <param name="topic">Topic parent of the Comment</param>
    private void ShowCommentDetails(BCFmarkup.Markup topic, BCFmarkup.Comment comment) {
      list.Clear();
      list.Columns.Add("COMMENT", -1, HorizontalAlignment.Left);
      list.Columns.Add("", -1, HorizontalAlignment.Left);
      AddProperty("Comment", comment.CommentProperty);
      AddProperty("Creation Date", formatDate(comment.Date));
      AddProperty("Creation Author", comment.Author);
      if(comment.ModifiedAuthor != null && comment.ModifiedDate != null) {
        if(comment.ModifiedAuthor != comment.Author || comment.ModifiedDate != comment.Date) {
          AddProperty("Modified Date", formatDate(comment.ModifiedDate));
          AddProperty("Modified Author", comment.ModifiedAuthor);
        }
      }
      AddProperty("Viewpoint", (comment.Viewpoint != null ? "Yes" : "No"));
      // Viewpoint
      BCFmarkup.ViewPoint vp = null;
      if(comment.Viewpoint != null) {
        vp = comment.MarkupViewPoint;
      } else if(topic.Viewpoints.Count > 0) {
        vp = topic.Viewpoints[0];
      }
      if(vp != null) { // Viewpoint-Image
        if(vp.Image != null) {
          pict.Image = vp.Image;
          ratio = (double)vp.Image.Height / (double)vp.Image.Width;
          ResizeImage();
        } else { // No Image
          pict.Image = null;
        }
      }
    }

    /// <summary> Fills the details of the selected Node </summary>
    /// <param name="sender">Not used</param>
    /// <param name="args">Not used</param>
    private void ShowDetails(Object sender, TreeViewEventArgs args) {
      list.Clear();
      BCFTreeNode tn = (BCFTreeNode)tree.SelectedNode;
      if(tn.NodeFile != null) {
        ShowFileDetails(tn.NodeFile);
      } else {
        BCFTreeNode pa = (BCFTreeNode)tn.Parent;
        if(tn.NodeTopic != null) {
          ShowTopicDetails(tn.NodeTopic);
        } else {
          BCFTreeNode gp = (BCFTreeNode)pa.Parent;
          ShowCommentDetails(pa.NodeTopic, tn.NodeComment);
        }
      }
      list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
    }

    /// <summary> Select a BCF file </summary>
    /// <returns>The file name or an empty string if the user cancels.</returns>
    private string SelectFile() {
      string filePath = null;
      using(OpenFileDialog openFileDialog = new OpenFileDialog()) {
        openFileDialog.Filter = "BCF files (*.bcf)|*.bcf*|BCFZIP files (*.bcfzip)|*.bcfzip|All files (*.*)|*.*";
        openFileDialog.FilterIndex = 1;
        openFileDialog.RestoreDirectory = true;
        if(openFileDialog.ShowDialog() == DialogResult.OK) filePath = openFileDialog.FileName;
      }
      return filePath;
    }

    /// <summary> Response to the menu event : load BCF file </summary>
    /// <param name="sender">Not used</param>
    /// <param name="args">Not used</param>
    public void OpenFile() {
      string filePath = SelectFile();
      if(filePath != null) {
        bcfs.Clear();
        bcfs.Add(filePath);
        Populate();
      }
    }

    /// <summary> Response to the menu event : append BCF file </summary>
    /// <param name="sender">Not used</param>
    /// <param name="args">Not used</param>
    public void AppendFile() {
      string filePath = SelectFile();
      if(filePath != null) {
        bcfs.Add(filePath);
        Populate();
      }
    }

    /// <summary> Populate the tree with the Files, Topics, and Comments  </summary>
    private void Populate() {
      tree.Nodes.Clear();
      foreach(BCFfile file in bcfs.BCFfiles) {
        BCFTreeNode tnf = AddFile(file);
        foreach(BCFmarkup.Markup markup in file.MarkupsList) {
          BCFTreeNode tnm = AddTopic(tnf, markup);
          foreach(BCFmarkup.Comment comment in markup.Comment) {
            AddComment(tnm, comment);
          }
        }
      }
      // Select the first Topic of the last File
      if(tree.Nodes.Count > 0) {
        tree.ExpandAll();
        TreeNode last = tree.Nodes[tree.Nodes.Count - 1];
        tree.SelectedNode = (last.Nodes.Count > 0 ? last.Nodes[0] : tree.SelectedNode = last);
      }
      // Resize picture
      ResizeImage();
    }

    private void pict_DoubleClick(Object sender, EventArgs args) {
      BCFmarkup.ViewPoint v = null;
      BCFTreeNode tn = (BCFTreeNode)tree.SelectedNode;
      if(tn == null) return;
      if(tn.NodeTopic != null) {
        BCFmarkup.Markup topic = tn.NodeTopic;
        if(topic.Viewpoints.Count > 0) v = topic.Viewpoints[0];
      } else if(tn.NodeComment != null) {
        BCFmarkup.Comment comment = tn.NodeComment;
        if(comment.Viewpoint != null) {
          v = comment.MarkupViewPoint;
        } else {
          BCFTreeNode tp = (BCFTreeNode)tree.SelectedNode.Parent;
          BCFmarkup.Markup topic = tp.NodeTopic;
          if(topic.Viewpoints.Count > 0) v = topic.Viewpoints[0];
        }
      }
      if(v != null) {
        if(v.Bcfv.CameraDefined()) {
          if(MoveCamera != null) MoveCamera(v.Bcfv);
          else MessageBox.Show(v.Bcfv.CameraText(), "Camera:");
        }
      }
    }

    #region "Panel utilities and Handlers"

    /// <summary> Move the vertical split to match the image ratio (Width / Height) </summary>
    private void ResizeImage() {
      if(ratio > 0) {
        Vsplit.Panel1Collapsed = false;
        int maxHeight = Vsplit.Height - Vsplit.Panel2MinSize - Vsplit.SplitterWidth;
        int newHeight = (int)((double)Vsplit.Width * ratio);
        if(maxHeight > 0) {
          Vsplit.SplitterDistance = (newHeight < maxHeight ? newHeight : maxHeight);
        }
      } else { // Ratio==0
        Vsplit.Panel1Collapsed = true;
      }
    }

    /// <summary> Response to the event that occurs when the panel is resized </summary>
    /// <param name="sender">Not used</param>
    /// <param name="args">Not used</param>
    private void BCFpanel_Resize(Object sender, EventArgs args) {
      Hsplit.Orientation = (((Panel)sender).Height > ((Panel)sender).Width ? Orientation.Horizontal : Orientation.Vertical);
      ResizeImage();
    }

    /// <summary> Response to the event that occurs when the horizontal splitter is moving </summary>
    /// <param name="sender">Not used</param>
    /// <param name="args">Not used</param>
    private void HSplit_Moving(Object sender, SplitterCancelEventArgs args) {
      Cursor.Current = Cursors.VSplit;
    }

    /// <summary> Response to the event that occurs after the horizontal splitter has been moved </summary>
    /// <param name="sender">Not used</param>
    /// <param name="args">Not used</param>
    private void HSplit_Moved(Object sender, SplitterEventArgs args) {
      Cursor.Current = Cursors.Default;
      ResizeImage();
    }

    /// <summary> Format an ISO date in readable string </summary>
    /// <param name="isodate">Date in ISO format. Ex : 2014-10-16T14:35:29+00:00</param>
    private string formatDate(DateTime isodate) {
      string res;
      try {
        res = isodate.ToString("dd-MM-yyyy HH:mm:ss");
      } catch { res = "-"; }
      return res;
    }

    #endregion

    #region "Export of Topics"

    public List<BCFclass.BCFfile> getFiles() { return bcfs.BCFfiles; }

    #endregion

  }
}
