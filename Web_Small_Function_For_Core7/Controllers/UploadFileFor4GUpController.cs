using Microsoft.AspNetCore.Mvc;

namespace Web_Small_Function_For_Core7.Controllers
{
    public class UploadFileFor4GUpController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UploadFileFor4GUpFiles");
        public UploadFileFor4GUpController(ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }
        public IActionResult UploadFileFor4GUp()
        {
            return View();
        }

        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            var File = file.FileName.Split('_')[0];
            var tempPath = Path.Combine(uploadPath, "temp");
            tempPath = Path.Combine(tempPath, File);
            try
            {
                // 處理檔案上傳，並存到temp目錄
                if (file != null && file.Length > 0)
                {

                    if (!Directory.Exists(tempPath))
                    {
                        Directory.CreateDirectory(tempPath);
                    }

                    var filePath = Path.Combine(tempPath, file.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    return Ok(new { message = "上傳成功" });
                }
            }
            catch
            {
                Directory.Delete(tempPath, true);
                return BadRequest(new { message = "失敗" });
            }
            return BadRequest(new { message = "失敗" });
        }

        [HttpPost]
        public IActionResult MergeFiles(string fileName)
        {
            string FindfileName = fileName.Split('.')[0];
            // 合併文件塊
            var tempPath = Path.Combine(uploadPath, "temp");
            tempPath = Path.Combine(tempPath, FindfileName);

            var outputPath = Path.Combine(uploadPath, "complete", fileName);
            var tempFiles = Directory.GetFiles(tempPath).Where(f => f.StartsWith(tempPath));
            try
            {
                using (var output = new FileStream(outputPath, FileMode.Create))
                {
                    foreach (var tempFile in tempFiles)
                    {
                        using (var input = new FileStream(tempFile, FileMode.Open))
                        {
                            input.CopyTo(output);
                        }
                        System.IO.File.Delete(tempFile); // 删除临时文件
                    }
                }
            }
            finally
            {
                Directory.Delete(tempPath, true);
            }

            return Ok(new { message = "檔案合併成功" });
        }

    }
}
