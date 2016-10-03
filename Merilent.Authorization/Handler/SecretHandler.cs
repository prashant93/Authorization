using Merilent.Logger;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace Merilent.Authorization
{
    /// <summary>
    /// Gets API secret from DB
    /// </summary>
    internal class SecretHandler
    {
        private static AuthContext _context;

        internal static string GetAPISecret(string authConnectionString, string appName)
        {
            try
            {
                if (authConnectionString == null || appName == null)
                {
                    throw new Exception("ConnectionString AppName cannot be null");
                }
                LogHelper.Log($"Connecting to: {authConnectionString} ", LogLevel.INFORMATION);
                LogHelper.Log($"Getting secret key for: {appName} ", LogLevel.INFORMATION);
                string apiSecret = string.Empty;

                using (_context = new AuthContext(authConnectionString))
                {
                    apiSecret = (from v in _context.AppSecrets
                                 where (v.ApplicationName == appName && v.IsActive == true && v.ApplicationSecret != null)
                                 select v.ApplicationSecret).First();
                }
                LogHelper.Log($"Secret key for: {appName} generated sucessfully ", LogLevel.INFORMATION);
                return apiSecret;
            }
            catch (SqlException sqlex)
            {
                var errorMessage = $"Cannot open connection to {authConnectionString}";
                LogHelper.Log($"{errorMessage} {sqlex}", LogLevel.ERROR);
                throw new Exception(errorMessage, sqlex);
            }
            catch (Exception ex)
            {
                var errorMessage = $"Cannot open connection to {authConnectionString} and get secret key for {appName}:";
                LogHelper.Log($"{errorMessage} {ex} ", LogLevel.ERROR);
                throw new Exception(errorMessage, ex);
            }
        }
    }
}