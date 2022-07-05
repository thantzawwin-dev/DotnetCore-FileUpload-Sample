using Microsoft.AspNetCore.Mvc;

using FileUploadApi.Models;
using FileUploadApi.Common;
using System.Net;

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

		if(!FileUploadHandler.CheckIfImageFile(request.file))
		{
			return new FileUploadResponse(HttpStatusCode.UnsupportedMediaType, "Just Allow Image!");
		}

		ResultStatus fileResult = FileUploadHandler.WriteFileUpload(request.file, FileUploadHandler.subFolder);
		if (fileResult.status) {
			response.file = fileResult.data;
			response.Status = HttpStatusCode.OK;
			response.Message = "Success";
		}
        
		return response;
    }
}
