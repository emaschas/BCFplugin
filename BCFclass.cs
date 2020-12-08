/// \mainpage 
/// <h1>BCFclass</h1>
/// The BCFclass class contains the structures and the methods to read a BCF file.<br/><br/>
/// <h2>BCF</h2>
/// BIM Collaboration Format (BCF) allows different BIM applications to communicate model-based issues with each other by leveraging IFC models that have been previously shared among project collaborators.<br/>
/// More specifically, BCF works by transferring XML formatted data, which is contextualized information about an issue or problem directly referencing a view, captured via PNG and IFC coordinates, and elements of a BIM, as referenced via their IFC GUIDs, from one application to another.<br/>
/// BCF files contain the issues or problems (deisgnated as topics" but not the BIM model itself.<br/>
/// <h2>References</h2>
/// Decsription of BCF files and their usage : <br/>
/// https://technical.buildingsmart.org/standards/bcf/<br/>
/// The BCF file schema is detailed in :<br/>
/// https://github.com/buildingSMART/BCF-XML/tree/master/Documentation<br/>
/// <hr>
/// By Emmanuel Maschas - 25-11-2020

#region "Usings"
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Xml.Linq;
using System.IO;
using System.IO.Compression;
using System.Globalization;
#endregion

namespace BCFclass {

  #region "BCF Structures"

  /// <summary> Enumartion of the type of camera associated to a viewpoint</summary>
  /// The type of camera can be :
  /// <list type="table">
  /// <item><term><see cref="NoCamera"/></term><description> No camera attached to the viewpoint </description></item>
  /// <item><term><see cref="Perspective"/></term><description> Perspective camera (uses Field of View)</description></item>
  /// <item><term><see cref="Orthogonal"/></term><description> Orthogonal camera  (uses View to World Scale)</description></item>
  /// </list>
  public enum CameraType { NoCamera, Perspective, Orthogonal }

  /// <summary> Viewpoint of a BCF file (referenced by a Topic or by a Comment)</summary>
  /// The Viewpoint defines the position of the camera and includes also a snapshot of the view as it was when the Topic or the Comment was created.<br/>
  /// The Viewpoint includes the following properties :
  /// <list type="table">
  /// <item><term><see cref="GUID"/></term><description> Globally Unique Identifier</description></item>
  /// <item><term><see cref="Index"/></term><description> Index of the viewpoint</description></item>
  /// <item><term><see cref="Bcfv"/></term><description> Visualization information file name</description></item>
  /// <item><term><see cref="Snapshot"/></term><description> Snapshot file name</description></item>
  /// <item><term><see cref="CamType"/></term><description> Camera type (NoCamera, Perspective or Orthogonal)</description></item>
  /// <item><term><see cref="CamX"/>, <see cref="CamY"/>, <see cref="CamZ"/></term><description> Camera location (in meters)</description></item>
  /// <item><term><see cref="DirX"/>, <see cref="DirY"/>, <see cref="DirZ"/></term><description> Camera direction</description></item>
  /// <item><term><see cref="UpX"/>, <see cref="UpY"/>, <see cref="UpZ"/></term><description> Camera Up vector</description></item>
  /// <item><term><see cref="Field"/></term><description> Field of view in degrees</description></item>
  /// <item><term><see cref="ViewToWorldScale"/></term><description> View to World Scale</description></item>
  /// <item><term><see cref="Components"/></term><description> List of visible components</description></item>
  /// <item><term><see cref="Image"/></term><description> Content of the snapshot file (Bitmap)</description></item>
  /// </list>
  public class Viewpoint {
    /// <summary>Globally Unique Identifier</summary>
    public string GUID { get; set; }
    /// <summary>Index of the viewpoint</summary>
    public int Index { get; set; }
    /// <summary>Visualization information file name</summary>
    public string Bcfv { get; set; }
    /// <summary>Snapshot file name</summary>
    public string Snapshot { get; set; }
    /// <summary>Camera type (NoCamera, Perspective or Orthogonal)</summary>
    public CameraType CamType { get; set; }
    /// <summary>Camera location, X component in meters</summary>
    public double CamX { get; set; }
    /// <summary>Camera location, Y component in meters</summary>
    public double CamY { get; set; }
    /// <summary>Camera location, Z component in meters</summary>
    public double CamZ { get; set; }
    /// <summary>Camera direction, X component</summary>
    public double DirX { get; set; }
    /// <summary>Camera direction, Y component</summary>
    public double DirY { get; set; }
    /// <summary>Camera direction, Z component</summary>
    public double DirZ { get; set; }
    /// <summary>Up vector, X component</summary>
    public double UpX { get; set; }
    /// <summary>Up vector, Y component</summary>
    public double UpY { get; set; }
    /// <summary>Up vector, Z component</summary>
    public double UpZ { get; set; }
    /// <summary>Field of view in degrees</summary>
    public double Field { get; set; }
    /// <summary>View to World Scale</summary>
    public double ViewToWorldScale { get; set; }
    /// <summary>List of visible components</summary>
    public List<string> Components { get; set; }
    /// <summary>Content (Bitmap) of the snapshot file </summary>
    public Bitmap Image { get; set; }
  }

