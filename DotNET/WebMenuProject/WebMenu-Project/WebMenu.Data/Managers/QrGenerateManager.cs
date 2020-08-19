using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMenu.Data.Data;
using WebMenu.Data.Misc.Enums;
using WebMenu.Data.Misc.Structs;
using WebMenu.Data.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.AspNetCore.Http;

namespace WebMenu.Data.Managers
{
     public interface IQrGenerateManager
     {
          public Task<ReturnObject<ErrorReturns, Picture>> GenerateQrAsync(string qrLink,IWebHostEnvironment _env, HttpRequest req);
     }
     public class QrGenerateManager : IQrGenerateManager
     {
          public async Task<ReturnObject<ErrorReturns, Picture>> GenerateQrAsync(string qrLink, IWebHostEnvironment _env, HttpRequest req)
          {

               if (_env==null)
                    return new ReturnObject<ErrorReturns, Picture>(ErrorReturns.NotFound, null, null);
               var picture = new Picture();
               picture.CreateDate = DateTime.Now;
               picture.PictureId = Guid.NewGuid();
               string dy = "Https://"+req.Host.Value + "/qrImages/";
               string dr = _env.WebRootPath + "/qrImages/";
               if (!Directory.Exists(dr))
               {
                    Directory.CreateDirectory(dr);
               }
               string url= Path.Combine(dy,picture.PictureId.ToString() + ".png");
               string path = Path.Combine(dr, picture.PictureId.ToString()+".png");
               using (MemoryStream ms = new MemoryStream())
               {

                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrLink, QRCodeGenerator.ECCLevel.H);
                    QRCode qrCode = new QRCode(qrCodeData);

                    try
                    {
                         using (Bitmap qrCodeImage = qrCode.GetGraphic(20, Color.Black, Color.White, (Bitmap)Bitmap.FromFile(_env.WebRootFileProvider.GetFileInfo("qrLogo/vanilla_logo.png")?.PhysicalPath)))
                         {
                              qrCodeImage.Save(ms, ImageFormat.Png);
                              string ImageStr = Convert.ToBase64String(ms.ToArray());
                              using (MemoryStream ms2 = new MemoryStream(Convert.FromBase64String(ImageStr)))
                              {
                                   using (Bitmap bm2 = new Bitmap(ms2))
                                   {
                                        bm2.Save(path);
                                   }
                              }
                         }
                    }
                    catch (Exception e)
                    {

                         Console.WriteLine(e.Message);
                         return new ReturnObject<ErrorReturns, Picture>(ErrorReturns.ServerError, null, e);
                    }

               }
               picture.URL = url;

               return  new ReturnObject<ErrorReturns, Picture>(ErrorReturns.Ok, picture,null);
          }
     }
}
