using Spire.Pdf;
using Spire.Pdf.HtmlConverter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace DealerManagementSystem.Utils
{
    public static class PdfGenerator
    {
        public static MemoryStream Generate(XElement element, string xsltFileName, PdfPageSettings settings)
        {           
            try
            {
                MemoryStream stream = new MemoryStream();

                XslCompiledTransform _transform = new XslCompiledTransform(false);
                _transform.Load(Path.Combine(HostingEnvironment.ApplicationPhysicalPath, @"App_Data\", string.Format("{0}.xslt", xsltFileName)));

                using (StringWriter sw = new StringWriter())
                {
                    _transform.Transform(new XDocument(element).CreateReader(), new XsltArgumentList(), sw);

                    using (PdfDocument pdf = new PdfDocument())
                    {
                        PdfHtmlLayoutFormat htmlLayoutFormat = new PdfHtmlLayoutFormat() { IsWaiting = false };                        

                        Thread thread = new Thread(() => pdf.LoadFromHTML(sw.ToString(), false, settings, htmlLayoutFormat));
                        thread.SetApartmentState(ApartmentState.STA);
                        thread.Start();
                        thread.Join();

                        pdf.SaveToStream(stream);
                    }
                }

                return stream;
            }
            catch
            {
                return null;
            }
        }
    }
}