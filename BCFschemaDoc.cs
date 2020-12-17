//
//  The BCF file XSD schemas are converted into C# classes with xsgen :
//    xscgen --dc -v -n =BCFversion version.xsd
//    xscgen --dc -v -n =BCFproject project.xsd
//    xscgen --dc -v -n =BCFmarkup  markup.xsd
//    xscgen --dc -v -n =BCFvisinfo visinfo.xsd
//
// This file adds code Documentation to the extracted classes
//

/// <summary> BCF file Markup elements </summary>
/// Definition of elements appearing in the **markup.bcf** file
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
/// Definition of elements appearing in the <b>*.bcfv</b> files
namespace BCFvisinfo {

  /// <summary>Visualization Info (.bcfv) definition</summary>
  /// The Visualization Info includes the following properties :
  /// <list type="table">
  /// <item><term><see cref="Components"/></term><description> The components node contains a set of Component references<br/>
  ///   The components node has also the DefaultVisibility attribute which indicates true or false for all components of the viewpoint</description></item>
  /// <item><term><see cref="OrthogonalCamera"/></term><description> This element describes a viewpoint using orthogonal camera</description></item>
  /// <item><term><see cref="PerspectiveCamera"/></term><description> This element describes a viewpoint using perspective camera</description></item>
  /// <item><term><see cref="Lines"/></term><description> Lines can be used to add markup in 3D</description></item>
  /// <item><term><see cref="ClippingPlanes"/></term><description> ClippingPlanes can be used to define a subsection of a building model that is related to the topic</description></item>
  /// <item><term><see cref="Bitmap"/></term><description> A list of bitmaps can be used to add more information, for example, text in the visualization</description></item>
  /// <item><term><see cref="Guid"/></term><description> Guid of the viewpoint</description></item>
  /// </list>
  ///   The numeric values in this file are all given in fixed units (meters for length and degrees for angle)<br/>
  ///   Unit conversion is not required, since the values are not relevant to the user<br/>
  public partial class VisualizationInfo {
    /// \fn Components Components[get, set]
    ///   The components node contains a set of Component references<br/>
    ///   The components node has also the DefaultVisibility attribute which indicates true or false for all components of the viewpoint
    /// \fn OrthogonalCamera OrthogonalCamera[get, set]
    ///   This element describes a viewpoint using orthogonal camera
    /// \fn PerspectiveCamera PerspectiveCamera[get, set]
    ///   This element describes a viewpoint using perspective camera<br/>
    /// \fn System.Collections.ObjectModel.Collection< Line > Lines[get]
    ///   Lines can be used to add markup in 3D<br/>
    ///   Each line is defined by three dimensional Start Point and End Point<br/>
    ///   Lines that have the same start and end points are to be considered points and may be displayed accordingly
    /// \fn System.Collections.ObjectModel.Collection< ClippingPlane > ClippingPlanes[get]
    ///   ClippingPlanes can be used to define a subsection of a building model that is related to the topic<br/>
    ///   Each clipping plane is defined by a location Point and a normal Direction
    /// \fn System.Collections.ObjectModel.Collection< VisualizationInfoBitmap > Bitmap[get]
    ///   A list of bitmaps can be used to add more information, for example, text in the visualization
    /// \fn string Guid[get, set]
    ///   Guid of the viewpoint
  }

  /// <summary> The PerspectiveCamera element describes a viewpoint using perspective camera </summary>
  /// It has the following elements:
  /// <list type="table">
  /// <item><term><see cref="CameraViewPoint"/></term><description>Camera location</description></item>
  /// <item><term><see cref="CameraDirection"/></term><description>Camera direction</description></item>
  /// <item><term><see cref="CameraUpVector"/></term><description>Camera up vector</description></item>
  /// <item><term><see cref="FieldOfView"/></term><description>Camera’s field of view angle in degrees</description></item>
  /// </list>
  /// The FieldOfView is currently restricted to a value between 45 and 60 degrees<br/>
  /// There may be viewpoints that are not within this range, therefore imports should be expecting any values between 0 and 360 degrees<br/>
  /// The limitation will be dropped in the next schema release
  public partial class PerspectiveCamera {
    /// \fn Point CameraViewPoint[get, set]
    ///   Camera location Point (in meters)
    /// \fn Direction CameraDirection[get, set]
    ///   Camera view Direction
    /// \fn Direction CameraUpVector[get, set]
    ///   Camera up vector Direction
    /// \fn double FieldOfView[get, set]
    ///   Camera’s field of view angle (in degrees)<br/>
    ///   The FieldOfView is currently restricted to a value between 45 and 60 degrees<br/>
    ///   There may be viewpoints that are not within this range, therefore imports should be expecting any values between 0 and 360 degrees<br/>
    ///   The limitation will be dropped in the next schema release
  }

  /// <summary> The OrthogonalCamera element describes a viewpoint using orthogonal camera </summary>
  /// It has the following elements:
  /// <list type="table">
  /// <item><term><see cref="CameraViewPoint"/></term><description>Camera location</description></item>
  /// <item><term><see cref="CameraDirection"/></term><description>Camera direction</description></item>
  /// <item><term><see cref="CameraUpVector"/></term><description>Camera up vector</description></item>
  /// <item><term><see cref="ViewToWorldScale"/></term><description>Scaling from view to world</description></item>
  /// </list>
  public partial class OrthogonalCamera {
    /// \fn Point CameraViewPoint[get, set]
    ///   Camera location Point (in meters)
    /// \fn Direction CameraDirection[get, set]
    ///   Camera view Direction
    /// \fn Direction CameraUpVector[get, set]
    ///   Camera up vector Direction
    /// \fn double ViewToWorldScale[get, set]
    ///   Scaling from view to world
  }

  /// <summary>3D direction unitary vector<summary>
  public partial class Direction {
    /// \fn double X[get, set]
    ///   X axis component of the 3D direction vector
    /// \fn double Y[get, set]
    ///   Y axis component of the 3D direction vector
    /// \fn double Z[get, set]
    ///   Z axis component of the 3D direction vector
  }

  /// <summary>3D point definition<summary>
  public partial class Point {
    /// \fn double X[get, set]
    ///   X axis component of the 3D point
    /// \fn double Y[get, set]
    ///   Y axis component of the 3D point
    /// \fn double Z[get, set]
    ///   Z axis component of the 3D point
  }

}


/// <summary> BCF file Version elements </summary>
/// Definition of elements appearing in the **bcf.version** file
namespace BCFversion {

  /// <summary> Version of the BCF schema used<summary>
  public partial class Version {
    /// \fn string DetailedVersion[get, set]
    ///   Detailed Version of the BCF schema used
    /// \fn string VersionId[get, set]
    ///   Version ID of the BCF schema used
  }
}