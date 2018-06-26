using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DealerManagementSystem.Utils;
using DealerManagementSystem.Models;
using Newtonsoft.Json;
using OfficeOpenXml;
using System.Net.Mime;
using Spire.Pdf;
using System.Security.Cryptography.Pkcs;
using OfficeOpenXml.Style;

namespace DealerManagementSystem.Controllers
{
    [ValidateUserFilter]    
    public class HomeController : BaseController
    {
        BusinessLogic _bl = new BusinessLogic();
        
        public ActionResult Index()
        {
            ViewBag.currentLanguage = _bl.GetLanguageCookieValue();

            return View();
        }
        
        public ActionResult Catalog()
        {
            ViewBag.gropProducts = _bl.GetGroupProducts();
            ViewBag.currentLanguage = _bl.GetLanguageCookieValue();

            return View();
        }

        public ActionResult SearchCatalog(string search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                ViewBag.searchPhrase = search;
            }

            ViewBag.currentLanguage = _bl.GetLanguageCookieValue();

            return View();
        }
        
        public ActionResult ShoppingCart()
        {
            ViewBag.currentLanguage = _bl.GetLanguageCookieValue();

            return View();
        }

        public ActionResult ProductDetails(int id)
        {
            var product = _bl.GetProductDetails(id);

            if(product == null)
            {
                return null;
            }
            else
            {
                ViewBag.product = product;
                ViewBag.currentLanguage = _bl.GetLanguageCookieValue();

                return View();
            }
        }
        
        public ActionResult Orders()
        {
            ViewBag.currentLanguage = _bl.GetLanguageCookieValue();

            return View();
        }
        
        public ActionResult ProfileInfo()
        {
            ViewBag.contragent = _bl.GetCurrentContragent();
            ViewBag.currentLanguage = _bl.GetLanguageCookieValue();

            return View();
        }
        
        public ActionResult History()
        {
            ViewBag.currentLanguage = _bl.GetLanguageCookieValue();

            return View();
        }



        public JsonResult GetProductsByGroupId(int groupId)
        {
            return Json(_bl.GetProductsByGroupId(groupId));
        }

        public JsonResult AddToShoppingCart(ShoppingCart shoppingCart)
        {
            return Json(_bl.AddToShoppingCart(shoppingCart));
        }

        public JsonResult GetContragentShoppingCartProducts()
        {
            List<Product> products = _bl.GetContragentShoppingCartProducts();

            return Json(new { products = products, subTotalSum = products == null ? 0 : products.Sum(p => p.TotalSum) });
        }

        public JsonResult UpdateShoppingCart(ShoppingCart shoppingCart)
        {
            return Json(_bl.UpdateShoppingCart(shoppingCart));
        }

        public JsonResult RemoveShoppingCart(int productId)
        {
            return Json(_bl.RemoveShoppingCart(productId));
        }

        public JsonResult GetContragentAddresses()
        {
            return Json(_bl.GetContragentAddresses());
        }

        public JsonResult PlaceOrder(int contragentAddressId)
        {
            return Json(_bl.PlaceOrder(contragentAddressId));
        }
        
        public JsonResult SearchProducts(string searchPhrase)
        {
            return Json(_bl.SearchProducts(searchPhrase));
        }

        public JsonResult GetFinaOrders()
        {
            return Json(_bl.GetFinaOrders());
        }

        public JsonResult ChangePassword(string oldPwd, string newPwd)
        {
            return Json(_bl.ChangePassword(oldPwd, newPwd));
        }

        public JsonResult GetHistory(string dateFrom, string dateTo, int currencyId)
        {           
            return Json(_bl.GetHistory(dateFrom, dateTo, currencyId));
        }

        public JsonResult GetCurrencies()
        {
            return Json(_bl.GetCurrencies());
        }

        public JsonResult GetContragentCurrency()
        {
            return Json(_bl.GetContragentCurrency());
        }

        public FileResult GenerateHistoryExcel(string dateFrom, string dateTo, int currencyId)
        {
            using (var package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(Resources.Translator.History);

                //logic here
                KeyValuePair<double?, List<History>> historyResult = _bl.GetHistory(dateFrom, dateTo, currencyId);
                List<History> historyItems = historyResult.Value;

                worksheet.Cells[1, 1].Value = Resources.Translator.Date;
                worksheet.Cells[1, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                worksheet.Cells[1, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                worksheet.Cells[1, 2].Value = Resources.Translator.Number;
                worksheet.Cells[1, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                worksheet.Cells[1, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                worksheet.Cells[1, 3].Value = Resources.Translator.AmountIn;
                worksheet.Cells[1, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, 3].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                worksheet.Cells[1, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                worksheet.Cells[1, 4].Value = Resources.Translator.AmountOut;
                worksheet.Cells[1, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, 4].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                worksheet.Cells[1, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                worksheet.Cells[1, 5].Value = Resources.Translator.Rest;
                worksheet.Cells[1, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, 5].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                worksheet.Cells[1, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                for (int i = 2, j = 0; i <= historyItems.Count + 1 && j < historyItems.Count; i++, j++)
                {
                    worksheet.Cells[i, 1].Value = historyItems[j].Tdate.ToString("yyyy-MM-dd");
                    worksheet.Cells[i, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                    worksheet.Cells[i, 2].Value = historyItems[j].WaybillNum == null ? historyItems[j].DocNum.ToString() : historyItems[j].WaybillNum;
                    worksheet.Cells[i, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                    worksheet.Cells[i, 3].Value = Math.Round(historyItems[j].AmountIn, 2);
                    worksheet.Cells[i, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                    worksheet.Cells[i, 4].Value = Math.Round(historyItems[j].AmountOut, 2);
                    worksheet.Cells[i, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                    worksheet.Cells[i, 5].Value = historyItems[j].Rest;
                    worksheet.Cells[i, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                }

                worksheet.Column(1).AutoFit();
                worksheet.Column(2).AutoFit();
                worksheet.Column(3).AutoFit();
                worksheet.Column(4).AutoFit();
                worksheet.Column(5).AutoFit();
                //

                return File(package.GetAsByteArray(), MTManager.GetMimeType(".xlsx"), string.Format("{0} {1}.xlsx", Resources.Translator.History, DateTime.Now.ToString("yyyy-MM-dd HH:mm.ss")));
            }            
        }

        public FileResult GenerateInvoicePdf(int generalId)
        {            
            Response.AppendHeader("Content-Disposition", "inline; filename=" + string.Format("{0} {1}.pdf", Resources.Translator.Invoice, DateTime.Now.ToString("yyyy-MM-dd HH:mm.ss")));
            return File(PdfGenerator.Generate(_bl.GetFinaOrderInvoiceXml(generalId), "Invoice", new PdfPageSettings { Orientation = PdfPageOrientation.Portrait }).ToArray(), MTManager.GetMimeType(".pdf"));
        }

        public FileResult GenerateWaybillPdf(int generalId)
        {
            Response.AppendHeader("Content-Disposition", "inline; filename=" + string.Format("{0} {1}.pdf", Resources.Translator.Waybill, DateTime.Now.ToString("yyyy-MM-dd HH:mm.ss")));
            return File(PdfGenerator.Generate(_bl.GetHistoryWaybillXml(generalId), "Waybill", new PdfPageSettings { Orientation = PdfPageOrientation.Portrait }).ToArray(), MTManager.GetMimeType(".pdf"));
        }
    }
}