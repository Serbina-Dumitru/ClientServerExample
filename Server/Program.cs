using System.Net.Sockets;
using FrameworkClass;

var inStream  = new Messenger();
var outStream = new Messenger();

List<TcpClient> _clients = new List<TcpClient>();

TcpListener listner = new TcpListener(Constants.Address,Constants.PORT);
listner.Start();

Console.WriteLine("Server started");

while(true){
  AcceptClients();
  ReceiveMessage();
}

void AcceptClients(){
  for(int i = 0; i < 5; i++){
    if(!listner.Pending()) continue;
    var client = listner.AcceptTcpClient();
    _clients.Add(client);
    Console.WriteLine("Client Accepted" );
  }

}

void ReceiveMessage(){
  foreach(var client in _clients){
    NetworkStream stream = client.GetStream();
    if(stream.DataAvailable){
      byte[] buffer = new byte[client.ReceiveBufferSize];
      int bytesRead = stream.Read(buffer,0,buffer.Length);
      (int? opcode, string? message) = inStream.ParseMessagePacket(buffer.Take(bytesRead).ToArray());
      Console.WriteLine($"Received: [{opcode}] - [{message}]");
      Broadcast(client, message);
    }
  }
}

void Broadcast(TcpClient sender, string message){
  foreach(var client in _clients.Where(x=> x != sender)){
    var packet = outStream.CreateMessagePacket(10,message);
    client.GetStream().Write(packet);
  }
}
