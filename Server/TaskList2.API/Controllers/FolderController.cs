using Microsoft.AspNetCore.Mvc;
using TaskList2.Data.DAL;
using TaskList2.Data.Models;

namespace TaskList2.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FolderController : ControllerBase
    {
        private readonly IFolderDAO _folderDAO;
        public FolderController(IFolderDAO folderDAO)
        {
            _folderDAO = folderDAO;
        }

        // https://localhost:7021/Folder/1
        [HttpGet("{id}")]
        public IActionResult GetFolder(int id)
        {
            Folder f = _folderDAO.GetFolder(id);

            if (f != null)
                return Ok(f);
            else
                return NotFound();
        }


        // https://localhost:7021/Folder
        [HttpGet]
        public IActionResult GetFolders()
        {
            List<Folder> fList = _folderDAO.GetFolders();

            if (fList != null && fList.Any())
                return Ok(fList);
            else
                return NotFound();
        }

        // https://localhost:7021/Folder
        [HttpPost]
        public IActionResult AddFolder(Folder folderToAdd)
        {
            Folder created = _folderDAO.AddFolder(folderToAdd);

            if (created != null && created.Id > 0)
                return Created("Folder/" + created.Id, created);
            else
                return BadRequest();
        }

        // https://localhost:7021/Folder/1
        [HttpPut("{id}")]
        public IActionResult UpdateFolder(Folder folderToUpdate)
        {
            Folder existing = _folderDAO.GetFolder(folderToUpdate.Id);

            if (existing == null)
                return NotFound("Folder not found");
            else if (folderToUpdate.IsRenameable)
            {
                Folder updated = _folderDAO.UpdateFolder(folderToUpdate);
                return updated != null ? Ok(updated) : StatusCode(500); //if delete is successful, return NoContent, otherwise 500 internal server error
            }
            else
                return BadRequest();
        }

        // https://localhost:7021/Folder/1
        [HttpDelete("{id}")]
        public IActionResult DeleteFolder(int id)
        {
            bool success = false;
            Folder deleted = _folderDAO.GetFolder(id);

            if (deleted == null)
                return NotFound("Folder not found");
            else if (deleted.IsDeleteable)
                success = _folderDAO.DeleteFolder(id);

            return success ? NoContent() : StatusCode(500); //if delete is successful, return NoContent, otherwise 500 internal server error
        }
    }
}