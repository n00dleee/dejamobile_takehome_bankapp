using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dejamobile_takehome_bankapp.Service
{
    public abstract class ServiceTemplate : Service.IService
    {
        public bool isStarted { get; set; }

        private IEventAggregator _eventAggregator;
        public IEventAggregator eventAggregator
        {
            get { return _eventAggregator; }
            set { _eventAggregator = value; }
        }

        public ServiceTemplate(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }

        public void start()
        {
            if (isStarted == false)
            {
                subscribe();
                isStarted = true;
            }
        }

        public void stop()
        {
            if (isStarted == true)
            {
                isStarted = false;
                unsubscribe();
            }
        }

        public abstract void subscribe();
        public abstract void unsubscribe();
    }
}
