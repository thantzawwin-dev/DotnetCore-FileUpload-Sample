using Microsoft.AspNetCore.Mvc;

using FileUploadApi.Models;
using FileUploadApi.Common;

namespace FileUploadApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileUploadController : ControllerBase
{

  private readonly ILogger<FileUploadController> _logger;

  public FileUploadController(ILogger<FileUploadController> logger)
  {
    _logger = logger;
  }

  [HttpPost("[action]")]
  public FileUploadResponse Save([FromForm] FileUploadRequest request)
  {
    FileUploadResponse response = new FileUploadResponse();

    ResultStatus fileResult = FileUploadHandler.WriteFileUpload(request.file, FileUploadHandler.subFolder);
    if (fileResult.status) request.file = fileResult.data;
    return response;
  }
}
