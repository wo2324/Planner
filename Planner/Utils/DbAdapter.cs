using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planner.Utils
{
    public static class DbAdapter
    {
        #region Participant handle

        public static bool ParticipantCheck(string login, string password)
        {
            string connectionString = ConfigurationManager.AppSettings["connectionStirng"].ToString();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.CommandText = "plann.usp_ParticipantCheck";
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Participant_Name", login));
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Participant_Password", password));
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                    if (sqlConnection.State == ConnectionState.Closed)
                    {
                        sqlConnection.Open();
                    }

                    DataSet dataSet = new DataSet();
                    sqlDataAdapter.Fill(dataSet);

                    return Convert.ToBoolean(dataSet.Tables[0].Rows[0]["CheckSentence"]);
                }
            }
        }

        public static void ParticipantAdd(string login, string password)
        {
            string connectionString = ConfigurationManager.AppSettings["connectionStirng"].ToString();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.CommandText = "plann.usp_ParticipantAdd";
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Participant_Name", login));
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Participant_Password", password));
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        public static void EditPassword(string participantName, string newPassword)
        {
            string connectionString = ConfigurationManager.AppSettings["connectionStirng"].ToString();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.CommandText = "plann.usp_PasswordEdit";
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Participant_Name", participantName));
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Participant_NewPassword", newPassword));
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        public static void DeleteAccount(string participantName)
        {
            string connectionString = ConfigurationManager.AppSettings["connectionStirng"].ToString();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.CommandText = "plann.usp_ParticipantDelete";
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Participant_Name", participantName));
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        #endregion

        public static string ParticipantPasswordGet(string participantName)
        {
            string connectionString = ConfigurationManager.AppSettings["connectionStirng"].ToString();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.CommandText = "mc.usp_ParticipantPasswordGet";
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Participant_Id", participantName));
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                    if (sqlConnection.State == ConnectionState.Closed)
                    {
                        sqlConnection.Open();
                    }

                    DataSet dataSet = new DataSet();
                    sqlDataAdapter.Fill(dataSet);

                    if (dataSet.Tables[0].Rows.Count != 0)
                    {
                        return dataSet.Tables[0].Rows[0]["Participant_Password"].ToString();
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public static DataTable GetPlanners(string participantName)
        {
            string connectionString = ConfigurationManager.AppSettings["connectionStirng"].ToString();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.CommandText = "mc.usp_Planner_NameGet";
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Participant_Id", participantName));
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                    if (sqlConnection.State == ConnectionState.Closed)
                    {
                        sqlConnection.Open();
                    }

                    DataSet dataSet = new DataSet();
                    sqlDataAdapter.Fill(dataSet);

                    return dataSet.Tables[0];
                }
            }
        }

        public static List<string> ExtractPlannersList(DataTable dataTable)
        {
            List<string> Planners = new List<string>();
            foreach (DataRow dataRow in dataTable.Rows)
            {
                Planners.Add(dataRow["Planner_Name"].ToString());
            }
            return Planners;
        }

        public static List<string> ExtractTasksTypesList(DataTable dataTable)
        {
            List<string> TasksTypes = new List<string>();
            foreach (DataRow dataRow in dataTable.Rows)
            {
                TasksTypes.Add(dataRow["TaskType_Name"].ToString());
            }
            return TasksTypes;
        }

        public static int GetPlannerId(string participantName, string plannerName)
        {
            string connectionString = ConfigurationManager.AppSettings["connectionStirng"].ToString();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.CommandText = "mc.usp_PlannerIdGet";
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Planner_Participant_Id", participantName));
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Planner_Name", plannerName));
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                    if (sqlConnection.State == ConnectionState.Closed)
                    {
                        sqlConnection.Open();
                    }

                    DataSet dataSet = new DataSet();
                    sqlDataAdapter.Fill(dataSet);

                    return Convert.ToInt32(dataSet.Tables[0].Rows[0]["Planner_Id"]);
                }
            }
        }

        public static DataTable GetPlannerTask(int plannerId)
        {
            string connectionString = ConfigurationManager.AppSettings["connectionStirng"].ToString();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.CommandText = "mc.usp_PlannerTaskGet";
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Task_Planner_Id", plannerId));
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                    if (sqlConnection.State == ConnectionState.Closed)
                    {
                        sqlConnection.Open();
                    }

                    DataSet dataSet = new DataSet();
                    sqlDataAdapter.Fill(dataSet);

                    return dataSet.Tables[0];
                }
            }
        }

        public static DataTable GetPlanner(string participantName, string plannerName)
        {
            string connectionString = ConfigurationManager.AppSettings["connectionStirng"].ToString();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.CommandText = "mc.usp_WholePlannerGet";
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Participant_Id", participantName));
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Planner_Name", plannerName));

                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                    if (sqlConnection.State == ConnectionState.Closed)
                    {
                        sqlConnection.Open();
                    }

                    DataSet dataSet = new DataSet();
                    sqlDataAdapter.Fill(dataSet);

                    return dataSet.Tables[0];
                }
            }
        }

        public static DataTable GetTasksTypes(int plannerId)
        {
            string connectionString = ConfigurationManager.AppSettings["connectionStirng"].ToString();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.CommandText = "mc.usp_TaskTypeGet";
                    sqlCommand.Parameters.Add(new SqlParameter("@v_TaskType_Planner_Id", plannerId));

                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                    if (sqlConnection.State == ConnectionState.Closed)
                    {
                        sqlConnection.Open();
                    }

                    DataSet dataSet = new DataSet();
                    sqlDataAdapter.Fill(dataSet);

                    return dataSet.Tables[0];
                }
            }
        }

        public static void CreatePlanner(string participantName, string plannerName, string plannerDescription, string firstDay, string startHour, string stopHour, string timeSpan) //Do poprawy!
        {
            string connectionString = ConfigurationManager.AppSettings["connectionStirng"].ToString();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.CommandText = "mc.usp_PlannerAdd";
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Planner_Participant_Id", participantName));
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Planner_Name", plannerName));
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Planner_Description", plannerDescription));
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Planner_FirstDay", firstDay));
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Planner_StartHour", startHour));
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Planner_StopHour", stopHour));
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Planner_TimeSpan", timeSpan));
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        public static void CreatePlanner(string participantName, string plannerName, string firstDay, string startHour, string stopHour, string timeSpan, DataTable task) //Do poprawy!
        {
            string connectionString = ConfigurationManager.AppSettings["connectionStirng"].ToString();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.CommandText = "mc.usp_CreatePlanner";
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Planner_Participant_Id", participantName));
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Planner_Name", plannerName));
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Planner_FirstDay", firstDay));
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Planner_StartHour", startHour));
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Planner_StopHour", stopHour));
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Planner_TimeSpan", timeSpan));
                    sqlCommand.Parameters.Add(new SqlParameter("@p_tvp_Task", task));
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        public static void TaskAdd(int plannerId, DataTable task)
        {
            string connectionString = ConfigurationManager.AppSettings["connectionStirng"].ToString();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.CommandText = "mc.usp_TaskAdd";
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Task_Planner_Id", plannerId));
                    sqlCommand.Parameters.Add(new SqlParameter("@p_tvp_Task", task));
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        //
        public static void EditTask(int plannerId, DataTable task)
        {
            string connectionString = ConfigurationManager.AppSettings["connectionStirng"].ToString();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.CommandText = "mc.usp_TaskEdit";
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Planner_Id", plannerId));
                    sqlCommand.Parameters.Add(new SqlParameter("@p_tvp_Task", task));
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        public static void TaskTypeAdd(int plannerId, string taskTypeName, bool taskTypeTextVisibility, string color)
        {
            string connectionString = ConfigurationManager.AppSettings["connectionStirng"].ToString();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.CommandText = "mc.usp_TaskTypeAdd";
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Planner_Id", plannerId));
                    sqlCommand.Parameters.Add(new SqlParameter("@p_TaskType_Name", taskTypeName));
                    sqlCommand.Parameters.Add(new SqlParameter("@p_TaskType_Description", ""));
                    sqlCommand.Parameters.Add(new SqlParameter("@p_TaskType_TextVisibility", taskTypeTextVisibility));
                    sqlCommand.Parameters.Add(new SqlParameter("@p_TaskType_Color", color));

                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        public static void CopyPlanner(string participantName, string plannerName, string plannerCopyName)
        {
            string connectionString = ConfigurationManager.AppSettings["connectionStirng"].ToString();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.CommandText = "mc.usp_CopyPlanner";
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Participant_Id", participantName));
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Planner_Name", plannerName));
                    sqlCommand.Parameters.Add(new SqlParameter("@p_NewPlannerName", plannerCopyName));
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        public static void RenamePlanner(string participantName, string plannerName, string plannerNewName)
        {
            string connectionString = ConfigurationManager.AppSettings["connectionStirng"].ToString();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.CommandText = "mc.usp_PlannerRename";
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Participant_Id", participantName));
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Planner_Name", plannerName));
                    sqlCommand.Parameters.Add(new SqlParameter("@p_PlannerNewName", plannerNewName));
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        public static void DeletePlanner(string participantName, string plannerName)
        {
            string connectionString = ConfigurationManager.AppSettings["connectionStirng"].ToString();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.CommandText = "mc.usp_PlannerDelete";
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Participant_Id", participantName));
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Planner_Name", plannerName));
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        public static DataTable GetTasksDataTable()
        {
            DataTable plannerTasks = new DataTable("PlannerTasks");
            plannerTasks.Columns.Add("tvp_Task_Name", typeof(string));
            plannerTasks.Columns.Add("tvp_Task_NameVisibility", typeof(bool));
            plannerTasks.Columns.Add("tvp_Task_Color", typeof(string));
            plannerTasks.Columns.Add("tvp_Task_Day", typeof(string));
            plannerTasks.Columns.Add("tvp_Task_Time", typeof(string));
            return plannerTasks;
        }
    }
}
