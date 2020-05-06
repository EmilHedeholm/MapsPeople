using DatabaseAccess;

namespace MessageBrokers {
    //This is an interface that handles the message brokers that sends updates to the core. 
    public interface IMessageBroker {
        void HandleUpdateFromConsumer(IDataAccess dataAccess);
    }
}
