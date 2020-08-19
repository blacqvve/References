using System;
using System.Collections.Generic;
using System.Text;

namespace CastonFactory.Data.Enums
{
    public enum ActionReturn
     {
          NotFound,
          ServerError,
          TypeMismatch,
          AccesDenied,
          NotReady,
          Ok
     }
     public static class ActionReturnExtensions
     {
          public static string GetActionMessage(this ActionReturn error)
          {
               switch (error)
               {
                    case ActionReturn.NotFound:
                         return "Aranan veri bulunamadı.";
                    case ActionReturn.ServerError:
                         return "Sunucu hatası.";
                    case ActionReturn.TypeMismatch:
                         return "Gönderilen veri, talep edilen veri ile uyuşmuyor";
                    case ActionReturn.AccesDenied:
                         return "Erişim izni yok.";
                    case ActionReturn.Ok:
                         return "İşlem Başarılı";
                    case ActionReturn.NotReady:
                         return "İçerik işleminiz için hazır değil.";
                    default:
                         return "";
               }
          }
     }
}
