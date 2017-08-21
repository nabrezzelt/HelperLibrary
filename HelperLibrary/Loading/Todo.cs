using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelperLibrary.Loading
{
    public class ToDo
    {
        private string description;
        private Action toDoAction;

        public string Description { get => description; set => description = value; }
        public Action ToDoAction { get => toDoAction; set => toDoAction = value; }

        public ToDo(string desciption, Action toDoAction)
        {           
            Description = desciption;
            ToDoAction = toDoAction;
        }
    }
}
