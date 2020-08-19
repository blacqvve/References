using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MagicBox.Data.Helpers
{
  public class Enums
  {
    public enum UserAPIBadRequest
    {
      EMPTY_MAIL,
      WRONG_MAIL_OR_PW,
      LOCKOUT,
      UNABLE_TO_LOGIN,
      UNABLE_TO_FIND_PLAYER,
      EMPTY_FIELD,
      UNKNOWN_ERROR,
      SESSION_DROPPED
    };

    public enum TransactionStatus
    {
      INSUFFICIENT_FUNDS,
      OK,
      UNKNOWN_ERROR,
      UNABLE_TO_FIND_ITEM
    }
  }
}
