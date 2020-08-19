using System;
using System.Collections.Generic;
using System.Text;

namespace CastonFactory.Data.Enums
{
     public enum SupportState
     {
          Open,
          Resolving,
          Closed
     }

     public static class SupportStateExtensions
     {
          public static string  GetStateText(this SupportState state)
          {
               switch (state)
               {
                    case SupportState.Open:
                         return "Açık";
                    case SupportState.Resolving:
                         return "İşlemde" ;
                    case SupportState.Closed:
                         return "Kapandı";
                    default:
                         return "";
               }
          }
     }
}
