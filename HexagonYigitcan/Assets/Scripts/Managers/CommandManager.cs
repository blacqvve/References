using System.Collections;
using System.Collections.Generic;
using Toolbox;
using UnityEngine;

public class CommandManager : Singleton<CommandManager>
{

     List<ICommand> rotateCommands = new List<ICommand>();


     public void AddCommand(ICommand cmd)
     {
          rotateCommands.Add(cmd);
     }

     public void UndueLastCommand()
     {
          rotateCommands[rotateCommands.Count-1].Undue();
     }
}
