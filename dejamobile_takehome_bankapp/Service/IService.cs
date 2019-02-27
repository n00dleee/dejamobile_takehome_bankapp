using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dejamobile_takehome_bankapp.Service
{
    interface IService
    {
        bool isStarted { get; set; }
        void start();
        void stop();
        void subscribe();
        void unsubscribe();
    }
}
