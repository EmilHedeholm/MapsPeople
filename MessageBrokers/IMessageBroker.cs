using DatabaseAccess;
using DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBrokers {
    public interface IMessageBroker {
        void SendUpdateToUsers(List<ExternalModel> messages);
        void SendUpdateToCore(List<Location> locations);
        void ReceiveUpdateFromConsumer(IDataAccess dataAccess);
        void RecieveUpdateFromCore(string userName, string LocationID);
    }
}
