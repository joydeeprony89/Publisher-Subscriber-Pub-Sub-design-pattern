namespace Publisher_Subscriber_Pub_Sub_design_pattern
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Publisher pub = new Publisher();
            pub.OnChange += () => { Console.WriteLine("subscriber 1"); };
            pub.OnChange += () => { Console.WriteLine("subscriber 2"); };
            pub.Raise();

            Publisher_With_EventHandler publisher_With_EventHandler = new Publisher_With_EventHandler();
            publisher_With_EventHandler.OnChange += (sender, e) => 
            {
                Console.WriteLine("subscriber 1 of event handler"+ e._value);
            };
            publisher_With_EventHandler.OnChange += (sender, e) =>
            {
                Console.WriteLine("subscriber 2 of event handler");
            };
            publisher_With_EventHandler.Raise();

            var int_pub = new Publisher<int>();
            var sub1 = new Subscriber<int>(int_pub);
            sub1.Publisher.OnHandleMessage += handle_int_publisher;
            sub1.Publisher.Publish(new Message<int>(1));

            var string_pub = new Publisher<string>();
            var sub2 = new Subscriber<string>(string_pub);
            sub2.Publisher.OnHandleMessage += handle_string_publisher;
            sub2.Publisher.Publish(new Message<string>("Hello"));
        }

        private static void handle_string_publisher(object? sender, Message<string> e)
        {
            Console.WriteLine($"sender is ={sender?.GetType()} and message is = {e._message}");
        }

        private static void handle_int_publisher(object? sender, Message<int> e)
        {
            Console.WriteLine($"sender is ={sender?.GetType()} and message is = {e._message}");
        }
    }

    public class Publisher
    {
        public event Action OnChange = delegate { };
        public void Raise()
        {
            OnChange();
        }
    }

    public class MyEventArgs : EventArgs
    {
        public int _value = 0;
        public MyEventArgs(int value) { _value = value; }

    }

    public class Publisher_With_EventHandler
    {
        public event EventHandler<MyEventArgs> OnChange = delegate { };
        public void Raise()
        {
            OnChange(this, new MyEventArgs(1));
        }
    }

    public class Message<T> : EventArgs
    {
        public T _message;
        public Message(T message)
        {
            _message = message;
        }
    }

    public interface IPublisher<T>
    {
        event EventHandler<Message<T>> OnHandleMessage;
        void MessageHandler(Message<T> message);
        void Publish(Message<T> message);
    }

    public class Publisher<T> : IPublisher<T>
    {
        public event EventHandler<Message<T>> OnHandleMessage = delegate { };

        public void MessageHandler(Message<T> message)
        {
            OnHandleMessage(this, message);
        }

        public void Publish(Message<T> message)
        {
            MessageHandler(message);
        }
    }

    public class Subscriber<T>
    {
        public IPublisher<T> Publisher { get; private set; }
        public Subscriber(IPublisher<T> publisher)
        {
            Publisher = publisher;
        }
    }
}
