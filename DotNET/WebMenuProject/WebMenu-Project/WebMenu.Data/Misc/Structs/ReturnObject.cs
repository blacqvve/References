
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMenu.Data.Misc.Structs
{
     public struct ReturnObject<TReturn,TObject> where TReturn : Enum 
     {
          public  TReturn Return;
          public  TObject Object;
          public Exception Exception;

          public ReturnObject(TReturn _return,TObject _object,Exception exception)
          {
               Return = _return;
               Object = _object;
               Exception = exception;
          }
     }
}
