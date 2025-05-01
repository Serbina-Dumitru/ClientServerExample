using System.Net;
using System.Net.Sockets;
using System.Text;
namespace FrameworkClass;

public class Constants{
  public static int PORT = 4789;
  public static IPAddress Address = IPAddress.Loopback;
}
 

public class Messenger{
  public static IPAddress GetHostIpAddres(){
    var host = Dns.GetHostEntry(Dns.GetHostName());
    foreach(var ip in host.AddressList){
      if(ip.AddressFamily == AddressFamily.InterNetwork){
        return ip;
      }
    }
    return Constants.Address;
  }
  public byte[] CreateMessagePacket(int opcode, string message){
    using MemoryStream ms = new MemoryStream();
    using BinaryWriter bw = new BinaryWriter(ms);
    bw.Write(opcode);
    bw.Write(message);
    return ms.ToArray();
  }
  public (int opcode, string message) ParseMessagePacket(byte[] data){
    using MemoryStream ms = new MemoryStream(data);
    using BinaryReader br = new BinaryReader(ms,Encoding.ASCII);
    int    opcode  = 0;
    string message = "";

    try{
      opcode  = br.ReadInt32();
    }catch(Exception e){ 
      Console.WriteLine($"--------------------------------------------------");
      Console.WriteLine($"Error: {e}");
      Console.WriteLine($"--------------------------------------------------");
    }
    try{
      message = br.ReadString();
    }catch(Exception e){
      Console.WriteLine($"--------------------------------------------------");
      Console.WriteLine($"Error: {e}");
      Console.WriteLine($"--------------------------------------------------");
    }

    if(opcode == 0 || message == string.Empty) return (0,string.Empty);   
    return (opcode,message);
  }
}
