/// \mainpage 
/// # BCFclass
/// The BCFclass class contains the structures and the methods to read a BCF file.
///
/// In addition, the following classes have been extracted from the BCF file schema (see [References](#References))
/// - BCFversion : Version of the file
/// - BCFmarkup : Topics, Comments and Viewpoints
/// - BCFvisinfo : Visualisation information
///
/// # BCF Files
/// BIM Collaboration Format (BCF) allows different BIM applications to communicate model-based issues with each other by leveraging IFC models that have been previously shared among project collaborators.<br/>
/// More specifically, BCF works by transferring XML formatted data, which is contextualized information about an issue or problem directly referencing a view, captured via PNG and IFC coordinates, and elements of a BIM, as referenced via their IFC GUIDs, from one application to another.<br/>
/// BCF files contain the issues or problems (deisgnated as topics" but not the BIM model itself.
///
/// ## References {#References}
/// __Description of BCF files and their usage :__<br/>
/// <https://technical.buildingsmart.org/standards/bcf>
///
/// __The BCF file schema is detailed in :__<br/>
/// <https://github.com/buildingSMART/BCF-XML/tree/master/Documentation>
///
/// ## Application Example : Navisworks Plugin
/// ![Screenshot](../Screenshot.png)
/// <hr>
/// By Emmanuel Maschas - 16-12-2020

#region "Usings"
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Xml.Linq;
using System.IO;
using System.IO.Compression;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;
#endregion

#region "Schema additions"

// Additions to the Classes extracted with xscgen
// Plus Documentation

/// <summary> BCF file Markup objects </summary>
namespace BCFmarkup {

  /// <summary> Markup of a BCF file </summary>
  /// They are used to store the Topic, the list of <see cref="Comment">Comments</see> and the list of <see cref="ViewPoint">Viewpoints</see>.<br/>
  /// The Markup includes the following properties :
  /// <list type="table">
  /// <item><term><see cref="Header"/></term><description>Contains information about the IFC files relevant to this Topic</description></item>
  /// <item><term><see cref="Topic"/></term><description>The <see cref="Topic"/> of this Markup</description></item>
  /// <item><term><see cref="Comment"/></term><description>List of <see cref="Comment">Comments</see> associated to the Topic</description></item>
  /// <item><term><see cref="Viewpoints"/></term><description>List of <see cref="ViewPoint">Viewpoints</see> associated to the Topic</description></item>
  /// </list>
  public partial class Markup {
    /// \fn System.Collections.ObjectModel.Collection< HeaderFile > Header[get]
    /// Headers of the Markup
    /// \fn Topic 	Topic[get, set]
    /// Topic of the Markup (only one)
    /// \fn System.Collections.ObjectModel.Collection< Comment > Comment[get]
    /// Collection of Comments related to the Topic
    /// \fn System.Collections.ObjectModel.Collection< ViewPoint > Viewpoints[get]
    /// Collection of Viewpoints referred to by the Topic or the Comments
  }

