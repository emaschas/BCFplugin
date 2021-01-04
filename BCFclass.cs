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
/// ![Screenshot Image](Screenshot.png)
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

namespace BCFmarkup {

  public partial class ViewPoint {
    /// <summary>Content of the VisualisationInfo (.bcfv) file </summary>
    /// _In addition to the class extracted from the BCF schema_
    public BCFvisinfo.VisualizationInfo Bcfv { get; set; }
    /// <summary>Content (Bitmap) of the snapshot file </summary>
    /// _In addition to the class extracted from the BCF schema_
    public Bitmap Image { get; set; }
  }

  public partial class Comment {
    /// <summary>Content of the VisualisationInfo (.bcfv) file </summary>
    /// _In addition to the class extracted from the BCF schema_
    public BCFmarkup.ViewPoint MarkupViewPoint { get; set; }
  }

}

namespace BCFvisinfo {

  public partial class VisualizationInfo {
    /// <summary>Returns if a Camera is defined (either Orthogonal or Perspective</summary>
    /// _In addition to the class extracted from the BCF schema_
    public Boolean CameraDefined() {
      return (this.OrthogonalCamera != null || this.PerspectiveCamera != null);
    }
    /// <summary>Returns if an Orthogonal Camera is defined</summary>
    /// _In addition to the class extracted from the BCF schema_
    public Boolean OrthogonalDefined() {
      return (this.OrthogonalCamera != null);
    }
    /// <summary>Returns if an Orthogonal Camera is defined</summary>
    /// _In addition to the class extracted from the BCF schema_
    public Boolean PerspectiveDefined() {
      return (this.PerspectiveCamera != null);
    }
    /// <summary>Returns a text with Camera definition</summary>
    /// _In addition to the class extracted from the BCF schema_
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
/// _In addition to the BCF schema_
namespace BCFclass {

  /// <summary> Content of a BCF file </summary>
  /// _In addition to the classes extracted from the BCF schema_
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