  /// <summary> Comment of a BCF file (referenced by a Topic) </summary>
  /// A Topic may contain multiple <see cref="Comment">Comments</see>, reflecting the discussion on its subject.<br/>
  /// The Comment includes the following properties :
  /// <list type="table">
  /// <item><term><see cref="Date"/></term><description>Creation date of the comment</description></item>
  /// <item><term><see cref="Author"/></term><description>Creation author of the comment</description></item>
  /// <item><term><see cref="Text"/></term><description>Text of the comment</description></item>
  /// <item><term><see cref="ModifiedDate"/></term><description>Last modification date of the comment</description></item>
  /// <item><term><see cref="ModifiedAuthor"/></term><description>Last modification author of the comment</description></item>
  /// <item><term><see cref="VPGuid"/></term><description>GUID of the viewpoint associated to the comment (optional)</description></item>
  /// <item><term><see cref="Viewpoint"/></term><description>Viewpoint associated to the comment or <c>null</c></description></item>
  /// </list>
  public class Comment {
    /// <summary>Creation date of the comment</summary>
    public string Date { get; set; }
    /// <summary>Creation author of the comment</summary>
    public string Author { get; set; }
    /// <summary>Text of the comment</summary>
    public string Text { get; set; }
    /// <summary>Last modification date of the comment</summary>
    public string ModifiedDate { get; set; }
    /// <summary>Last modification author of the comment</summary>
    public string ModifiedAuthor { get; set; }
    /// <summary>GUID of the viewpoint associated to the comment (optional)</summary>
    public string VPGuid { get; set; }
    /// <summary>Viewpoint associated to the comment or <i>null</i></summary>
    public Viewpoint Viewpoint { get; set; }
  }

