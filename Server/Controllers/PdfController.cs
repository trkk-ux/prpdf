using Microsoft.AspNetCore.Mvc;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using System.Drawing;
using System.IO;

namespace prpdf.Server.Controllers;

[ApiController]
[Route("api/pdf")]
public class PdfController : ControllerBase
{
    [HttpPost]
    public IActionResult GeneratePdf([FromBody] string text)
    {
        try
        {
            // إنشاء مستند PDF جديد
            using var document = new PdfDocument();
            var page = document.Pages.Add();

            // تحديد مسار الخط
            var fontPath = Path.Combine("wwwroot", "fonts", "Amiri-Regular.ttf");

            if (!System.IO.File.Exists(fontPath))
                return BadRequest("❌ ملف الخط Amiri-Regular.ttf غير موجود في wwwroot/fonts");

            // تحميل الخط
            using var fontStream = new FileStream(fontPath, FileMode.Open, FileAccess.Read);
            var font = new PdfTrueTypeFont(fontStream, 16);

            // تنسيق النص من اليمين لليسار
            var format = new PdfStringFormat
            {
                RightToLeft = true,
                Alignment = PdfTextAlignment.Right
            };

            // طباعة النص في الصفحة
            page.Graphics.DrawString(text, font, PdfBrushes.Black,
                new RectangleF(0, 0, page.GetClientSize().Width, 100), format);

            // حفظ المستند في الذاكرة
            using var stream = new MemoryStream();
            document.Save(stream);
            stream.Position = 0;

            return File(stream.ToArray(), "application/pdf", "تقرير.pdf");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"❌ حدث خطأ أثناء توليد PDF: {ex.Message}");
        }
    }
}