  /// <summary> Topic of a BCF file </summary>
  /// They are used to store the properties, the list of <see cref="Comment">Comments</see> and the list of <see cref="ViewPoint">Viewpoints</see>.<br/>
  /// The Topic includes the following properties :
  /// <list type="table">
  /// <item><term><see cref="Title"/></term><description>Title of the Topic</description></item>
  /// <item><term><see cref="Priority"/></term><description>Priority of the Topic</description></item>
  /// <item><term><see cref="Index"/></term><description>Index of the Topic</description></item>
  /// <item><term><see cref="Labels"/></term><description>List of Labels associated to the Topic</description></item>
  /// <item><term><see cref="CreationDate"/></term><description>Date of creation of the Topic</description></item>
  /// <item><term><see cref="CreationAuthor"/></term><description>Author that created the Topic</description></item>
  /// <item><term><see cref="ModifiedDate"/></term><description>Last date of modification of the Topic</description></item>
  /// <item><term><see cref="ModifiedAuthor"/></term><description>Last Author that modified the Topic</description></item>
  /// <item><term><see cref="DueDate"/></term><description>Due date for the resolution of the Topic</description></item>
  /// <item><term><see cref="AssignedTo"/></term><description>Person in charge of the resolution of the Topic</description></item>
  /// <item><term><see cref="Stage"/></term><description>Stage of the Topic</description></item>
  /// <item><term><see cref="Description"/></term><description>Decription of the Topic</description></item>
  /// <item><term><see cref="DocumentReference"/></term><description>Additional payloads or links with topics. The references may point to a file within the .bcfzip or to an external location</description></item>
  /// <item><term><see cref="ReferenceLink"/></term><description>List of references to the topic, for example, a work request management system or an URI to a model</description></item>
  /// <item><term><see cref="RelatedTopic"/></term><description>list of related topics</description></item>
  /// <item><term><see cref="Guid"/></term><description>Globally Unique Identifier</description></item>
  /// <item><term><see cref="TopicType"/></term><description>Type of Topic</description></item>
  /// <item><term><see cref="TopicStatus"/></term><description>Status of the Topic</description></item>
  /// </list>
  public partial class Topic {
    /// \fn System.Collections.ObjectModel.Collection< string > ReferenceLink[get]
    /// List of references to the Topic, for example, a work request management system or an URI to a model
    /// \fn string Title[get, set]
    /// Title of the Topic
    /// \fn string Priority[get, set]
    /// Priority of the Topic
    /// \fn int Index[get, set]
    /// Index number to maintain the order of the Topics
    /// \fn System.Collections.ObjectModel.Collection< string > Labels[get]
    /// Tags for grouping Topics
    /// \fn System.DateTime CreationDate[get, set]
    /// Date when the Topic was created
    /// \fn string CreationAuthor[get, set]
    /// User who created the Topic
    /// \fn System.DateTime ModifiedDate[get, set]
    /// Date when the Topic was last modified. Exists only when Topic has been modified after creation
    /// \fn string ModifiedAuthor[get, set]
    /// User who modified the Topic. Exists only when Topic has been modified after creation
    /// \fn System.DateTime DueDate[get, set]
    /// Date until when the Topics issue needs to be resolved
    /// \fn string AssignedTo[get, set]
    /// The user to whom this Topic is assigned to<br/>
    /// (Recommended to be in email format)
    /// \fn string Stage[get, set]
    /// Stage this Topic is part of
    /// \fn string Description[get, set]
    /// Description of the Topic
    /// \fn BimSnippet BimSnippet[get, set]
    /// BimSnippet is an additional file containing information related to one or multiple Topics<br/>
    /// For example, it can be an IFC file containing provisions for voids
    /// \fn System.Collections.ObjectModel.Collection< TopicDocumentReference > DocumentReference[get]
    /// DocumentReference provides a means to associate additional payloads or links with Topics<br/>
    /// The references may point to a file within the .bcfzip or to an external location
    /// \fn System.Collections.ObjectModel.Collection< TopicRelatedTopic > RelatedTopic[get]
    /// Relation between Topics
    /// \fn string Guid[get, set]
    /// Guid of the Topic
    /// \fn string TopicType[get, set]
    /// Type of the Topic
    /// \fn string TopicStatus[get, set]    
    /// Status of the Topic
  }

