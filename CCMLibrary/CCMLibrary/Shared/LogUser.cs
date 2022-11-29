using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCMLibrary
{
    public class LogUser
    {
        protected Action<object[]> _onConsoleAction;

        public LogUser(Action<object[]> onConsoleAction)
        {
            _onConsoleAction = onConsoleAction; 
        }

        public void Print(params object[] values)
        {
            _onConsoleAction(values);
        }
    }
}
