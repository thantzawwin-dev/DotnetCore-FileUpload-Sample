using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using FileUploadApi.Models;

using Newtonsoft.Json;
using FileUploadApi.Common;

namespace FileUploadApi.Common;
public class FileUploadHandler
{

  public static string baseFolder = "./UploadData";
  public static string subFolder = "Images";

  public static void InitDevImagePath()
  {
    baseFolder = "./UploadData";
    subFolder = "Images";
  }

  public static void InitProductionImagePath()
  {
    baseFolder = "./UploadData";
    subFolder = "Images";
  }

  public enum ImageFormat
  {
    bmp,
    jpeg,
    gif,
    tiff,
    png,
    unknown
  }

  public enum FileFormat
  {
    bmp,
    jpeg,
    gif,
    tiff,
    png,
    doc,
    docx,
    xls,
    xlsx,
    ppt,
    pptx,
    pdf,
    unknown
  }

  public static ImageFormat GetImageFormat(byte[] bytes)
  {
    var bmp = Encoding.ASCII.GetBytes("BM");     // BMP
    var gif = Encoding.ASCII.GetBytes("GIF");    // GIF
    var png = new byte[] { 137, 80, 78, 71 };    // PNG
    var tiff = new byte[] { 73, 73, 42 };         // TIFF
    var tiff2 = new byte[] { 77, 77, 42 };         // TIFF
    var jpeg = new byte[] { 255, 216, 255, 224 }; // jpeg
    var jpeg2 = new byte[] { 255, 216, 255, 225 }; // jpeg canon

    if (bmp.SequenceEqual(bytes.Take(bmp.Length)))
      return ImageFormat.bmp;

    if (gif.SequenceEqual(bytes.Take(gif.Length)))
      return ImageFormat.gif;

    if (png.SequenceEqual(bytes.Take(png.Length)))
      return ImageFormat.png;

    if (tiff.SequenceEqual(bytes.Take(tiff.Length)))
      return ImageFormat.tiff;

    if (tiff2.SequenceEqual(bytes.Take(tiff2.Length)))
      return ImageFormat.tiff;

    if (jpeg.SequenceEqual(bytes.Take(jpeg.Length)))
      return ImageFormat.jpeg;

    if (jpeg2.SequenceEqual(bytes.Take(jpeg2.Length)))
      return ImageFormat.jpeg;

    return ImageFormat.unknown;
  }

  public static FileFormat GetFileFormat(string extension)
  {
    /*var bmp = Encoding.ASCII.GetBytes("BM");     // BMP
    var gif = Encoding.ASCII.GetBytes("GIF");    // GIF*/
    var doc = "application/msword"; // DOC
    var docx = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";    // DOCX
    var xls = "application/vnd.ms-excel";    // XLS
    var xlsx = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";    // xlsx
    var ppt = "application/vnd.ms-powerpoint";    // PPT
    var pptx = "application/vnd.openxmlformats-officedocument.presentationml.presentation";    // PPTX
    var pdf = "application/pdf";    // pdf
    /*var png = new byte[] { 137, 80, 78, 71 };    // PNG
    var tiff = new byte[] { 73, 73, 42 };         // TIFF
    var tiff2 = new byte[] { 77, 77, 42 };         // TIFF
    var jpeg = new byte[] { 255, 216, 255, 224 }; // jpeg
    var jpeg2 = new byte[] { 255, 216, 255, 225 }; // jpeg canon*/

    if (extension == doc)
      return FileFormat.doc;
    if (extension == docx)
      return FileFormat.docx;
    if (extension == xls)
      return FileFormat.xls;
    if (extension == xlsx)
      return FileFormat.xlsx;
    if (extension == ppt)
      return FileFormat.ppt;
    if (extension == pptx)
      return FileFormat.pptx;
    if (extension == pdf)
      return FileFormat.pdf;

    return FileFormat.unknown;
  }

  public static bool CheckIfImageFile(IFormFile file)
  {
    byte[] fileBytes;
    using (var ms = new MemoryStream())
    {
      file.CopyTo(ms);
      fileBytes = ms.ToArray();
    }

    return GetImageFormat(fileBytes) != ImageFormat.unknown;
  }

  public static ResultStatus WriteFileUpload(IFormFile? file, string relativePath)
  {
    ResultStatus result = new ResultStatus();
    string path = "";
    string fileName = "";
    string extension = "";
    string fileDir = "";
    string savePath = "";
    try
    {
      extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
      path = relativePath.Substring(0, relativePath.LastIndexOf('/') + 1);
      fileName = relativePath.Split('/')[relativePath.Split('/').Length - 1];
      fileName = fileName.Substring(0, fileName.Length - 4) + extension;

      fileDir = Path.Combine(Directory.GetCurrentDirectory(), baseFolder, path);
      savePath = Path.Combine(fileDir, fileName);

      if (!Directory.Exists(fileDir))
      { //check if the folder exists;
        Directory.CreateDirectory(fileDir);
      }
      using (var bits = new FileStream(savePath, FileMode.Create))
      {
        // file.CopyToAsync(bits);
        file.CopyTo(bits);
        bits.Flush();
      }
      result.data = new MyFile()
      {
        name = file.FileName,
        fileName = fileName,
        path = "/" + path,
        size = file.Length,
        contentType = file.ContentType,
      };
    }
    catch (Exception e)
    {
      // result.status = false;
      // result.message = e.Message;
      return new ResultStatus(false, e.Message);
      // return result;
    }
    return result;
  }

