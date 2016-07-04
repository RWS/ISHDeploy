using System;
using System.Data;
using System.Reflection;

namespace ISHDeploy.Data.Actions.DataBase.Mapper
{
    /// <summary>
    /// Casts data from DataRow to specified type
    /// </summary>
    public class DataRowToModelMapper
    {
        /// <summary>
        /// Map data from DataRow to specified type
        /// </summary>
        /// <param name="row">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static TResult Map<TResult>(DataRow row) where TResult : class
        {
            TResult resultIstance = Activator.CreateInstance<TResult>();
            var porpsh = resultIstance.GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
            foreach (var psh in porpsh)
            {
                try
                {
                    psh.SetValue(resultIstance, Convert.IsDBNull(row[psh.Name]) ? null : row[psh.Name], null);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Can not cast {typeof(DataRow)} to {typeof(TResult)} ", ex);
                }
            }

            return resultIstance;
        }
    }
}
