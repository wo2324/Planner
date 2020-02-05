using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planner.Utils
{
    public class Planner
    {
        public int PlannerId { get; }
        public string PlannerName { get; }
        public DataTable Task { get; }

        public Planner(int PlannerId, string PlannerName, DataTable Task)
        {
            this.PlannerId = PlannerId;
            this.PlannerName = PlannerName;
            this.Task = Task;
        }
    }
}
