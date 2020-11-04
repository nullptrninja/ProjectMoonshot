namespace Core.Systems.Messaging {
    public class GenericMessage : BaseMessage {
        public GenericMessage(int groupId, int subId)
            : base(NextAvailableMessageId(), groupId, subId) {
        }
    }
}
