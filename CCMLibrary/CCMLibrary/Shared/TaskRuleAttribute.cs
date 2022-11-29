using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCMLibrary
{
    public enum TaskExecution { Sync, Async };

    public enum Cores { Max };

    [AttributeUsage(AttributeTargets.Class)]
    public class TaskRuleAttribute : Attribute
    {
        public TaskExecution TaskExecution;
        public Cores Cores;
        public int CoresMultiply = -1;
        public int TasksCount;

        public TaskRuleAttribute(TaskExecution taskRun, int tasksCount)
        {
            TasksCount = tasksCount;
            TaskExecution = taskRun;
        }

        public TaskRuleAttribute(TaskExecution taskRun, Cores cores, int multiply)
        { 
            TaskExecution = taskRun;
            Cores = cores;
            CoresMultiply = multiply;
        }

        public TaskRuleAttribute()
        {
            TaskExecution = TaskExecution.Async;
            Cores = Cores.Max;
            CoresMultiply = 1;
        }

    }
}
