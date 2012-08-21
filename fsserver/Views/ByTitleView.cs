using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NMaier.sdlna.FileMediaServer.Folders;
using NMaier.sdlna.Server;

namespace NMaier.sdlna.FileMediaServer.Views
{
  class ByTitleView : IView
  {
    private class TitlesFolder : KeyedVirtualFolder<VirtualFolder>
    {
      public TitlesFolder(FileServer aServer, IFileServerFolder aParent) : base(aServer, aParent, "") { }
    }


    private static Regex regClean = new Regex(@"[^\d\w]+", RegexOptions.Compiled);



    public string Description
    {
      get { return "Reorganizes files into folders by title"; }
    }

    public string Name
    {
      get { return "bytitle"; }
    }




    public void Transform(FileServer Server, IMediaFolder Root)
    {
      var root = Root as IFileServerFolder;
      var titles = new TitlesFolder(Server, root);
      SortFolder(Server, root, titles);
      foreach (var i in root.ChildFolders.ToList()) {
        root.ReleaseItem(i as IFileServerMediaItem);
      }
      foreach (var i in titles.ChildFolders.ToList()) {
        root.AdoptItem(i as IFileServerFolder);
      }
    }

    private void SortFolder(FileServer server, IFileServerFolder folder, TitlesFolder titles)
    {
      foreach (var f in folder.ChildFolders.ToList()) {
        SortFolder(server, f as IFileServerFolder, titles);
      }

      foreach (var c in folder.ChildItems.ToList()) {
        var pre = regClean.Replace(c.Title, "");
        if (string.IsNullOrEmpty(pre)) {
          pre = "Unnamed";
        }
        pre = pre.First().ToString().ToUpper();
        titles.GetFolder(pre).AdoptItem(c as IFileServerMediaItem);
      }
    }
  }
}