  /// <summary> Topic of a BCF file </summary>
  /// They are used to store the properties, the list of <see cref="Comment">Comments</see> and the list of <see cref="Viewpoint">Viewpoints</see>.<br/>
  /// The Topic includes the following properties :
  /// <list type="table">
  /// <item><term><see cref="ZipFile"/></term><description>Full name of the *.bcfzip file, source of this Topic</description></item>
  /// <item><term><see cref="TopicType"/></term><description>Type of Topic</description></item>
  /// <item><term><see cref="TopicStatus"/></term><description>Status of the Topic</description></item>
  /// <item><term><see cref="Title"/></term><description>Title of the Topic</description></item>
  /// <item><term><see cref="Priority"/></term><description>Priority of the Topic</description></item>
  /// <item><term><see cref="Index"/></term><description>Index of the Topic</description></item>
  /// <item><term><see cref="CreationDate"/></term><description>Date of creation of the Topic</description></item>
  /// <item><term><see cref="CreationAuthor"/></term><description>Author that created the Topic</description></item>
  /// <item><term><see cref="ModifiedDate"/></term><description>Last date of modification of the Topic</description></item>
  /// <item><term><see cref="ModifiedAuthor"/></term><description>Last Author that modified the Topic</description></item>
  /// <item><term><see cref="Description"/></term><description>Decription of the Topic</description></item>
  /// <item><term><see cref="Comments"/></term><description>List of <see cref="Comment">Comments</see> associated to the Topic</description></item>
  /// <item><term><see cref="Viewpoints"/></term><description>List of <see cref="Viewpoint">Viewpoints</see> associated to the Topic</description></item>
  /// </list>
  public class Topic {
    /// <summary>Type of Topic</summary>
    /// <value><list type="bullet"><item>Comment</item><item>Issue</item><item>Request</item><item>Solution</item></list></value>
    public string TopicType { get; set; }
    /// <summary>Status of the Topic</summary>
    /// <value><list type="bullet"><item>Open</item><item>In Progress</item><item>Closed</item><item>ReOpened</item></list></value>
    public string TopicStatus { get; set; }
    /// <summary>Title of the Topic</summary>
    public string Title { get; set; }
    /// <summary>Priority of the Topic</summary>
    public string Priority { get; set; }
    /// <summary>Index of the Topic</summary>
    public int Index { get; set; }
    /// <summary>Date of creation of the Topic</summary>
    public string CreationDate { get; set; }
    /// <summary>Author that created the Topic</summary>
    public string CreationAuthor { get; set; }
    /// <summary>Last date of modification of the Topic</summary>
    public string ModifiedDate { get; set; }
    /// <summary>Last Author that modified the Topic</summary>
    public string ModifiedAuthor { get; set; }
    /// <summary>Decription of the Topic</summary>
    public string Description { get; set; }
    /// <summary>List of Comments associated to the Topic</summary>
    public List<Comment> Comments { get; set; }
    /// <summary>List of Viewpoints associated to the Topic</summary>
    public List<Viewpoint> Viewpoints { get; set; }
  }

  #endregion

  /// <summary> Content of one BCF files. Contains a list of <see cref="Topic">Topics</see> in the property <see cref="TopicsList"/></summary>
  /// BCF : BIM Collaboration Format<br/>
  /// See : https://technical.buildingsmart.org/standards/bcf/<br/>
  /// and : https://github.com/buildingSMART/BCF-XML/tree/master/Documentation<br/>
  /// There are two constructors :
  /// <list><item><see cref="BCFfile()"/> that creates an empty object </item>
  /// <item><see cref="BCFfile(string)">BCFfile(FileName)</see> that creates an object and reads the content of a BCF file.</item></list>
  /// The class provides one method :
  /// <list><item><see cref="ReadBCF()"/> to read or append a BCF file into the object.</item></list>
  /// And one property :
  /// <list><item><see cref="TopicsList"/> that references all the Topics that have been read and appended  with the above methods.</item></list>
  public class BCFfile {

    /// <summary> List of BCF Topics loaded in the BCFfile object </summary>
    public List<Topic> TopicsList = new List<Topic>();
    /// <summary> BCF version </summary>
    public string Version;
    /// <summary> Name of the File (without path and extension) </summary>
    public string Name;
    /// <summary> Full Name of the File (including path and extension) </summary>
    public string FullName;
    /// <summary> Index of the File (set by BCFfileList for sorting purposes) </summary>
    public int Index;

    /// <summary> ZipFile of the BCF file in progress </summary>
    private ZipArchive bcfzip;

    #region "BCF Utilities"

    /// <summary> Utility to read the value of the XML element <paramref name="field"/> of the XML <paramref name="element"/></summary>
    /// <returns> The value of the XML element <paramref name="field"/> or "-" if unset </returns>
    private string ReadXML(XElement element, string field) {
      if(element.Element(field) != null)
        return element.Element(field).Value;
      else
        return "-";
    }

