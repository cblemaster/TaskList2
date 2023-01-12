using TaskList2.Data.Models;

namespace TaskList2.Data.DAL
{
	public interface IFolderDAO
	{
		Folder GetFolder(int id);
		List<Folder> GetFolders();
		Folder AddFolder(Folder folderToAdd);
		Folder UpdateFolder(Folder folderToUpdate);
		bool DeleteFolder(int id);
	}
}
