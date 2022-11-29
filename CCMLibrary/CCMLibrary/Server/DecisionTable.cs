using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CCMLibrary
{
    internal class DecisionTable<Tx, Ty>
    {
        private List<Tx> X = new List<Tx>();
        private List<Ty> Y = new List<Ty>();
        private Func<object, object, (Enum, object)>[,] actions;

        public DecisionTable(IEnumerable<Tx> x, IEnumerable<Ty> y, Func<object, object, (Enum, object)>[,] actions)
        {
            X = x.ToList();
            Y = y.ToList();
            this.actions = actions;
        }

        public DecisionTable(IEnumerable<Tx> x, IEnumerable<Ty> y)
        {
            X = x.ToList();
            Y = y.ToList();
            actions = new Func<object, object, (Enum, object)>[X.Count, Y.Count];
        }

        public void SetAction(Tx xValue, Ty yValue, Func<object, object, (Enum, object)> action)
        {
            int xIndex = X.IndexOf(xValue);
            int yIndex = Y.IndexOf(yValue);
            actions[xIndex, yIndex] = action;
        }

        public (Enum, object) GetActionResults(Tx xValue, Ty yValue, object data1, object data2)
        {
            int xIndex = X.IndexOf(xValue);
            int yIndex = Y.IndexOf(yValue);

            Func<object,object, (Enum, object)> action = actions[xIndex, yIndex];
            try
            {
                return action(data1, data2);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