    /// <summary> Utility to read the <paramref name="attribute"/> of the XML <paramref name="element"/> </summary>
    /// <returns> The <paramref name="attribute"/> value or "-" if unset </returns>
    private string ReadATT(XElement element, string attribute) {
      string res = "-";
      foreach(XAttribute att in element.Attributes(attribute)) res = att.Value;
      return res;
    }

    #endregion

    #region "Read Routines"

    /// <summary>Read Topic attributes and properties</summary>
    private Topic ReadTopic(XElement Xtopic) {
      Topic NewTopic = new Topic();
      NewTopic.TopicType = ReadATT(Xtopic, "TopicType");
      NewTopic.TopicStatus = ReadATT(Xtopic, "TopicStatus");
      NewTopic.Title = ReadXML(Xtopic, "Title");
      NewTopic.Priority = ReadXML(Xtopic, "Priority");
      try {
        NewTopic.Index = int.Parse(ReadXML(Xtopic, "Index"));
      } catch {
        NewTopic.Index = 0;
      };
      NewTopic.CreationDate = ReadXML(Xtopic, "CreationDate");
      NewTopic.CreationAuthor = ReadXML(Xtopic, "CreationAuthor");
      NewTopic.ModifiedDate = ReadXML(Xtopic, "ModifiedDate");
      NewTopic.ModifiedAuthor = ReadXML(Xtopic, "ModifiedAuthor");
      NewTopic.Description = ReadXML(Xtopic, "Description");
      if(NewTopic.ModifiedDate == "-") NewTopic.ModifiedDate = NewTopic.CreationDate;
      if(NewTopic.ModifiedAuthor == "-") NewTopic.ModifiedAuthor = NewTopic.CreationAuthor;
      return NewTopic;
    }

    /// <summary>Read Comment attributes and properties</summary>
    private Comment ReadComment(XElement Xcomment) {
      Comment NewComment = new Comment();
      NewComment.Text = ReadXML(Xcomment, "Comment");
      NewComment.Date = ReadXML(Xcomment, "Date");
      NewComment.Author = ReadXML(Xcomment, "Author");
      NewComment.ModifiedAuthor = ReadXML(Xcomment, "ModifiedAuthor");
      NewComment.ModifiedDate = ReadXML(Xcomment, "ModifiedDate");
      if(NewComment.ModifiedAuthor == "-") NewComment.ModifiedAuthor = NewComment.Author;
      if(NewComment.ModifiedDate == "-") NewComment.ModifiedDate = NewComment.Date;
      XElement vp = Xcomment.Element("Viewpoint");
      NewComment.VPGuid = (vp != null ? ReadATT(vp, "Guid") : "-");
      return NewComment;
    }

    /// <summary>Read Viewpoint attributes and properties</summary>
    private Viewpoint ReadViewpoint(XElement Xviewpoint, string folder) {
      Viewpoint NewViewpoint = new Viewpoint();
      NewViewpoint.GUID = ReadATT(Xviewpoint, "Guid");
      NewViewpoint.Bcfv = ReadXML(Xviewpoint, "Viewpoint");
      NewViewpoint.Snapshot = ReadXML(Xviewpoint, "Snapshot");
      if(NewViewpoint.Snapshot != "-") NewViewpoint.Snapshot = folder + NewViewpoint.Snapshot;
      return NewViewpoint;
    }

    /// <summary>Read Viewpoint attributes and properties</summary>
    private Bitmap ReadSnapshot(string filename) {
      Bitmap image = null;
      // Viewpoint-Image
      if(filename != "-") {
        ZipArchiveEntry snap = bcfzip.GetEntry(filename);
        if(snap != null) image = new Bitmap(snap.Open());
      }
      return image;
    }

    /// <summary>Read File properties and Version</summary>
    private void ReadFile() {
      Name = Path.GetFileNameWithoutExtension(FullName);
      Version = "-";
      ZipArchiveEntry bcfversion = bcfzip.GetEntry("bcf.version");
      if(bcfversion != null) {
        XDocument Xbcfversion = XDocument.Load(bcfversion.Open());
        if(Xbcfversion != null) {
          XElement Xversion = Xbcfversion.Element("Version");
          if(Xversion != null) {
            Version = Xversion.Attribute("VersionId").Value;
          }
        }
      }
    }

