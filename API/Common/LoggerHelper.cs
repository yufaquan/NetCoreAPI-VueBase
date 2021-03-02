
using log4net;
using log4net.Config;
using log4net.Repository;
using System.IO;

namespace Commons
{

    public class LogHelper<T> where T:class
    {
        public LogHelper()
        {
            var name = typeof(T).Name;
            Configure(name);
        }
        private  ILoggerRepository repository { get; set; }
        private  ILog _log;
        private  ILog log
        {
            get
            {
                if (_log == null)
                {
                    Configure();
                }
                return _log;
            }
        }

        private  void Configure(string repositoryName = "NETCoreRepository", string configFile = "log4net.config")
        {
            repository = LogManager.CreateRepository(repositoryName);
            XmlConfigurator.Configure(repository, new FileInfo(configFile));
            _log = LogManager.GetLogger(repositoryName, "");
        }

        public void Info(string msg)
        {
            log.Info(msg);
        }

        public  void Warn(string msg)
        {
            log.Warn(msg);
        }

        public  void Error(string msg)
        {
            log.Error(msg);
        }
    }
}
