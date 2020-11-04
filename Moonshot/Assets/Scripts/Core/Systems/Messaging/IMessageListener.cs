namespace Core.Systems.Messaging {
    public interface IMessageListener {
        MessageRouter.Result ReceiveMessage(BaseMessage message);
    }
}
