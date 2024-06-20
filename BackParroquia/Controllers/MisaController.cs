using BackParroquia.Models;
using BackParroquia.Repositories;
//using iTextSharp.text;
//using iTextSharp.text.pdf;
//using iTextSharp.tool.xml;
using Microsoft.AspNetCore.Mvc;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System.Transactions;
using System.Text;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static iTextSharp.text.pdf.PdfReader;

namespace BackParroquia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MisaController : ControllerBase
    {
        private readonly IMisaRepository _iMisaRepository;
        private readonly INombresRepository _iNombresRepository;
        public MisaController(IMisaRepository pIMisaRepository, INombresRepository iNombresRepository)
        {
            _iMisaRepository = pIMisaRepository;
            _iNombresRepository = iNombresRepository;
        }
        [HttpGet]
        public async Task<List<Misa>> GetAll()
        {
            var result = await _iMisaRepository.GetAll();
            return result;
        }
        [HttpGet("{pIdMisa:int}")]
        public async Task<Misa> GetById(int pIdMisa)
        {
            var result = await _iMisaRepository.GetById(pIdMisa);
            return result;
        }
        [HttpPost]
        public async Task<IActionResult> Insert([FromBody] MisaDTO pMisa)
        {
            if (pMisa == null)
                return BadRequest();
            bool result; //10.05
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                pMisa.IdMisa = await _iMisaRepository.GetNextId();
                result = await _iMisaRepository.Insert(pMisa);
                if (!(pMisa.ListNombres == null || pMisa.ListNombres.Count == 0))
                {
                    foreach (var item in pMisa.ListNombres)
                    {
                        await _iNombresRepository.Insert(pMisa.IdMisa, item);
                    }
                }
                scope.Complete();
            }
            //return NoContent();
            return StatusCode(StatusCodes.Status200OK,
                new
                {
                    isSuccess = result,
                    message = "buena, ya lo insertaste"
                }); //10.05
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] MisaDTO pMisa)
        {
            if (pMisa == null)
                return BadRequest();

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var result = await _iMisaRepository.Update(pMisa);
                if (!(pMisa.ListNombres == null || pMisa.ListNombres.Count == 0))
                {
                    foreach (var item in pMisa.ListNombres)
                    {
                        if (item.IdName == 0)
                            await _iNombresRepository.Insert(pMisa.IdMisa, item);
                        else
                            await _iNombresRepository.Update(item);
                    }
                }
                scope.Complete();
            }
            return NoContent();
        }
        [HttpPut]
        [Route("delete")]
        public async Task<IActionResult> Delete([FromQuery] int pId)
        {
            if (pId == 0)
                return BadRequest();

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var result = await _iMisaRepository.Delete(pId);
                scope.Complete();
            }
            return NoContent();
        }
        //[HttpGet]
        //[Route("print2")]
        //public async Task<IActionResult> Print(int pId)
        //{
        //    var misa = await _iMisaRepository.GetById(pId);
        //    var nombres = await _iNombresRepository.GetById(pId);
        //    if (misa == null)
        //    {
        //        return NotFound();
        //    }

        //    // Generar el PDF
        //    byte[] pdfBytes = await GenerarPdfAsync(misa, nombres);
        //    // Devolver el archivo PDF
        //    return File(pdfBytes, "application/pdf", $"ticket-{pId}.pdf");
        //}
        //private async Task<byte[]> GenerarPdfAsync(Misa misa, List<Nombres> pNombres)
        //{
        //    using (MemoryStream memoryStream = new MemoryStream())
        //    {
        //        // Definir el tamaño inicial del documento
        //        var pageSize = new Rectangle(212.598f, 200f);

        //        Document document = new Document(pageSize, 10f, 10f, 0f, 0f); // Margen superior ajustado a 0f
        //        PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
        //        document.Open();

        //        // Crear un PdfPTable para el contenido
        //        PdfPTable table = new PdfPTable(1);
        //        //table.TotalWidth = document.PageSize.Width - 20; // Margen de 36 por lado
        //        table.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin; // Ajustar por márgenes
        //        table.HorizontalAlignment = Element.ALIGN_LEFT; // Alineación a la izquierda
        //        table.LockedWidth = true;

        //        // Añadir contenido al PdfPTable
        //        AddCellToTable(table, $"PARROQUIA CRISTO REY", FontFactory.HELVETICA_BOLD, 16);
        //        AddCellToTable(table, $"PUEBLO NUEVO - CHINCHA", FontFactory.HELVETICA_BOLD, 16);
        //        AddCellToTable(table, $"Telf. 056-287876", FontFactory.HELVETICA_BOLD, 13);
        //        AddCellToTable(table, $"Tipo de Misa: {misa.TipoMisa!.Tipo}");
        //        AddCellToTable(table, $"Motivo de Misa: {misa.MotivoMisa!.Motivo}");
        //        AddCellToTable(table, $"Motivo: {misa.Motivo}");
        //        AddCellToTable(table, $"Fecha y Hora de Misa: {misa.FhMisa}");
        //        AddCellToTable(table, $"Donación: {misa.Donacion}");
        //        AddCellToTable(table, $"Misa Personal: {(misa.FlgMisaPersonal ? "Sí" : "No")}");
        //        AddCellToTable(table, $"Observaciones: {misa.Observaciones}");
        //        AddCellToTable(table, "Nombres:");
        //        foreach (var nombre in pNombres)
        //        {
        //            AddCellToTable(table, $"{nombre.Nombre} - {nombre.Celular}");
        //        }

        //        // Calcular la altura total del contenido
        //        float tableHeight = table.TotalHeight;

        //        // Crear un nuevo documento con la altura ajustada
        //        //var adjustedPageSize = new Rectangle(212.598f, tableHeight + 20); // Añadir márgenes
        //        var adjustedPageSize = new Rectangle(212.598f, tableHeight + document.TopMargin + document.BottomMargin); // Ajustar por márgenes

        //        document.SetPageSize(adjustedPageSize);
        //        document.NewPage();

        //        // Añadir la tabla al documento
        //        document.Add(table);
        //        document.Close();

        //        return memoryStream.ToArray();
        //    }
        //}
        //private void AddCellToTable(PdfPTable table, string text, string font = FontFactory.HELVETICA, int size = 10)
        //{
        //    PdfPCell cell = new PdfPCell(new Phrase(text, FontFactory.GetFont(font, size)));
        //    cell.Border = Rectangle.NO_BORDER;
        //    table.AddCell(cell);
        //}

        //[HttpGet]
        //[Route("print")]
        //public async Task<IActionResult> Print2(int pId)
        //{
        //    var misa = await _iMisaRepository.GetById(pId);
        //    var nombres = await _iNombresRepository.GetById(pId);
        //    if (misa == null)
        //    {
        //        return NotFound();
        //    }

        //    // Generar el PDF
        //    byte[] pdfBytes = await GenerarPdfAsync3(misa, nombres);
        //    // Devolver el archivo PDF
        //    return File(pdfBytes, "application/pdf", $"ticket-{pId}.pdf");
        //}
        //private async Task<byte[]> GenerarPdfAsync2(Misa misa, List<Nombres> pNombres)
        //{
        //    // Ruta del archivo HTML
        //    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Ticket2.html");

        //    if (!System.IO.File.Exists(filePath))
        //    {
        //        throw new FileNotFoundException("La plantilla de ticket HTML no fue encontrada.", filePath);
        //    }

        //    // Leer el contenido del archivo HTML
        //    string ticketHtml = await System.IO.File.ReadAllTextAsync(filePath);

        //    // Modificar el contenido del HTML con datos dinámicos
        //    ticketHtml = ticketHtml.Replace("@FECHA_EMISION", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
        //    ticketHtml = ticketHtml.Replace("@CLIENTE", "JOSE EDUARDO HUAMAN DEL AGUILA");
        //    ticketHtml = ticketHtml.Replace("@DNI", "74950975");
        //    ticketHtml = ticketHtml.Replace("@FILAS", GenerarFilasHtml(pNombres));
        //    ticketHtml = ticketHtml.Replace("@TOTAL", misa.Donacion.ToString());
        //    ticketHtml = ticketHtml.Replace("@T_LETRAS", HelperClass.NumeroALetras((decimal)238.50));
        //    // Realiza otras sustituciones necesarias aquí

        //    // Generar el PDF a partir del HTML modificado
        //    return await ConvertHtmlToPdfAsync(ticketHtml);
        //}
        //private async Task<byte[]> ConvertHtmlToPdfAsync2(string htmlContent)
        //{
        //    using (MemoryStream memoryStream = new MemoryStream())
        //    {
        //        // Define el tamaño de la página (75mm de ancho, altura variable)
        //        var pageWidth = 75 / 25.4f * 72; // Convertir mm a puntos
        //        var pageSize = new Rectangle(pageWidth, PageSize.A4.Height); // Usa una altura grande para ajustar

        //        Document document = new Document(pageSize, 5f, 5f, 5f, 5f); // Margen ajustado
        //        PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
        //        document.Open();
        //        document.Add(new Phrase(""));

        //        using (StringReader sr = new StringReader(htmlContent))
        //        {
        //            XMLWorkerHelper.GetInstance().ParseXHtml(writer, document, sr);
        //        }

        //        document.Close();
        //        return memoryStream.ToArray();
        //    }
        //}
        //private async Task<byte[]> ConvertHtmlToPdfAsync3(string htmlContent)
        //{
        //    using (MemoryStream memoryStream = new MemoryStream())
        //    {
        //        // Define el tamaño de la página (75mm de ancho, altura variable)
        //        var pageWidth = 75 / 25.4f * 72; // Convertir mm a puntos
        //        var pageSize = new Rectangle(pageWidth, PageSize.A4.Height); // Usa una altura grande para ajustar

        //        // Primero, crea un documento temporal para calcular la altura del contenido
        //        Document tempDocument = new Document(pageSize, 5f, 5f, 5f, 5f);
        //        PdfWriter tempWriter = PdfWriter.GetInstance(tempDocument, new MemoryStream());
        //        tempDocument.Open();

        //        // Añadir el contenido HTML al documento temporal
        //        using (StringReader sr = new StringReader(htmlContent))
        //        {
        //            XMLWorkerHelper.GetInstance().ParseXHtml(tempWriter, tempDocument, sr);
        //        }

        //        tempDocument.Close();

        //        // Obtener la altura del contenido del documento temporal
        //        float contentHeight = tempWriter.GetVerticalPosition(true);

        //        // Crear el documento final con la altura ajustada
        //        var adjustedPageSize = new Rectangle(pageWidth, contentHeight + 10f); // Ajustar la altura con un margen adicional

        //        Document document = new Document(adjustedPageSize, 5f, 5f, 5f, 5f); // Margen ajustado
        //        PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
        //        document.Open();
        //        document.Add(new Phrase(""));

        //        // Añadir el contenido HTML al documento final
        //        using (StringReader sr = new StringReader(htmlContent))
        //        {
        //            XMLWorkerHelper.GetInstance().ParseXHtml(writer, document, sr);
        //        }

        //        document.Close();
        //        return memoryStream.ToArray();
        //    }
        //}
        //private async Task<byte[]> ConvertHtmlToPdfAsync(string htmlContent)
        //{
        //    using (MemoryStream memoryStream = new MemoryStream())
        //    {
        //        // Define el tamaño de la página (75mm de ancho, altura variable)
        //        var pageWidth = 75 / 25.4f * 72; // Convertir mm a puntos
        //        float pageHeight;
        //        // Primer paso: calcular la altura del contenido
        //        Document document = new Document(new Rectangle(pageWidth, PageSize.A4.Height), 5f, 5f, 5f, 5f);
        //        using (MemoryStream tempStream = new MemoryStream())
        //        {
        //            PdfWriter tempWriter = PdfWriter.GetInstance(document, tempStream);
        //            document.Open();
        //            using (StringReader sr = new StringReader(htmlContent))
        //            {
        //                XMLWorkerHelper.GetInstance().ParseXHtml(tempWriter, document, sr);
        //            }
        //            document.Close();
        //            tempWriter.Close();
        //            tempStream.Flush();

        //            // Calcular la altura necesaria para el contenido
        //            var totalContentHeight = tempWriter.GetVerticalPosition(true);
        //            pageHeight = totalContentHeight + document.BottomMargin + document.TopMargin;
        //            pageHeight = pageHeight < 100 ? 100 : pageHeight; // Asegurarse de que la altura mínima sea 100 puntos
        //        }

        //        // Segundo paso: crear el documento con la altura ajustada
        //        Document adjustedDocument = new Document(new Rectangle(pageWidth, pageHeight), 5f, 5f, 5f, 5f);
        //        PdfWriter writer = PdfWriter.GetInstance(adjustedDocument, memoryStream);
        //        adjustedDocument.Open();
        //        using (StringReader sr = new StringReader(htmlContent))
        //        {
        //            XMLWorkerHelper.GetInstance().ParseXHtml(writer, adjustedDocument, sr);
        //        }
        //        adjustedDocument.Close();

        //        return memoryStream.ToArray();
        //    }
        //}
        //private async Task<byte[]> GenerarPdfAsync3(Misa misa, List<Nombres> pNombres)
        //{
        //    // Crear el documento PDF
        //    Document document = new Document();
        //    using (MemoryStream memoryStream = new MemoryStream())
        //    {
        //        PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
        //        document.Open();

        //        // Agregar contenido al documento
        //        AgregarContenido(document, misa, pNombres);

        //        document.Close();

        //        return memoryStream.ToArray();
        //    }
        //}
        //private void AgregarContenido(Document document, Misa misa, List<Nombres> nombres)
        //{
        //    // Crear tabla para el contenido
        //    PdfPTable table = new PdfPTable(4); // 4 columnas

        //    // Ajustar el ancho de las columnas
        //    float[] columnWidths = { 12f, 50f, 19f, 19f };
        //    table.SetWidths(columnWidths);

        //    // Agregar encabezados
        //    table.AddCell(new PdfPCell(new Phrase("Cant")));
        //    table.AddCell(new PdfPCell(new Phrase("Descripción")));
        //    table.AddCell(new PdfPCell(new Phrase("P. Unit")));
        //    table.AddCell(new PdfPCell(new Phrase("Importe")));

        //    // Agregar filas de nombres
        //    foreach (var nombre in nombres)
        //    {
        //        table.AddCell(new PdfPCell(new Phrase("2"))); // Ejemplo estático, puedes cambiarlo
        //        table.AddCell(new PdfPCell(new Phrase("COMPRA"))); // Ejemplo estático, puedes cambiarlo
        //        table.AddCell(new PdfPCell(new Phrase(10.ToString("F2")))); // Precio unitario
        //        table.AddCell(new PdfPCell(new Phrase(20.ToString()))); // Importe
        //    }

        //    // Agregar subtotal
        //    PdfPCell subtotalCell = new PdfPCell(new Phrase("Subtotal:"));
        //    subtotalCell.Colspan = 3;
        //    subtotalCell.HorizontalAlignment = Element.ALIGN_RIGHT;
        //    table.AddCell(subtotalCell);

        //    table.AddCell(new PdfPCell(new Phrase(misa.Donacion.ToString()))); // Subtotal

        //    // Agregar total
        //    PdfPCell totalLabelCell = new PdfPCell(new Phrase("Total:"));
        //    totalLabelCell.Colspan = 3;
        //    totalLabelCell.HorizontalAlignment = Element.ALIGN_RIGHT;
        //    table.AddCell(totalLabelCell);

        //    table.AddCell(new PdfPCell(new Phrase(misa.Donacion.ToString()))); // Total

        //    // Agregar tabla al documento
        //    document.Add(table);

        //    // Agregar texto de Son:
        //    document.Add(new Paragraph($"Son: {HelperClass.NumeroALetras((decimal)238.50)}"));

        //    // Agregar otros elementos si es necesario

        //    // Por ejemplo:
        //    // document.Add(new Paragraph("Otro contenido aquí"));
        //}
        //// Método para generar las filas HTML para los detalles de la misa
        //private string GenerarFilasHtml(List<Nombres> nombres)
        //{
        //    string filas = string.Empty;
        //    foreach (var nombre in nombres)
        //    {
        //        //filas += $"<tr><td>{nombre.Cantidad}</td><td>{nombre.Descripcion}</td><td>{nombre.PrecioUnitario:F2}</td><td>{nombre.Importe:F2}</td></tr>";COMPRA Y VENTA DE ARTICULOS DISPONIBLES EN LA PARROQUIA CRISTO REY
        //        filas += $"<tr><td class=\"right-align\">2</td><td>COMPRA </td><td class=\"right-align\">{10:F2}</td><td class=\"right-align\">{20:F2}</td></tr>";
        //    }
        //    return filas;
        //}

        [HttpGet]
        [Route("print3")]
        public async Task<IActionResult> Print3(int pId)
        {
            var misa = await _iMisaRepository.GetById(pId);
            var nombres = await _iNombresRepository.GetById(pId);

            // Crear un nuevo documento PDF
            PdfDocument document = new PdfDocument();

            // Establecer las unidades a milímetros
            //document.Unit = PdfUnit.Millimeter;

            // Crear una nueva página con el ancho deseado
            PdfPage page = document.AddPage();
            page.Width = 75f; // Ancho de 75mm

            // Usar XGraphics para dibujar en la página
            XGraphics gfx = XGraphics.FromPdfPage(page);

            // Definir fuentes para el texto
            XFont fontHeader = new XFont("Verdana", 10, XFontStyleEx.Bold);
            XFont fontBody = new XFont("Verdana", 8);
            XFont fontFooter = new XFont("Verdana", 8, XFontStyleEx.Italic);

            // Dibujar la cabecera
            gfx.DrawString("Boleta de Venta", fontHeader, XBrushes.Black,
                new XRect(0, 0, page.Width, page.Height), XStringFormats.TopCenter);

            // Dibujar el cuerpo con los detalles de la compra
            // Suponiendo que tienes una lista de detalles en 'misDetalles'
            int yPoint = 20; // Iniciar después de la cabecera
            foreach (var detalle in nombres)
            {
                gfx.DrawString($"{misa.TipoMisa.Tipo} - {misa.Donacion}", fontBody, XBrushes.Black,
                    new XRect(0, yPoint, page.Width, page.Height), XStringFormats.TopLeft);
                yPoint += 10; // Aumentar el punto Y para el siguiente detalle
            }

            // Dibujar el pie de página
            gfx.DrawString("Gracias por su compra", fontFooter, XBrushes.Black,
                new XRect(0, yPoint + 10, page.Width, page.Height), XStringFormats.TopCenter);

            // Guardar el documento PDF en un MemoryStream
            MemoryStream stream = new MemoryStream();
            document.Save(stream, false);

            // Devolver el archivo PDF como un archivo descargable
            return File(stream.ToArray(), "application/pdf", $"ticket-{pId}.pdf");
        }

        [HttpGet]
        [Route("print4")]
        public async Task<IActionResult> Print4(int pId)
        {
            var misa = await _iMisaRepository.GetById(pId);
            var nombres = await _iNombresRepository.GetById(pId);
            if (misa == null)
            {
                return NotFound();
            }

            // Crear un nuevo documento PDF con tamaño de hoja personalizado
            PdfDocument document = new PdfDocument();
            PdfPage page = document.AddPage();
            page.Width = XUnit.FromMillimeter(75);
            page.Height = XUnit.FromMillimeter(150);

            // Crear una fuente para el texto
            XFont font = new XFont("Arial", 10);
            // Crear un objeto XGraphics para dibujar en la página
            XGraphics gfx = XGraphics.FromPdfPage(page);

            // Inicializar la posición vertical
            double yPos = 10;
            // Dibujar el encabezado
            DrawHeader(gfx, font, misa, page, ref yPos);
            // Agregar un espacio después del encabezado
            yPos += 8;
            // Dibujar el cuerpo (tabla de detalles de la venta)
            DrawBody(gfx, font, misa, nombres, ref yPos);
            DrawFooter(gfx, font, nombres, ref yPos, page);



            // Cabeza de la boleta de venta
            //DrawHeader(gfx, font, misa, page);
            //// Cuerpo de la boleta de venta
            //DrawBody(gfx, font, misa, nombres);
            // Guardar el PDF en memoria
            MemoryStream stream = new MemoryStream();
            document.Save(stream, true);
            // Devolver el PDF como respuesta
            return File(stream.ToArray(), "application/pdf", "boleta_de_venta.pdf");
        }

        // Método para dibujar la cabeza de la boleta de venta
        //private void DrawHeader(XGraphics gfx, XFont font, Misa misa, PdfPage pag)
        private void DrawHeader(XGraphics gfx, XFont font, Misa misa, PdfPage pag, ref double yPos)
        {
            //int maxWidth = 75; // Ancho máximo de la página en mm
            XStringFormat xFormatCenter = new XStringFormat
            {
                Alignment = XStringAlignment.Center,
                LineAlignment = XLineAlignment.Near
            };
            XStringFormat xFormatLeft = new XStringFormat
            {
                Alignment = XStringAlignment.Near,
                LineAlignment = XLineAlignment.Near
            };
            XStringFormat xFormatRight = new XStringFormat
            {
                Alignment = XStringAlignment.Far,
                LineAlignment = XLineAlignment.Near
            };
            //gfx.DrawString("PARROQUIA CRISTO REY", font, XBrushes.Black, new XPoint(10, 10), xFormatCenter);
            //gfx.DrawString("Av. Oscar Benavides 500", font, XBrushes.Black, new XPoint(10, 20), xFormatCenter);
            //gfx.DrawString("PUEBLO NUEVO - CHINCHA - ICA", font, XBrushes.Black, new XPoint(10, 30), xFormatCenter);
            //gfx.DrawString("Telf: 056-287876", font, XBrushes.Black, new XPoint(10, 40), xFormatCenter);
            //gfx.DrawString("BOLETA ELECTRONICA", font, XBrushes.Black, new XPoint(10, 50), xFormatCenter);
            //gfx.DrawString("B001-0000000001", font, XBrushes.Black, new XPoint(10, 60), xFormatCenter);
            //// Dibujar la información del cliente
            //gfx.DrawString($"Fecha Emision: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}", font, XBrushes.Black, new XPoint(10, 80), xFormatLeft);
            //gfx.DrawString($"Nombre: JOSE EDUARDO HUAMAN DEL AGUILA", font, XBrushes.Black, new XPoint(10, 90), xFormatLeft);
            //gfx.DrawString($"DNI: 74950975", font, XBrushes.Black, new XPoint(10, 100), xFormatLeft);

            XUnit maxWidth = pag.Width; // Ancho máximo de la página en mm
            XStringFormat format = new XStringFormat();
            format.Alignment = XStringAlignment.Center;

            //// Ajustar tamaño de fuente si es necesario
            //float fontSize = 10;
            //XSize size;
            //do
            //{
            //    font = new XFont("Arial", fontSize);
            //    size = gfx.MeasureString("PARROQUIA CRISTO REY", font); // Medir el texto más largo en el encabezado
            //    fontSize -= 0.5f;
            //} while (size.Width > maxWidth);

            // Dibujar texto
            //DrawWrappedText(gfx, "PARROQUIA CRISTO REY", font, XBrushes.Black, new XRect(0, 10, maxWidth, 20), format);
            //DrawWrappedText(gfx, "Av. Oscar Benavides 500", font, XBrushes.Black, new XRect(0, 20, maxWidth, 30), format);
            //DrawWrappedText(gfx, "PUEBLO NUEVO - CHINCHA - ICA", font, XBrushes.Black, new XRect(0, 30, maxWidth, 40), format);
            //DrawWrappedText(gfx, "Telf: 056-287876", font, XBrushes.Black, new XRect(0, 40, maxWidth, 50), format);
            //DrawWrappedText(gfx, "BOLETA ELECTRONICA", font, XBrushes.Black, new XRect(0, 50, maxWidth, 60), format);
            //DrawWrappedText(gfx, "B001-0000000001", font, XBrushes.Black, new XRect(0, 60, maxWidth, 70), format);

            //// Dibujar la información del cliente
            //DrawWrappedText(gfx, $"Fecha Emision: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}", font, XBrushes.Black, new XRect(0, 80, maxWidth, 90), xFormatLeft);
            //DrawWrappedText(gfx, $"Nombre: JOSE EDUARDO HUAMAN DEL AGUILA", font, XBrushes.Black, new XRect(0, 90, maxWidth, 100), xFormatLeft);
            //DrawWrappedText(gfx, $"DNI: 74950975", font, XBrushes.Black, new XRect(0, 100, maxWidth, 110), xFormatLeft);

            //double yPos = 10; // Posición vertical inicial
            double leftMargin = 10;
            double rightMargin = 10;
            // Dibujar texto
            DrawWrappedText(gfx, "PARROQUIA CRISTO REY", font, XBrushes.Black, ref yPos, maxWidth, format, 0, 0);
            DrawWrappedText(gfx, "Av. Oscar Benavides 500", font, XBrushes.Black, ref yPos, maxWidth, format, 0, 0);
            DrawWrappedText(gfx, "PUEBLO NUEVO - CHINCHA - ICA", font, XBrushes.Black, ref yPos, maxWidth, format, 0, 0);
            DrawWrappedText(gfx, "Telf: 056-287876", font, XBrushes.Black, ref yPos, maxWidth, format, 0, 0);
            DrawWrappedText(gfx, "BOLETA ELECTRONICA", font, XBrushes.Black, ref yPos, maxWidth, format, 0, 0);
            DrawWrappedText(gfx, "B001-0000000001", font, XBrushes.Black, ref yPos, maxWidth, format, 0, 0);
            // Dibujar la información del cliente
            DrawWrappedText(gfx, $"Fecha de Emision: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}", font, XBrushes.Black, ref yPos, maxWidth, xFormatLeft, leftMargin, rightMargin);
            DrawWrappedText(gfx, $"Nombre: MIGUEL EDUARDO MARTINEZ MARTINEZ", font, XBrushes.Black, ref yPos, maxWidth, xFormatLeft, leftMargin, rightMargin);
            DrawWrappedText(gfx, $"DNI: 54121212", font, XBrushes.Black, ref yPos, maxWidth, xFormatLeft, leftMargin, rightMargin);
            gfx.DrawLine(XPens.Black, leftMargin, yPos + 3, maxWidth - leftMargin, yPos + 3);
        }

        // Método para dibujar texto envuelto en varias líneas si excede el ancho máximo
        private void DrawWrappedText(XGraphics gfx, string text, XFont font, XBrush brush, ref double yPos, double maxWidth, XStringFormat format, double leftMargin, double rightMargin)
        {
            // Dividir el texto en palabras
            string[] words = text.Split(' ');

            // Construir líneas de texto
            StringBuilder sb = new StringBuilder();
            List<string> lines = new List<string>();
            double availableWidth = maxWidth - leftMargin - rightMargin;
            foreach (string word in words)
            {
                if (gfx.MeasureString(sb.ToString() + " " + word, font).Width > availableWidth)
                {
                    lines.Add(sb.ToString());
                    sb.Clear();
                }
                sb.Append((sb.Length == 0 ? "" : " ") + word);
            }
            lines.Add(sb.ToString());

            // Dibujar líneas de texto
            foreach (string line in lines)
            {
                if (text.Contains("Fecha de Emision:")) yPos += 6;
                gfx.DrawString(line, font, brush, new XRect(leftMargin, yPos, availableWidth, gfx.MeasureString(line, font).Height), format);
                yPos += gfx.MeasureString(line, font).Height;
            }
        }
        // Método para dibujar el cuerpo de la boleta de venta
        //private void DrawBody(XGraphics gfx, XFont font, Misa misa, List<Nombres> nombres)
        //{
        //    // Dibujar la tabla de detalles de la compra
        //    int yPos = 120;
        //    foreach (var detalle in nombres)
        //    {
        //        gfx.DrawString($"CANT ", font, XBrushes.Black, new XPoint(10, yPos));
        //        gfx.DrawString($"{misa.TipoMisa.Tipo} x {misa.Donacion}", font, XBrushes.Black, new XPoint(10, yPos));
        //        yPos += 15;
        //    }
        //}
        // Método para dibujar el cuerpo de la boleta de venta
        private void DrawBody(XGraphics gfx, XFont font, Misa misa, List<Nombres> nombres, ref double yPos)
        {
            double maxWidth = 75; // Ancho máximo de la página en mm
            double leftMargin = 10; // Margen izquierdo de la tabla
            double rightMargin = 10; // Margen derecho de la tabla
            double availableWidth = maxWidth - leftMargin - rightMargin;
            double columnGap = 2; // Espacio entre columnas

            // Encabezados de la tabla
            string[] headers = { "Cant", "Descripcion", "P. Unit", "Importe" };
            double[] columnWidths = { 25, 90, 35, 35 }; // Ancho de cada columna en mm

            // Dibujar encabezados
            double currentX = leftMargin;
            foreach (var header in headers)
            {
                gfx.DrawString(header, font, XBrushes.Black, new XRect(currentX, yPos, columnWidths[Array.IndexOf(headers, header)], gfx.MeasureString(header, font).Height), XStringFormats.TopLeft);
                currentX += columnWidths[Array.IndexOf(headers, header)] + columnGap;
            }
            yPos += gfx.MeasureString(headers[0], font).Height + 5; // Ajustar la posición vertical para las filas de datos

            // Dibujar filas de datos
            foreach (var nombre in nombres)
            {
                currentX = leftMargin;
                gfx.DrawString("2", font, XBrushes.Black, new XRect(currentX, yPos, columnWidths[0], gfx.MeasureString("2", font).Height), XStringFormats.Center);
                currentX += columnWidths[0] + columnGap;

                gfx.DrawString("nombre.Descripcion", font, XBrushes.Black, new XRect(currentX, yPos, columnWidths[1], gfx.MeasureString("nombre.Descripcion", font).Height), XStringFormats.TopLeft);
                currentX += columnWidths[1] + columnGap;

                gfx.DrawString(10.ToString("F2"), font, XBrushes.Black, new XRect(currentX, yPos, columnWidths[2], gfx.MeasureString(10.ToString("F2"), font).Height), XStringFormats.TopRight);
                currentX += columnWidths[2] + columnGap;

                gfx.DrawString(20.ToString("F2"), font, XBrushes.Black, new XRect(currentX, yPos, columnWidths[3], gfx.MeasureString(20.ToString("F2"), font).Height), XStringFormats.TopRight);
                currentX += columnWidths[3] + columnGap;

                yPos += gfx.MeasureString("2", font).Height + 1; // Ajustar la posición vertical para la siguiente fila

            }
        }

        private void DrawFooter(XGraphics gfx, XFont font, List<Nombres> nombres, ref double yPos, PdfPage pag)
        {
            double leftMargin = 10;
            double rightMargin = 10;
            XUnit maxWidth = pag.Width;
            // Dibujar una línea de separación
            gfx.DrawLine(XPens.Black, leftMargin, yPos, maxWidth - leftMargin, yPos);
            yPos += 3;

            // Calcular subtotal y total
            //double subtotal = nombres.Sum(n => n.Importe);
            double subtotal = 100;
            double total = subtotal + 29247.50; // Aquí podrías agregar impuestos o descuentos si es necesario

            // Dibujar subtotal y total
            gfx.DrawString("Subtotal:", font, XBrushes.Black, new XRect(120, yPos, 30, gfx.MeasureString("Subtotal:", font).Height), XStringFormats.TopRight);
            gfx.DrawString(subtotal.ToString("F2"), font, XBrushes.Black, new XRect(166, yPos, 35, gfx.MeasureString(subtotal.ToString("F2"), font).Height), XStringFormats.TopRight);
            yPos += gfx.MeasureString("Subtotal:", font).Height + 1;

            gfx.DrawString("Total:", font, XBrushes.Black, new XRect(120, yPos, 30, gfx.MeasureString("Total:", font).Height), XStringFormats.TopRight);
            gfx.DrawString(total.ToString("F2"), font, XBrushes.Black, new XRect(166, yPos, 35, gfx.MeasureString(total.ToString("F2"), font).Height), XStringFormats.TopRight);
            yPos += gfx.MeasureString("Total:", font).Height + 10;

            // Convertir el total a letras (por simplicidad, utilizaremos una función ficticia)
            string totalEnLetras = HelperClass.NumeroALetras((decimal)total); // Debes implementar esta función según tus necesidades

            // Dibujar el total en letras
            //gfx.DrawString($"Total en letras: {totalEnLetras}", font, XBrushes.Black, new XRect(10, yPos, 55, gfx.MeasureString($"Total en letras: {totalEnLetras}", font).Height), XStringFormats.TopLeft);
            //yPos += gfx.MeasureString($"Total en letras: {totalEnLetras}", font).Height + 5;
            // Dibujar el total en letras
            XStringFormat format = new XStringFormat();
            format.Alignment = XStringAlignment.Near;
            DrawWrappedText(gfx, $"Son: {totalEnLetras}", font, XBrushes.Black, ref yPos, maxWidth, format, leftMargin, rightMargin);
            gfx.DrawLine(XPens.Black, leftMargin, yPos + 3, maxWidth - leftMargin, yPos + 3);
            string strText = "Se deja constancia que el presente documento no tiene efectos legales dentro del sistema jurídico nacional, por corresponder a una relación de naturaleza interna de la Iglesia Católica, a través de la Parroquia Cristo Rey y sus fieles y parroquianos, bajo los principios de independencia y libertad de la que goza la Iglesia Católica en el Perú. Según se ha convenido en el acuerdo internacional suscrito entre la Santa Sede y la República del Perú, el 19 de julio de 1980.";
            yPos += 6;
            DrawWrappedText(gfx, $"{strText}", font, XBrushes.Black, ref yPos, maxWidth, format, leftMargin, rightMargin);
        }
    }
}
