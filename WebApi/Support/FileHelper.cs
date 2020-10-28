using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace WebApi.Support
{
    public static class FileHelper
    {
        ///
        /// Utility method used to validate the contents of uploaded files
        ///
        public static bool FileIsWebFriendlyImage(Stream stream)
        {
            try
            {
                //Read an image from the stream...
                Image i = Image.FromStream(stream);
                //Move the pointer back to the beginning of the stream
                stream.Seek(0, SeekOrigin.Begin);
                return ImageFormat.Jpeg.Equals(i.RawFormat) || ImageFormat.Png.Equals(i.RawFormat) || ImageFormat.Gif.Equals(i.RawFormat);
            }
            catch
            {
                return false;
            }
        }

        ///
        /// Utility method used to validate the contents of uploaded files using filesize
        /// <param name="size">The maximum size of the file in bytes</param>
        ///
        public static bool FileIsWebFriendlyImage(Stream stream, long size)
        {
            return stream.Length <= size && FileIsWebFriendlyImage(stream);
        }

        ///
        /// Utility method used to validate the contents of uploaded files using filesize
        /// <param name="size">The maximum size of the file in bytes</param>
        /// <param name="filename">the path of the file</param>
        ///
        public static bool FileIsValidFormat(string filename)
        {
            string extension = Path.GetExtension(filename).ToLower();
            if
            (
            extension == ".rar" ||
            extension == ".zip" ||
            extension == ".csv" ||
            extension == ".jpeg" ||
            extension == ".jpg" ||
            extension == ".png" ||
            extension == ".psd" ||
            extension == ".svg" ||
            extension == ".ico" ||
            extension == ".odp" ||
            extension == ".pps" ||
            extension == ".ppt" ||
            extension == ".pptx" ||
            extension == ".ods" ||
            extension == ".xls" ||
            extension == ".xlsx" ||
            extension == ".txt" ||
            extension == ".pdf" ||
            extension == ".doc" ||
            extension == ".docx" ||
            extension == ".odt" ||
            extension == ".mp4"
            ) return true;
            else return false;
        }

        ///
        /// Utility method used to validate the contents of uploaded files using filesize
        /// <param name="size">The maximum size of the file in bytes</param>
        /// <param name="filename">the path of the file</param>
        ///
        public static bool FileIsValidFormat(string filename, long size, Stream stream)
        {
            return stream.Length <= size && FileIsValidFormat(filename);
        }

        ///
        /// Utility method used to recover binary string from filepath to add images without revealing the underlying directories
        /// <param name="path">The path to be processed</param>
        ///
        public static string ImageBytes(IWebHostEnvironment env, string path)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(path) && FileExists(env, path))
                {
                    string fullPath = PathCombine(env.WebRootPath, path);
                    return $"data:image/{Path.GetExtension(fullPath).Replace(".", "")};base64,{Convert.ToBase64String(File.ReadAllBytes(fullPath))}";
                }
                else return "";
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static bool FileExists(IWebHostEnvironment env, string path)
        {
            try
            {
                return File.Exists(PathCombine(env.WebRootPath, path));
            }
            catch
            {
                return false;
            }
        }

        private static readonly string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        /// <summary>
        /// Converts file size into appropriate suffix
        /// </summary>
        /// <param name="value">Length of file</param>
        /// <param name="decimalPlaces">Number of decimal places</param>
        /// <returns></returns>
        public static string SizeSuffix(long value, int decimalPlaces = 2)
        {
            if (decimalPlaces < 0) { throw new ArgumentOutOfRangeException(nameof(decimalPlaces)); }
            if (value < 0) { return $"-{SizeSuffix(-value)}"; }
            if (value == 0) { return string.Format("{0:n" + decimalPlaces + "} bytes", 0); }

            // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            int mag = (int)Math.Log(value, 1024);

            // 1L << (mag * 10) == 2 ^ (10 * mag)
            // [i.e. the number of bytes in the unit corresponding to mag]
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            // make adjustment when the value is large enough that
            // it would round up to 1000 or more
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}",
                adjustedSize,
                SizeSuffixes[mag]);
        }

        ///
        /// Utility method used to validate the contents of uploaded files
        ///
        public static bool FileIsPDFSafe(string fileType)
        {
            try
            {
                List<string> acceptedFileTypes = new List<string>
                {
                    ".pdf"
                };

                if (acceptedFileTypes.Contains(fileType.ToLower()))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        ///
        /// Utility method used to validate the contents of uploaded files using filesize
        /// <param name="size">The maximum size of the file in bytes</param>
        ///
        public static bool FileIsPDFSafe(string fileType, long fileSize)
        {
            return fileSize <= 10240000 && FileIsPDFSafe(fileType);
        }

        public static string PathCombine(string path1, string path2)
        {
            if (Path.IsPathRooted(path2))
            {
                path2 = path2.TrimStart(Path.DirectorySeparatorChar);
                path2 = path2.TrimStart(Path.AltDirectorySeparatorChar);
            }

            return Path.Combine(path1, path2);
        }

        public static string PathCombine(params string[] paths)
        {
            string finalpath = "";
            if (paths.Length > 0) finalpath = paths[0];
            for (int i = 1; i < paths.Length; i++)
            {
                finalpath = PathCombine(finalpath, paths[i]);
            }
            return finalpath;
        }

        public static async Task<RequestFeedback> UploadFormFile(IFormFile file, IWebHostEnvironment env, string path)
        {
            RequestFeedback request = new RequestFeedback();
            try
            {
                string mediaPath = PathCombine(env.WebRootPath, path);
                Directory.CreateDirectory(mediaPath);

                string Filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string fullpath = PathCombine(mediaPath, Filename);
                using (FileStream fileStream = new FileStream(fullpath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                request.Success = true;
                request.Data = PathCombine(path, Filename);
                return request;
            }
            catch (Exception e)
            {
                request.Data = e.Message;
                return request;
            }
        }

        public static async Task<RequestFeedback> UploadFileStream(FileStream file, string FileName, IWebHostEnvironment env, string path)
        {
            RequestFeedback request = new RequestFeedback();
            try
            {
                string mediaPath = PathCombine(env.WebRootPath, path);
                Directory.CreateDirectory(mediaPath);

                string Filename = Guid.NewGuid().ToString() + Path.GetExtension(FileName);
                string fullpath = PathCombine(mediaPath, Filename);
                using (FileStream fileStream = new FileStream(fullpath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                request.Success = true;
                request.Data = PathCombine(path, Filename);
                return request;
            }
            catch (Exception e)
            {
                request.Data = e.Message;
                return request;
            }
        }

        public static RequestFeedback DeleteFormFile(IWebHostEnvironment env, string path)
        {
            RequestFeedback request = new RequestFeedback();
            try
            {
                string fullpath = PathCombine(env.WebRootPath, path);
                if (File.Exists(fullpath))
                {
                    File.Delete(fullpath);
                    request.Success = true;
                }
                return request;
            }
            catch (Exception e)
            {
                request.Data = e.Message;
                return request;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2200:Rethrow to preserve stack details.", Justification = "This method will not treat it's own exception and should throw it forward")]
        public static FileContentResult GetFile(Controller controller, IWebHostEnvironment env, string Path, string FileName = "File")
        {
            try
            {
                string fullpath = PathCombine(env.WebRootPath, Path);
                if (File.Exists(fullpath))
                {
                    byte[] fileBytes = File.ReadAllBytes(fullpath);
                    string fileName = FileName + System.IO.Path.GetExtension(fullpath);
                    return controller.File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                }
                else throw new Exception("File not found.");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}