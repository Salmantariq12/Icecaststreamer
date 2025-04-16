# Live Audio Streaming with Icecast & Google Cloud Text-to-Speech (TTS)

This project streams **live audio** using **Google Cloud Text-to-Speech (TTS)** and **Icecast**. It announces the current time every 5 seconds and can be played in a web browser.

## Prerequisites

Before running the project, ensure you have the following installed:

### 1. Install Icecast
  1. Download Icecast from `https://icecast.org/download/`. 
  2. Run the installer and complete the setup.
  3. Navigate to the Icecast installation folder (usually `C:\Program Files (x86)\Icecast\bin`).
  4. Open this path in cmd, by opening cmd as an administrator and type `cd C:\Program Files (x86)\Icecast\bin`.
  5. Now open `icecast.xml` file, by opening notepad as an administrator and locate this path : `C:\Program Files (x86)\Icecast` and select `All Files` in file type and then select icecast.xml file from there.
  6. Open `icecast.xml` and **modify the following settings**:
           ```
           <authentication>
               <source-password>your-source-password</source-password>
               <admin-user>your-admin-username</admin-user>
               <admin-password>your-admin-password</admin-password>
           </authentication>

           <icecast>
                <location>yourlocation</location>
                <admin>emailAddress</admin>
                <hostname>localhost:8080</hostname> can also use any port
            </icecast>

            <paths>
                <logdir>C:\Program Files (x86)\Icecast\log</logdir>
                <webroot>C:\Program Files (x86)\Icecast\web</webroot>
                <adminroot>C:\Program Files (x86)\Icecast\admin</adminroot>
            </paths>

            <limits>
                <client-timeout>120</client-timeout>
            <limits>
           ```
  7. Move`mime.types` file from `C:\Program Files (x86)\Icecast` to `C:\Program Files (x86)\Icecast\bin` folder. 
  8. Save the file and start Icecast by running this command in previously opened cmd: `icecast -c "C:\Program Files (x86)\Icecast\icecast.xml"`.

### 3. Set Up Google Cloud Text-to-Speech API

  #### Steps:
   1. **Create a Google Cloud project** at [Google Cloud Console](https://console.cloud.google.com/).
   2. **Enable the Text-to-Speech API**.
   3. **Create a Service Account & Download the JSON key**:
     - Go to **IAM & Admin > Service Accounts**.
     - Create a new service account.
     - Assign **Text-to-Speech User** role.
     - Generate and download the JSON key file.
     - Place it in a known location, e.g., `C:\Users\YourUser\Downloads\gcp-key.json`.

  ## Running the C# Program

  ### 1. Install Required Dependencies
   Ensure you have .NET 8 SDK installed.

  ### 2. Modify `Program.cs`
  Open `Program.cs` and update:
  ```
   string jsonPath = @"C:\Users\YourUser\Downloads\gcp-key.json";
   string password = "your-source-password";
  ```
  Replace with the path to your **Google Cloud credentials JSON file** and your source-password you changed in icecast.xml file.

  Run the C# Program

  Expected output:
    ```
    Starting Icecast streaming...
    Generating speech: The current time is 10:30 AM
    Generating speech: The current time is 10:35 AM
    ...
    ```

  ## Access the Live Stream
  Once the program is running, open a browser and enter:
    ```
    http://localhost:8000/live
    ```
You should hear the generated speech announcing the current time every 5 seconds.
---

### 🎉 Your live audio stream should now be running successfully! 🚀

### After 1st time initialization of this project, then every time you run this project you have to first run 'icecast -c "C:\Program Files (x86)\Icecast\icecast.xml' in cmd by opening `C:\Program Files (x86)\Icecast\bin` in cmd,
### then you have to start this project and then access `http://localhost:8000/live`