  /// <summary> Viewpoint of a BCF file (referenced by a Topic or by a Comment)</summary>
  /// The Viewpoint defines the position of the camera and includes also a snapshot of the view as it was when the Topic or the Comment was created.<br/>
  /// Viewpoints are immutable, therefore they should never be changed once created. If new comments on a topic require different visualization, new viewpoints should be added.
  /// The Viewpoint includes the following properties :
  /// <list type="table">
  /// <item><term><see cref="Viewpoint"/></term><description> Visualisation Info (.bcfv) file name</description></item>
  /// <item><term><see cref="Snapshot"/></term><description> Snapshot file name</description></item>
  /// <item><term><see cref="Index"/></term><description> Index of the viewpoint</description></item>
  /// <item><term><see cref="Guid"/></term><description> Globally Unique Identifier</description></item>
  /// <item><term><see cref="Bcfv"/></term><description> Visualization information object</description></item>
  /// <item><term><see cref="Image"/></term><description> Content of the snapshot file (Bitmap)</description></item>
  /// </list>
  public partial class ViewPoint {
    /// <summary>Content of the VisualisationInfo (.bcfv) file </summary>
    public BCFvisinfo.VisualizationInfo Bcfv { get; set; }
    /// <summary>Content (Bitmap) of the snapshot file </summary>
    public Bitmap Image { get; set; }
    /// \fn string Viewpoint [get, set]
    /// <summary>Name of the Visualisation Info (.bcfv) file</summary>
    /// \fn string Snapshot [get, set]
    /// <summary>Name of the Image (.png) file</summary>
    /// \fn int Index [get, set]
    /// <summary>Index number of the ViewPoint for sorting</summary>
    /// \fn string Guid [get, set]
    /// <summary>Guid of the ViewPoint</summary>
  }

  /// <summary> Comment of a BCF file (referenced by a Markup)</summary>
  /// A Markup may contain multiple <see cref="Comment">Comments</see>, reflecting the discussion on its subject.<br/>
  /// The Comment includes the following properties :
  /// <list type="table">
  /// <item><term><see cref="Date"/></term><description>Creation date of the Comment</description></item>
  /// <item><term><see cref="Author"/></term><description>Creation author of the Comment</description></item>
  /// <item><term><see cref="CommentProperty"/></term><description>Text of the Comment</description></item>
  /// <item><term><see cref="ModifiedDate"/></term><description>Last modification date of the Comment</description></item>
  /// <item><term><see cref="ModifiedAuthor"/></term><description>Last modification author of the Comment</description></item>
  /// <item><term><see cref="Viewpoint"/></term><description>Viewpoint associated to the Comment or <c>null</c></description></item>
  /// <item><term><see cref="MarkupViewPoint"/></term><description>Visualization information object of the viewpoint associated to the Comment</description></item>
  /// </list>
  public partial class Comment {
    /// <summary>Content of the VisualisationInfo (.bcfv) file </summary>
    public BCFmarkup.ViewPoint MarkupViewPoint { get; set; }
    /// \fn System.DateTime Date[get, set]
    /// Date of the Comment
    /// \fn string Author[get, set]
    /// Comment author
    /// \fn string CommentProperty[get, set]
    /// The text of the Comment
    /// \fn CommentViewpoint Viewpoint[get, set]
    /// Back reference to the viewpoint GUID.
    /// \fn System.DateTime ModifiedDate[get, set]
    /// The date when Comment was modified
    /// \fn string ModifiedAuthor[get, set]
    /// The author who modified the Comment
    /// \fn string Guid[get, set]
    /// Guid of the Comment
  }

}

/// <summary> BCF file Visualisation Info objects </summary>
namespace BCFvisinfo {

