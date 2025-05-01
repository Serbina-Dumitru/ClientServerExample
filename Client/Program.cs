using System.Net.Sockets;
using FrameworkClass;

TcpClient client = new TcpClient();
client.Connect(Messenger.GetHostIpAddres(), Constants.PORT);
Console.WriteLine("Connected to the server!");

Messenger inStream  = new Messenger();
Messenger outStream = new Messenger();

//List<string> outgoingMessages = new List<string>();
Stack<string> outgoingMessages  = new Stack<string>();

new TaskFactory().StartNew(()=>{
  while(true){
    var msg = Console.ReadLine();
    if(msg == null) continue;
    outgoingMessages.Push(msg);
  }
});

while(true){
  ReadPackets();
  SendPackets();
}

void ReadPackets(){
  var stream = client.GetStream();
  for(int i = 0; i < 10; i++){
    if(stream.DataAvailable){
      byte[] buffer = new byte[client.ReceiveBufferSize];
      int bytesRead = stream.Read(buffer,0,buffer.Length);
      (int opcode, string message) = inStream.ParseMessagePacket(buffer.Take(bytesRead).ToArray());
      Console.WriteLine($"Received [{opcode}] - [{message}]");
    }
  }
}

void SendPackets(){ 
  if(outgoingMessages.Count > 0){
    var msg = outgoingMessages.Pop();
    var packet = outStream.CreateMessagePacket(10,msg);
    client.GetStream().Write(packet);
    //outgoingMessages.Pop();
  }
}
