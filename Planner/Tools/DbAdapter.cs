using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Planner.Tools
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

        #region Planner manage

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
                    sqlCommand.CommandText = "plann.usp_PlannersGet";
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Participant_Name", participantName));
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

        public static List<string> ExtractPlanners(DataTable dataTable)
        {
            List<string> Planners = new List<string>();
            foreach (DataRow dataRow in dataTable.Rows)
            {
                Planners.Add(dataRow["Planner_Name"].ToString());
            }
            return Planners;
        }

        public static DataTable GetTasksDataTable()
        {
            DataTable plannerTasks = new DataTable();
            plannerTasks.Columns.Add("tvp_Task_Day", typeof(string));
            plannerTasks.Columns.Add("tvp_Task_Time", typeof(string));
            plannerTasks.Columns.Add("tvp_Task_TaskType_Id", typeof(int));
            return plannerTasks;
        }

        public static void CreatePlanner(string participantName, string plannerName, string firstDay, string startHour, string stopHour, string timeSpan, DataTable task)
        {
            string connectionString = ConfigurationManager.AppSettings["connectionStirng"].ToString();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.CommandText = "plann.usp_CreatePlanner";
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Planner_Participant_Name", participantName));
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Planner_Name", plannerName));
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Planner_FirstDay", firstDay));
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Planner_StartTime", startHour));
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Planner_StopTime", stopHour));
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Planner_Interval", timeSpan));
                    sqlCommand.Parameters.Add(new SqlParameter("@p_tvp_Task", task));
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
                    sqlCommand.CommandText = "plann.usp_CopyPlanner";
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Participant_Name", participantName));
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Planner_Name", plannerName));
                    sqlCommand.Parameters.Add(new SqlParameter("@p_PlannerCopy_Name", plannerCopyName));
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
                    sqlCommand.CommandText = "plann.usp_PlannerRename";
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Participant_Name", participantName));
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
                    sqlCommand.CommandText = "plann.usp_DeletePlanner";
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Participant_Name", participantName));
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Planner_Name", plannerName));
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        #endregion

        #region Planner handle

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
                    sqlCommand.CommandText = "plann.usp_PlannerGet";
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Participant_Name", participantName));
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

        public static DataTable GetTask(string participantName, string plannerName)
        {
            string connectionString = ConfigurationManager.AppSettings["connectionStirng"].ToString();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.CommandText = "plann.usp_TaskGet";
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Participant_Name", participantName));
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

        public static DataTable GetTasksTypes(string participantName, string plannerName)
        {
            string connectionString = ConfigurationManager.AppSettings["connectionStirng"].ToString();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.CommandText = "plann.usp_TasksTypesGet";
                    sqlCommand.Parameters.Add(new SqlParameter("@v_Participant_Name", participantName));
                    sqlCommand.Parameters.Add(new SqlParameter("@v_Planner_Name", plannerName));

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

        public static List<string> ExtractTasksTypes(DataTable dataTable)
        {
            List<string> TasksTypes = new List<string>();
            foreach (DataRow dataRow in dataTable.Rows)
            {
                TasksTypes.Add(dataRow["TaskType_Name"].ToString());
            }
            return TasksTypes;
        }

        public static void TaskTypeAdd(string participantName, string plannerName, string taskTypeName, string taskTypeForeground, string taskTypeBackground)
        {
            string connectionString = ConfigurationManager.AppSettings["connectionStirng"].ToString();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.CommandText = "plann.usp_TaskTypeAdd";
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Participant_Name", participantName));
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Planner_Name", plannerName));
                    sqlCommand.Parameters.Add(new SqlParameter("@p_TaskType_Name", taskTypeName));
                    sqlCommand.Parameters.Add(new SqlParameter("@p_TaskType_Foreground", taskTypeForeground));
                    sqlCommand.Parameters.Add(new SqlParameter("@p_TaskType_Background", taskTypeBackground));

                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        public static void EditTasks(string participantName, string plannerName, DataTable task)
        {
            string connectionString = ConfigurationManager.AppSettings["connectionStirng"].ToString();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.CommandText = "plann.usp_TaskEdit";
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Participant_Name", participantName));
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Planner_Name", plannerName));
                    sqlCommand.Parameters.Add(new SqlParameter("@p_tvp_Task", task));
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        public static void DeleteTaskType(string participantName, string plannerName, string taskTypeName)
        {
            string connectionString = ConfigurationManager.AppSettings["connectionStirng"].ToString();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.CommandText = "plann.usp_TaskTypeDelete";
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Participant_Name", participantName));
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Planner_Name", plannerName));
                    sqlCommand.Parameters.Add(new SqlParameter("@p_TaskType_Name", taskTypeName));
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        #endregion

        #region Planner details

        public static DataTable GetOccurrencesNumber(string participantName, string plannerName, string taskTypeName)
        {
            string connectionString = ConfigurationManager.AppSettings["connectionStirng"].ToString();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.CommandText = "meta.usp_GetOccurrencesNumber";
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Participant_Name", participantName));
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Planner_Name", plannerName));
                    sqlCommand.Parameters.Add(new SqlParameter("@p_TaskType_Name", taskTypeName));

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

        #endregion
    }
}
