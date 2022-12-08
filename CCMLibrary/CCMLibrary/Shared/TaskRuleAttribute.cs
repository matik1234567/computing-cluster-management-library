using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCMLibrary
{
    public enum TaskExecution { Sync, Async };

    [AttributeUsage(AttributeTargets.Class)]
    public class TaskRuleAttribute : Attribute
    {
        public TaskExecution TaskExecution;
        public float Cores = -1;
        public int CoresMultiply = 1;
        public int TasksCount = 1;

        public TaskRuleAttribute(TaskExecution taskRun, int tasksCount)
        {
            TasksCount = tasksCount;
            TaskExecution = taskRun;
        }

        public TaskRuleAttribute(TaskExecution taskRun, float cores, int multiply)
        { 
            TaskExecution = taskRun;
            Cores = cores;
            CoresMultiply = multiply;
        }

        public TaskRuleAttribute()
        {
            TaskExecution = TaskExecution.Sync;
        }

    }
}
