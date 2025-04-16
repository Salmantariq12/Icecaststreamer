using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.TextToSpeech.V1;

class Program
{
    static void Main()
    {
        string icecastServer = "localhost";
        int icecastPort = 8000;
        string mountPoint = "live";
        string username = "source";
        string password = "12345"; // your-source-password
        string jsonPath = @"C:\Users\hassan.javed\Downloads\geminivoiceai-519f3ac43fdc.json"; //path to JSON key file

        GoogleCredential credential;
        using (var stream = new FileStream(jsonPath, FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream);
        }

        var ttsClient = new TextToSpeechClientBuilder { Credential = credential }.Build();

        Console.WriteLine("Starting Icecast streaming...");
        StreamToIcecast(icecastServer, icecastPort, mountPoint, username, password, ttsClient);
    }

    static void StreamToIcecast(string server, int port, string mount, string user, string pass, TextToSpeechClient ttsClient)
    {
        while (true)
        {
            try
            {
                string auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user}:{pass}"));
                string headers = $"SOURCE /{mount} HTTP/1.0\r\n" +
                                 $"Host: {server}:{port}\r\n" +
                                 "Content-Type: audio/mpeg\r\n" +
                                 "Authorization: Basic " + auth + "\r\n" +
                                 "Expect:\r\n\r\n";

                using (TcpClient client = new TcpClient(server, port))
                using (NetworkStream stream = client.GetStream())
                {
                    byte[] headerBytes = Encoding.ASCII.GetBytes(headers);
                    stream.Write(headerBytes, 0, headerBytes.Length);
                    Console.WriteLine("Connected to Icecast!");

                    while (true)
                    {
                        string text = $"The current time is {DateTime.Now:hh:mm tt}";
                        Console.WriteLine("Generating speech: " + text);
                        byte[] mp3Data = GenerateSpeech(ttsClient, text);

                        if (mp3Data != null)
                        {
                            stream.Write(mp3Data, 0, mp3Data.Length);
                            stream.Flush();
                        }

                        byte[] silence = new byte[1024];
                        for (int i = 0; i < 10; i++)
                        {
                            stream.Write(silence, 0, silence.Length);
                            stream.Flush();
                            Thread.Sleep(200);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Icecast streaming error: " + ex.Message + " - Reconnecting in 5 seconds...");
                Thread.Sleep(5000);
            }
        }
    }


    static byte[] GenerateSpeech(TextToSpeechClient client, string text)
    {
        try
        {
            var request = new SynthesizeSpeechRequest
            {
                Input = new SynthesisInput { Text = text },
                Voice = new VoiceSelectionParams { LanguageCode = "en-US", SsmlGender = SsmlVoiceGender.Female },
                AudioConfig = new AudioConfig { AudioEncoding = AudioEncoding.Mp3 }
            };

            var response = client.SynthesizeSpeech(request);
            return response.AudioContent.ToByteArray();
        }
        catch (Exception ex)
        {
            Console.WriteLine("TTS Error: " + ex.Message);
            return null;
        }
    }
}