using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Powells.CouponCode;
using static MagicBox.Data.Helpers.Enums;

namespace MagicBox.Data.Helpers
{
  public static class Utils
  {
    public static byte[] GetImageAsByte(string path)
    {
      try
      {
        byte[] imgdata = System.IO.File.ReadAllBytes(path);
        return imgdata;
      }
      catch (Exception)
      {
        return new byte[] { };
      }
    }

    public static string GetImageSourceFromByteArray(byte[] array)
    {
      try
      {
        var base64 = Convert.ToBase64String(array);
        return String.Format("data:image/gif;base64,{0}", base64);
      }
      catch (Exception)
      {
        return "";
      }
    }

    public static class UserAPI
    {
      public static string GenerateCouponCode(int digits = 4)
      {
        var opts = new Powells.CouponCode.Options();
        opts.PartLength = digits;
        opts.Parts = 1;
        var ccb = new CouponCodeBuilder();
        var code = ccb.Generate(opts);
        return code;
      }
      public static string GetErrorStringFormatted(UserAPIBadRequest errorCode) //add localisation in future
      {
        switch (errorCode)
        {
          case UserAPIBadRequest.EMPTY_MAIL:
            return "Email veya şifre alanları boş bırakılamaz";
          case UserAPIBadRequest.WRONG_MAIL_OR_PW:
            return "Yanlış email veya şifre girildi.";
          case UserAPIBadRequest.LOCKOUT:
            return "Çok fazla yanlış denemeden dolayı hesap kilitlenmiştir. Lütfen biraz bekleyip tekrar deneyin.";
          case UserAPIBadRequest.UNABLE_TO_LOGIN:
            return "Bilinmeyen bir hatadan dolayı sisteme giriş yapılamıyor.";
          case UserAPIBadRequest.UNABLE_TO_FIND_PLAYER:
            return "Talep edilen kullanıcı bilgileri veritabanında bulunamadı.";
          case UserAPIBadRequest.EMPTY_FIELD:
            return "Boş alan bilgisi tespit edildi ('{0}')";
          case UserAPIBadRequest.UNKNOWN_ERROR:
            return "Beklenmedik bir hata oluştu";
          case UserAPIBadRequest.SESSION_DROPPED:
            return "Oturumunuzun süresi bitmiş görünüyor. Lütfen hesabınıza yeniden giriş yapınız.";
          default:
            return "Beklenmedik bir hata oluştu";
        }
      }

      public static string GetErrorStringFormatted(TransactionStatus errorCode) //add localisation in future
      {
        switch (errorCode)
        {
          case TransactionStatus.INSUFFICIENT_FUNDS:
            return "Hesabınızda yeterli puan bulunmadığı için işlem tamamlanamadı.";
          case TransactionStatus.UNABLE_TO_FIND_ITEM:
            return "Gönderilen bilgilere ait bir item bulunamadı.";
          default:
            return "Beklenmedik bir hata oluştu.";
        }
      }
    }
  }
}
