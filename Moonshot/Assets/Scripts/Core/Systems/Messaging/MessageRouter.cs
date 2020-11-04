using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Systems.Messaging {
    /// <summary>
    /// The MessageRouter is an observer-pattern. Objects that can receive messages (IMessageListener) must register themselves
    /// with the GameSession-specific instance of the MessageRouter. It is important to know that once a level has been unloaded
    /// (or whenever the StateHook is no longer running) then the MessageRouter is no longer viable and everything is unregistered.
    /// 
    /// Messages are divvied into three tiers of organization:
    /// - Channel: a globally well known pathway separated by high level functionality such as "UI channel" or "Game Event channel" etc
    /// - Group ID: a message can be assigned a group ID which receivers can look for speficially.
    /// - SubID: a sub ID used to further refine the message target.
    /// Additionally, messages have a Message ID field, which listeners can listen for a very specific Message ID to respond to.
    /// 
    /// Messages can be sent in one of two modes:
    /// - Immediate, which propagates the message to all listeners in a channel within the same cycle. Use this with care as to not
    ///   cause game performance degradation.
    /// - Async, which enqueues the message for processing. It will eventually be sent depending on the internal game update rate. This
    ///   is the default sending mode for messages. Message senders should expect that a message may not be immediately received.
    /// </summary>
    public class MessageRouter {
        public enum Result {
            AcceptedConsumed = 0,       // Message was received and was relevant. Message WILL NOT propagate.            
            AcceptedPassive,            // Message was received and may be relavent. Message can continue to propagate.
            Ignored,                    // Message was received but not relevant. Message can continue to propagate.
        }
        
        private const int DefaultListenersPerChannel = 16;        
        private const int DefaultQueueSize = 128;

        private List<IMessageListener>[] mListeners;     // Integer keys are faster
        private Queue<QueuedMessage> mMessageQueue;

        private MonoBehaviour mHost;
        private bool mQueueProcessingStarted;

        public MessageRouter(int numChannels, MonoBehaviour hostObject) {
            mQueueProcessingStarted = false;
            mHost = hostObject;         // HACK: This is a bit ghetto but we need the Unity coroutine processor to process messages
                                        // We'll need a cleaner way to do this later.

            mListeners = new List<IMessageListener>[numChannels];
            for (int i = 0; i < numChannels; i++) {
                mListeners[i] = null;
            }

            mMessageQueue = new Queue<QueuedMessage>(DefaultQueueSize);
        }

        public void AddListener(int channel, IMessageListener listener) {
            if (!HasChannel(channel)) {
                AddChannel(channel);
            }

            GetChannel(channel).Add(listener);            
        }

        public void RemoveListener(int channel, IMessageListener listener) {
            if (HasChannel(channel)) {
                GetChannel(channel).Remove(listener);
            }
        }

        public void RemoveListener(IMessageListener listener) {
            foreach (var channel in mListeners) {
                channel.Remove(listener);
            }
        }

        public void ClearChannel(int channel) {
            if (HasChannel(channel)) {
                GetChannel(channel).Clear();
            }
        }

        public void ClearAllChannels() {
            foreach (var channel in mListeners) {
                channel.Clear();
            }
        }
        
        private void EnqueueMessage(BaseMessage message, int channel) {
            mMessageQueue.Enqueue(new QueuedMessage(channel, message));

            // Async queue processing is done via coroutine attached to the current
            // level's state hook.
            if (!mQueueProcessingStarted) {
                mHost.StartCoroutine(BeginQueueProcessing());
            }
        }

        public void Send(int channel, BaseMessage message) {
            if (HasChannel(channel)) {
                switch (message.Priority) {
                    case BaseMessage.DeliveryPriority.Immediate:
                        SendMessageImmediate(message, channel);
                        break;

                    case BaseMessage.DeliveryPriority.Async:
                        EnqueueMessage(message, channel);
                        break;
                }
            }
        }

        private IEnumerator BeginQueueProcessing() {
            mQueueProcessingStarted = true;
            while (mMessageQueue.Count > 0) {
                QueuedMessage qm = mMessageQueue.Dequeue();
                SendMessageImmediate(qm.Message, qm.Channel);
                yield return null;
            }

            mQueueProcessingStarted = false;
        }        
        
        private void SendMessageImmediate(BaseMessage message, int channel) {
            foreach (var m in GetChannel(channel)) {
                if (m.ReceiveMessage(message) == Result.AcceptedConsumed) {
                    return;
                }
            }
        }

        private bool HasChannel(int channel) {
            return mListeners[channel] != null;
        }

        private List<IMessageListener> GetChannel(int channel) {
            return mListeners[channel];
        }

        private void AddChannel(int channel) {
            if (!HasChannel(channel)) {
                mListeners[channel] = new List<IMessageListener>(MessageRouter.DefaultListenersPerChannel);
            }
        }
    }

    struct QueuedMessage {
        public int Channel;
        public BaseMessage Message;

        public QueuedMessage(int channel, BaseMessage msg) {
            this.Channel = channel;
            this.Message = msg;
        }
    }
}