    /// <summary> Read the Camera settings for the selected <paramref name="viewpoint"/>.bcfv file </summary>
    /// <param name="bcfzip">ZipArchive in which the bcfv file is located</param>
    /// <param name="filename">Name of bcfv viewpoint file within the ZIP archive</param>
    /// <param name="viewpoint">Viewpoint in which the camera settings will be stored</param>
    /// <returns>Nothing</returns>
    private void ReadBCFV(string filename, Viewpoint viewpoint) {
      ZipArchiveEntry bcfvzip = bcfzip.GetEntry(filename);
      // Default values
      viewpoint.CamType = CameraType.NoCamera;
      viewpoint.CamX = 0.0; viewpoint.CamY = 0.0; viewpoint.CamZ = 0.0;
      viewpoint.DirX = 1.0; viewpoint.DirY = 0.0; viewpoint.DirZ = 0.0;
      viewpoint.UpX = 0.0; viewpoint.UpY = 0.0; viewpoint.UpZ = 1.0;
      viewpoint.Field = 0.0; // Identifies unset camera.
      viewpoint.Components = new List<String>();
      if(bcfvzip != null) {
        XDocument bcfv = XDocument.Load(bcfvzip.Open()); // Parse bcfv XML document
        XElement camera = bcfv.Root.Element("PerspectiveCamera");
        if(camera != null) {
          viewpoint.CamType = CameraType.Perspective;
        } else {
          camera = bcfv.Root.Element("OrthogonalCamera");
          if(camera != null) viewpoint.CamType = CameraType.Orthogonal;
        }
        if(viewpoint.CamType != CameraType.NoCamera) {
          XElement point = camera.Element("CameraViewPoint");
          viewpoint.CamX = Double.Parse(point.Element("X").Value);
          viewpoint.CamY = Double.Parse(point.Element("Y").Value);
          viewpoint.CamZ = Double.Parse(point.Element("Z").Value);
          point = camera.Element("CameraDirection");
          viewpoint.DirX = Double.Parse(point.Element("X").Value);
          viewpoint.DirY = Double.Parse(point.Element("Y").Value);
          viewpoint.DirZ = Double.Parse(point.Element("Z").Value);
          point = camera.Element("CameraUpVector");
          viewpoint.UpX = Double.Parse(point.Element("X").Value);
          viewpoint.UpY = Double.Parse(point.Element("Y").Value);
          viewpoint.UpZ = Double.Parse(point.Element("Z").Value);
          if(viewpoint.CamType == CameraType.Perspective) viewpoint.Field = Double.Parse(camera.Element("FieldOfView").Value);
          if(viewpoint.CamType == CameraType.Orthogonal) viewpoint.ViewToWorldScale = Double.Parse(camera.Element("ViewToWorldScale").Value);
          XElement cps = bcfv.Root.Element("Components");
          if(cps != null) {
            XElement sel = cps.Element("Selection");
            if(sel != null) {
              foreach(XElement comp in sel.Elements("Component")) {
                viewpoint.Components.Add(ReadATT(comp, "IfcGuid"));
              }
            }
          }
        }
      } else {
        MessageBox.Show(filename + "  Not found");
      }
    }

