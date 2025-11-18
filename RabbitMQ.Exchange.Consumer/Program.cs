// See https://aka.ms/new-console-template for more information

//1 - BAĞLANTI OLUŞTURMA
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var url = "amqps://fvubfoqc:PxHgWhpdj7G160iMO1HRMpBWsvA-uCK3@jackal.rmq.cloudamqp.com/fvubfoqc";

ConnectionFactory factory = new ConnectionFactory();
factory.Uri = new Uri(url);

IConnection connection = await factory.CreateConnectionAsync();
IChannel channel = await connection.CreateChannelAsync();


//1.ADIM
await channel.ExchangeDeclareAsync(
    exchange: "hello-direct-exhange-type",
    type: ExchangeType.Direct);

//2.ADIM
var queueName = channel.QueueDeclareAsync().Result.QueueName;

//3.ADIM
await channel.QueueBindAsync(
    queue: queueName,
    exchange: "hello-direct-exhange-type",
    routingKey: "hello-routingKey");

AsyncEventingBasicConsumer consumer = new(channel);
await channel.BasicConsumeAsync(
    queue: queueName,
    autoAck: true,
    consumer: consumer);

consumer.ReceivedAsync += async (@sender, e) =>
{
    string message = Encoding.UTF8.GetString(e.Body.Span);
    Console.WriteLine(message);
};


Console.Read();

//1.Adım Publisherdaki ki exhange ile birebir aynı isim ve typea sahip exchange tanımlanmalıdır.

//2.Adım Publisher tarafından routingkey değerindeki kuyruğa gönderilen mesajları, kendi oluşturduuğumuz kuyruğa yönlendirerek tüketmemiz gerekecek.