  /// <summary>Visualization Info (.bcfv) definition</summary>
  /// The Visualization Info includes the following properties :
  /// <list type="table">
  /// <item><term><see cref="CamType"/></term><description> Camera type (NoCamera, Perspective or Orthogonal)</description></item>
  /// <item><term><see cref="CamX"/>, <see cref="CamY"/>, <see cref="CamZ"/></term><description> Camera location (in meters)</description></item>
  /// <item><term><see cref="DirX"/>, <see cref="DirY"/>, <see cref="DirZ"/></term><description> Camera direction</description></item>
  /// <item><term><see cref="UpX"/>, <see cref="UpY"/>, <see cref="UpZ"/></term><description> Camera Up vector</description></item>
  /// <item><term><see cref="Field"/></term><description> Field of view in degrees</description></item>
  /// <item><term><see cref="ViewToWorldScale"/></term><description> View to World Scale</description></item>
  /// <item><term><see cref="Components"/></term><description> List of visible components</description></item>
  /// </list>
  public partial class VisualizationInfo {
    /// <summary>Returns if a Camera is defined (either Orthogonal or Perspective</summary>
    public Boolean CameraDefined() {
      return (this.OrthogonalCamera != null || this.PerspectiveCamera != null);
    }
    /// <summary>Returns if an Orthogonal Camera is defined</summary>
    public Boolean OrthogonalDefined() {
      return (this.OrthogonalCamera != null);
    }
    /// <summary>Returns if an Orthogonal Camera is defined</summary>
    public Boolean PerspectiveDefined() {
      return (this.PerspectiveCamera != null);
    }
    /// <summary>Returns a text with Camera definition</summary>
    public String CameraText() {
      String txt = "No Camera";
      if(this.OrthogonalDefined()) {
        BCFvisinfo.OrthogonalCamera c = this.OrthogonalCamera;
        txt =
          $"Pos. :\t{c.CameraViewPoint.X:F3},\t{c.CameraViewPoint.Y:F3},\t{c.CameraViewPoint.Z:F3} (meters)\n" +
          $"Dir. :\t{c.CameraDirection.X:F3},\t{c.CameraDirection.Y:F3},\t{c.CameraDirection.Z:F3}\n" +
          $"Up :\t{c.CameraUpVector.X:F3},\t{c.CameraUpVector.Y:F3},\t{c.CameraUpVector.Z:F3}\n" +
          $"Scale :\t{c.ViewToWorldScale:F1}";
      } else if(this.PerspectiveDefined()) {
        BCFvisinfo.PerspectiveCamera c = this.PerspectiveCamera;
        txt =
          $"Pos. :\t{c.CameraViewPoint.X:F3},\t{c.CameraViewPoint.Y:F3},\t{c.CameraViewPoint.Z:F3} (meters)\n" +
          $"Dir. :\t{c.CameraDirection.X:F3},\t{c.CameraDirection.Y:F3},\t{c.CameraDirection.Z:F3}\n" +
          $"Up :\t{c.CameraUpVector.X:F3},\t{c.CameraUpVector.Y:F3},\t{c.CameraUpVector.Z:F3}\n" +
          $"Field :\t{c.FieldOfView:F1}°";
      }
      return txt;
    }
  }

}

#endregion

/// <summary> BCF file namespace</summary>
namespace BCFclass {

  /// <summary> Content of a BCF file </summary>
  public class BCFfile {

    /// <summary> List of BCF markups in the BCFfile object </summary>
    public List<BCFmarkup.Markup> MarkupsList = new List<BCFmarkup.Markup>();
    /// <summary> List of BCF visinfo in the BCFfile object </summary>
    public List<BCFvisinfo.VisualizationInfo> VisinfoList = new List<BCFvisinfo.VisualizationInfo>();
    /// <summary> List of BCF images in the BCFfile object </summary>
    public List<Bitmap> ImageList = new List<Bitmap>();
    /// <summary> BCF version </summary>
    public BCFversion.Version Version;
    /// <summary> Name of the File (without path and extension) </summary>
    public string Name;
    /// <summary> Full Name of the File (including path and extension) </summary>
    public string FullName;
    /// <summary> Index of the File (set by BCFfileList for sorting purposes) </summary>
    public int Index;

    /// <summary> ZipFile of the BCF file in progress </summary>
    private ZipArchive bcfzip;

    #region "Read BCF file"

