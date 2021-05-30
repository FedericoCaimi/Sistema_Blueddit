using SistemaBlueddit.Domain;
using SistemaBlueddit.LogServer.Logic.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SistemaBlueddit.LogServer.Logic
{
    public class LogLogic: ILogLogic
    {
        private List<Log> logs = new List<Log>();

        public void AddLog(Log log)
        {
            logs.Add(log);
        }

        public IEnumerable<Log> GetAll(string type, string dateString)
        {
            var filteredLogs = logs;
            if(type != null && dateString == null)
            {
                filteredLogs = logs.Where(log => log.LogType.Equals(type)).ToList();
            }
            if(dateString != null)
            {
                var date = DateTime.ParseExact(dateString, "yyyyMMdd", CultureInfo.InvariantCulture);
                if (type == null)
                {
                    filteredLogs = logs.Where(log => log.CreationDate.Day == date.Day 
                    && log.CreationDate.Month == date.Month 
                    && log.CreationDate.Year == date.Year).ToList();
                }
                else
                {
                    filteredLogs = logs.Where(log => log.CreationDate.Day == date.Day
                    && log.CreationDate.Month == date.Month
                    && log.CreationDate.Year == date.Year
                    && log.LogType.Equals(type)).ToList();
                }
            }
            return filteredLogs;
        }
    }
}
