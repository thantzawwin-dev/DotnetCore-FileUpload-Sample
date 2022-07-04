using System.Net;

namespace FileUploadApi.Models;

public class Response {
  public HttpStatusCode Status {get;set;}
  public string? Message {get;set;}
}

public class FileUploadRequest
{
  public IFormFile? file { get; set; }

  public FileUploadRequest()
  {
    file = null;
  }
}

public class FileUploadResponse: Response
{
  public MyFile? file { get; set; }

  public FileUploadResponse()
  {
    file = null;
  }
}