    /// <summary>Read Snapshot image</summary>
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
      ZipArchiveEntry entry = bcfzip.GetEntry("bcf.version");
      XmlSerializer serializer = new XmlSerializer(typeof(BCFversion.Version));
      if(entry != null) {
        Version = (BCFversion.Version)serializer.Deserialize(new XmlTextReader(entry.Open()));
      } else {
        Version = new BCFversion.Version();
        Version.VersionId = "1.0";
        Version.DetailedVersion = "Undefined Version";
      }

    }

    /// <summary> Read the Camera settings for the selected <paramref name="viewpoint"/>.bcfv file </summary>
    /// <param name="bcfzip">ZipArchive in which the bcfv file is located</param>
    /// <param name="filename">Name of bcfv viewpoint file within the ZIP archive</param>
    /// <param name="viewpoint">Viewpoint in which the camera settings will be stored</param>
    /// <returns>Nothing</returns>
    private BCFvisinfo.VisualizationInfo ReadBCFV(string filename) {
      ZipArchiveEntry entry = bcfzip.GetEntry(filename);
      XmlSerializer serializer = new XmlSerializer(typeof(BCFvisinfo.VisualizationInfo));
      BCFvisinfo.VisualizationInfo bcfv = (BCFvisinfo.VisualizationInfo)serializer.Deserialize(new XmlTextReader(entry.Open()));
      return bcfv;
    }

    /// <summary> Load a BCF File </summary> 
    /// <param name="FileName">Full path of the file to be loadded or appended </param>
    /// <returns>Nothing</returns>
    public void ReadBCF(String FileName) {
      FullName = FileName;
      MarkupsList.Clear();
      using(bcfzip = ZipFile.OpenRead(FileName)) {
        ReadFile();
        foreach(ZipArchiveEntry entry in bcfzip.Entries) {
          if(entry.FullName.EndsWith("markup.bcf", StringComparison.OrdinalIgnoreCase)) {
            string folder = entry.FullName.Substring(0, entry.FullName.LastIndexOf("/") + 1);
            XmlSerializer serializer = new XmlSerializer(typeof(BCFmarkup.Markup));
            BCFmarkup.Markup markup = (BCFmarkup.Markup)serializer.Deserialize(new XmlTextReader(entry.Open()));
            MarkupsList.Add(markup);
            // Search for Viewpoints for Version 2.0
            foreach(BCFmarkup.ViewPoint vp in markup.Viewpoints) {
              vp.Bcfv = ReadBCFV(folder + vp.Viewpoint);
              vp.Image = ReadSnapshot(folder + vp.Snapshot);
            }
            // If Viewpoints is empty, try Version 1.0 : viewpoint.bcfv files.
            if(markup.Viewpoints.Count == 0 && bcfzip.GetEntry(folder + "viewpoint.bcfv") != null) {
              // Found viewpoint.bcfv !
              BCFmarkup.ViewPoint NewViewpoint = new BCFmarkup.ViewPoint();
              NewViewpoint.Guid = "viewpointbcfv"; // pseudo GUID
              NewViewpoint.Viewpoint = "viewpoint.bcfv";
              NewViewpoint.Snapshot = "snapshot.png";
              NewViewpoint.Bcfv = ReadBCFV(folder + NewViewpoint.Viewpoint);
              NewViewpoint.Image = ReadSnapshot(folder + NewViewpoint.Snapshot);
              if(NewViewpoint.Image == null) NewViewpoint.Snapshot = null; // Image not found !
              markup.Viewpoints.Add(NewViewpoint);
            }
            foreach(BCFmarkup.Comment com in markup.Comment) {
              if(com.Viewpoint != null) {
                foreach(BCFmarkup.ViewPoint vp in markup.Viewpoints) {
                  if(vp.Guid == com.Viewpoint.Guid) {
                    com.MarkupViewPoint = vp;
                  }
                }
              }
            }
          } // Endif markup
        } // Next entry
      } // End using
    }


    /// <summary> Clear the Topics List </summary> 
    /// <returns> Nothing </returns>
    public void NewBCF() {
      MarkupsList.Clear();
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