  public static ResultStatus DeleteFile(string filePath)
  {
    string fullPath = "";
    // Files to be deleted    
    // string authorsFile = "Authors.txt";  
    ResultStatus result = new ResultStatus();
    try
    {
      // Check if file exists with its full path    
      fullPath = Path.Combine(Directory.GetCurrentDirectory(), baseFolder, filePath.Substring(1, filePath.Length - 1));
      if (File.Exists(fullPath))
      {
        // If file found, delete it    
        File.Delete(fullPath);
        Console.WriteLine("File deleted.");
      }
      else
        Console.WriteLine("File not found");
    }
    catch (IOException ioExp)
    {
      result.status = false;
      result.message = ioExp.Message;
      return result;
    }
    return result;
  }

  #region File Upload Handling By Base64 String

  // public static ResultStatus WriteFileUpload(string base64string, string relativeFileDir)
  // {
  //   ResultStatus result = new ResultStatus();

  //   string s = base64string.Remove(base64string.IndexOf(';'));
  //   string extension = s.Remove(0, s.IndexOf(':') + 1);            

  //   base64string = base64string.Remove(0, base64string.IndexOf(',') + 1);
  //   var fileBytes = Convert.FromBase64String(base64string);// a.base64image 

  //   FileFormat fileFormat = GetFileFormat(extension);
  //   if (fileFormat == FileFormat.unknown)
  //   {
  //     result.status = false;
  //     result.message = "Invalid file format.";
  //     return result;
  //   }
  //   string fileName = Guid.NewGuid().ToString() + "." + fileFormat.ToString(); //Create a new Name for the file due to security reasons.
  //   string fileDir = Path.Combine(Directory.GetCurrentDirectory(), baseFolder, relativeFileDir);
  //   string savePath = Path.Combine(fileDir, fileName);

  //   if (!Directory.Exists(fileDir))
  //   { //check if the folder exists;
  //     Directory.CreateDirectory(fileDir);
  //   }
  //   Debug.WriteLine(savePath);
  //   try
  //   {
  //     if (fileBytes.Length > 0)
  //     {
  //       using (var stream = new FileStream(savePath, FileMode.Create))
  //       {
  //         stream.Write(fileBytes, 0, fileBytes.Length);
  //         stream.Flush();
  //       }
  //     }
  //   }
  //   catch (Exception exp)
  //   {
  //     result.status = false;
  //     result.message = exp.Message;
  //     return result;
  //   }
  //   string dbFilePath = Path.Combine("/", relativeFileDir, fileName);
  //   dbFilePath = dbFilePath.Replace('\\', '/');
  //   result.data = dbFilePath;
  //   return result;
  // }

  // public static ResultStatus DeleteImage(string[] Images, string[] ImagesToDelete, string path) {
  //   ResultStatus result1 = new ResultStatus();
  //   foreach (string imgPath in ImagesToDelete)
  //   {
  //     if (string.IsNullOrEmpty(imgPath) || imgPath.IndexOf("static/") == -1) continue;
  //     string basePath = Path.Combine(Directory.GetCurrentDirectory(), baseFolder);
  //     basePath = basePath.Replace("\\","/");
  //     System.IO.File.Delete(basePath+imgPath);
  //   }

  //   List<string> ImagePaths = new List<string>();
  //   foreach (string imgStr in Images)
  //   {
  //     if (string.IsNullOrEmpty(imgStr)) continue; 
  //     if (imgStr.IndexOf("static/") != -1)
  //     {
  //       ImagePaths.Add(imgStr);
  //     } else {
  //       ResultStatus result = WriteImage(imgStr, path);
  //     if (result.status)
  //       ImagePaths.Add(result.data.ToString());
  //     }
  //   }
  //   result1.status = true;
  //   result1.data = String.Join(';', ImagePaths.ToArray());
  //   return result1;
  // }

  // public static ResultStatus DeleteFile(string[] Files, string[] FilesToDelete, string path)
  // {
  //   ResultStatus result1 = new ResultStatus();
  //   foreach (string filePath in FilesToDelete)
  //   {
  //     if (string.IsNullOrEmpty(filePath) || filePath.IndexOf("static/") == -1) continue;
  //     string basePath = Path.Combine(Directory.GetCurrentDirectory(), baseFolder);
  //     basePath = basePath.Replace("\\", "/");
  //     System.IO.File.Delete(basePath + filePath);
  //   }

  //   List<string> FilePaths = new List<string>();
  //   foreach (string fileStr in Files)
  //   {
  //     if (string.IsNullOrEmpty(fileStr)) continue;
  //     if (fileStr.IndexOf("static/") != -1)
  //     {
  //       FilePaths.Add(fileStr);
  //     }
  //     else
  //     {
  //       ResultStatus result = WriteFileUpload(fileStr, path);
  //       if (result.status)
  //         FilePaths.Add(result.data.ToString());
  //     }
  //   }
  //   result1.status = true;
  //   result1.data = String.Join(';', FilePaths.ToArray());
  //   return result1;
  // }

  #endregion
}