

namespace FileUploadApi.Models;

public class ResultStatus
{
  public bool status { get; set; }
  public string message { get; set; }
  public dynamic? data { get; set; }

  public ResultStatus()
  {
    status = true;
    message = string.Empty;
    data = null;
  }

  public ResultStatus(bool sts, string msg)
  {
    status = sts;
    message = msg;
  }
}

public class Collection
{
  public int totalRecord { get; set; }
  public dynamic? records { get; set; }

  public Collection()
  {
    totalRecord = 0;
    records = null;
  }
}

public class Select
{
  public dynamic? value { get; set; }
  public string? label { get; set; }
}

public class TreeViewSelect
{
  public dynamic? value { get; set; }
  public string? label { get; set; }
  public List<Select>? children { get; set; }
}

public class MyFile
{
  public string name { get; set; }
  public string fileName { get; set; }
  public string path { get; set; }
  public string contentType { get; set; }
  public long size { get; set; }
  public bool isToDelete { get; set; }

  public MyFile()
  {
    name = string.Empty;
    fileName = string.Empty;
    path = string.Empty;
    size = 0;
    isToDelete = false;
    contentType = string.Empty;
  }
}