namespace Core.Systems.Messaging {
    public class BaseMessage {
        private static int RollingMessageIdIndex = 100000;              // Keep the rolling Ids at 100,000 and higher.
        public static int NextAvailableMessageId() { return RollingMessageIdIndex++; }

        public enum DeliveryPriority {
            Immediate = 0,                  // For immediate delivery within the same cycle
            Async                           // For low priority messages; it'll get there soon-ish
        }

        protected BaseMessage(int msgId, int grpId, int subId = -1) {
            this.MessageId = msgId;
            this.GroupId = grpId;
            this.SubId = subId;

            this.ObjectData = null;
            this.StringData = null;
            this.IntegerData = -1;
        }

        public BaseMessage CopyWithInt(int intData) {
            var msg = new BaseMessage(this.MessageId, this.GroupId, this.SubId) {
                IntegerData = intData
            };
            return msg;
        }

        public BaseMessage CopyWithString(string strData) {
            var msg = new BaseMessage(this.MessageId, this.GroupId, this.SubId) {
                StringData = strData
            };
            return msg;
        }

        public BaseMessage CopyWithObject(object objData) {
            var msg = new BaseMessage(this.MessageId, this.GroupId, this.SubId) {
                ObjectData = objData
            };
            return msg;
        }

        // -- Properties -----

        public int MessageId { get; set; }

        public int GroupId { get; set; }

        public int SubId { get; set; }

        public object ObjectData { get; set; }

        public string StringData { get; set; }

        public int IntegerData { get; set; }

        public DeliveryPriority Priority { get; set; }
    }
}