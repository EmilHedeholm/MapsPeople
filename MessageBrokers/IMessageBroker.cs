using DatabaseAccess;
using DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBrokers {
    public interface IMessageBroker {
        void ReceiveUpdateFromConsumer(IDataAccess dataAccess);
    }
}