    /// <summary> Load a BCF File </summary> 
    /// <param name="FileName">Full path of the file to be loadded or appended </param>
    /// <returns>Nothing</returns>
    public void ReadBCF(String FileName) {
      FullName = FileName;
      TopicsList.Clear();
      using(bcfzip = ZipFile.OpenRead(FileName)) {
        ReadFile();
        foreach(ZipArchiveEntry entry in bcfzip.Entries) {
          if(entry.FullName.EndsWith("markup.bcf", StringComparison.OrdinalIgnoreCase)) {
            string folder = entry.FullName.Substring(0, entry.FullName.LastIndexOf("/") + 1);
            XDocument markup = XDocument.Load(entry.Open()); // Parse XML document
                                                             // Topic
            XElement Xtopic = markup.Root.Element("Topic");
            if(Xtopic != null) {
              Topic NewTopic = ReadTopic(Xtopic);
              // Comments
              NewTopic.Comments = new List<Comment>();
              foreach(XElement Xcomment in markup.Root.Elements("Comment")) {
                Comment NewComment = ReadComment(Xcomment);
                NewTopic.Comments.Add(NewComment);
              }
              // Viewpoints
              NewTopic.Viewpoints = new List<Viewpoint>();
              // Search for Viewpoints for Version 2.0
              foreach(XElement vp in markup.Root.Elements("Viewpoints")) {
                Viewpoint NewViewpoint = ReadViewpoint(vp, folder);
                ReadBCFV(folder + NewViewpoint.Bcfv, NewViewpoint);
                NewViewpoint.Image = ReadSnapshot(NewViewpoint.Snapshot);
                if(NewViewpoint.Image == null) NewViewpoint.Snapshot = "-"; // Image not found !
                NewTopic.Viewpoints.Add(NewViewpoint);
              }
              // If Viewpoints is empty : 
              // try version 1.0 BCF : viewpoint.bcfv files.
              if(NewTopic.Viewpoints.Count == 0 && bcfzip.GetEntry(folder + "viewpoint.bcfv") != null) {
                // Found viewpoint.bcfv !
                Version = "1.0";
                Viewpoint NewViewpoint = new Viewpoint();
                NewViewpoint.GUID = "viewpointbcfv"; // pseudo GUID
                NewViewpoint.Bcfv = "viewpoint.bcfv";
                NewViewpoint.Snapshot = folder + "snapshot.png";
                ReadBCFV(folder + NewViewpoint.Bcfv, NewViewpoint);
                NewViewpoint.Image = ReadSnapshot(NewViewpoint.Snapshot);
                if(NewViewpoint.Image == null) NewViewpoint.Snapshot = "-"; // Image not found !
                NewTopic.Viewpoints.Add(NewViewpoint);
              }
              // Link Viewpoint and Comments
              foreach(Comment com in NewTopic.Comments) if(com.VPGuid != "-") {
                  foreach(Viewpoint vp in NewTopic.Viewpoints) if(vp.GUID == com.VPGuid) {
                      com.Viewpoint = vp;
                    }
                }
              TopicsList.Add(NewTopic);
            }
          }
        }
      }
    }

    /// <summary> Clear the Topics List </summary> 
    /// <returns> Nothing </returns>
    public void NewBCF() {
      TopicsList.Clear();
    }

    #endregion

    #region "Constructors"

    /// <summary> Create a BCFfile object, and read the content of the file designetd by <paramref name="FileName"/> </summary>
    /// <param name="FileName">Name of the BCF file to read</param>
    public BCFfile(string FileName) {
      this.ReadBCF(FileName);
    }

    /// <summary> Create an empty BCFfile object </summary>
    public BCFfile() { }

    #endregion

  }

  /// <summary> Collection of multiple <see cref="BCFfile">BCF files.</see></summary>
  public class BCFfileList {

    /// <summary> List of <see cref="BCFfile">BCF files</see> in the list.</summary>
    public List<BCFfile> BCFfiles { get; set; }

    /// <summary> Clear the list of <see cref="BCFfile">BCF files.</see></summary>
    public void Clear() {
      BCFfiles.Clear();
    }

    /// <summary> Add the content of <paramref name="FileName"/> in a new <see cref="BCFfile"/> object and add it to the set.</see>.</summary>
    /// <param name="FileName">Name of the BCF file to be loaded.</param>
    public void Add(string FileName) {
      BCFfile bcf = new BCFfile(FileName);
      BCFfiles.Add(bcf);
      bcf.Index = BCFfiles.Count - 1;
    }

    public BCFfileList() {
      BCFfiles = new List<BCFfile>();
    }
  }

}
