using System;
using System.Data;
using System.Diagnostics;

namespace BicyclesSuite.Shared
{
    /// <summary>
    /// Extension methods for System.Data classes
    /// </summary>
    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    public static class DataExtension
    {
        /// <summary>
        /// Wrap anonymous delegate with transaction
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="method"></param>
        /// <param name="il"></param>
        /// <param name="beginTransactionMethod"></param>
        public static void TransactionAdapter(
            this IDbConnection connection,
            Action<IDbTransaction> method,
            IsolationLevel il = IsolationLevel.Unspecified,
            Func<IDbTransaction> beginTransactionMethod = null)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("Connection can't be null");
#warning Constant.Exception.Runtime...CreateException();
            }
            connection.Open();
            IDbTransaction transaction = null;
            try
            {
                transaction = beginTransactionMethod != null
                    ? beginTransactionMethod()
                    : connection.BeginTransaction(il);

                if (transaction == null)
                {
                    throw new NullReferenceException("Transaction can't be null");
#warning Constant.Exception.Runtime...CreateException();
                }

                method(transaction);

                transaction.Commit();
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
#warning Apply ILogger extension
                //method.GetType().Error(ex);
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Apply .ToString() method to DataRow with checking DBNull value
        /// </summary>
        /// <param name="row"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetString(this DataRow row, string name)
        {
            object value = row[name];
            return value != DBNull.Value ? value.ToString() : string.Empty;
        }
    }
}
