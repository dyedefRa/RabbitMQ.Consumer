// See https://aka.ms/new-console-template for more information
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;


//1 - BAĞLANTI OLUŞTURMA
var url = "amqps://fvubfoqc:PxHgWhpdj7G160iMO1HRMpBWsvA-uCK3@jackal.rmq.cloudamqp.com/fvubfoqc";
ConnectionFactory factory = new();
factory.Uri = new Uri(url);

//2 - AKTİFLEŞTİRME  ve KANAL AÇMA
using IConnection connection = await factory.CreateConnectionAsync();
using IChannel channel = await connection.CreateChannelAsync();

//Queue Oluşturma
var queueName = "example-queue";
await channel.QueueDeclareAsync(queue: queueName, exclusive: false, durable: true);
//Consumerdaki kuyruk publisherdaki gibi birebir aynı yapılandırılmalıdır.

Console.WriteLine("📩 Mesajlar dinleniyor... Çıkmak için CTRL+C bas.");


//Kuyruktan mesajı okuma.
AsyncEventingBasicConsumer consumer = new(channel);
await channel.BasicConsumeAsync(queue: queueName, false, consumer: consumer);
consumer.ReceivedAsync += async (@sender, e) =>
{
    //Kuyruğa gelen mesajın işlendiği yerdir!
    //e.Body : Kuyruktaki mesajın verisini bütünsel olarak getirir.
    //e.Body.Span veya e.Body.ToArray() = byte verisini getirir.
    var body = e.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine(message);

    //// İsteğe bağlı: Mesaj işlendi bilgisini RabbitMQ'ya bildir
    await channel.BasicAckAsync(e.DeliveryTag, multiple: false);
};

Console.Read();


//// 5 - KUYRUKTAN MESAJLARI TÜKET
//await channel.BasicConsumeAsync(
//    queue: queueName,
//    autoAck: false,
//    consumer: consumer);

//Console.ReadLine();
