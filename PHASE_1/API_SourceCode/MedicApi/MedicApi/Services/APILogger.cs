using NLog;

namespace MedicApi.Services
{
    public class APILogger
    {
        protected Logger logger; 
        public APILogger()
        {
            this.logger = LogManager.GetCurrentClassLogger();
        }
    }
}
