
namespace ReplicaSetRole {

    using Microsoft.WindowsAzure.Diagnostics;
    using Microsoft.WindowsAzure.ServiceRuntime;

    using System;
    using System.Diagnostics;
    using System.IO;

    internal class TraceHelper {

        private static TextWriter traceWriter = null;

        static TraceHelper() {
            var diagObj = DiagnosticMonitor.GetDefaultInitialConfiguration();
            diagObj.Logs.ScheduledTransferPeriod = Constants.DiagnosticTransferInterval;
            AddPerfCounters(diagObj);
            diagObj.PerformanceCounters.ScheduledTransferPeriod = Constants.PerfCounterTransferInterval;
            diagObj.Logs.ScheduledTransferLogLevelFilter = LogLevel.Verbose;
            DiagnosticMonitor.Start(Constants.DiagnosticsConnectionString, diagObj);

            var localStorage = RoleEnvironment.GetLocalResource(Constants.MongoTraceDir);
            var fileName = Path.Combine(localStorage.RootPath, Constants.TraceLogFile);
            traceWriter = new StreamWriter(fileName);
            TraceInformation(string.Format("Local log file is {0}", fileName));
        }

        private static void AddPerfCounters(DiagnosticMonitorConfiguration diagObj) {
            AddPerfCounter(diagObj, @"\LogicalDisk(*)\% Disk Read Time", 30);
            AddPerfCounter(diagObj, @"\LogicalDisk(*)\% Disk Write Time", 30);
            AddPerfCounter(diagObj, @"\LogicalDisk(*)\% Free Space", 30);
            AddPerfCounter(diagObj, @"\LogicalDisk(*)\Disk Read Bytes/sec", 30);
            AddPerfCounter(diagObj, @"\LogicalDisk(*)\Disk Write Bytes/sec", 30);
            AddPerfCounter(diagObj, @"\Memory\Available MBytes", 30);
            AddPerfCounter(diagObj, @"\Network Interface(*)\Bytes Received/sec", 30);
            AddPerfCounter(diagObj, @"\Network Interface(*)\Bytes Sent/sec", 30);
            AddPerfCounter(diagObj, @"\Processor(*)\% Processor Time", 30);
            AddPerfCounter(diagObj, @"\PhysicalDisk(*)\% Disk Read Time", 30);
            AddPerfCounter(diagObj, @"\PhysicalDisk(*)\% Disk Write Time", 30);
        }

        private static void AddPerfCounter(DiagnosticMonitorConfiguration config, string name, double seconds) {
            var perfmon = new PerformanceCounterConfiguration();
            perfmon.CounterSpecifier = name;
            perfmon.SampleRate = System.TimeSpan.FromSeconds(seconds);
            config.PerformanceCounters.DataSources.Add(perfmon);
        }

        internal static void TraceInformation(string message) {
            Trace.TraceInformation(message);
            WriteTraceMessage(message, "INFORMATION");
        }

        internal static void TraceWarning(string message) {
            Trace.TraceWarning(message);
            WriteTraceMessage(message, "WARNING");
        }

        internal static void TraceError(string message) {
            Trace.TraceError(message);
            WriteTraceMessage(message, "ERRROR");
        }

        private static void WriteTraceMessage(string message, string type) {
            if (traceWriter != null) {
                try {
                    var messageString = string.Format("{0}-{1}-{2}", DateTime.UtcNow.ToString(), type, message);
                    traceWriter.WriteLine(messageString);
                    traceWriter.Flush();
                } catch {
                    // ignore trace message errors
                }
            }
        }
    }
}
