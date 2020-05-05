using DatabaseAccess;

namespace MessageBrokers {
    public interface IMessageBroker {
        void ReceiveUpdateFromConsumer(IDataAccess dataAccess);
    }
}
